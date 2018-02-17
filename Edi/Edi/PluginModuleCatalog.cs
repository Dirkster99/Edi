using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Edi
{
    public class PluginModuleCatalog : ModuleCatalog
    {
        private readonly string pluginsDirectory = string.Empty;

        private readonly IModuleCatalog fallbackCatalog;

        public PluginModuleCatalog(string pluginsDirectory, IModuleCatalog fallbackCatalog)
        {
            if (Directory.Exists(pluginsDirectory))
            {
                this.pluginsDirectory = pluginsDirectory;
            }

            this.fallbackCatalog = fallbackCatalog;
        }

        private AppDomain CreateChildAppDomain(AppDomain parentDomain, string baseDirectory)
        {
            System.Security.Policy.Evidence evidence = new System.Security.Policy.Evidence(parentDomain.Evidence);

            AppDomainSetup setupInformation = parentDomain.SetupInformation;
            setupInformation.ApplicationBase = baseDirectory;

            return AppDomain.CreateDomain("PluginModuleDiscovery", evidence, setupInformation);
        }

        private void ParseDirectory(string directory)
        {
            ////DirectoryInfo di = new DirectoryInfo(directory);
            
            string expandedDirectory = Path.GetFullPath(Environment.ExpandEnvironmentVariables(directory));
            string fileName = Path.GetFileName(expandedDirectory);

            string[] candidateAssemblyPaths = Directory.GetFiles(expandedDirectory, string.Format(CultureInfo.InvariantCulture, @"{0}.dll", fileName), SearchOption.TopDirectoryOnly);

            if (candidateAssemblyPaths.Length > 0)
            {
                AppDomain childAppDomain = CreateChildAppDomain(AppDomain.CurrentDomain, expandedDirectory);
                Type loaderType = typeof(InnerModuleInfoLoader);

                try
                {
                    InnerModuleInfoLoader loader = (InnerModuleInfoLoader)childAppDomain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
                    this.Items.AddRange(loader.LoadModules(candidateAssemblyPaths));
                }
                catch (FileNotFoundException)
                {
                    // Continue loading assemblies even if an assembly can not be loaded in the new AppDomain
                }
                finally
                {
                    AppDomain.Unload(childAppDomain);
                }
            }

            foreach (string d in Directory.GetDirectories(expandedDirectory))
            {
                ParseDirectory(d);
            }
        }

        private Assembly ChildAppDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                return Assembly.ReflectionOnlyLoad(args.Name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override void Initialize()
        {
            fallbackCatalog.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// Drives the main logic of building the child domain and searching for the assemblies.
        /// </summary>
        protected override void InnerLoad()
        {
            if (fallbackCatalog != null)
            {
                foreach (ModuleInfo module in fallbackCatalog.Modules)
                {
                    this.Items.Add(module);
                }
            }

            if (!string.IsNullOrWhiteSpace(pluginsDirectory))
            {
                try
                {
                    ParseDirectory(pluginsDirectory);
                }
                catch (Exception)
                {
                    // Fail silently
                }
            }
        }

        private class InnerModuleInfoLoader : MarshalByRefObject
        {
            private static Assembly OnReflectionOnlyResolve(ResolveEventArgs args)
            {
                Assembly loadedAssembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault(
                    asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));
                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);

                DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(path));
                if (directory != null)
                {
                    AssemblyName assemblyName = new AssemblyName(args.Name);
                    string dependentAssemblyFilename = Path.Combine(directory.FullName, assemblyName.Name + ".dll");
                    if (File.Exists(dependentAssemblyFilename))
                    {
                        return Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilename);
                    }
                }

                return Assembly.ReflectionOnlyLoad(args.Name);
            }

            internal ModuleInfo[] LoadModules(string[] assemblies)
            {
                IList<ModuleInfo> moduleList = new List<ModuleInfo>();

                ResolveEventHandler resolveEventHandler = delegate (object sender, ResolveEventArgs args) { return OnReflectionOnlyResolve(args); };

                try
                {
                    AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;

                    Assembly moduleReflectionOnlyAssembly = Assembly.ReflectionOnlyLoad(typeof(ModuleExportAttribute).Assembly.FullName);

                    Type ModuleExportAttributeType = moduleReflectionOnlyAssembly.GetType(typeof(ModuleExportAttribute).FullName);

                    foreach (string assemblyPath in assemblies)
                    {
                        try
                        {
                            Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                            Type[] assemblyTypes = assembly.GetTypes();

                            foreach (Type type in assemblyTypes)
                            {
                                foreach (CustomAttributeData attributeData in type.GetCustomAttributesData())
                                {
                                    if (attributeData.AttributeType == ModuleExportAttributeType)
                                    {
                                        if (!type.IsAbstract)
                                        {
                                            moduleList.Add(CreateModuleInfo(type));
                                        }
                                    }
                                }
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            // Continue loading assemblies even if an assembly can not be loaded in the new AppDomain
                        }
                    }


                }
                catch (Exception)
                {
                    // Fail silently
                }
                finally
                {
                    AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;
                }

                int modulesCount = moduleList.Count;
                ModuleInfo[] modules = new ModuleInfo[modulesCount];

                if (modulesCount > 0)
                {
                    moduleList.CopyTo(modules, 0);
                }

                return modules;
            }

            private static ModuleInfo CreateModuleInfo(Type type)
            {
                string moduleName = type.Name;
                List<string> dependsOn = new List<string>();
                bool onDemand = false;
                var moduleAttribute = CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleAttribute).FullName);

                if (moduleAttribute != null)
                {
                    foreach (CustomAttributeNamedArgument argument in moduleAttribute.NamedArguments)
                    {
                        string argumentName = argument.MemberInfo.Name;
                        switch (argumentName)
                        {
                            case "ModuleName":
                                moduleName = (string)argument.TypedValue.Value;
                                break;

                            case "OnDemand":
                                onDemand = (bool)argument.TypedValue.Value;
                                break;

                            case "StartupLoaded":
                                onDemand = !((bool)argument.TypedValue.Value);
                                break;
                        }
                    }
                }

                var moduleDependencyAttributes = CustomAttributeData.GetCustomAttributes(type).Where(cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleDependencyAttribute).FullName);
                foreach (CustomAttributeData cad in moduleDependencyAttributes)
                {
                    dependsOn.Add((string)cad.ConstructorArguments[0].Value);
                }

                ModuleInfo moduleInfo = new ModuleInfo(moduleName, type.AssemblyQualifiedName)
                {
                    InitializationMode =
                        onDemand
                            ? InitializationMode.OnDemand
                            : InitializationMode.WhenAvailable,
                    Ref = type.Assembly.CodeBase,
                };
                moduleInfo.DependsOn.AddRange(dependsOn);
                return moduleInfo;
            }
        }
    }


    /// <summary>
    /// Class that provides extension methods to Collection
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Add a range of items to a collection.
        /// </summary>
        /// <typeparam name="T">Type of objects within the collection.</typeparam>
        /// <param name="collection">The collection to add items to.</param>
        /// <param name="items">The items to add to the collection.</param>
        /// <returns>The collection.</returns>
        /// <exception cref="System.ArgumentNullException">An <see cref="System.ArgumentNullException"/> is thrown if <paramref name="collection"/> or <paramref name="items"/> is <see langword="null"/>.</exception>
        public static Collection<T> AddRange<T>(this Collection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (items == null) throw new ArgumentNullException("items");

            foreach (var each in items)
            {
                collection.Add(each);
            }

            return collection;
        }
    }
}

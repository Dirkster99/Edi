namespace MRULib.MRU.Models.Persist
{
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Source: https://codeoverload.wordpress.com/2010/07/30/c-object-persistence-with-xml/
    /// </summary>
    public class XmlSerializerUtil
    {
        /// <summary>
        /// Loads an XML file for a specified class type <typeparamref name="T"/>
        /// from the file system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path)
        {
            T savedObject = default(T);
            XmlSerializer x = new XmlSerializer(typeof(T));
            using (FileStream st = File.Open(path, FileMode.Open))
            {
                savedObject = (T)x.Deserialize(st);
            }

            return savedObject;
        }

        /// <summary>
        /// Returns a task that loads an XML file for a specified class
        /// type <typeparamref name="T"/>
        /// from the file system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<T> LoadAsync<T>(string path)
        {
            return Task.Run(() => {
                T savedObject = default(T);
                XmlSerializer x = new XmlSerializer(typeof(T));
                using (FileStream st = File.Open(path, FileMode.Open))
                {
                    savedObject = (T)x.Deserialize(st);
                }

                return savedObject;
            });
        }

        /// <summary>
        /// Loads an XML file for a specified class type <typeparamref name="T"/>
        /// from an in memory <seealso cref="StringReader"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        public static T LoadFromString<T>(string xmlContent)
        {
            T savedObject = default(T);
            XmlSerializer x = new XmlSerializer(typeof(T));

            using (StringReader st = new StringReader(xmlContent))
            {
                savedObject = (T)x.Deserialize(st);
                return savedObject;
            }
        }

        /// <summary>
        /// Returns a task that Loads an XML file for a specified class type
        /// <typeparamref name="T"/> from an in memory <seealso cref="StringReader"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        public static Task<T> LoadFromStringAsync<T>(string xmlContent)
        {
            return Task.Run(() => {
                T savedObject = default(T);
                XmlSerializer x = new XmlSerializer(typeof(T));

                using (StringReader st = new StringReader(xmlContent))
                {
                    savedObject = (T)x.Deserialize(st);
                    return savedObject;
                }
            });
        }

        /// <summary>
        /// Persists an XML file for a specified class type <typeparamref name="T"/>
        /// to the file system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        public static void Save<T>(string path, T obj)
        {
            XmlSerializer x = new XmlSerializer(typeof(T));
            using (FileStream st = File.Open(path, FileMode.Create))
            {
                x.Serialize(st, obj);
            }
        }

        /// <summary>
        /// Returns a task that persists an XML file for a specified
        /// class type <typeparamref name="T"/>
        /// to the file system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        public static Task SaveAsync<T>(string path, T obj)
        {
            return Task.Run(() =>
            {
                XmlSerializer x = new XmlSerializer(typeof(T));
                using (FileStream st = File.Open(path, FileMode.Create))
                {
                    x.Serialize(st, obj);
                }
            });
        }

        /// <summary>
        /// Persists an XML file for a specified class type <typeparamref name="T"/>
        /// to an in-memory string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SaveToString<T>(T obj)
        {
            XmlSerializer x = new XmlSerializer(typeof(T));

            using(StringWriter st = new StringWriter())
            {
                x.Serialize(st, obj);
                return st.ToString();
            }
        }
    }
}

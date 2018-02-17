﻿namespace Edi.Apps.ViewModels
{
    using Enums;
    using Core;
    using Core.Interfaces;
    using Core.ViewModels;
    using Core.ViewModels.Command;
    using Documents.ViewModels.EdiDoc;
    using Documents.ViewModels.StartPage;
    using Themes;
    using Files.ViewModels.RecentFiles;
    using MiniUML.Framework;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    public partial class ApplicationViewModel
    {
        private bool Closing_CanExecute()
        {
            if (mShutDownInProgress)
                return false;

            // Check if conditions within the WorkspaceViewModel are suitable to close the application
            // eg.: Prompt to Cancel long running background tasks such as Search - Replace in Files (if any)

            return true;
        }

        /// <summary>
        /// Bind a window to some commands to be executed by the viewmodel.
        /// </summary>
        /// <param name="win"></param>
        public void InitCommandBinding(Window win)
        {
            InitEditCommandBinding(win);

            win.CommandBindings.Add(new CommandBinding(AppCommand.Exit,
            (s, e) =>
            {
                AppExit_CommandExecuted();
                e.Handled = true;
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.About,
            (s, e) =>
            {
                AppAbout_CommandExecuted();
                e.Handled = true;
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.ProgramSettings,
            (s, e) =>
            {
                AppProgramSettings_CommandExecuted();
                e.Handled = true;
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.ShowToolWindow,
            (s, e) =>
            {
                if (e == null)
                    return;

                var toolwindowviewmodel = e.Parameter as IToolWindow;

                if (toolwindowviewmodel == null)
                    return;


                if (toolwindowviewmodel is IRegisterableToolWindow)
                {
                    IRegisterableToolWindow registerTW = toolwindowviewmodel as IRegisterableToolWindow;

                    registerTW.SetToolWindowVisibility(this, !toolwindowviewmodel.IsVisible);
                }
                else
                    toolwindowviewmodel.SetToolWindowVisibility(!toolwindowviewmodel.IsVisible);

                e.Handled = true;
            }));

            // Standard File New command binding via ApplicationCommands enumeration
            win.CommandBindings.Add(new CommandBinding(ApplicationCommands.New,
            (s, e) =>
            {
                TypeOfDocument t = TypeOfDocument.EdiTextEditor;

                if (e != null)
                {
                    e.Handled = true;

                    if (e.Parameter != null)
                    {
                        if (e.Parameter is TypeOfDocument)
                            t = (TypeOfDocument)e.Parameter;
                    }
                }

                OnNew(t);
            }
            ));

            // Standard File Open command binding via ApplicationCommands enumeration
            win.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
            (s, e) =>
            {
                string t = string.Empty;

                if (e != null)
                {
                    if (e.Parameter != null)
                    {
                        if (e.Parameter is string)
                            t = (string)e.Parameter;
                    }
                }

                OnOpen(t);
                e.Handled = true;
            }
            ));

            // Close Document command
            // Closes the FileViewModel document supplied in e.parameter
            // or the Active document
            win.CommandBindings.Add(new CommandBinding(AppCommand.CloseFile,
            (s, e) =>
            {
                try
                {
                    FileBaseViewModel f = null;

                    if (e != null)
                    {
                        e.Handled = true;
                        f = e.Parameter as FileBaseViewModel;
                    }

                    if (f != null)
                        Close(f);
                    else
                    {
                        if (ActiveDocument != null)
                            Close(ActiveDocument);
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) =>
            {
                try
                {
                    if (e != null)
                    {
                        e.Handled = true;
                        e.CanExecute = false;

                        EdiViewModel f = null;

                        if (e != null)
                        {
                            e.Handled = true;
                            f = e.Parameter as EdiViewModel;
                        }

                        if (f != null)
                            e.CanExecute = f.CanClose();
                        else
                        {
                            if (ActiveDocument != null)
                                e.CanExecute = ActiveDocument.CanClose();
                        }
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            }));

            // Change the WPF/TextEditor highlighting theme currently used in the application
            win.CommandBindings.Add(new CommandBinding(AppCommand.ViewTheme,
                                                            (s, e) => ChangeThemeCmd_Executed(s, e, win.Dispatcher)));

            win.CommandBindings.Add(new CommandBinding(AppCommand.BrowseURL,
            (s, e) =>
            {
                Process.Start(new ProcessStartInfo("https://github.com/Dirkster99/Edi"));
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.ShowStartPage,
            (s, e) =>
            {
                StartPageViewModel spage = GetStartPage(true);

                if (spage != null)
                {
                    logger.InfoFormat("TRACE Before setting startpage as ActiveDocument");
                    ActiveDocument = spage;
                    logger.InfoFormat("TRACE After setting startpage as ActiveDocument");
                }
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.ToggleOptimizeWorkspace,
            (s, e) =>
            {
                logger.InfoFormat("TRACE AppCommand.ToggleOptimizeWorkspace parameter is {0}.", (e == null ? "(null)" : e.ToString()));

                try
                {
                    var newViewSetting = !IsWorkspaceAreaOptimized;
                    IsWorkspaceAreaOptimized = newViewSetting;
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink, mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.LoadFile,
            (s, e) =>
            {
                try
                {
                    logger.InfoFormat("TRACE AppCommand.LoadFile parameter is {0}.", (e == null ? "(null)" : e.ToString()));

                    if (e == null)
                        return;

                    string filename = e.Parameter as string;

                    if (filename == null)
                        return;

                    logger.InfoFormat("TRACE AppCommand.LoadFile with: '{0}'", filename);

                    Open(filename);
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink, mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            }));

            win.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save,
            (s, e) =>
            {
                try
                {
                    if (e != null)
                        e.Handled = true;

                    if (ActiveDocument != null)
                        OnSave(ActiveDocument, false);
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                                 MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink, mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) =>
            {
                if (e != null)
                {
                    e.Handled = true;

                    if (ActiveDocument != null)
                        e.CanExecute = ActiveDocument.CanSave();
                }
            }));

            win.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs,
            (s, e) =>
            {
                try
                {
                    if (e != null)
                        e.Handled = true;

                    if (ActiveDocument != null)
                    {
                        if (OnSave(ActiveDocument, true))
                        {
                            var mruList = ServiceLocator.Current.GetInstance<IMRUListViewModel>();
                            mruList.UpdateEntry(ActiveDocument.FilePath);
                            mSettingsManager.SessionData.LastActiveFile = ActiveDocument.FilePath;
                        }
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) =>
            {
                try
                {
                    if (e != null)
                    {
                        e.Handled = true;
                        e.CanExecute = false;

                        if (ActiveDocument != null)
                            e.CanExecute = ActiveDocument.CanSaveAs();
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            }
            ));

            // Execute a command to save all edited files and current program settings
            win.CommandBindings.Add(new CommandBinding(AppCommand.SaveAll,
            (s, e) =>
            {
                try
                {
                    // Save all edited documents
                    if (mFiles != null)               // Close all open files and make sure there are no unsaved edits
                    {                                     // If there are any: Ask user if edits should be saved
                        IFileBaseViewModel activeDoc = ActiveDocument;

                        try
                        {
                            for (int i = 0; i < Files.Count; i++)
                            {
                                IFileBaseViewModel f = Files[i];

                                if (f != null)
                                {
                                    if (f.IsDirty && f.CanSaveData)
                                    {
                                        ActiveDocument = f;
                                        OnSave(f);
                                    }
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            _MsgBox.Show(exp.ToString(), Util.Local.Strings.STR_MSG_UnknownError_Caption, MsgBoxButtons.OK);
                        }
                        finally
                        {
                            if (activeDoc != null)
                                ActiveDocument = activeDoc;
                        }
                    }

                    // Save program settings
                    SaveConfigOnAppClosed();
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            }));

            // Execute a command to export UML editor content as image
            win.CommandBindings.Add(new CommandBinding(AppCommand.ExportUMLToImage,
            (s, e) =>
            {
                try
                {
                    if (vm_DocumentViewModel != null)
                    {
                        if ((vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready))
                        {
                            vm_DocumentViewModel.ExecuteExport(s, e, ActiveDocument.FileName + ".png");
                        }
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) =>  // Execute this command only if an UML document is currently active
            {
                if (vm_DocumentViewModel != null)
                    e.CanExecute = (vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
                else
                    e.CanExecute = false;
            }
            ));

            // Execute a command to export Text editor content as highlighted image content
            win.CommandBindings.Add(new CommandBinding(AppCommand.ExportTextToHTML,
            (s, e) =>
            {
                try
                {
                    if (ActiveEdiDocument != null)
                        ActiveEdiDocument.ExportToHTML(ActiveDocument.FileName + ".html",
                                                                                                mSettingsManager.SettingData.TextToHTML_ShowLineNumbers,
                                                                                                mSettingsManager.SettingData.TextToHTML_AlternateLineBackground);
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) =>  // Execute this command only if a Text document is currently active
            {
                if (ActiveEdiDocument != null)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            ));


            /// <summary>
            /// Removes ALL MRU entries (even pinned entries) from the current list of entries.
            /// </summary>
            win.CommandBindings.Add(new CommandBinding(AppCommand.ClearAllMruItemsCommand,
            (s, e) =>
            {
                GetToolWindowVM<RecentFilesViewModel>().MruList.Clear();
            }));

            /// <summary>
            /// Gets a command that removes all items that are older
            /// than a given <see cref="GroupType"/>.
            /// Eg.: Remove all MRU entries older than yesterday.
            /// </summary>
            win.CommandBindings.Add(new CommandBinding(AppCommand.RemoveItemsOlderThanThisCommand,
            (s, e) =>
            {
                if (e.Parameter is GroupType == false)
                    return;

                var param = (GroupType)e.Parameter;

                GetToolWindowVM<RecentFilesViewModel>().MruList.RemoveEntryOlderThanThis(param);
            },
            (s, e) =>
            {
                if (e.Parameter is GroupType == false)
                {
                    e.CanExecute = false;
                    return;
                }

                e.CanExecute = true;
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.MovePinnedMruItemUPCommand,
            (s, e) =>
            {
                if (e.Parameter is IMRUEntryViewModel == false)
                    return;

                var param = e.Parameter as IMRUEntryViewModel;

                GetToolWindowVM<RecentFilesViewModel>().MruList.MovePinnedEntry(MoveMRUItem.Up, param);
            },
            (s, e) =>
            {
                if (e.Parameter is IMRUEntryViewModel == false)
                {
                    e.CanExecute = false;
                    return;
                }

                if ((e.Parameter as IMRUEntryViewModel).IsPinned == 0)  //Make sure it is pinned
                {
                    e.CanExecute = false;
                    return;
                }

                e.CanExecute = true;
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.MovePinnedMruItemDownCommand,
            (s, e) =>
            {
                if (e.Parameter is IMRUEntryViewModel == false)
                    return;

                var param = e.Parameter as IMRUEntryViewModel;

                GetToolWindowVM<RecentFilesViewModel>().MruList.MovePinnedEntry(MoveMRUItem.Down, param);
            },
            (s, e) =>
            {
                if (e.Parameter is IMRUEntryViewModel == false)
                {
                    e.CanExecute = false;
                    return;
                }

                if ((e.Parameter as IMRUEntryViewModel).IsPinned == 0)  //Make sure it is pinned
                {
                    e.CanExecute = false;
                    return;
                }

                e.CanExecute = true;
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.PinItemCommand,
            (s, e) =>
            {
                GetToolWindowVM<RecentFilesViewModel>().MruList.PinUnpinEntry(true, e.Parameter as IMRUEntryViewModel);
            },
            (s, e) =>
            {
                if (e.Parameter is IMRUEntryViewModel == false)
                {
                    e.CanExecute = false;
                    return;
                }

                if ((e.Parameter as IMRUEntryViewModel).IsPinned == 0)  //Make sure it is pinned
                {
                    e.CanExecute = true;
                    return;
                }

                e.CanExecute = false;
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.UnPinItemCommand,
            (s, e) =>
            {
                if (e.Parameter is IMRUEntryViewModel == false)
                    return;

                var param = e.Parameter as IMRUEntryViewModel;

                GetToolWindowVM<RecentFilesViewModel>().MruList.PinUnpinEntry(false, e.Parameter as IMRUEntryViewModel);
            },
            (s, e) =>
            {
                if (e.Parameter is IMRUEntryViewModel == false)
                {
                    e.CanExecute = false;
                    return;
                }

                if ((e.Parameter as IMRUEntryViewModel).IsPinned == 0)  //Make sure it is pinned
                {
                    e.CanExecute = false;
                    return;
                }

                e.CanExecute = true;
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.PinUnpin,
            (s, e) =>
            {
                PinCommand_Executed(e.Parameter, e);
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.RemoveMruEntry,
            (s, e) =>
            {
                RemoveMRUEntry_Executed(e.Parameter, e);
            }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.AddMruEntry,
            (s, e) =>
            {
                AddMRUEntry_Executed(e.Parameter, e);
            }));
        }

        /// <summary>
        /// This procedure changes the current WPF Application Theme into another theme
        /// while the application is running (re-boot should not be required).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="disp"></param>
        private void ChangeThemeCmd_Executed(object s,
                                            ExecutedRoutedEventArgs e,
                                            Dispatcher disp)
        {
            string oldTheme = ThemesManager.DefaultThemeName;

            try
            {
                if (e == null)
                    return;

                if (e.Parameter == null)
                    return;

                string newThemeName = e.Parameter as string;

                // Check if request is available
                if (newThemeName == null)
                    return;

                oldTheme = mSettingsManager.SettingData.CurrentTheme;

                // The Work to perform on another thread
                ThreadStart start = delegate
                {
                    // This works in the UI tread using the dispatcher with highest Priority
                    disp.Invoke(DispatcherPriority.Send,
                    (Action)(() =>
                    {
                        try
                        {
                            if (mThemesManager.SetSelectedTheme(newThemeName))
                            {
                                mSettingsManager.SettingData.CurrentTheme = newThemeName;
                                ResetTheme();                        // Initialize theme in process
                            }
                        }
                        catch (Exception exp)
                        {
                            logger.Error(exp.Message, exp);
                            _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                         mAppCore.IssueTrackerLink,
                                         mAppCore.IssueTrackerLink,
                                         Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                        }
                    }));
                };

                // Create the thread and kick it started!
                Thread thread = new Thread(start);

                thread.Start();
            }
            catch (Exception exp)
            {
                mSettingsManager.SettingData.CurrentTheme = oldTheme;

                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }

        #region EditorCommands

        /// <summary>
        /// Set command bindings necessary to perform copy/cut/paste operations
        /// </summary>
        /// <param name="win"></param>
        public void InitEditCommandBinding(Window win)
        {
            win.CommandBindings.Add(new CommandBinding(AppCommand.DisableHighlighting,    // Select all text in a document
            (s, e) =>
            {
                try
                {

                    if (ActiveDocument is EdiViewModel)
                    {
                        EdiViewModel f = ActiveDocument as EdiViewModel;
                        f.DisableHighlighting();
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) =>
            {

                if (ActiveDocument is EdiViewModel)
                {
                    EdiViewModel f = ActiveDocument as EdiViewModel;

                    if (f.HighlightingDefinition != null)
                    {
                        e.CanExecute = true;
                        return;
                    }
                }

                e.CanExecute = false;
            }));

            #region GotoLine FindReplace
            win.CommandBindings.Add(new CommandBinding(AppCommand.GotoLine,    // Goto line n in a document
            (s, e) =>
            {
                try
                {
                    e.Handled = true;

                    ShowGotoLineDialog();
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.FindText,    // Find text in a document
            (s, e) =>
            {
                try
                {
                    e.Handled = true;

                    ShowFindReplaceDialog();
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.FindPreviousText,    // Find text in a document
            (s, e) =>
            {
                try
                {
                    e.Handled = true;


                    if (ActiveDocument is EdiViewModel)
                    {
                        EdiViewModel f = ActiveDocument as EdiViewModel;

                        if (FindReplaceVM != null)
                        {
                            FindReplaceVM.FindNext(FindReplaceVM, true);
                        }
                        else
                        {
                            ShowFindReplaceDialog();
                        }
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.FindNextText,    // Find text in a document
            (s, e) =>
            {
                try
                {
                    e.Handled = true;


                    if (ActiveDocument is EdiViewModel)
                    {
                        EdiViewModel f = ActiveDocument as EdiViewModel;

                        if (FindReplaceVM != null)
                        {
                            FindReplaceVM.FindNext(FindReplaceVM, false);
                        }
                        else
                        {
                            ShowFindReplaceDialog();
                        }
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));

            win.CommandBindings.Add(new CommandBinding(AppCommand.ReplaceText, // Find and replace text in a document
            (s, e) =>
            {
                try
                {
                    e.Handled = true;

                    ShowFindReplaceDialog(false);
                }
                catch (Exception exp)
                {
                    logger.Error(exp.Message, exp);
                    _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                 mAppCore.IssueTrackerLink,
                                 mAppCore.IssueTrackerLink,
                                 Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                }
            },
            (s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));
            #endregion GotoLine FindReplace
        }

        #region ToggleEditorOptionCommand
        RelayCommand<ToggleEditorOption> _toggleEditorOptionCommand = null;
        public ICommand ToggleEditorOptionCommand
        {
            get
            {
                if (_toggleEditorOptionCommand == null)
                {
                    _toggleEditorOptionCommand = new RelayCommand<ToggleEditorOption>
                                        ((p) => OnToggleEditorOption(p),
                                         (p) => CanExecuteIfActiveDocumentIsEdiViewModel());
                }

                return _toggleEditorOptionCommand;
            }
        }

        private void OnToggleEditorOption(object parameter)
        {
            EdiViewModel f = ActiveDocument as EdiViewModel;

            if (f == null)
                return;

            if (parameter == null)
                return;

            if ((parameter is ToggleEditorOption) == false)
                return;

            ToggleEditorOption t = (ToggleEditorOption)parameter;

            if (f != null)
            {
                switch (t)
                {
                    case ToggleEditorOption.WordWrap:
                        f.WordWrap = !f.WordWrap;
                        break;

                    case ToggleEditorOption.ShowLineNumber:
                        f.ShowLineNumbers = !f.ShowLineNumbers;
                        break;

                    case ToggleEditorOption.ShowSpaces:
                        f.TextOptions.ShowSpaces = !f.TextOptions.ShowSpaces;
                        break;

                    case ToggleEditorOption.ShowTabs:
                        f.TextOptions.ShowTabs = !f.TextOptions.ShowTabs;
                        break;

                    case ToggleEditorOption.ShowEndOfLine:
                        f.TextOptions.ShowEndOfLine = !f.TextOptions.ShowEndOfLine;
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion ToggleEditorOptionCommand

        private bool CanExecuteIfActiveDocumentIsEdiViewModel()
        {

            if (ActiveDocument is EdiViewModel)
            {
                //EdiViewModel f = this.ActiveDocument as EdiViewModel;

                return true;
            }

            return false;
        }
        #endregion EditorCommands
    }
}

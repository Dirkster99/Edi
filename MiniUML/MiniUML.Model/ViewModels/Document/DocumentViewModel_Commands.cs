namespace MiniUML.Model.ViewModels.Document
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Printing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Xps.Packaging;
    using Microsoft.Win32;
    using Framework;
    using Shapes;

    public partial class DocumentViewModel : AbstractDocumentViewModel
    {
        #region private classes
        /// <summary>
        /// Utility class used by the command implementations.
        /// </summary>
        private class CommandUtility
        {
            #region fields
            private DocumentViewModel mViewModel;
            #endregion fields

            public CommandUtility(DocumentViewModel viewModel)
            {
                mViewModel = viewModel;

                // Initialize commands.
                viewModel.cmd_New = new NewCommandModel(viewModel);
                viewModel.cmd_Open = new OpenCommandModel(viewModel);
                viewModel.cmd_Save = new SaveCommandModel(viewModel);
                viewModel.cmd_SaveAs = new SaveAsCommandModel(viewModel);
                viewModel.cmd_Export = new ExportCommandModel(viewModel);
                viewModel.cmd_Print = new PrintCommandModel(viewModel);
                viewModel.cmd_Undo = new UndoCommandModel(viewModel);
                viewModel.cmd_Redo = new RedoCommandModel(viewModel);
            }

            public PrintTicket PrintTicket { get; set; }

            public PrintQueue PrintQueue { get; set; }

            public Rectangle GetDocumentRectangle()
            {
                FrameworkElement canvasView = mViewModel.v_CanvasView;

                // Create a VisualBrush representing the contents of the  document
                // and use it to paint a rectangle of the same size as the page.
                Rectangle rect = new Rectangle()
                {
                    Width = canvasView.ActualWidth,
                    Height = canvasView.ActualHeight,
                    Fill = new VisualBrush(canvasView) { TileMode = TileMode.None }
                };

                // Measure and arrange.
                rect.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                rect.Arrange(new Rect(new Size(canvasView.ActualWidth, canvasView.ActualHeight)));

                return rect;
            }

            public Rectangle GetDocumentRectangle(Size desiredSize)
            {
                FrameworkElement canvasView = mViewModel.v_CanvasView;

                // Create a VisualBrush representing the contents of the  document.
                VisualBrush v = new VisualBrush(canvasView) { TileMode = TileMode.None };

                // Scale the brush to fit within the specified bounds.
                if (canvasView.ActualWidth > desiredSize.Width || canvasView.ActualHeight > desiredSize.Height)
                    v.Stretch = Stretch.Uniform;
                else
                    v.Stretch = Stretch.None;

                // Use the brush to paint a rectangle of the specified size.
                Rectangle rect = new Rectangle()
                {
                    Width = desiredSize.Width,
                    Height = desiredSize.Height,
                    Fill = v
                };

                // Measure and arrange.
                rect.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                rect.Arrange(new Rect(rect.DesiredSize));

                return rect;
            }
        }

        /// <summary>
        /// Private implementation of the Save command.
        /// </summary>
        private class SaveCommandModel : CommandModel
        {
            private DocumentViewModel mViewModel;

            public SaveCommandModel(DocumentViewModel viewModel)
                : base(ApplicationCommands.Save)
            {
                mViewModel = viewModel;
                Description = Framework.Local.Strings.STR_CMD_SAVE_DOCUMENT;
                Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Save"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                Execute();
            }

            public bool Execute()
            {
                string file = mViewModel.prop_DocumentFilePath;

                try
                {
                    if (File.Exists(file))
                    {
                        // Save document to the existing file.
                        mViewModel.mDataModel.Save(file);
                        return true;
                    }
                    else
                    {
                        // Execute SaveAs command.
                        return ((SaveAsCommandModel)mViewModel.cmd_SaveAs).Execute();
                    }
                }
                catch (Exception ex)
                {
                    var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                    msgBox.Show(ex, string.Format(Framework.Local.Strings.STR_SaveFILE_MSG, file),
                                Framework.Local.Strings.STR_SaveFILE_MSG_CAPTION);
                }

                return false;
            }
        }

        /// <summary>
        /// Private implementation of the Save As command.
        /// </summary>
        private class SaveAsCommandModel : CommandModel
        {
            private DocumentViewModel mViewModel;

            public SaveAsCommandModel(DocumentViewModel viewModel)
                : base(ApplicationCommands.SaveAs)
            {
                mViewModel = viewModel;
                Description = Framework.Local.Strings.STR_CMD_SAVEAS_DOCUMENT;
                Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.SaveAs"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                Execute();
            }

            public bool Execute()
            {
                // Create and configure SaveFileDialog.
                FileDialog dlg = new SaveFileDialog()
                {
                    Filter = Framework.Local.Strings.STR_FILETYPE_FILTER_SAVE,
                    AddExtension = true,
                    ValidateNames = true
                };

                // Show dialog; return if canceled.
                if (!dlg.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
                    return false;

                try
                {
                    // Save document.
                    mViewModel.mDataModel.Save(dlg.FileName);
                    mViewModel.prop_DocumentFilePath = dlg.FileName;

                    return true;
                }
                catch (Exception ex)
                {
                    var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                    msgBox.Show(ex, string.Format(Framework.Local.Strings.STR_SaveFILE_MSG, dlg.FileName),
                                Framework.Local.Strings.STR_SaveFILE_MSG_CAPTION);
                }

                return false;
            }
        }

        /// <summary>
        /// Private implementation of the Export command.
        /// </summary>
        private class ExportCommandModel : CommandModel
        {
            private DocumentViewModel mViewModel;

            public ExportCommandModel(DocumentViewModel viewModel)
            {
                mViewModel = viewModel;
                Name = Framework.Local.Strings.STR_CMD_Export_Command;
                Description = Framework.Local.Strings.STR_CMD_Export_Command_DESCR;
                Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Export"];
            }

            public static void ExportUMLToImage(DocumentViewModel viewModel, string defaultFileName = "")
            {
                // Create and configure SaveFileDialog.
                FileDialog dlg = new SaveFileDialog()
                {
                    ValidateNames = true,
                    AddExtension = true,
                    Filter = Framework.Local.Strings.STR_FILETYPE_FILTER_EXPORT,
                    FileName = defaultFileName
                };

                // Show dialog; return if canceled.
                if (!dlg.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
                    return;

                try
                {
                    // Save document to a file of the specified type.
                    switch (dlg.FilterIndex)
                    {
                        case 1:
                            saveAsBitmap(dlg.FileName, new PngBitmapEncoder(), true, viewModel);
                            break;

                        case 2:
                            saveAsBitmap(dlg.FileName, new JpegBitmapEncoder(), false, viewModel);
                            break;

                        case 3:
                            saveAsBitmap(dlg.FileName, new BmpBitmapEncoder(), false, viewModel);
                            break;

                        case 4:
                            saveAsXPS(dlg.FileName, viewModel);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                    msgBox.Show(ex, string.Format(Framework.Local.Strings.STR_SaveFILE_MSG, dlg.FileName),
                                Framework.Local.Strings.STR_SaveFILE_MSG_CAPTION);
                }
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                ExportUMLToImage(mViewModel);
            }

            private static void saveAsXPS(string file, DocumentViewModel viewModel)
            {
                // Deselect shapes while saving.
                List<ShapeViewModelBase> selectedItems = new List<ShapeViewModelBase>(viewModel.vm_CanvasViewModel.SelectedItem.Shapes);
                viewModel.vm_CanvasViewModel.SelectedItem.Clear();

                // Get a rectangle representing the page.
                Rectangle page = viewModel.mCommandUtility.GetDocumentRectangle();

                try
                {
                    using (Package package = Package.Open(file, FileMode.Create))
                    {
                        using (XpsDocument xpsDocument = new XpsDocument(package))
                        {
                            // Write the document.
                            System.Windows.Xps.XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                            xpsWriter.Write(page);
                        }
                    }
                }
                finally
                {
                    // Reselect shapes.
                    viewModel.vm_CanvasViewModel.SelectedItem.SelectShapes(selectedItems);
                }
            }

            private static void saveAsBitmap(string file,
                                                                                BitmapEncoder encoder,
                                                                                bool enableTransparentBackground,
                                                                                DocumentViewModel viewModel)
            {
                // Create and configure ExportDocumentWindowViewModel.
                ExportDocumentWindowViewModel windowViewModel = new ExportDocumentWindowViewModel()
                {
                    prop_Resolution = 96,
                    prop_EnableTransparentBackground = enableTransparentBackground,
                    prop_TransparentBackground = true
                };

                //// TODO XXX FIXME
                ////        // Create and configure ExportDocumentWindow.
                ////        IFactory windowFactory = Application.Current.Resources["ExportDocumentWindowFactory"] as IFactory;
                ////        Window window = windowFactory.CreateObject() as Window;
                ////        window.Owner = Application.Current.MainWindow;
                ////        window.DataContext = windowViewModel;
                ////
                ////
                ////
                ////        // Show window; return if canceled.
                ////        if (!window.ShowDialog().GetValueOrDefault())
                ////          return;

                // Deselect shapes while saving.
                List<ShapeViewModelBase> selectedItems = new List<ShapeViewModelBase>(viewModel.vm_CanvasViewModel.SelectedItem.Shapes);
                viewModel.vm_CanvasViewModel.SelectedItem.Clear();

                // Get a rectangle representing the page and wrap it in a border to allow a background color to be set.
                Border page = new Border() { Child = viewModel.mCommandUtility.GetDocumentRectangle() };

                // Use transparent or white background?
                if (!windowViewModel.prop_TransparentBackground)
                    page.Background = Brushes.White;

                // Measure and arrange.
                page.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                page.Arrange(new Rect(page.DesiredSize));

                try
                {
                    // Save the document.
                    using (FileStream fs = new FileStream(file, FileMode.Create))
                    {
                        double scaleFactor = windowViewModel.prop_Resolution / 96;
                        RenderTargetBitmap bmp = new RenderTargetBitmap((int)(page.ActualWidth * scaleFactor), (int)(page.ActualHeight * scaleFactor),
                                windowViewModel.prop_Resolution, windowViewModel.prop_Resolution, PixelFormats.Pbgra32);
                        bmp.Render(page);
                        encoder.Frames.Add(BitmapFrame.Create(bmp));
                        encoder.Save(fs);
                    }
                }
                finally
                {
                    // Reselect shapes.
                    viewModel.vm_CanvasViewModel.SelectedItem.SelectShapes(selectedItems);
                }
            }
        }

        /// <summary>
        /// Private implementation of the Print command.
        /// </summary>
        private class PrintCommandModel : CommandModel
        {
            private DocumentViewModel mViewModel;

            public PrintCommandModel(DocumentViewModel viewModel)
                : base(ApplicationCommands.Print)
            {
                mViewModel = viewModel;
                Description = Framework.Local.Strings.STR_CMD_PRINT_DESCRIPTION;
                Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Print"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Saving);
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                // Create PrintDialog
                PrintDialog dlg = new PrintDialog();

                // Get previously used PrintTicket.
                if (mViewModel.mCommandUtility.PrintTicket != null)
                    dlg.PrintTicket = mViewModel.mCommandUtility.PrintTicket;

                // Get previously used PrintQueue.
                if (mViewModel.mCommandUtility.PrintQueue != null)
                    dlg.PrintQueue = mViewModel.mCommandUtility.PrintQueue;

                // Show dialog; return if canceled.
                if (!dlg.ShowDialog().GetValueOrDefault()) return;

                // Store the PrintTicket and PrintQueue for later use.
                mViewModel.mCommandUtility.PrintTicket = dlg.PrintTicket;
                mViewModel.mCommandUtility.PrintQueue = dlg.PrintQueue;

                // Deselect shapes while printing.
                List<ShapeViewModelBase> selectedItems = new List<ShapeViewModelBase>(mViewModel.vm_CanvasViewModel.SelectedItem.Shapes);
                mViewModel.vm_CanvasViewModel.SelectedItem.Clear();

                // Print the document.
                Rectangle page = mViewModel.mCommandUtility.GetDocumentRectangle(new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
                dlg.PrintVisual(page, mViewModel.prop_DocumentFileName);

                // Reselect shapes.
                mViewModel.vm_CanvasViewModel.SelectedItem.SelectShapes(selectedItems);
            }
        }

        /// <summary>
        /// Private implementation of the Undo command.
        /// </summary>
        private class UndoCommandModel : CommandModel
        {
            private DocumentViewModel mViewModel;

            public UndoCommandModel(DocumentViewModel viewModel)
                : base(ApplicationCommands.Undo)
            {
                mViewModel = viewModel;
                Description = Framework.Local.Strings.STR_CMD_UNDO_DESCRIPTION;
                Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Undo"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = ((mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid) &&
                                                mViewModel.mDataModel.HasUndoData);

                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                mViewModel.mDataModel.Undo(mViewModel.vm_CanvasViewModel);
            }
        }

        /// <summary>
        /// Private implementation of the Redo command.
        /// </summary>
        private class RedoCommandModel : CommandModel
        {
            private DocumentViewModel mViewModel;

            public RedoCommandModel(DocumentViewModel viewModel)
                : base(ApplicationCommands.Redo)
            {
                mViewModel = viewModel;
                Description = Framework.Local.Strings.STR_CMD_REDO_DESCRIPTION;
                Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Redo"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = ((mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid) &&
                                                mViewModel.mDataModel.HasRedoData);

                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                mViewModel.mDataModel.Redo(mViewModel.vm_CanvasViewModel);
            }
        }

        /// <summary>
        /// Private implementation of the New command.
        /// </summary>
        private class NewCommandModel : CommandModel
        {
            private DocumentViewModel mViewModel;

            public NewCommandModel(DocumentViewModel viewModel)
                : base(ApplicationCommands.New)
            {
                mViewModel = viewModel;
                Name = Framework.Local.Strings.STR_CMD_CreateNew;
                Description = Framework.Local.Strings.STR_CMD_CreateNewDocument;
                Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.New"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid);

                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                // Save changes before closing current document?
                if (mViewModel.QuerySaveChanges() == false)
                    return;

                Size currentPageSize = mViewModel.dm_DocumentDataModel.PageSize;

                Thickness currentPageMargins = mViewModel.dm_DocumentDataModel.PageMargins;

                // Create NewDocumentWindow 
                IFactory newDocumentWindowFactory = Application.Current.Resources["NewDocumentWindowFactory"] as IFactory;
                Window newDocumentWindow = newDocumentWindowFactory.CreateObject() as Window;

                // Configure window.
                PageViewModelBase newDocumentWindowViewModel = new PageViewModelBase()
                {
                    prop_PageSize = currentPageSize,
                    prop_PageMargins = currentPageMargins
                };
                newDocumentWindow.DataContext = newDocumentWindowViewModel;
                newDocumentWindow.Owner = Application.Current.MainWindow;

                // Show window; return if canceled.
                if (!newDocumentWindow.ShowDialog().GetValueOrDefault())
                    return;

                // Create document.
                newDocumentWindow.DataContext = null;
                mViewModel.prop_DocumentFilePath = null;
                mViewModel.vm_CanvasViewModel.SelectedItem.Clear();
                mViewModel.mDataModel.New(newDocumentWindowViewModel);
            }
        }

        /// <summary>
        /// Private implementation of the Open command.
        /// </summary>
        private class OpenCommandModel : CommandModel
        {
            private DocumentViewModel mViewModel;

            public OpenCommandModel(DocumentViewModel viewModel)
                : base(ApplicationCommands.Open)
            {
                mViewModel = viewModel;
                Description = Framework.Local.Strings.STR_CMD_OpenDocument;
                Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Open"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid);

                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                // Save changes before closing current document?
                if (mViewModel.QuerySaveChanges() == false)
                    return;

                // Create and configure OpenFileDialog.
                FileDialog dlg = new OpenFileDialog()
                {
                    Filter = Framework.Local.Strings.STR_FILETYPE_FILTER,
                    DefaultExt = "xml",
                    AddExtension = true,
                    ValidateNames = true,
                    CheckFileExists = true
                };

                // Show dialog; return if canceled.
                if (!dlg.ShowDialog(Application.Current.MainWindow).GetValueOrDefault()) return;

                try
                {
                    // Open the document.
                    mViewModel.LoadFile(dlg.FileName);
                }
                catch (Exception ex)
                {
                    var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                    msgBox.Show(ex, string.Format(Framework.Local.Strings.STR_OpenFILE_MSG, dlg.FileName),
                                Framework.Local.Strings.STR_OpenFILE_MSG_CAPTION);
                }
            }
        }
        #endregion private classes
    }
}

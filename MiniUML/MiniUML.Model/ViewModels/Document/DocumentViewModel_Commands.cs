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
    using Edi.Core;
    using Microsoft.Win32;
    using MiniUML.Framework;
    using MiniUML.Model.ViewModels.Shapes;
    using MsgBox;

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

            public CommandUtility(DocumentViewModel viewModel, IMessageBoxService msgBox)
            {
                this.mViewModel = viewModel;

                // Initialize commands.
                viewModel.cmd_New = new NewCommandModel(viewModel, msgBox);
                viewModel.cmd_Open = new OpenCommandModel(viewModel, msgBox);
                viewModel.cmd_Save = new SaveCommandModel(viewModel, msgBox);
                viewModel.cmd_SaveAs = new SaveAsCommandModel(viewModel, msgBox);
                viewModel.cmd_Export = new ExportCommandModel(viewModel, msgBox);
                viewModel.cmd_Print = new PrintCommandModel(viewModel);
                viewModel.cmd_Undo = new UndoCommandModel(viewModel);
                viewModel.cmd_Redo = new RedoCommandModel(viewModel);
            }

            public PrintTicket PrintTicket { get; set; }

            public PrintQueue PrintQueue { get; set; }

            public Rectangle GetDocumentRectangle(Rect maxBounds = default(Rect))
            {
                FrameworkElement canvasView = this.mViewModel.v_CanvasView;
                Size size = default(Size);

                // Create a VisualBrush representing the contents of the  document
                // and use it to paint a rectangle of the same size as the page.
                if (maxBounds == default(Rect))
                    size = new Size(canvasView.ActualWidth, canvasView.ActualHeight);
                else
                    size = new Size(maxBounds.Width, maxBounds.Height);

                Rectangle rect = new Rectangle()
                {
                    Width = size.Width,
                    Height = size.Height,
                    Fill = new VisualBrush(canvasView) { TileMode = TileMode.None }
                };

                // Measure and arrange.
                rect.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                rect.Arrange(new Rect(new Size(size.Width, size.Height)));

                return rect;
            }

            public Rectangle GetDocumentRectangle(Size desiredSize)
            {
                FrameworkElement canvasView = this.mViewModel.v_CanvasView;

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
            #region fields
            private DocumentViewModel mViewModel;
            private readonly IMessageBoxService _MsgBox;
            #endregion fields

            public SaveCommandModel(DocumentViewModel viewModel, IMessageBoxService msgBox)
                : base(ApplicationCommands.Save)
            {
                _MsgBox = msgBox;
                this.mViewModel = viewModel;
                this.Description = MiniUML.Framework.Local.Strings.STR_CMD_SAVE_DOCUMENT;
                this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Save"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                this.Execute();
            }

            public bool Execute()
            {
                string file = this.mViewModel.prop_DocumentFilePath;

                try
                {
                    if (File.Exists(file))
                    {
                        // Save document to the existing file.
                        this.mViewModel._DataModel.Save(file);
                        return true;
                    }
                    else
                    {
                        // Execute SaveAs command.
                        return ((SaveAsCommandModel)this.mViewModel.cmd_SaveAs).Execute();
                    }
                }
                catch (Exception ex)
                {
                    _MsgBox.Show(ex, string.Format(MiniUML.Framework.Local.Strings.STR_SaveFILE_MSG, file),
                                MiniUML.Framework.Local.Strings.STR_SaveFILE_MSG_CAPTION);
                }

                return false;
            }
        }

        /// <summary>
        /// Private implementation of the Save As command.
        /// </summary>
        private class SaveAsCommandModel : CommandModel
        {
            #region fields
            private DocumentViewModel mViewModel;
            private readonly IMessageBoxService _MsgBox;
            #endregion fields

            public SaveAsCommandModel(DocumentViewModel viewModel, IMessageBoxService msgBox)
                : base(ApplicationCommands.SaveAs)
            {
                _MsgBox = msgBox;
                this.mViewModel = viewModel;
                this.Description = MiniUML.Framework.Local.Strings.STR_CMD_SAVEAS_DOCUMENT;
                this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.SaveAs"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                this.Execute();
            }

            public bool Execute()
            {
                // Create and configure SaveFileDialog.
                FileDialog dlg = new SaveFileDialog()
                {
                    Filter = MiniUML.Framework.Local.Strings.STR_FILETYPE_FILTER_SAVE,
                    AddExtension = true,
                    ValidateNames = true
                };

                // Show dialog; return if canceled.
                if (!dlg.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
                    return false;

                try
                {
                    // Save document.
                    this.mViewModel._DataModel.Save(dlg.FileName);
                    this.mViewModel.prop_DocumentFilePath = dlg.FileName;

                    return true;
                }
                catch (Exception ex)
                {
                    _MsgBox.Show(ex, string.Format(MiniUML.Framework.Local.Strings.STR_SaveFILE_MSG, dlg.FileName),
                                MiniUML.Framework.Local.Strings.STR_SaveFILE_MSG_CAPTION);
                }

                return false;
            }
        }

        /// <summary>
        /// Private implementation of the Export command.
        /// </summary>
        private class ExportCommandModel : CommandModel
        {
            #region fields
            private DocumentViewModel mViewModel;
            private readonly IMessageBoxService _MsgBox;
            #endregion fields

            public ExportCommandModel(DocumentViewModel viewModel, IMessageBoxService msgBox)
            {
                _MsgBox = msgBox;
                this.mViewModel = viewModel;
                this.Name = MiniUML.Framework.Local.Strings.STR_CMD_Export_Command;
                this.Description = MiniUML.Framework.Local.Strings.STR_CMD_Export_Command_DESCR;
                this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Export"];
            }

            /// <summary>
            /// Save an exported image document (eg: png) into the file system.
            /// </summary>
            /// <param name="viewModel"></param>
            /// <param name="defaultFileName"></param>
            public static void ExportUMLToImage(DocumentViewModel viewModel,
                                                string defaultFileName = "")
            {
                // Create and configure SaveFileDialog.
                FileDialog dlg = new SaveFileDialog()
                {
                    ValidateNames = true,
                    AddExtension = true,
                    Filter = MiniUML.Framework.Local.Strings.STR_FILETYPE_FILTER_EXPORT,
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
                    StaticServices.MsgBox.Show(ex, string.Format(MiniUML.Framework.Local.Strings.STR_SaveFILE_MSG, dlg.FileName),
                                               MiniUML.Framework.Local.Strings.STR_SaveFILE_MSG_CAPTION);
                }
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                ExportUMLToImage(this.mViewModel);
            }

            private static void saveAsXPS(string file, DocumentViewModel viewModel)
            {
                // Deselect shapes while saving.
                List<ShapeViewModelBase> selectedItems = new List<ShapeViewModelBase>(viewModel.vm_CanvasViewModel.SelectedItem.Shapes);
                viewModel.vm_CanvasViewModel.SelectedItem.Clear();

                // Get a rectangle representing the page.
                Rectangle page = viewModel._CommandUtility.GetDocumentRectangle();

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
                ExportDocumentWindowViewModel windowViewModel =
                    new ExportDocumentWindowViewModel(96,enableTransparentBackground, true);

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
                var visual = viewModel._CommandUtility.GetDocumentRectangle(default(Rect));
                Border page = new Border() { Child = visual };

                // Use transparent or white background?
                if (!windowViewModel.prop_TransparentBackground)
                    page.Background = Brushes.White;

                // Measure and arrange.
                page.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                page.Arrange(new Rect(page.DesiredSize));

                // Clip to this region of visible content
                var bounds = viewModel.GetMaxBounds(new Rect(0,0,12,12));

                try
                {
                    // Save the document.
                    using (FileStream fs = new FileStream(file, FileMode.Create))
                    {
                        RenderTargetBitmap bmp = new RenderTargetBitmap(
                            (int)(bounds.Width),
                            (int)(bounds.Height),
                                windowViewModel.prop_Resolution,
                                windowViewModel.prop_Resolution, PixelFormats.Pbgra32);

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
                this.mViewModel = viewModel;
                this.Description = MiniUML.Framework.Local.Strings.STR_CMD_PRINT_DESCRIPTION;
                this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Print"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Saving);
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                // Create PrintDialog
                PrintDialog dlg = new PrintDialog();

                // Get previously used PrintTicket.
                if (this.mViewModel._CommandUtility.PrintTicket != null)
                    dlg.PrintTicket = this.mViewModel._CommandUtility.PrintTicket;

                // Get previously used PrintQueue.
                if (this.mViewModel._CommandUtility.PrintQueue != null)
                    dlg.PrintQueue = this.mViewModel._CommandUtility.PrintQueue;

                // Show dialog; return if canceled.
                if (!dlg.ShowDialog().GetValueOrDefault()) return;

                // Store the PrintTicket and PrintQueue for later use.
                this.mViewModel._CommandUtility.PrintTicket = dlg.PrintTicket;
                this.mViewModel._CommandUtility.PrintQueue = dlg.PrintQueue;

                // Deselect shapes while printing.
                List<ShapeViewModelBase> selectedItems = new List<ShapeViewModelBase>(this.mViewModel.vm_CanvasViewModel.SelectedItem.Shapes);
                this.mViewModel.vm_CanvasViewModel.SelectedItem.Clear();

                // Print the document.
                Rectangle page = this.mViewModel._CommandUtility.GetDocumentRectangle(new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
                dlg.PrintVisual(page, this.mViewModel.prop_DocumentFileName);

                // Reselect shapes.
                this.mViewModel.vm_CanvasViewModel.SelectedItem.SelectShapes(selectedItems);
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
                this.mViewModel = viewModel;
                this.Description = MiniUML.Framework.Local.Strings.STR_CMD_UNDO_DESCRIPTION;
                this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Undo"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = ((this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid) &&
                                                this.mViewModel._DataModel.HasUndoData);

                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                this.mViewModel._DataModel.Undo(this.mViewModel.vm_CanvasViewModel);
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
                this.mViewModel = viewModel;
                this.Description = MiniUML.Framework.Local.Strings.STR_CMD_REDO_DESCRIPTION;
                this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Redo"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = ((this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid) &&
                                                this.mViewModel._DataModel.HasRedoData);

                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                this.mViewModel._DataModel.Redo(this.mViewModel.vm_CanvasViewModel);
            }
        }

        /// <summary>
        /// Private implementation of the New command.
        /// </summary>
        private class NewCommandModel : CommandModel
        {
            private readonly IMessageBoxService _MsgBox;
            private DocumentViewModel mViewModel;

            public NewCommandModel(DocumentViewModel viewModel, IMessageBoxService msgBox)
                : base(ApplicationCommands.New)
            {
                _MsgBox = msgBox;
                this.mViewModel = viewModel;
                this.Name = MiniUML.Framework.Local.Strings.STR_CMD_CreateNew;
                this.Description = MiniUML.Framework.Local.Strings.STR_CMD_CreateNewDocument;
                this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.New"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid);

                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                // Save changes before closing current document?
                if (this.mViewModel.QuerySaveChanges() == false)
                    return;

                Size currentPageSize = this.mViewModel.dm_DocumentDataModel.PageSize;

                Thickness currentPageMargins = this.mViewModel.dm_DocumentDataModel.PageMargins;

                // Create NewDocumentWindow 
                IFactory newDocumentWindowFactory = Application.Current.Resources["NewDocumentWindowFactory"] as IFactory;
                Window newDocumentWindow = newDocumentWindowFactory.CreateObject(_MsgBox) as Window;

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
                this.mViewModel.prop_DocumentFilePath = null;
                this.mViewModel.vm_CanvasViewModel.SelectedItem.Clear();
                this.mViewModel._DataModel.New(newDocumentWindowViewModel);
            }
        }

        /// <summary>
        /// Private implementation of the Open command.
        /// </summary>
        private class OpenCommandModel : CommandModel
        {
            #region fields
            private DocumentViewModel mViewModel;
            private readonly IMessageBoxService _MsgBox;
            #endregion fields

            public OpenCommandModel(DocumentViewModel viewModel, IMessageBoxService msgBox)
                : base(ApplicationCommands.Open)
            {
                _MsgBox = msgBox;
                this.mViewModel = viewModel;
                this.Description = MiniUML.Framework.Local.Strings.STR_CMD_OpenDocument;
                this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Open"];
            }

            public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                                                this.mViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid);

                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                // Save changes before closing current document?
                if (this.mViewModel.QuerySaveChanges() == false)
                    return;

                // Create and configure OpenFileDialog.
                FileDialog dlg = new OpenFileDialog()
                {
                    Filter = MiniUML.Framework.Local.Strings.STR_FILETYPE_FILTER,
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
                    this.mViewModel.LoadFile(dlg.FileName);
                }
                catch (Exception ex)
                {
                    _MsgBox.Show(ex, string.Format(MiniUML.Framework.Local.Strings.STR_OpenFILE_MSG, dlg.FileName),
                                 MiniUML.Framework.Local.Strings.STR_OpenFILE_MSG_CAPTION);
                }
            }
        }
        #endregion private classes
    }
}

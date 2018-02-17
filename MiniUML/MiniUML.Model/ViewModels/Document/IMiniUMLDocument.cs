namespace MiniUML.Model.ViewModels.Document
{
  /// <summary>
  /// Define an interface to reference to a MiniUML Document.
  /// This interface is used to reference MiniUML documents in any MiniUML PLugIn.
  /// </summary>
  public interface IMiniUMLDocument
  {
    AbstractDocumentViewModel vm_DocumentViewModel { get; }
  }
}

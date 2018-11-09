namespace MiniUML.Framework
{
    using MsgBox;

    public interface IFactory
    {
        object CreateObject(IMessageBoxService msgBox);
    }
}

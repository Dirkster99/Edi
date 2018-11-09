namespace Edi.Core
{
    using MsgBox;

    public static class StaticServices
    {
        private static IMessageBoxService _MsgBox = null;
        private static object _LockObject = new object();

        public static IMessageBoxService MsgBox
        {
            get
            {
                lock (_LockObject)
                {
                    if (_MsgBox == null)
                        _MsgBox = new MessageBoxService();
                }

                return _MsgBox;
            }
        }
    }
}

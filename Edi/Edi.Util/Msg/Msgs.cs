namespace Edi.Util.Msg
{
    using System.Collections.Generic;

    /// <summary>
    /// This class hosts a collection of (error) message objects
    /// to enable summary reports.
    /// </summary>
    public class Msgs
    {
        #region Fields
        private readonly object _syncRoot = new object();

        private List<Msg> _msgs = new List<Msg>();
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Standard constructor
        /// </summary>
        public Msgs()
        {
            _msgs = new List<Msg>();
        }

        /// <summary>
        /// Copy constructor of the <seealso cref="ToolMsgs"/> class
        /// </summary>
        /// <param name="inTMs"></param>
        public Msgs(Msgs inTMs)
        {
            _msgs = new List<Msg>(inTMs._msgs);
        }

        /// <summary>
        /// Standard (convinience) constructor of the <seealso cref="ToolMsgs"/> class
        /// </summary>
        public Msgs(Msg te)
        {
            _msgs.Add(te);
        }
        #endregion Constructors

        #region properties
        /// <summary>
        /// Count number of messages retrieved
        /// </summary>
        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_msgs == null)
                        return 0;

                    return _msgs.Count;
                }
            }
        }

        public List<Msg> Messages
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_msgs == null)
                        return new List<Msg>();

                    return new List<Msg>(_msgs);
                }
            }
        }
        #endregion properties

        #region Methods
        public void Add(Msg tm)
        {
            lock (_syncRoot)
            {
                if (tm == null)
                    return;

                if (_msgs == null)
                    _msgs = new List<Msg>();

                _msgs.Add(tm);
            }
        }

        public void Add(string msg, Msg.MsgCategory cat)
        {
            lock (_syncRoot)
            {
                if (_msgs == null)
                    _msgs = new List<Msg>();

                _msgs.Add(new Msg(msg, cat));
            }
        }

        public void Add(Msgs msgColl)
        {
            lock (_syncRoot)
            {
                if (msgColl == null)
                    return;

                if (_msgs == null)
                    _msgs = new List<Msg>();

                foreach (var item in msgColl._msgs)
                {
                    _msgs.Add(new Msg(item));
                }
            }
        }
        #endregion Methods
    }
}

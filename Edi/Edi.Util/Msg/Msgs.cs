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
            this._msgs = new List<Msg>();
        }

        /// <summary>
        /// Copy constructor of the <seealso cref="ToolMsgs"/> class
        /// </summary>
        /// <param name="inTMs"></param>
        public Msgs(Msgs inTMs)
        {
            this._msgs = new List<Msg>(inTMs._msgs);
        }

        /// <summary>
        /// Standard (convinience) constructor of the <seealso cref="ToolMsgs"/> class
        /// </summary>
        public Msgs(Msg te)
        {
            this._msgs.Add(te);
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
                lock (this._syncRoot)
                {
                    if (this._msgs == null)
                        return 0;

                    return this._msgs.Count;
                }
            }
        }

        public List<Msg> Messages
        {
            get
            {
                lock (this._syncRoot)
                {
                    if (this._msgs == null)
                        return new List<Msg>();

                    return new List<Msg>(this._msgs);
                }
            }
        }
        #endregion properties

        #region Methods
        public void Add(Msg tm)
        {
            lock (this._syncRoot)
            {
                if (tm == null)
                    return;

                if (this._msgs == null)
                    this._msgs = new List<Msg>();

                this._msgs.Add(tm);
            }
        }

        public void Add(string msg, Msg.MsgCategory cat)
        {
            lock (this._syncRoot)
            {
                if (this._msgs == null)
                    this._msgs = new List<Msg>();

                this._msgs.Add(new Msg(msg, cat));
            }
        }

        public void Add(Msgs msgColl)
        {
            lock (this._syncRoot)
            {
                if (msgColl == null)
                    return;

                if (this._msgs == null)
                    this._msgs = new List<Msg>();

                foreach (var item in msgColl._msgs)
                {
                    this._msgs.Add(new Msg(item));
                }
            }
        }
        #endregion Methods
    }
}

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
        private readonly object syncRoot = new object();

        private List<Msg> msgs = new List<Msg>();
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Standard constructor
        /// </summary>
        public Msgs()
        {
            this.msgs = new List<Msg>();
        }

        /// <summary>
        /// Copy constructor of the <seealso cref="ToolMsgs"/> class
        /// </summary>
        /// <param name="inTMs"></param>
        public Msgs(Msgs inTMs)
        {
            this.msgs = new List<Msg>(inTMs.msgs);
        }

        /// <summary>
        /// Standard (convinience) constructor of the <seealso cref="ToolMsgs"/> class
        /// </summary>
        public Msgs(Msg te)
        {
            this.msgs.Add(te);
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
                lock (this.syncRoot)
                {
                    if (this.msgs == null)
                        return 0;

                    return this.msgs.Count;
                }
            }
        }

        public List<Msg> Messages
        {
            get
            {
                lock (this.syncRoot)
                {
                    if (this.msgs == null)
                        return new List<Msg>();

                    return new List<Msg>(this.msgs);
                }
            }
        }
        #endregion properties

        #region Methods
        public void Add(Msg tm)
        {
            lock (this.syncRoot)
            {
                if (tm == null)
                    return;

                if (this.msgs == null)
                    this.msgs = new List<Msg>();

                this.msgs.Add(tm);
            }
        }

        public void Add(string Msg, Msg.MsgCategory cat)
        {
            lock (this.syncRoot)
            {
                if (this.msgs == null)
                    this.msgs = new List<Msg>();

                this.msgs.Add(new Msg(Msg, cat));
            }
        }

        public void Add(Msgs msgColl)
        {
            lock (this.syncRoot)
            {
                if (msgColl == null)
                    return;

                if (this.msgs == null)
                    this.msgs = new List<Msg>();

                foreach (var item in msgColl.msgs)
                {
                    this.msgs.Add(new Msg(item));
                }
            }
        }
        #endregion Methods
    }
}

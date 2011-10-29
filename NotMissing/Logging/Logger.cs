using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NotMissing.Logging
{
    public class Logger : ILogParent
    {
        static readonly Logger instance = new Logger();
        static public Logger Instance
        {
            get { return instance; }
        }

        protected bool InSync = false;
        protected readonly object SyncRoot = new object();
        public List<ILogListener> Listeners = new List<ILogListener>();

        void InSyncCheck()
        {
            if (InSync)
                throw new NotSupportedException("Calling Register/Log inside a Log call is not allowed");
        }

        public virtual void Register(ILogListener listener)
        {
            InSyncCheck();
            lock (SyncRoot)
            {
                InSync = true;
                if (Listeners.Contains(listener))
                    return;
                listener.Parents.Add(this);
                Listeners.Add(listener);
                InSync = false;
            }
        }
        public virtual void Unregister(ILogListener listener)
        {
            InSyncCheck();
            lock (SyncRoot)
            {
                InSync = true;
                if (!Listeners.Contains(listener))
                    return;
                Listeners.Remove(listener);
                InSync = false;
            }
        }

        public virtual void Log(Levels level, object obj)
        {
            InSyncCheck();
            lock (SyncRoot)
            {
                InSync = true;
                foreach (var listener in Listeners)
                {
                    if ((listener.Level & level) != 0 && listener.LogFunc!=null)
                    {
                        listener.LogFunc(obj);
                    }
                }
                InSync = false;
            }
        }

        public virtual void Trace(object obj) { Log(Levels.Trace, obj); }
        public virtual void Debug(object obj) { Log(Levels.Debug, obj); }
        public virtual void Warning(object obj) { Log(Levels.Warning, obj); }
        public virtual void Info(object obj) { Log(Levels.Info, obj); }
        public virtual void Error(object obj) { Log(Levels.Error, obj); }
        public virtual void Fatal(object obj) { Log(Levels.Fatal, obj); }
    }
}

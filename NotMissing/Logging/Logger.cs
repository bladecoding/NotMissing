using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NotMissing.Logging
{
    public class Logger : ILogParent
    {
        readonly Logger instance = new Logger();
        public Logger Instance
        {
            get { return instance; }
        }

        bool InSync = false;
        readonly object SyncRoot = new object();
        public List<ILogListener> Listeners = new List<ILogListener>();

        void InSyncCheck()
        {
            if (InSync)
                throw new NotSupportedException("Calling Register/Log inside a Log call is not allowed");
        }

        public void Register(ILogListener listener)
        {
            InSyncCheck();
            lock (SyncRoot)
            {
                InSync = true;
                listener.Parents.Add(this);
                Listeners.Add(listener);
                InSync = false;
            }
        }
        public void Unregister(ILogListener listener)
        {
            InSyncCheck();
            lock (SyncRoot)
            {
                InSync = true;
                Listeners.Remove(listener);
                InSync = false;
            }
        }

        public void Log(Levels level, object obj)
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

        public void Trace(object obj) { Log(Levels.Trace, obj); }
        public void Debug(object obj) { Log(Levels.Debug, obj); }
        public void Warning(object obj) { Log(Levels.Warning, obj); }
        public void Info(object obj) { Log(Levels.Info, obj); }
        public void Error(object obj) { Log(Levels.Error, obj); }
        public void Fatal(object obj) { Log(Levels.Fatal, obj); }
    }
}

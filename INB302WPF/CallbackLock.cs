using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace INB302WPF
{
    class CallbackLock : IDisposable
    {
        private readonly object lockObject;

        public CallbackLock(object lockObject)
        {
            this.lockObject = lockObject;
            Monitor.Enter(lockObject);
        }

        public delegate void LockExitEventHandler();

        public event LockExitEventHandler LockExit;

        public void Dispose()
        {
            Monitor.Exit(lockObject);
            if (LockExit != null)
            {
                LockExit();
            }
        }
    }
}

namespace FolderBrowser.Semaphores
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Semaphores are great for throttling and resource management.  You can give a semaphore an
    /// initial count of the number of things to protect, and then it’ll only allow that many consumers
    /// to successfully acquire the semaphore, forcing all others to wait until a resource is freed up
    /// and count on the semaphore is released.  That resource to protect could be the right to enter
    /// a particular region of code, and the count could be set to 1: in this way, you can use a
    /// semaphore to achieve mutual exclusion.
    /// 
    /// private readonly AsyncSemaphore m_lock = new AsyncSemaphore(1); 
    /// … 
    /// await m_lock.WaitAsync(); 
    /// try 
    /// { 
    ///     … // protected code here 
    /// }  
    /// finally { m_lock.Release(); }
    /// 
    /// Source: http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx
    /// </summary>
    public class AsyncSemaphore
    {
        private readonly static Task s_completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();
        private int m_currentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            m_currentCount = initialCount;
        }

        public Task WaitAsync()
        {
            lock (m_waiters)
            {
                if (m_currentCount > 0)
                {
                    --m_currentCount;
                    return s_completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    m_waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waiters)
            {
                if (m_waiters.Count > 0)
                    toRelease = m_waiters.Dequeue();
                else
                    ++m_currentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
}

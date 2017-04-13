/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-3-31
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Services;

namespace OpenIZ.Mobile.Core.Xamarin.Threading
{
    /// <summary>
    /// Represents a thread pool which is implemented separately from the default .net
    /// threadpool, this is to reduce the load on the .net framework thread pool
    /// </summary>
    public class OpenIZThreadPool : IThreadPoolService, IDisposable
    {
        
        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(OpenIZThreadPool));
        // Number of threads to keep alive
        private int m_concurrencyLevel = System.Environment.ProcessorCount * 4;
        // Queue of work items
        private Queue<WorkItem> m_queue = null;
        private Queue<WorkItem> m_priorityQueue = null;
        // Timers
        private List<Timer> m_timers = new List<Timer>();
        // Non queue operations
        private List<Thread> m_nonQueueOperations = new List<Thread>();

        // Active threads
        private Thread[] m_threadPool = null;
        // Hint of the number of threads waiting to be executed
        private int m_threadWait = 0;
        // True when the thread pool is being disposed
        private bool m_disposing = false;

        /// <summary>
        /// Concurrency
        /// </summary>
        public int Concurrency { get { return this.m_concurrencyLevel; } }

        /// <summary>
        /// Waiting threads
        /// </summary>
        public int WaitingThreads { get { return this.m_queue.Count + this.m_priorityQueue.Count; } }

        /// <summary>
        /// Active timers
        /// </summary>
        public int ActiveTimers { get { return this.m_timers.Count; } }

        /// <summary>
        /// Non queue threads
        /// </summary>
        public int NonQueueThreads { get { return this.m_nonQueueOperations.Count; } }
        
        /// <summary>
        /// Active threads
        /// </summary>
        public List<String> Threads {
            get
            {
                return this.m_threadPool.Select(o => o.Name).Union(this.m_timers.Select(o => "Timer")).Union(this.m_nonQueueOperations.Select(o => $"=>[NQ]=>{o.Name}")).ToList();
            }
        }

        /// <summary>
        /// Active threads
        /// </summary>
        public int ActiveThreads { get { return this.m_concurrencyLevel - this.m_threadWait; } }

        /// <summary>
        /// Creates a new instance of the wait thread pool
        /// </summary>
        public OpenIZThreadPool() 
        {
            if (ApplicationContext.Current?.Configuration?.GetSection<ApplicationConfigurationSection>()?.AppSettings?.Find(o => o.Key == "queue_process_concurrency") != null)
                this.m_concurrencyLevel = Int32.Parse(ApplicationContext.Current?.Configuration?.GetSection<ApplicationConfigurationSection>()?.AppSettings?.Find(o => o.Key == "queue_process_concurrency").Value);
            this.m_queue = new Queue<WorkItem>(this.m_concurrencyLevel);
            this.m_priorityQueue = new Queue<WorkItem>();
        }

        /// <summary>
        /// Worker data structure
        /// </summary>
        private struct WorkItem
        {
            /// <summary>
            /// The callback to execute on the worker
            /// </summary>
            public Action<Object> Callback { get; set; }
            /// <summary>
            /// The state or parameter to the worker
            /// </summary>
            public object State { get; set; }
            /// <summary>
            /// The execution context
            /// </summary>
            public ExecutionContext ExecutionContext { get; set; }
        }

        // Number of remaining work items
        private int m_remainingWorkItems = 1;
        // Thread is done reset event
        private ManualResetEvent m_threadDoneResetEvent = new ManualResetEvent(false);

        /// <summary>
        /// Queue a work item to be completed
        /// </summary>
        public void QueueUserWorkItem(TimeSpan timeout, Action<Object> callback, Object parm)
        {
            if (timeout == TimeSpan.MinValue)
                this.QueueWorkItemInternal(callback, parm, true);
            else
            {
                Timer timer = null;
                timer = new Timer((o) =>
                {
                    var kv = (KeyValuePair<Action<Object>, Object>)o;
                    this.QueueUserWorkItem(kv.Key, kv.Value);
                    timer.Dispose();
                    lock (this.m_timers)
                        this.m_timers.Remove(timer);
                }, new KeyValuePair<Action<Object>, Object>(callback, parm), (int)timeout.TotalMilliseconds, Timeout.Infinite);
                lock (this.m_timers)
                    this.m_timers.Add(timer);
            }
        }

        /// <summary>
        /// Queue a work item to be completed
        /// </summary>
        public void QueueUserWorkItem(Action<Object> callback)
        {
            QueueUserWorkItem(callback, null);
        }

        /// <summary>
        /// Queue a user work item with the specified parameters
        /// </summary>
        public void QueueUserWorkItem(Action<Object> callback, object state)
        {
            this.QueueWorkItemInternal(callback, state);
        }

        /// <summary>
        /// Queues a non-pooled work item
        /// </summary>
        public void QueueNonPooledWorkItem(Action<object> action, object parm)
        {
            Thread thd = new Thread(new ParameterizedThreadStart((o) =>
            {
                try
                {
                    action(o);
                }
                catch (Exception e) { this.m_tracer.TraceError("!!!!!! 0118 999 881 999 119 7253 : THREAD DEATH !!!!!!!\r\nUncaught Exception on worker thread: {0}", e); }
            }
            ));
            thd.IsBackground = true;
            thd.Name = $"OpenIZBackground-{action}";
            thd.Start(parm);
        }

        /// <summary>
        /// Perform queue of workitem internally
        /// </summary>
        private void QueueWorkItemInternal(Action<Object> callback, object state, bool isPriority = false)
        {
            ThrowIfDisposed();

            try
            {
                WorkItem wd = new WorkItem()
                {
                    Callback = callback,
                    State = state,
                    ExecutionContext = ExecutionContext.Capture()
                };
                lock (this.m_threadDoneResetEvent) this.m_remainingWorkItems++;
                this.EnsureStarted(); // Ensure thread pool threads are started
                lock (m_queue)
                {
                    if (!isPriority)
                        m_queue.Enqueue(wd);
                    else // priority items get inserted at the head so that they are executed first
                        this.m_priorityQueue.Enqueue(wd);
                                        
                    if (m_threadWait > 0)
                        Monitor.Pulse(m_queue);
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error queueing work item: {0}", e);
            }
        }

        /// <summary>
        /// Ensure the thread pool threads are started
        /// </summary>
        private void EnsureStarted()
        {
            if (m_threadPool == null)
            {
                lock (m_queue)
                    if (m_threadPool == null)
                    {
                        m_threadPool = new Thread[m_concurrencyLevel];
                        for (int i = 0; i < m_threadPool.Length; i++)
                        {
                            m_threadPool[i] = new Thread(DispatchLoop);
                            m_threadPool[i].Name = String.Format("OIZ-ThreadPoolThread-{0}", i);
                            m_threadPool[i].IsBackground = true;
                            m_threadPool[i].Start();
                        }
                    }
            }
        }

        /// <summary>
        /// Dispatch loop
        /// </summary>
        private void DispatchLoop()
        {
            while (true)
            {
                WorkItem wi = default(WorkItem);
                lock (m_queue)
                {
                    try
                    {
                        if (m_disposing) return; // Shutdown requested
                        while (m_queue.Count == 0 && m_priorityQueue.Count == 0)
                        {
                            m_threadWait++;
                            try { Monitor.Wait(m_queue); }
                            finally { m_threadWait--; }
                            if (m_disposing)
                                return;
                        }
                        if (this.m_priorityQueue.Count > 0)
                            wi = this.m_priorityQueue.Dequeue();
                        else
                            wi = m_queue.Dequeue();
                    }
                    catch(Exception e)
                    {
                        this.m_tracer.TraceError("Error in dispatchloop {0}", e);
                    }
                }
                DoWorkItem(wi);
            }
        }


        /// <summary>
        /// Wait until the thread is complete
        /// </summary>
        /// <returns></returns>
        public bool WaitOne() { return WaitOne(-1, false); }

        /// <summary>
        /// Wait until the thread is complete or the specified timeout elapses
        /// </summary>
        public bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            return WaitOne((int)timeout.TotalMilliseconds, exitContext);
        }

        /// <summary>
        /// Wait until the thread is completed
        /// </summary>
        public bool WaitOne(int timeout, bool exitContext)
        {
            ThrowIfDisposed();
            DoneWorkItem();
            bool rv = this.m_threadDoneResetEvent.WaitOne(timeout, exitContext);
            lock (this.m_threadDoneResetEvent)
            {
                if (rv)
                {
                    this.m_remainingWorkItems = 1;
                    this.m_threadDoneResetEvent.Reset();
                }
                else this.m_remainingWorkItems++;
            }
            return rv;
        }

        /// <summary>
        /// Perform the work if the specified work data
        /// </summary>
        private void DoWorkItem(WorkItem state)
        {
            this.m_tracer.TraceVerbose("Starting task on {0} ---> {1}", Thread.CurrentThread.Name, state.Callback.Target.ToString());
            AuthenticationContext.Current = new AuthenticationContext(ApplicationContext.Current.ThreadDefaultPrincipal) ?? new AuthenticationContext(AuthenticationContext.AnonymousPrincipal);
            var worker = (WorkItem)state;
            try {
                worker.Callback(worker.State);
            }
            catch(Exception e) { this.m_tracer.TraceError("!!!!!! 0118 999 881 999 119 7253 : THREAD DEATH !!!!!!!\r\nUncaught Exception on worker thread: {0}", e); }
            finally {
                DoneWorkItem();
            }
        }

        /// <summary>
        /// Complete a workf item
        /// </summary>
        private void DoneWorkItem()
        {
            lock (this.m_threadDoneResetEvent)
            {
                --this.m_remainingWorkItems;
                if (this.m_remainingWorkItems == 0) this.m_threadDoneResetEvent.Set();
            }
        }

        /// <summary>
        /// Throw an exception if the object is disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.m_threadDoneResetEvent == null) throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            if (this.m_threadDoneResetEvent != null)
            {
                if (this.m_remainingWorkItems > 0)
                    this.WaitOne();

                ((IDisposable)m_threadDoneResetEvent).Dispose();
                this.m_threadDoneResetEvent = null;
                m_disposing = true;
                lock (m_queue)
                    Monitor.PulseAll(m_queue);

                if (m_threadPool != null)
                    for (int i = 0; i < m_threadPool.Length; i++)
                    {
                        m_threadPool[i].Join();
                        m_threadPool[i] = null;
                    }
            }
        }

        #endregion

    }
}
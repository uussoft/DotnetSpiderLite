﻿using DotnetSpiderLite;
using DotnetSpiderLite.Logs;
using DotnetSpiderLite.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetSpiderLite.Infrastructure;
using System.Runtime.CompilerServices;

namespace DotnetSpiderLite.Scheduler
{
    /// <summary>
    ///  简单队列
    /// </summary>
    public class SampleQueueScheduler : IScheduler, ISchedulerMonitor, ISchedulerDuplicateRemover
    {
        private HashSet<string> _allUrls = new HashSet<string>();
        private ConcurrentQueue<Request> _queue = new ConcurrentQueue<Request>();
        private readonly object _lock = new object();
        private long _count = 0;

        private AutomicLong _successCount = new AutomicLong(0);
        private AutomicLong _errorCount = new AutomicLong(0);

        public ILogger Logger { get; set; }

        public long LeftRequestsCount
        {
            get
            {
                lock (_lock)
                {
                    return _queue.Count;
                }
            }
        }

        public long TotalRequestsCount => _count;

        public long SuccessRequestsCount => _successCount.Value;

        public long ErrorRequestsCount => _errorCount.Value;

        public Request Pull()
        {
            if (_queue.TryDequeue(out Request item))
                return item;

            return null;
        }

        public void Dispose()
        {
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Push(Request request)
        {
            var requestIdentity = request.GetIdentity();

            if (IsDuplicate(request))
            {
                return;
            }

            _allUrls.Add(requestIdentity);

            _queue.Enqueue(request);
            _count++;
        }

        public void IncreaseSuccessCount()
        {
            _successCount.Increment();
        }

        public void IncreaseErrorCount()
        {
            _errorCount.Increment();
        }

        public bool IsDuplicate(Request request)
        {
            var requestIdentity = request.GetIdentity();

            return _allUrls.Contains(requestIdentity);
        }

        public void ResetDuplicateCheck()
        {
            _allUrls.Clear();
        }
    }
}

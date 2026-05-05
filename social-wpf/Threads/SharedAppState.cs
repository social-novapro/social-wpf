using social_wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace social_wpf.Threads
{
    public class SharedAppState
    {
        public bool IsRunning { get; set; } = false;

        public readonly object FeedLock = new();
        public readonly object StatusLock = new();
        public object PostQueueLock = new();
        public object MediaCacheLock = new();

        public List<FeedData> Posts { get; } = new();
        public Queue<PostDraft> PostQueue { get; } = new();
        public List<ThreadStatus> ThreadStatuses { get; } = new();

        public Dictionary<string, BitmapImage> MediaCache { get; } = new();

        public void UpdateThreadStatus(string threadName, string state, string message)
        {
            lock (StatusLock)
            {
                ThreadStatus? status = ThreadStatuses.FirstOrDefault(s => s.Name == threadName);
                if (status == null)
                {
                    status = new ThreadStatus { Name = threadName };
                    ThreadStatuses.Add(status);
                }

                status.State = state;
                status.Message = message;
                status.LastUpdated = DateTime.Now;
            }
        }
    }
}

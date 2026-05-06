using social_wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// Shared state used by the UI thread and worker threads.
// Locks protect shared collections from concurrent access.
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

        public string? PrevIndexId { get; set; }
        public bool IsLoadingMoreFeed { get; set; } = false;
        public bool LoadMoreRequested { get; set; } = false;
        public DateTime LastFeedRefresh { get; set; } = DateTime.MinValue;
        public bool RefreshFeedRequested { get; set; } = true;

        public string LastPostUploadMessage { get; set; } = string.Empty;
        public DateTime LastPostUploadAt { get; set; } = DateTime.MinValue;
        public string? LastUploadedPostId { get; set; }

        public void UpdateThreadStatus(
            string threadName,
            string state,
            string message,
            TimeSpan? nextUpdateIn = null
        ) {
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
                status.NextUpdateAt = nextUpdateIn == null
                    ? null
                    : DateTime.Now.Add(nextUpdateIn.Value);
            }
        }

        public void RequestLoadMoreFeed()
        {
            string? indexToShow;
            bool isLoading;

            lock (FeedLock)
            {
                indexToShow = PrevIndexId;
                isLoading = IsLoadingMoreFeed;

                if (!string.IsNullOrWhiteSpace(PrevIndexId) && !IsLoadingMoreFeed)
                {
                    LoadMoreRequested = true;

                    UpdateThreadStatus("FeedSyncWorker", "Requested", $"Load more requested with index {PrevIndexId}");

                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(indexToShow))
            {
                UpdateThreadStatus("FeedSyncWorker", "No More", "Bottom reached, but there is no next index.");
            }
            else if (isLoading)
            {
                UpdateThreadStatus("FeedSyncWorker", "Loading", "Bottom reached, but older posts are already loading.");
            }
        }

        public void RequestRefreshFeed()
        {
            lock (FeedLock)
            {
                RefreshFeedRequested = true;
            }
        }

        public void MarkPostUploaded(string postId)
        {
            lock (StatusLock)
            {
                LastUploadedPostId = postId;
                LastPostUploadAt = DateTime.Now;
                LastPostUploadMessage = $"Uploaded post {postId} at {LastPostUploadAt:T}";
            }

            RequestRefreshFeed();
        }

        public void MarkPostUploadFailed(string message)
        {
            lock (StatusLock)
            {
                LastPostUploadAt = DateTime.Now;
                LastPostUploadMessage = $"Upload failed: {message}";
            }
        }
    }
}

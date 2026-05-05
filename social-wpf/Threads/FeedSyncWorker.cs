using social_wpf.Models;
using social_wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Threads
{
    public class FeedSyncWorker
    {
        private readonly SharedAppState appState;
        private readonly InteractApiClient apiClient;

        public FeedSyncWorker(SharedAppState appState, InteractApiClient apiClient)
        {
            this.appState = appState;
            this.apiClient = apiClient;
        }

        public void Run()
        {
            Thread.CurrentThread.Name = "FeedSyncWorker";
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            Thread.CurrentThread.IsBackground = true;

            while (appState.IsRunning)
            {
                try
                {
                    appState.UpdateThreadStatus("FeedSyncWorker", "Running", "Fetching latest feed...");

                    FeedResponse feed = apiClient.GetUserFeed().GetAwaiter().GetResult();

                    lock (appState.FeedLock)
                    {
                        appState.Posts.Clear();
                        appState.Posts.AddRange(feed.posts);
                    }

                    appState.UpdateThreadStatus("FeedSyncWorker", "Idle", $"Loaded {feed.posts.Count} posts");
                }
                catch (Exception ex)
                {
                    appState.UpdateThreadStatus("FeedSyncWorker", "Error", ex.Message);
                }

                Thread.Sleep(5000); // Sleep for 5 seconds before next sync
            }
        }
    }
}

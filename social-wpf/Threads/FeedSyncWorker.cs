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
                    bool shouldLoadMore = false;
                    bool shouldRefresh = false;
                    string? nextIndexId = null;

                    lock (appState.FeedLock)
                    {
                        shouldLoadMore = appState.LoadMoreRequested && 
                            !appState.IsLoadingMoreFeed && 
                            !string.IsNullOrWhiteSpace(appState.NextIndexId);

                        if (shouldLoadMore)
                        {
                            nextIndexId = appState.NextIndexId;
                            appState.LoadMoreRequested = false;
                            appState.IsLoadingMoreFeed = true;
                        }
                        else
                        {
                            bool feedIsEmpty = appState.Posts.Count == 0;
                            bool refreshIntervalPassed = (DateTime.Now - appState.LastFeedRefresh).TotalSeconds > 30;

                            shouldRefresh = appState.RefreshFeedRequested ||
                                feedIsEmpty || refreshIntervalPassed;

                            if (shouldRefresh)
                            {
                                appState.RefreshFeedRequested = false;
                            }
                        }
                    }
                    
                    if (shouldLoadMore && !string.IsNullOrWhiteSpace(nextIndexId))
                    {
                        LoadMorePosts(nextIndexId);
                    }
                    else if (shouldRefresh)
                    {
                        RefreshFirstPage();
                    } else
                    {
                        appState.UpdateThreadStatus("FeedSyncWorker", "Idle", "Waiting for next sync...");
                    }
                }
                catch (Exception ex)
                {
                    appState.UpdateThreadStatus("FeedSyncWorker", "Error", ex.Message);
                }

                Thread.Sleep(15000); // Sleep for 15 seconds before next sync
            }

            appState.UpdateThreadStatus("FeedSyncWorker", "Stopped", "Feed sync stopped");
        }
        private void RefreshFirstPage()
        {
            appState.UpdateThreadStatus("FeedSyncWorker", "Running", "Refreshing latest feed...");

            FeedResponse feed = apiClient.GetAllPosts().GetAwaiter().GetResult();
            List<FeedData> normalizedPosts = NormalizeApiOrder(feed.posts);

            lock (appState.FeedLock)
            {
                appState.Posts.Clear();
                appState.Posts.AddRange(normalizedPosts);

                appState.NextIndexId = feed.nextIndexId;
                appState.LastFeedRefresh = DateTime.Now;
                appState.IsLoadingMoreFeed = false;
            }

            appState.UpdateThreadStatus("FeedSyncWorker", "Idle", $"Loaded latest {feed.posts.Count} posts");
        }

        private void LoadMorePosts(string nextIndexId)
        {
            appState.UpdateThreadStatus("FeedSyncWorker", "Running", "Loading older posts...");

            FeedResponse feed = apiClient.GetAllPosts(nextIndexId).GetAwaiter().GetResult();

            List<FeedData> normalizedPosts = NormalizeApiOrder(feed.posts);

            lock (appState.FeedLock)
            {
                foreach (FeedData post in feed.posts)
                {
                    bool alreadyExists = appState.Posts.Any(p => p.postData._id == post.postData._id);

                    if (!alreadyExists)
                    {
                        appState.Posts.Add(post);
                    }
                }

                appState.NextIndexId = feed.nextIndexId;
                appState.IsLoadingMoreFeed = false;
            }

            appState.UpdateThreadStatus("FeedSyncWorker", "Idle", $"Loaded {feed.posts.Count} older posts");
        }

        private List<FeedData> NormalizeApiOrder(List<FeedData> posts)
        {
            return posts
                .AsEnumerable()
                .Reverse()
                .ToList();
        }
    }
}

using social_wpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static social_wpf.Services.ApiRoutes;

namespace social_wpf.Threads
{
    public class MediaCacheWorker
    {
        private readonly SharedAppState appState;
        private readonly HttpClient httpClient = new();

        public MediaCacheWorker(SharedAppState appState)
        {
            this.appState = appState;
        }

        public void Run()
        {
            Thread.CurrentThread.Name = "MediaCacheWorker";
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            Thread.CurrentThread.IsBackground = true;

            while (appState.IsRunning)
            {
                List<FeedData> postsCopy;

                lock (appState.FeedLock)
                {
                    postsCopy = appState.Posts.ToList();
                }

                appState.UpdateThreadStatus("MediaCacheWorker", "Running", $"Checking {postsCopy.Count} posts for media...");

                foreach (FeedData post in postsCopy)
                {
                    if (!appState.IsRunning)
                    {
                        break;
                    }

                    string profileURL = post.userData.profileURL;
                    GetAndCacheImage(post, profileURL);

                    foreach (AttachmentData attachment in post.postData.attachments ?? new List<AttachmentData>())
                    {
                        if (attachment.type == "image")
                        {
                            GetAndCacheImage(post, attachment.url);
                        }
                    }
                }
                appState.UpdateThreadStatus("MediaCacheWorker", "Idle", $"Checked media for {postsCopy.Count} posts", TimeSpan.FromMilliseconds(5000));
                Thread.Sleep(5000);
            }
            appState.UpdateThreadStatus("MediaCacheWorker", "Stopped", "Media cache worker has stopped");
        }

        private void GetAndCacheImage(FeedData post, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return;
            }

            bool alreadyCached;

            lock (appState.MediaCacheLock)
            {
                alreadyCached = appState.MediaCache.ContainsKey(imageUrl);
            }

            if (alreadyCached)
            {
                appState.UpdateThreadStatus("MediaCacheWorker", "Caching", $"Profile image for {post.userData.username} already cached");
                return;
            }

            try
            {
                appState.UpdateThreadStatus("MediaCacheWorker", "Caching", $"Downloading media for {post.userData.username}...");
               
                byte[] imageBytes = httpClient.GetByteArrayAsync(imageUrl).GetAwaiter().GetResult();
                BitmapImage image = CreateFrozenBitmapImage(imageBytes);

                lock (appState.MediaCacheLock)
                {
                    if (!appState.MediaCache.ContainsKey(imageUrl))
                    {
                        appState.MediaCache.Add(imageUrl, image);
                    }
                }

                appState.UpdateThreadStatus("MediaCacheWorker", "Caching", $"Cached media for {post.userData.username}");
            }
            catch
            {
                appState.UpdateThreadStatus("MediaCacheWorker", "Error", $"Failed to cache media for {post.userData.username}");
            }
        }

        private BitmapImage CreateFrozenBitmapImage(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();

                image.Freeze();
                return image;
            }
        }
    }
}

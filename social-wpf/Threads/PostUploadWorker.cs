using social_wpf.Models;
using social_wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Producer-consumer queue.
// UI thread adds posts to the queue.
// Upload thread waits using Monitor.Wait until Monitor.Pulse wakes it.
namespace social_wpf.Threads
{
    public class PostUploadWorker
    {
        private readonly SharedAppState appState;
        private readonly InteractApiClient apiClient;

        [ThreadStatic]
        private static int postsUploadedByThisThread = 0;

        public PostUploadWorker(SharedAppState appState, InteractApiClient apiClient)
        {
            this.appState = appState;
            this.apiClient = apiClient;
        }

        public void Run()
        {
            Thread.CurrentThread.Name = "PostUploadWorker";
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            Thread.CurrentThread.IsBackground = true;

            while (appState.IsRunning)
            {
                PostDraft? draft = WaitForPost();

                if (draft == null)
                {
                    continue;
                }

                try
                {
                    appState.UpdateThreadStatus("PostUploadWorker", "Uploading", $"Post Content: {draft.content}");

                    PostData createdPost = apiClient.CreatePost(draft).GetAwaiter().GetResult();

                    if (string.IsNullOrWhiteSpace(createdPost._id))
                    {
                        throw new Exception("API returned no post ID.");
                    }

                    postsUploadedByThisThread++;

                    appState.UpdateThreadStatus("PostUploadWorker", "Uploaded", $"Uploaded post {createdPost._id}. Total uploaded by this thread: {postsUploadedByThisThread}");

                    appState.MarkPostUploaded(createdPost._id);

                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    appState.UpdateThreadStatus("PostUploadWorker", "Error", ex.Message);
                    appState.MarkPostUploadFailed(ex.Message);
                }
            }

            appState.UpdateThreadStatus("PostUploadWorker", "Stopped", $"Total posts uploaded by this thread: {postsUploadedByThisThread}");
        }

        private PostDraft? WaitForPost()
        {
            Monitor.Enter(appState.PostQueueLock);

            try
            {
                while (appState.PostQueue.Count == 0 && appState.IsRunning)
                {
                    appState.UpdateThreadStatus("PostUploadWorker", "Idle", "Waiting for posts to upload...");
                    Monitor.Wait(appState.PostQueueLock);
                }

                if (!appState.IsRunning)
                {
                    return null;
                }

                return appState.PostQueue.Dequeue();
            }
            finally
            {
                Monitor.Exit(appState.PostQueueLock);
            }
        }
    } 
}

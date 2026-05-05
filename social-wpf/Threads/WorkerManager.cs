using social_wpf.Models;
using social_wpf.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Threads
{
    public class WorkerManager
    {
        private readonly SharedAppState appState;
        private readonly InteractApiClient apiClient;
        private readonly AppSettings appSettings;

        private Thread? feedThread;
        private Thread? uploadThread;
        private Thread? mediaThread;

        private bool hasStarted = false;

        public WorkerManager(SharedAppState appState, InteractApiClient apiClient, AppSettings appSettings)
        {
            this.appState = appState;
            this.apiClient = apiClient;
            this.appSettings = appSettings;
        }

        public void Start()
        {
            if (hasStarted)
            {
                return;
            }
            appState.IsRunning = true;
            feedThread = new Thread(() => new FeedSyncWorker(appState, apiClient).Run());
            uploadThread = new Thread(() => new PostUploadWorker(appState, apiClient).Run());
            mediaThread = new Thread(() => new MediaCacheWorker(appState).Run());
            feedThread.Start();
            uploadThread.Start();
            mediaThread.Start();
            hasStarted = true;
        }

        public void QueuePost(string content, string? replyId, string? quoteId)
        {
            PostDraft draft = new PostDraft
            {
                userID = appSettings.UserId,
                content = content,
                replyingPostId = replyId,
                quotingPostId = quoteId
            };

            Monitor.Enter(appState.PostQueueLock);

            try
            {
                appState.PostQueue.Enqueue(draft);
                Monitor.Pulse(appState.PostQueueLock);
            }
            finally
            {
                Monitor.Exit(appState.PostQueueLock);
            }
        }

        public void Stop()
        {
            if (!hasStarted)
            {
                return;
            }

            appState.IsRunning = false;
            Monitor.Enter(appState.PostQueueLock);

            try
            {
                Monitor.PulseAll(appState.PostQueueLock);
            }
            finally
            {
                Monitor.Exit(appState.PostQueueLock);
            }

            feedThread?.Join(2000);
            uploadThread?.Join(2000);
            mediaThread?.Join(2000);

            hasStarted = false;
        }
    }
}

using social_wpf.Controls;
using social_wpf.Models;
using social_wpf.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace social_wpf.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly SharedAppState appState;
        private readonly WorkerManager workerManager;
        private readonly DispatcherTimer refreshTimer;
        public HomePage(SharedAppState appState, WorkerManager workerManager)
        {
            InitializeComponent();
            
            this.appState = appState;
            this.workerManager = workerManager;

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(1);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();

            RefreshUi();
        }

        private void QueuePostButton_Click(object sender, RoutedEventArgs e)
        {
            string content = PostTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(content))
            {
                PostStatusTextBlock.Text = "Post content cannot be empty.";
                return;
            }

            workerManager.QueuePost(content, null, null);

            PostTextBox.Clear();
            PostStatusTextBlock.Text = "Post queued for upload.";
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshUi();
        }

        private void RefreshUi()
        {
            RefreshFeed();
            RefreshThreadStatuses();
            RefreshSummary();
        }

        private void RefreshSummary()
        {
            int postCount;
            int queuePostCount;
            int cachedMediaCount;

            lock (appState.FeedLock)
            {
                postCount = appState.Posts.Count;
            }

            lock (appState.PostQueueLock)
            {
                queuePostCount = appState.PostQueue.Count;
            }

            lock (appState.MediaCacheLock)
            {
                cachedMediaCount = appState.MediaCache.Count;
            }

            SummaryTextBlock.Text = $"Posts: {postCount} | Queued Posts: {queuePostCount} | Cached Media: {cachedMediaCount}";
        }

        private void RefreshFeed()
        {
            List<FeedData> postsCopy;
            lock (appState.FeedLock)
            {
                postsCopy = appState.Posts.ToList();
            }

            FeedStackPanel.Children.Clear();

            if (postsCopy.Count == 0)
            {
                FeedStackPanel.Children.Add(new TextBlock
                {
                    Text = "No posts loaded yet. Feed Sync Thread should load them soon.",
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap
                });

                return;
            }

            foreach (FeedData post in postsCopy)
            {
                FeedStackPanel.Children.Add(new PostCardControl(post, appState));
            }
        }

        private void RefreshThreadStatuses()
        {
            List<ThreadStatus> statusesCopy;

            lock (appState.StatusLock)
            {
                statusesCopy = appState.ThreadStatuses.ToList();
            }

            ThreadStatusStackPanel.Children.Clear();

            if (statusesCopy.Count == 0)
            {
                ThreadStatusStackPanel.Children.Add(new TextBlock
                {
                    Text = "No worker threads started yet.",
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap
                });

                return;
            }

            foreach (ThreadStatus status in statusesCopy)
            {
                ThreadStatusStackPanel.Children.Add(new ThreadStatusCardControl(status));
            }
        }
    }
}

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

namespace social_wpf.Controls
{
    /// <summary>
    /// Interaction logic for PostCardControl.xaml
    /// </summary>
    public partial class PostCardControl : UserControl
    {
        private readonly SharedAppState appState;

        public PostCardControl(FeedData post, SharedAppState appState)
        {
            InitializeComponent();

            this.appState = appState;

            RenderPost(post);
        }

        private void RenderPost(FeedData post)
        {
            UsernameTextBlock.Text = string.IsNullOrWhiteSpace(post.userData.username)
                ? "Unknown User"
                : post.userData.username;

            ContentTextBlock.Text = post.postData.content;

            MetaTextBlock.Text = $"{post.postData.totalLikes} likes - {post.postData.totalReplies} replies - {post.postData.totalQuotes} quotes";

            BitmapImage? profileImage = GetCachedImage(post.userData.profileURL);

            if (profileImage != null)
            {
                AvatarImage.Source = profileImage;
            }

            RenderAttachments(post);
        }

        private void RenderAttachments(FeedData post)
        {
            AttachmentsPanel.Children.Clear();

            foreach (AttachmentData attachment in post.postData.attachments ?? new List<AttachmentData>())
            {
                if (attachment.type != "image")
                {
                    continue;
                }

                BitmapImage? image = GetCachedImage(attachment.url);

                if (image == null)
                {
                    Border placeholder = new Border
                    {
                        Width = 220,
                        Height = 140,
                        Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                        CornerRadius = new System.Windows.CornerRadius(8),
                        Margin = new System.Windows.Thickness(0, 0, 8, 8),
                        Child = new TextBlock
                        {
                            Text = "image loading...",
                            Foreground = Brushes.Gray,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center
                        }
                    };
                    AttachmentsPanel.Children.Add(placeholder);
                    continue;
                }

                Border imageBorder = new Border
                {
                    MaxWidth = 520,
                    MaxHeight = 320,
                    CornerRadius = new CornerRadius(8),
                    ClipToBounds = true,
                    Margin = new Thickness(0, 0, 8, 8),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(225, 225, 225)),
                    BorderThickness = new Thickness(1)
                };

                Image imageControl = new Image
                {
                    Source = image,
                    Stretch = Stretch.Uniform,
                    MaxHeight = 320
                };

                imageBorder.Child = imageControl;
                AttachmentsPanel.Children.Add(imageBorder);
            }
        }
        private BitmapImage? GetCachedImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return null;
            }

            lock (appState.MediaCacheLock)
            {
                if (appState.MediaCache.TryGetValue(imageUrl, out BitmapImage? image))
                {
                    return image;
                }
            }

            return null;
        }
    }
}

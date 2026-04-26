using social_wpf.Models;
using social_wpf.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace social_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IsolatedStorageService storage;
        private readonly AppSettings settings;
        private readonly InteractApiClient apiClient;

        public MainWindow()
        {
            InitializeComponent();
            storage = new IsolatedStorageService();
            settings = storage.LoadSettings();
            apiClient = new InteractApiClient(settings);
            if (settings.IsLoggedIn)
            {
                apiClient.SetTokens(settings);
                MainFrame.Navigate(new Pages.HomePage(/*apiClient, storage, settings*/));
            }
            else
            {
                MainFrame.Navigate(new Pages.LoginPage(/*apiClient, storage, settings*/));

            }
        }

        public void NavigateToLogin()
        {
            ShowLoggedOutStatus();
            StatusTextBlock.Text = "Please login";
            MainFrame.Navigate(new Pages.LoginPage(/*apiClient, storage, settings*/));
        }

        public void NavigateToFeed()
        {
            ShowLoggedInStatus();
            StatusTextBlock.Text = "Welcome back!";
        }

        public void SetStatus(string message)
        {
            StatusTextBlock.Text = message;
        }

        private void ShowLoggedInStatus()
        {
            UserStatusTextBlock.Text = string.IsNullOrWhiteSpace(settings.LastUsername)
                ? "Logged in" : $"Logged in as {settings.LastUsername}";

        }
        private void ShowLoggedOutStatus()
        {
            UserStatusTextBlock.Text = "Not logged in";
        }
        
        protected override void OnClosed(EventArgs e)
        {
            storage.SaveSettings(settings);
            base.OnClosed(e);
        }
    }
}
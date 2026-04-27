using social_wpf.Models;
using social_wpf.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace social_wpf.Pages
{
    public partial class LoginPage : Page
    {
        private readonly InteractApiClient apiClient;
        private readonly IsolatedStorageService storage;
        private readonly AppSettings settings;
        private readonly MainWindow mainWindow;

        public LoginPage(
            InteractApiClient apiClient,
            IsolatedStorageService storage,
            AppSettings settings,
            MainWindow mainWindow
        ) {
            InitializeComponent();

            this.apiClient = apiClient;
            this.storage = storage;
            this.settings = settings;
            this.mainWindow = mainWindow;

            UsernameTextBox.Text = settings.LastUsername;
            UsernameTextBox.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await TryLogin();
        }

        private async void PasswordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await TryLogin();
            }
        }

        private async Task TryLogin()
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                StatusTextBlock.Text = "Enter your username and password.";
                return;
            }

            LoginButton.IsEnabled = false;
            UsernameTextBox.IsEnabled = false;
            PasswordInput.IsEnabled = false;

            StatusTextBlock.Foreground = System.Windows.Media.Brushes.Gray;
            StatusTextBlock.Text = "Logging in...";
            mainWindow.SetStatus("Logging in...");

            try
            {
                bool success = await apiClient.Login(
                    username,
                    password,
                    settings,
                    storage
                );

                if (!success)
                {
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Crimson;
                    StatusTextBlock.Text = "Login failed. Check your username or password.";
                    mainWindow.SetStatus("Login failed.");
                    return;
                }

                settings.LastUsername = username;
                storage.SaveSettings(settings);

                mainWindow.NavigateToFeed();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Foreground = System.Windows.Media.Brushes.Crimson;
                StatusTextBlock.Text = "Login error: " + ex.Message;
                mainWindow.SetStatus("Login error.");
            }
            finally
            {
                LoginButton.IsEnabled = true;
                UsernameTextBox.IsEnabled = true;
                PasswordInput.IsEnabled = true;
            }
        }
    }
}
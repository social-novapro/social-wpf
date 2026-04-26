using social_wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace social_wpf.Services
{
    public class InteractApiClient
    {
        private readonly HttpClient httpClient;

        private readonly object _authLock = new();

        public InteractApiClient(AppSettings settings)
        {
            httpClient = new()
            {
                BaseAddress = new Uri(ApiRoutes.BaseUrl)
            };

            lock (_authLock)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("devtoken", ApiRoutes.DevToken);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("apptoken", ApiRoutes.AppToken);
            }
        }

        public void ClearTokens()
        {
            lock (_authLock)
            {
                httpClient.DefaultRequestHeaders.Remove("usertoken");
                httpClient.DefaultRequestHeaders.Remove("userid");
                httpClient.DefaultRequestHeaders.Remove("accesstoken");
            }
        }
        public void SetTokens(AppSettings settings)
        {
            ClearTokens();
            lock (_authLock)
            {
                if (!settings.IsLoggedIn)
                {
                    return;
                }

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("usertoken", settings.UserToken);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("userid", settings.UserId);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accesstoken", settings.AccessToken);
            }
        }


        public async Task<bool> Login(
            string username,
            string password,
            AppSettings settings,
            IsolatedStorageService storageService
        )
        {
            var loginData = new
            {
                username,
                password
            };

            string json = JsonSerializer.Serialize(loginData);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ApiRoutes.Auth.Login, httpContent);
            
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            string responseContent = await response.Content.ReadAsStringAsync();
            AuthTokens? authTokens = JsonSerializer.Deserialize<AuthTokens>(responseContent);

            if (authTokens == null)
            {
                return false;
            }

            settings.IsLoggedIn = true;
            settings.UserToken = authTokens.usertoken;
            settings.UserId = authTokens.userid;
            settings.AccessToken = authTokens.accesstoken;
            storageService.SaveSettings(settings);
            SetTokens(settings);
            return true;
        }

        public void Logout(AppSettings settings, IsolatedStorageService storageService)
        {
            settings.IsLoggedIn = false;
            settings.UserToken = string.Empty;
            settings.UserId = string.Empty;
            settings.AccessToken = string.Empty;
            storageService.SaveSettings(settings);
            ClearTokens();
        }


        //public void CreateRequest(HttpMethod, string url)
        //{
        //}

        public async Task<FeedResponse> GetUserFeed(string userId, string url, string? prevIndexId = null)
        {
            string getUserFeed = prevIndexId == null 
                ? ApiRoutes.Feed.AllPosts
                : ApiRoutes.Feed.AllPostsIndex(prevIndexId);

            FeedResponse? feedResponse = await httpClient.GetFromJsonAsync<FeedResponse>(getUserFeed);
            if (feedResponse == null)
            {
                return new FeedResponse();
            }

            return feedResponse;
        }

        public async Task<PostData> CreatePost(PostDraft postDraft)
        {
            //var request = new
            string json = JsonSerializer.Serialize(postDraft);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(ApiRoutes.Posts.CreatePost, httpContent);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                PostData? postData = JsonSerializer.Deserialize<PostData>(responseContent);
                if (postData != null)
                {
                    return postData;
                }
            }
            return new PostData();
        }
    }
 }

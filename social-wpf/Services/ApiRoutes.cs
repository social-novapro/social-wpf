using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Services
{
    public static class ApiRoutes
    {
        public static string BaseUrl { get; set; } = "https://interact-api.novapro.net/v1/";

        /* The dev token and app token are used for authentication when making API calls. 
         * Ideally, these should be stored securely, but for the sake of this project, they are hardcoded here. 
         */
        public static string DevToken { get; } = "872d2652-992b-49f5-906a-afffab3fa7b1";
        public static string AppToken { get; } = "c0df050d-efb9-46ab-9538-54baa374f41a";

        public static class Auth
        {
            public const string Login = "auth/userLogin";
        }

        public static class Feed
        {
            public static string UserFeed = "feeds/userFeed/v2";
            public static string UserFeedIndex(string prevIndexId) => $"feeds/userFeed/v2/{prevIndexId}";
            public static string AllPosts = "feeds/allPosts/v2";
            public static string AllPostsIndex(string prevIndexId) => $"feeds/allPosts/v2/{prevIndexId}";
        }

        public static class Posts
        {
            public const string CreatePost = "posts/create";
            public static string GetPost(string postId) => $"posts/get/full/{postId}";

        }

        public static class Users
        {
            public static string BasicInfo(string userId) => $"users/get/basic/{userId}";
        }
    }
}

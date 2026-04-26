using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Services
{
    public static class ApiRoutes
    {
        public static class Auth
        {
            public const string Login = "/auth/login";
            public const string Register = "/auth/register";
        }

        public static class Feed
        {
            public static string UserFeed = "/feeds/userFeed/v2";
            public static string AllPosts = "/feeds/allPosts/v2";
            public static string AllPostsIndex(string prevIndexId) => $"/feeds/allPosts/v2/{prevIndexId}";
        }

        public static class Posts
        {
            public static string CreatePost = "/posts/create";
            public static string GetPost(string postId) => $"/posts/get/full/{postId}";

        }

        public static class Users
        {
            public static string BasicInfo(string userId) => $"/users/get/basic/{userId}";
        }
    }
}

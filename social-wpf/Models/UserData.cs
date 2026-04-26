using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Models
{
    public class UserData
    {
        public string _id { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string displayName { get; set; } = string.Empty;
        public string profileURL { get; set; } = string.Empty;

        public int totalPosts { get; set; } = 0;
        public int totalReplies { get; set; } = 0;
        public int totalQuotes { get; set; } = 0;

        public int followerCount { get; set; } = 0;
        public int followingCount { get; set; } = 0;
        public int likeCount { get; set; } = 0;
        public int likedCount { get; set; } = 0;

        public string description { get; set; } = string.Empty;
        public string pronouns { get; set; } = string.Empty;
        public string statusTitle { get; set; } = string.Empty;

        public bool verified { get; set; } = false;
    }
}

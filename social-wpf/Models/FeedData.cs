using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Models
{
    public class FeedResponse
    {
        public string? prevIndexId { get; set; }
        public string? nextIndexId { get; set; }
        public int amount { get; set; } = 0;
        public string feedVersion { get; set; } = string.Empty;
        public List<FeedData> posts { get; set; } = [];

    }

    public class FeedData
    {
        public FeedType feedType { get; set; } = new FeedType();
        public UserData userData { get; set; } = new UserData();
        public PostData postData { get; set; } = new PostData();
        public ExtraData extraData { get; set; } = new ExtraData();
        public ReplyData? replyData { get; set; }
        public QuoteData? quoteData { get; set; }
    }

    public class FeedType
    {
        public string type { get; set; } = string.Empty;
        public string user { get; set; } = string.Empty;

        public string extra { get; set; } = string.Empty;

        public string reply { get; set; } = string.Empty;

        public string quote { get; set; } = string.Empty;
    }

    public class ExtraData
    {
        public bool liked { get; set; } = false;
        public bool pinned { get; set; } = false;
        public bool saved { get; set; } = false;
        public bool followed { get; set; } = false;
    }

    public class ReplyData
    {
        public PostData replyPost { get; set; } = new PostData();
        public UserData replyUser { get; set; } = new UserData();
    }
    public class QuoteData
    {
        public PostData quotePost { get; set; } = new PostData();
        public UserData quoteUser { get; set; } = new UserData();
    }
}

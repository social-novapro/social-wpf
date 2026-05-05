using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Models
{
    public class PostData
    {
        public string _id { get; set; } = string.Empty;
        public string userID { get; set; } = string.Empty;

        public int timePosted { get; set; } = 0;

        public string content { get; set; } = string.Empty;

        public int totalLikes { get; set; } = 0;

        public int totalReplies { get; set; } = 0;

        public int totalQuotes { get; set; } = 0;

        public bool isQuote { get; set; } = false;

        public bool isReply { get; set; } = false;

        public List<AttachmentData> attachments { get; set; } = [];
    }

    public class AttachmentData
    {
        public string _id { get; set; } = string.Empty;
        //public string index { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string host { get; set; } = string.Empty;
        public string url {  get; set; } = string.Empty;
    }
}

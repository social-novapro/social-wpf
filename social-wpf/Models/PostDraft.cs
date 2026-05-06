using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace social_wpf.Models
{
    public class PostDraft
    {
        public string userID { get; set; } = string.Empty;

        public string content { get; set; } = string.Empty;

        [JsonPropertyName("replyingPostID")]
        public string? replyingPostId { get; set; }

        [JsonPropertyName("quoteReplyPostID")]
        public string? quotingPostId { get; set; }
    }
}
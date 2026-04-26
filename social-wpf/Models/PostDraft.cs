using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Models
{
    public class PostDraft
    {
        public string userID { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public string? replyingPostId { get; set; }
        public string? quotingPostId { get; set; }
    }
}

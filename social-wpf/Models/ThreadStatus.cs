using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Models
{
    public class ThreadStatus
    {
        public string Name { get; set; } = string.Empty;
        public string State { get; set; } = "Stopped";
        public string Message { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public DateTime? NextUpdateAt { get; set; }
    }
}

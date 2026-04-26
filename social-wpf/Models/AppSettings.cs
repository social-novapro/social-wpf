using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Models
{
    public class AppSettings
    {
        public bool IsLoggedIn { get; set; } = false;
        public string LastUsername { get; set; } = string.Empty;
        public string UserToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}

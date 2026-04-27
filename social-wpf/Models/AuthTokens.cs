using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Models
{
    public class LoginResponse
    {
        public bool login { get; set; } = false;
        public UserData publicData { get; set; } = new UserData();
        public string accessToken { get; set; } = string.Empty;
        public string userToken { get; set; } = string.Empty;
        public string userID { get; set; } = string.Empty;
    }
}

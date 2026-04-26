using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_wpf.Models
{
    public class AppSettings
    {
        public string BaseUrl { get; set; } = "https://interact-api.novapro.net";
        public string LastUsername { get; set; } = string.Empty;
    }
}

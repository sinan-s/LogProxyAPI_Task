using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogProxyAPI.Model
{
    public class LogIncoming
    {
        public string id { get; set; }
        public DateTime receivedAt { get; set; }
        public string title { get; set; }
        public string text { get; set; }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;

namespace LogProxyAPI.Model
{
    public class LogOutgoing
    {
        public string id { get; set; }
        public DateTime receivedAt { get; set; }
        public string Summary { get; set; }
        public string Message { get; set; }

    }
}

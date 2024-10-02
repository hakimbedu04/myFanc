using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Core.Utility
{
    public class TokenStoreItem
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpirationTime { get; set; } = DateTime.UtcNow;
    }
}

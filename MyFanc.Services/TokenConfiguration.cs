using MyFanc.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Services
{
    public class TokenConfiguration : ITokenConfiguration
    {
        public string Key { get; set; } = string.Empty;

        public int EpiredInDays { get; set; }
    }
}

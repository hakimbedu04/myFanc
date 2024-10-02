using MyFanc.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Services.FancRadApi
{
    public class FancRADApiConfiguration : IFancRADApiConfiguration
    {
        public string BasePath { get; set; } = string.Empty;

        public string AuthenticationPath { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public string GrantType { get; set; } = string.Empty;

        public bool Log { get; set; } = false;
    }
}

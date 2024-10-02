using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Contracts.Services
{
    public interface IFancRADApiConfiguration
    {
        string BasePath { get; }

        string AuthenticationPath { get; }

        string ClientId { get; }

        string ClientSecret { get; }

        string Scope { get; }

        string GrantType { get; }

        bool Log { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Contracts.Services
{
    public interface IIdentityProviderConfiguration
    {
        string CSAM { get; }
        string ADFS { get; }
    }
}

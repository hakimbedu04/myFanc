using MyFanc.Contracts.Services;

namespace MyFanc.Services
{
    public class IdentityProviderConfiguration : IIdentityProviderConfiguration
    {
        public string CSAM { get; set; } = string.Empty;

        public string ADFS { get; set; } = string.Empty;
    }
}

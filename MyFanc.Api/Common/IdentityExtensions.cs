using MyFanc.Core;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Principal;

namespace MyFanc.Api.Common
{
    public static class IdentityExtensions
    {
        public static string? IdentityProvider(this ClaimsIdentity identity)
        {
            return identity.Claims.Where(c => c.Type == Constants.JwtIdentityProvider).Select(c => c.Value).SingleOrDefault();
        }

        public static ClaimsIdentity AsClaimIdentity(this IIdentity identity)
        {
            return (ClaimsIdentity) identity;
        }

        public static string GetUserExternalId(this ClaimsIdentity identity)
        {
            return identity.Claims.FirstOrDefault(x => x.Type == Constants.JwtSubUserId)?.Value ?? string.Empty;
        }
    }
}

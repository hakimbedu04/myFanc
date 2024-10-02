using Microsoft.Extensions.Caching.Memory;
using MyFanc.Contracts.Services;
using MyFanc.Core.Utility;
using MyFanc.DTO.External.Authentication;

namespace MyFanc.Services.FancRadApi
{
    public class FancAuthTokenStore : IAuthTokenStore
    {
        private readonly IFancAuthentication fancAuthentication;
        private readonly IMemoryCache memoryCache;
        private readonly IFancRADApiConfiguration fancRADApiConfiguration;

        private const string AuthenticationStoreKey = "FancApiAuthenticationStore";

        public FancAuthTokenStore(IFancAuthentication fancAuthentication, IMemoryCache memoryCache, IFancRADApiConfiguration fancRADApiConfiguration)
        {
            this.fancAuthentication = fancAuthentication;
            this.memoryCache = memoryCache;
            this.fancRADApiConfiguration = fancRADApiConfiguration;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            if (memoryCache.TryGetValue<TokenStoreItem>(AuthenticationStoreKey, out var tokenItem))
            {
                if (!string.IsNullOrWhiteSpace(tokenItem?.Token) && tokenItem.ExpirationTime > DateTime.Now.AddMinutes(10))
                {
                    return tokenItem.Token;
                }
            }

            if (tokenItem == null)
                tokenItem = new TokenStoreItem();

            var now = DateTime.Now;
            TokenResponse token = await RequestToken();
            double expiresIn = Convert.ToDouble(token.ExpiresIn);
            tokenItem.Token = token.AccessToken;
            tokenItem.ExpirationTime = now.AddSeconds(expiresIn);

            memoryCache.Set(AuthenticationStoreKey, tokenItem, now.AddSeconds(expiresIn));

            return token.AccessToken;
        }

        private Task<TokenResponse> RequestToken()
        {
            return fancAuthentication.GetToken(new Dictionary<string, string>
            {
                {"grant_type", fancRADApiConfiguration.GrantType},
                {"scope", fancRADApiConfiguration.Scope},
                {"client_id", fancRADApiConfiguration.ClientId},
                {"client_secret", fancRADApiConfiguration.ClientSecret}
            });
        }
    }
}

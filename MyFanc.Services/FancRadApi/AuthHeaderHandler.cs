using Microsoft.Extensions.Logging;
using MyFanc.Contracts.Services;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace MyFanc.Services.FancRadApi
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IAuthTokenStore authTokenStore;
        private readonly ILogger<AuthHeaderHandler> logger;
        private readonly IFancRADApiConfiguration fancRADApiConfiguration;
        private readonly ISharedDataCache _sharedDataCache;
        public AuthHeaderHandler(IAuthTokenStore authTokenStore, ILogger<AuthHeaderHandler> logger, IFancRADApiConfiguration fancRADApiConfiguration, ISharedDataCache sharedDataCache)
        {
            this.authTokenStore = authTokenStore ?? throw new ArgumentNullException(nameof(authTokenStore));
            this.logger = logger;
            this.fancRADApiConfiguration = fancRADApiConfiguration;
            _sharedDataCache = sharedDataCache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await authTokenStore.GetAuthTokenAsync().ConfigureAwait(false);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // doing cache in middleware, check if the request method is GET, then check in the cache,
            // if there response for current requesturi already cahched then return it, if not forward to call the api, save it in cache, and return it
            if (request.Method == HttpMethod.Get)
            {
                // Create a unique key for the request based on the request URI
                var cacheKey = request.RequestUri?.ToString();
                var cachedResponseString = _sharedDataCache.GetData<string>(cacheKey??"");
                // Check if the response is already in the cache
                if (cachedResponseString != null)
                {
                    if (fancRADApiConfiguration.Log)
                    {
                        logger.LogInformation("Request from CHACHE: {Method} {Uri}", request.Method, request.RequestUri);

                        if (request.Content != null)
                        {
                            var requestBody = await request.Content.ReadAsStringAsync();
                            logger.LogInformation("Request Body: {Body}", requestBody);
                        }
                    }
                    var content = new StringContent(JsonConvert.DeserializeObject<string>(cachedResponseString));

                    // Create a new HttpResponseMessage with the deserialized content
                    var cachedResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = content
                    };
                    return cachedResponse;
                }
            }

            if (fancRADApiConfiguration.Log)
            {
                logger.LogInformation("Request: {Method} {Uri}", request.Method, request.RequestUri);

                if (request.Content != null)
                {
                    var requestBody = await request.Content.ReadAsStringAsync();
                    logger.LogInformation("Request Body: {Body}", requestBody);
                }
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (fancRADApiConfiguration.Log)
            {
                logger.LogInformation("Response: {StatusCode}", response.StatusCode);

                if (response.Content != null)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    logger.LogInformation("Response Body: {Body}", responseBody);
                }
            }

            // Check if request is GET and the response is successful before storing it in the caches
            if (request.Method == HttpMethod.Get && response.IsSuccessStatusCode)
            {
                var cacheKey = request.RequestUri?.ToString();
                if(!string.IsNullOrEmpty(cacheKey))
                {
                    string content = response.Content?.ReadAsStringAsync().Result;
                    var data = JsonConvert.SerializeObject(content);
                    _sharedDataCache.SetData(cacheKey, data);
                }
            }

            if ((request.Method == HttpMethod.Post || request.Method == HttpMethod.Put || request.Method == HttpMethod.Patch || request.Method == HttpMethod.Delete) && response.IsSuccessStatusCode)
            {
                _sharedDataCache.ClearAllData();
            }

            return response;
        }
    }
}

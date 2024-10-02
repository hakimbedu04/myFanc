using MyFanc.DTO.External.Authentication;
using Refit;

namespace MyFanc.Services.FancRadApi
{
    public interface IFancAuthentication
    {

        [Post("/connect/token")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<TokenResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> formValues);
    }
}

using MyFanc.DTO.External.RADApi;
using Refit;

namespace MyFanc.Services.FancRadApi
{
    [Headers("Authorization: Bearer")]
    public interface IFancRADApi
    {
        [Get("/Geo/countries")]
        Task<List<GetCountryInfoResult>> GetCountry();
        
        [Get("/Geo/countries/{query.CountryCode}")]
        Task<GetCountryInfoResult> GetCountryCode([Query()] GetCountryRequest query);

        [Get("/Geo/Cities")]
        Task<List<GetCityInfoResult>> GetCities();

        [Get("/Geo/Cities/{query.CityCode}")]
        Task<List<GetCityInfoResult>> GetCityByCode([Query()] GetCityRequest query);


        [Get("/Organisation/Enterprise/{query.Reference}")]
        Task<GetOrganisationEnterpriseInfoResult> GetOrganisationEnterpriseReference([Query()] GetOrganisationEnterpriseRequest query);
        
        [Post("/Organisation/Enterprise")]
        Task UpdateOrganisationEnterprise(UpdateOrganisationEnterpriseInfoRequest body);

        [Post("/Organisation/BusinessUnit")]
        Task UpdateOrganisationBusinessUnit(UpdateOrganisationBusinessUnitInfoRequest body);

        [Get("/Organisation/LegalForms")]
        Task<List<GetLegalFormInfoResult>> GetLegalForms();

        [Get("/Organisation/LegalForms/{query.Code}")]
        Task<List<GetLegalFormInfoResult>> GetLegalFormByCode([Query()] GetLegalFormRequest query);


        [Get("/User/{query.UserId}")]
        Task<GetUserInfoResult> GetUser([Query()] GetUserRequest query);

        [Post("/User/{query.UserId}")]
        Task UpdateUser(UpdateUserInfoRequest body, [Query()] UpdateUserRequest query);
        
        [Post("/User/{query.UserId}/VerifyEmail")]
        Task VerifyEmail([Query()] GetUserRequest query);

        [Get("/User/{query.UserId}/OrganisationLinks")]
        Task<GetOrganisationLinkInfoResult> GetUserOrganisationLinks([Query()] GetUserOrganisationLinksRequest query);

        [Post("/User/{query.UserId}/OrganisationLinks")]
        Task UpdateUserOrganisationLinks(UpdateOrganisationLinkInfoRequest body, [Query()] UpdateUserRequest query);

        [Delete("/User/{query.UserId}/OrganisationLinks")]
        [Headers("Content-Type: application/json")]
        Task DeleteUserOrganisationLinks([Body]UpdateOrganisationLinkInfoRequest body, [Query()] UpdateUserRequest query);
    }
}

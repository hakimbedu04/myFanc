using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Persona;

namespace MyFanc.BLL
{
    public partial interface IBll
    {
        Task<IEnumerable<CountryDTO>> GetCountriesAsync();
        Task<CountryDTO> GetCountriesCodeAsync(string countryCode);
        IEnumerable<SectorDTO> GetSectors(string languageCode, string nacabelCode, int pageSize);
        Task<IEnumerable<CityDTO>> GetCitiesAsync();
        Task<IEnumerable<CityDTO>> GetCitiesByCodeAsync(string cityCode);
        Task<IEnumerable<LegalFormDTO>> GetLegalFormsAsync();
        Task<IEnumerable<LegalFormDTO>> GetLegalFormsByCodeAsync(string code);
        IEnumerable<NacabelsCodeDTO> GetMappedSectors(IEnumerable<string> nacabelCode, string languageCode, string cbeNumber);
        PersonaCategoriesDTO GetPersonaCategories(int type, string languageCode);
    }
}

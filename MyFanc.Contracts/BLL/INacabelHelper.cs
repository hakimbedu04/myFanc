

using MyFanc.DTO.Internal.Data;

namespace MyFanc.Contracts.BLL
{
    public interface INacabelHelper
    {
        IEnumerable<string> GetDescription(IEnumerable<string> nacabelCode, string languageCode);
        IEnumerable<SectorDTO> GetSectors(string languageCode, string nacabelCode, int pageSize);
        Task InsertOrUpdateNacabelSector(IEnumerable<int> sectors, string enterpriseCBENumber);
        IEnumerable<NacabelsCodeDTO> GetMappedSectors(IEnumerable<string> nacabelCode, string languageCode, string cbeNumber);
    }
}

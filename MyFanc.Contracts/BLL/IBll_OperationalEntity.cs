using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.OperationalEntity;

namespace MyFanc.BLL
{
    public partial interface IBll
    {
        Task<IEnumerable<OperationalEntityDTO>> ListOperationalEntityAsync(string userId);
        Task DeleteUserFromOeAsync(string userId, string fancOrganisationId, string requestingUserId);
        Task CreateOeAsync(CreateOeDTO createOeDTO);
        Task<IEnumerable<OeUserDTO>> ListUserLinkedToOeAsync(string reference, string fancOrganisationId, bool includeMissingCbeBusinessUnits, string languageCode);
        Task SendInvitationMailAsync(SendInvitationDTO sendInvitationDTO);
        Task ActivateOeAsync(ActivateOeDTO activateOeDTO);
        Task<OrganisationEnterpriseDTO> GetOrganisationEnterpriseDetailAsync(string reference, bool includeMissingCbeBusinessUnits, string languageCode, bool forceUpdate);
        Task<OrganisationDefaultLanguangeDTO> GetDefaultLanguageForOrganisationByPostalCodeAsync(string postCode);

    }
}

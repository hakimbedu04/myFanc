using MyFanc.BusinessObjects;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Persona;
using MyFanc.DTO.Internal.Users;

namespace MyFanc.BLL
{
    public partial interface IBll
    {
        Task<ProfileDTO> UpdateUserAsync(string id, UpdateProfileDTO user);
        Task<ProfileDTO> GetUserInfoAsync(GetUserRequest userRequest);
        Task<AuthRedirectionDTO> GetAuthRedirectionAsync(string id, string? idp);
        Task<UserIdentityDTO> GetUserIdentityAsync(GetUserRequest userRequest, string? idp);
        Task<bool> ForceEmailVerificationAsync(string userId);
        Task<string> AcceptInvitation(string userId, UpdateOrganisationLinkInfoRequest userRequest);
        Task<string> RefuseInvitation(string userId, UpdateOrganisationLinkInfoRequest userRequest);
        Task<List<string>> GetUserRoles(GetUserOrganisationLinksRequest userRequest);
        Task<SelectorUserDTO> UpdateCurrentSelector(string userId, SelectorUserDTO userRequest);
        PersonaDTO GetUserCompanyPersona(string userId, Guid OrganisationFancId, string languageCode = "en");
        Task<string> SaveUserAndCompanyPersona(string userId,AddOrUpdatePersonaDTO param);
    }
}

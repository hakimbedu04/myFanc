using MyFanc.DTO.External.RADApi;

namespace MyFanc.DTO.Internal.Users
{
    public class UserOrganisationRoleDTO
    {
        public string FancOrganisationId { get; set; } = string.Empty;
        public IEnumerable<OrganisationRole> Roles { get; set; } = Enumerable.Empty<OrganisationRole>();
    }
}

using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.OperationalEntity;

namespace MyFanc.DTO.Internal.Users
{
    public class UserOrganisationsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<OrganisationRoleDTO> Roles { get; set; } = new List<OrganisationRoleDTO>();
        public List<EstablishmentDTO> Establishment {get;set; } = new List<EstablishmentDTO>();
    }
}

using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.OperationalEntity;

namespace MyFanc.DTO.Internal.Users
{
    public class EstablishmentDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<OrganisationRoleDTO> Roles { get; set; } = new List<OrganisationRoleDTO>();
    }
}

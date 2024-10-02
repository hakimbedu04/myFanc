using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.OperationalEntity
{
    public class OrganisationEstablishmentsUserDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<OrganisationRoleDTO> OrganisationalRoles { get; set; } = Enumerable.Empty<OrganisationRoleDTO>();
    }
}

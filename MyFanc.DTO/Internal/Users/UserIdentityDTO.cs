using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.OperationalEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Users
{
    public class UserIdentityDTO
    {
        public string? FullName { get; set; } = string.Empty;
        public string? InterfaceLanguageCode { get; set; } = string.Empty;
        public List<OrganisationRoleDTO> GlobalRoles { get; set; } = new List<OrganisationRoleDTO>();
        public bool IsValidated { get; set; }
        public bool EmailConfirmed { get; set; }
        public Guid CurrentOrganisation { get; set; }
        public Guid CurrentEstablishment { get; set; }
        public List<UserOrganisationsDTO> UserOrganisations { get; set; } = new List<UserOrganisationsDTO>();
    }
}

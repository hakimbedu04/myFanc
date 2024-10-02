using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Persona
{
    public class AddOrUpdatePersonaDTO
    {
        public AddOrUpdateUserDTO? UserPersona { get; set; }
        public AddOrUpdateCompanyDTO? CompanyPersona { get; set; }
    }

    public class AddOrUpdateUserDTO
    {
        public string InamiNumber { get; set; } = string.Empty;
        public List<int> Categories { get; set; } = new List<int>();
    }

    public class AddOrUpdateCompanyDTO
    {
        public Guid OrganisationId { get; set; } = Guid.Empty;
        public string NacabelCode { get; set; } = string.Empty;
        public List<int> Categories { get; set; } = new List<int>();
    }

    public class AddUserPersonaDTO
    {
        public string InamiNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
    }

    public class AddUserPersonaCategoriesDTO
    {
        public int PersonaCategoryId { get; set; }
        public int UserPersonaId { get; set; }
    }

    public class AddCompanyPersonaDTO
    {
        public Guid OrganisationFancId { get; set; }
        public string NacabelCode { get; set; } = string.Empty;
    }

    public class AddCompanyPersonaCategoriesDTO
    {
        public int PersonaCategoryId { get; set; }
        public int CompanyPersonaId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Persona
{
    public class CompanyPersonaDTO
    {
        public Guid OrganisationId { get; set; }
        public string NacebelCode { get; set; } = string.Empty;
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }
}

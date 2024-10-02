using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Persona
{
    public class UserPersonaDTO
    {
        public string UserPersonaId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string InamiNumber { get; set; } = string.Empty;
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }
}

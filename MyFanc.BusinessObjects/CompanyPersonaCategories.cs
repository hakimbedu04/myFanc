using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class CompanyPersonaCategories
    {
        public int CompanyPersonaId { get; set; }
        public int PersonaCategoryId { get; set; }
        public PersonaCategories? PersonaCategory { get; set; }
        public CompanyPersonas? CompanyPersonas { get; set; }
    }
}

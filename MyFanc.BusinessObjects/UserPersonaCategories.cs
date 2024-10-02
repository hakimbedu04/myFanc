using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class UserPersonaCategories
    {
        public int UserPersonaId { get; set; }
        public int PersonaCategoryId { get; set; }
        public virtual PersonaCategories? PersonaCategory { get; set; }
        public virtual UserPersonas? UserPersonas { get; set; }
    }
}

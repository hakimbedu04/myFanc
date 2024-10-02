using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class PersonaCategories : AuditedEntity
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? Type { get; set; }
        public int? NacabelId { get; set; }

        public virtual Nacabel? Nacebel { get; set; }
        public virtual PersonaCategories? Parent { get; set; }
        public virtual ICollection<PersonaCategories> Children { get; set; } = new HashSet<PersonaCategories>();
        public virtual ICollection<CompanyPersonas> CompanyPersonas { get; set; } = new HashSet<CompanyPersonas>();
        public virtual ICollection<Translation> Labels { get; set; } = new HashSet<Translation>();
        public virtual ICollection<UserPersonas> UserPersonas { get; set; } = new HashSet<UserPersonas>();
    }
}

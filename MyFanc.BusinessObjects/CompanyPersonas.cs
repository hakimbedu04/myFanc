using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class CompanyPersonas : AuditedEntity
    {
        public CompanyPersonas()
        {
            PersonaCategories = new HashSet<PersonaCategories>();
        }

        public int Id { get; set; }
        public Guid OrganisationFancId { get; set; }
        public string? NacabelCode { get; set; }
        public virtual ICollection<PersonaCategories> PersonaCategories { get; set; }
    }
}

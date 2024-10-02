using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class UserPersonas : AuditedEntity
    {
        public UserPersonas()
        {
            PersonaCategories = new HashSet<PersonaCategories>();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? InamiNumber { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<PersonaCategories> PersonaCategories { get; set; }

    }
}

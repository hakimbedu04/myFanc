using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyFanc.BusinessObjects.UserPersonas;

namespace MyFanc.BusinessObjects
{
    public class User : AuditedEntity
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = String.Empty;
        public bool IsCSAMUser { get; set; }
        public DateTime LatestConnection { get; set; }
        public DateTime LatestSynchronization { get; set; }
        public Guid? LatestOrganisation { get; set; }
        public Guid? LatestEstablishment { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; } = new HashSet<UserRoles>();
        public virtual ICollection<UserPersonas> UserPersonas { get; set; } = new HashSet<UserPersonas>();
    }
}

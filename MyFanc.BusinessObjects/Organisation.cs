using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class Organisation : AuditedEntity
    {
        public int OrganisationId { get; set; }
        public int? EstabishmentId { get; set; }
        public int UserId { get; set; }
    }
}

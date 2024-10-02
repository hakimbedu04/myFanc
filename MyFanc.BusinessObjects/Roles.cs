using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class Roles : AuditedEntity
    {
        public int Id { get; set; }
        public string ExternalRole { get; set; }
        public string InternalRole { get; set; }
    }
}

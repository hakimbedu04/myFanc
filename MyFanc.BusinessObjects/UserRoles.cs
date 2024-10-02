using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class UserRoles : AuditedEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string InternalRole { get; set; }
    }
}

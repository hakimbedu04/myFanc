using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class AuditedEntity
    {
        public DateTime CreationTime { get; set; }
        public int? CreatorUserId { get; set; }
        public DateTime? DeletedTime { get; set; }
        public int? DeleterUserId { get; set; }
        public DateTime? LatestUpdateTime { get; set; }
        public int? LatestUpdateUserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class FormCategory : AuditedEntity
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;

        public virtual ICollection<Form> Forms { get; set; } = new HashSet<Form>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class NacabelTranslation: AuditedEntity
    {
        public int Id { get; set; }
        public int NacabelId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Nacabel Nacabel { get; set; }

    }
}

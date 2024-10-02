using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class PersonaCategoryLabelsTranslations
    {
        public int PersonaCategoryId { get; set; }
        public int LabelsId { get; set; }
        public virtual Translation Labels { get; set; } = new Translation();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class QuestionBreadcrumb
    {

        public int Id { get; set; }

        public int QuestionId { get; set; }

        public ICollection<QuestionBreadcrumbItem> Items { get; set; } = new HashSet<QuestionBreadcrumbItem>();

        public Question? Question { get; set; }
    }
}

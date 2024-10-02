using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class QuestionBreadcrumbItem
    {
        public int Id { get; set; }

        public int BreadcrumbId { get; set; }

        public int QuestionId { get; set; }

        public int Order { get; set; }

        public bool IsALoop { get; set; }

        public QuestionBreadcrumb? Breadcrumb { get; set; }

        public Question? Question { get; set; }
    }
}

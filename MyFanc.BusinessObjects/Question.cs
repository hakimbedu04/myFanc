

using MyFanc.Core.Utility;
using static MyFanc.Core.Enums;

namespace MyFanc.BusinessObjects
{
    [Link(typeof(Question), nameof(Id))]
    public class Question : AuditedEntity
    {

        public int Id { get; set; }
        public int WizardId { get; set; }
        public bool IsFirstQuestion { get; set; }
        
        [DataProcessing(DataProcessingType.Breadcrumb, nameof(Id))]
        public bool IsActive { get; set; }

        public ICollection<Translation> Titles { get; set; } = new HashSet<Translation>();
        public ICollection<Translation> Texts { get; set; } = new HashSet<Translation>();
        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
        public ICollection<QuestionBreadcrumb> Breadcrumbs { get; set; } = new HashSet<QuestionBreadcrumb>();
        public ICollection<QuestionBreadcrumbItem> BreadcrumbsItems { get; set; } = new HashSet<QuestionBreadcrumbItem>();
        public Wizard? Wizard { get; set; }

        public ICollection<Answer> LinkedAnswer { get; set; } = new HashSet<Answer>();
    }
}

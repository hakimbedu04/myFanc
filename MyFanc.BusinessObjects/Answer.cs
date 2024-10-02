using MyFanc.Core.Utility;
using static MyFanc.Core.Enums;

namespace MyFanc.BusinessObjects
{
    [Link(typeof(Answer), nameof(Id))]
    public class Answer : AuditedEntity
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int Order { get; set; }
        public ICollection<Translation> Labels { get; set; } = new HashSet<Translation>();
        public ICollection<Translation> Details { get; set; } = new HashSet<Translation>();
        public string Tags { get; set; } = string.Empty;
        public bool IsFinalAnswer { get; set; }
        public ICollection<Translation> FinalAnswerTexts { get; set; } = new HashSet<Translation>();

        [DataProcessing(DataProcessingType.Breadcrumb, nameof(Id))]
        public int? LinkedQuestionId { get; set; }
        public ICollection<Translation> Links { get; set; } = new HashSet<Translation>();

        public Question? Question { get; set; }
        public Question? LinkedQuestion { get; set; } 
    }
}

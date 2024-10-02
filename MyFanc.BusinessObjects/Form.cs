using static MyFanc.Core.Enums;

namespace MyFanc.BusinessObjects
{
    public class Form : AuditedEntity
    {
        public Guid Id { get; set; }
        public int ExternalId { get; set; }
        public ICollection<Translation> Labels { get; set; } = new HashSet<Translation>();
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; } = FormType.Block.ToString();
        public FormStatus Status { get; set; } = FormStatus.Online;
        public int? FormCategoryId { get; set; }
        public ICollection<Translation> Descriptions { get; set; } = new HashSet<Translation>();
        public ICollection<Translation> Urls { get; set; } = new HashSet<Translation>();
        public ICollection<FormNodes> FormNodes { get; set; } = new HashSet<FormNodes>();
        public ICollection<FormDocument> FormDocuments { get; set; } = new HashSet<FormDocument>();
        public ICollection<Nacabel> Nacabels { get; set; } = new HashSet<Nacabel>();

        public virtual FormCategory? FormCategory { get; set; }
        public virtual ICollection<FormSubmission> FormSubmissions { get; set; } = new HashSet<FormSubmission>();
    }
}

using static MyFanc.Core.Enums;

namespace MyFanc.BusinessObjects
{
    public class FormSubmission : AuditedEntity
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public int Version { get; set; }
        public string Value { get; set; } = string.Empty;
        public string FancOrganisationId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public DateTime SubmissionDate { get; set; }
        public string CompanyType { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;   
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public FormSubmissionType FormSubmissionType { get; set; }

        public virtual Form Form{ get; set; }
    }
}

namespace MyFanc.BusinessObjects
{
    public class FormDocument : AuditedEntity
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;

        public virtual Form Form { get; set; }
        public virtual ICollection<Document> Documents { get; set; } = new HashSet<Document>();
    }
}

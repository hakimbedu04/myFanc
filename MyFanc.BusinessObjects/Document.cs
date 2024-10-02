using static MyFanc.Core.Enums;

namespace MyFanc.BusinessObjects
{
    public class Document : AuditedEntity
    {
        public Guid Id { get; set; }
        public Guid FormDocumentId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;

        public virtual FormDocument FormDocument { get; set; }
    }
}

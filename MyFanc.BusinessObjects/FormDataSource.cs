

namespace MyFanc.BusinessObjects
{
    public class FormDataSource : AuditedEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; } = 1;
    }
}

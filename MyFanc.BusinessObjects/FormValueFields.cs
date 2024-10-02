namespace MyFanc.BusinessObjects
{
    public class FormValueFields : AuditedEntity
    {
        public Guid Id { get; set; }
        public Guid FormNodeFieldId { get; set; }
        public string Value { get; set; } = string.Empty;
        public ICollection<Translation> Labels { get; set; } = new HashSet<Translation>();
        public int Order { get; set; }

        public virtual FormNodeFields FormNodeFields { get; set; }
    }
}

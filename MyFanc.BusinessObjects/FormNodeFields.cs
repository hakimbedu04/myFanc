using static MyFanc.Core.Enums;

namespace MyFanc.BusinessObjects
{
    public class FormNodeFields : AuditedEntity
    {
        public Guid Id { get; set; }
        public Guid FormNodeId { get; set; }
        public string Property { get; set; } = string.Empty;
        public ICollection<Translation> Labels { get; set; } = new HashSet<Translation>();
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = FormNodeFieldEncodeType.Text.ToString();

        public virtual FormNodes FormNodes { get; set; }
        public ICollection<FormValueFields> FormValueFields { get; set; } = new HashSet<FormValueFields>();
        public ICollection<FormConditionals> FormConditionals { get; set; } = new HashSet<FormConditionals>();

    }
}

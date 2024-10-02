using Microsoft.AspNetCore.Identity;
using static MyFanc.Core.Enums;

namespace MyFanc.BusinessObjects
{
    public class FormNodes : AuditedEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Translation> Labels { get; set; } = new HashSet<Translation>();
        public int Version { get; set; } = 1;
        public Guid FormId { get; set; }
        public Guid? ParentId { get; set; }
        public string Type { get; set; } = FormNodeType.FormBlock.ToString();
        public string? FieldType { get; set; } = FormNodeFieldType.Text.ToString();
        public int Order { get; set; }

        public virtual Form Form { get; set; }
        public virtual FormNodes? Parent { get; set; }
        public virtual ICollection<FormNodes> Children { get; set; }
        public ICollection<FormNodeFields> FormNodeFields { get; set; } = new HashSet<FormNodeFields>();
        public ICollection<FormConditionals> FormConditionals { get; set; } = new HashSet<FormConditionals>();
    }
}

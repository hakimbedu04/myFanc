namespace MyFanc.BusinessObjects
{
    public class FormConditionals : AuditedEntity
    {
        public Guid Id { get; set; }
        public Guid FormNodeFieldId { get; set; }
        public Guid FormNodeId { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        

        public virtual FormNodeFields FormNodeField { get; set; }
        public virtual FormNodes FormNode { get; set; }
    }
}

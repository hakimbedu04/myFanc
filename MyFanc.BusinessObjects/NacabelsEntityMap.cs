namespace MyFanc.BusinessObjects
{
    public class NacabelsEntityMap : AuditedEntity
    {
        public int Id { get; set; }
        public int NacabelId { get; set; }
        public string CbeNumber { get; set; } = string.Empty;

        public virtual Nacabel Nacabel { get; set;}
    }
}

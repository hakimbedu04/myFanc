namespace MyFanc.BusinessObjects
{
    public class Nacabel : AuditedEntity
    {
        public int Id { get; set; }
        public string NacabelCode { get; set; } = string.Empty;

        public virtual ICollection<NacabelTranslation> NacabelTranslation { get; set; } = new HashSet<NacabelTranslation>();
        public virtual ICollection<NacabelsEntityMap> NacabelsEntityMap { get; set; } = new HashSet<NacabelsEntityMap>();
        public virtual ICollection<Form> Forms { get; set; } = new HashSet<Form>();
        public virtual ICollection<PersonaCategories> PersonaCategories { get; set; } = new HashSet<PersonaCategories>();
    }
}

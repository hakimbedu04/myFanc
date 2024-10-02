namespace MyFanc.DTO.Internal.Data
{
    public class LegalFormDTO
    {
        public string Code { get; set; } = string.Empty;
        public string DescriptionFR { get; set; } = string.Empty;
        public string DescriptionNL { get; set; } = string.Empty;
        public string DescriptionDE { get; set; } = string.Empty;
        public string DescriptionEN { get; set; } = string.Empty;
        public string AbbreviationFR { get; set; } = string.Empty;
        public string AbbreviationNL { get; set; } = string.Empty;
        public string AbbreviationDE { get; set; } = string.Empty;
        public string AbbreviationEN { get; set; } = string.Empty;
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

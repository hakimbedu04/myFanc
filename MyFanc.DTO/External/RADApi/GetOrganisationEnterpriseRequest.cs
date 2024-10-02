namespace MyFanc.DTO.External.RADApi
{
    public class GetOrganisationEnterpriseRequest
    {
        public string Reference { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = "en";
        public bool IncludeMissingCbeBusinessUnits { get; set; }
    }
}

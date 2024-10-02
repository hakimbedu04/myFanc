namespace MyFanc.DTO.External.RADApi
{
    public class GetOrganisationEnterpriseInfoResult : OrganisationInfoBase
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public UserInfoAddress? MainAddress { get; set; } = new UserInfoAddress();
        public UserInfoAddress? InvoiceAddress { get; set; } = new UserInfoAddress();
        public IEnumerable<string> Nacebel2008Codes { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<OrganisationEstablishment> Establishments { get; set; } = Enumerable.Empty<OrganisationEstablishment>();
    }
}

namespace MyFanc.DTO.External.RADApi
{
    public class OrganisationEstablishment : OrganisationInfoBase
    {
        public string BusinessUnitCBENumber { get; set; } = string.Empty;
        public UserInfoAddress? MainAddress { get; set; } = new UserInfoAddress();
        public UserInfoAddress? InvoiceAddress { get; set; } = new UserInfoAddress();
        public IEnumerable<OrganisationUser> Users { get; set; } = Enumerable.Empty<OrganisationUser>();
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public IEnumerable<string> Nacebel2008Codes { get; set; } = Enumerable.Empty<string>();
    }
}

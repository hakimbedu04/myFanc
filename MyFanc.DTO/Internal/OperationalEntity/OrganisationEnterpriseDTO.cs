using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Users;

namespace MyFanc.DTO.Internal.OperationalEntity
{
    public class OrganisationEnterpriseDTO
    {
        public string FancOrganisationId { get; set; } = string.Empty;
        public string FancNumber { get; set; } = string.Empty;
        public string EnterpriseCBENumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public ProfileInfoAddressDTO? MainAddress { get; set; } = new ProfileInfoAddressDTO();
        public ProfileInfoAddressDTO? InvoiceAddress { get; set; } = new ProfileInfoAddressDTO();
        public bool Activated { get; set; }
        public IEnumerable<string> Nacebel2008Codes { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<OrganisationEstablishmentsDTO> Establishments { get; set; } = Enumerable.Empty<OrganisationEstablishmentsDTO>();
        //public IEnumerable<string> Sectore { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<NacabelsCodeDTO> Sectors { get; set; } = Enumerable.Empty<NacabelsCodeDTO>();
        public bool MainAddressIsInvoiceAddress { get; set; }
        public string DefaultLanguageCode { get; set; } = string.Empty;
    }
}

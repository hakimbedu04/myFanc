using MyFanc.DTO.Internal.Users;

namespace MyFanc.DTO.Internal.LegalEntity
{
    public class ActivateLeDTO
    {
        public string? FancOrganisationId { get; set; } = string.Empty;
        public string EnterpriseCBENumber { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public bool MainAddressIsInvoiceAddress { get; set; }
        public ProfileInfoAddressDTO? InvoiceAddress { get; set; } = new ProfileInfoAddressDTO();
        public IEnumerable<int> Sectors { get; set; } = Enumerable.Empty<int>();
        public string DefaultLanguageCode { get; set; } = string.Empty;
    }
}

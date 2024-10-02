using MyFanc.DTO.Internal.Users;

namespace MyFanc.DTO.Internal.OperationalEntity
{
    public class CreateOeDTO
    {
        public string? FancOrganisationId { get; set; } = string.Empty;
        public string EnterpriseCBENumber { get; set; } 
        public string? BusinessUnitCBENumber { get;set; } = string.Empty;
        public bool DeclaredToCBE { get; set; }
        public string? ReasonForRegistrationBeforeCBEFinalization { get; set; } = string.Empty;
        public string Name { get; set; }
        public string? DisplayName { get; set; }
        public ProfileInfoAddressDTO MainAddress { get; set; } = new ProfileInfoAddressDTO();
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public bool MainAddressIsInvoiceAddress { get; set; }
        public ProfileInfoAddressDTO InvoiceAddress { get; set; } = new ProfileInfoAddressDTO();
        public string? DefaultLanguageCode { get; set; } = string.Empty;

        //not sure this field in ui is showed, but in post rad api parameter not included
        //public string DefaultLanguaage { get; set; } = string.Empty;
    }

    public class ActivateOeDTO : CreateOeDTO {
        public IEnumerable<int> Sectors { get; set; } = Enumerable.Empty<int>();
    }
}

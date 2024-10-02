using static MyFanc.Core.Enums;

namespace MyFanc.DTO.Internal.Users
{
    public class ProfileDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? NrNumber { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? LanguageCode { get; set; } = string.Empty;
        public string? InterfaceLanguageCode { get; set; } = string.Empty;
        public string? GenderCode { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? BirthPlace { get; set; } = string.Empty;
        public ProfileInfoAddressDTO? StructuredAddress { get; set; } = new ProfileInfoAddressDTO();
        public ProfileInfoAddressUnstructuredDTO? UnstructuredAddress { get; set; } = new ProfileInfoAddressUnstructuredDTO();
        public string? Email { get; set; } = string.Empty;
        public string? Phone1 { get; set; } = string.Empty;
        public string? Phone2 { get; set; } = string.Empty;
        public DateTime? LastAuthenticSourceRefreshDate { get; set; }
        public bool? ManualUpdateAllowed { get; set; }
        public ValidationStatus ValidationStatus { get; set; } = ValidationStatus.Pending;
        public string NationalityCode { get; set; } = string.Empty;
        public string ForeignIdentityOrPassportNumber { get; set; } = string.Empty;
    }
}

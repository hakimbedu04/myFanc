using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    public class GetUserInfoResult
    {
        public string? NrNumber { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? LanguageCode { get; set; } = string.Empty;
        public string? InterfaceLanguageCode { get; set; } = string.Empty;
        public string? GenderCode { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? BirthPlace { get; set; } = string.Empty;
        public UserInfoAddress? StructuredAddress { get; set; } = new UserInfoAddress();
        public UserInfoAddressUnstructured? UnstructuredAddress { get; set; } = new UserInfoAddressUnstructured();
        public string? Email { get; set; } = string.Empty;
        public string? Phone1 { get; set; } = string.Empty;
        public string? Phone2 { get; set; } = string.Empty;
        public DateTime? LastAuthenticSourceRefreshDate { get; set; }
        public bool? ManualUpdateAllowed { get; set; }
        public List<string>? AllowedLanguageCodes { get; set; } = new List<string>();
        public string NationalityCode { get;set; } = string.Empty;
        public string ForeignIdentityOrPassportNumber { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        public bool PrivacyDeclatationAccepted { get; set; } = false;
        public bool UserIsValidated { get; set; } = false;
    }

}

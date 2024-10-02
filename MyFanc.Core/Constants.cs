using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MyFanc.Core
{
    public static class Constants
    {
        public const string BelgianCountryCode = "BE";
        public const string HomeTagEn = "Home";
        public const string LangauageCodeEN = "en";
        public const string JwtIdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";
        public const string JwtSubUserId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
		public const string GlobalRolesBasicUser = "BasicUser";
        public const string GlobalRolesAdmin = "Admin";
        public const string RoleManager = "manager";
    }
}

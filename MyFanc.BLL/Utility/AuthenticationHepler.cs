using Microsoft.AspNetCore.Http;
using MyFanc.Contracts.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BLL.Utility
{
    public class AuthenticationHepler : IAuthenticationHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationHepler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetConnectedUserId()
        {
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (!Int32.TryParse(userId, out int intId))
            {
                return (int?)null;
            }

            return intId;
        }

        public bool HasPermission(string action)
        {
            bool permissionAllowed = false;
            var user = _httpContextAccessor?.HttpContext?.User;
            if (user != null)
            {
                var claims = user.Claims?.ToList();
                var permissionsString = claims?.FirstOrDefault(x => x.Type == "Permissions");
                if (permissionsString != null)
                {
                    var permissions = permissionsString.Value.Split(";");
                    permissionAllowed = permissions.Any(x => x == action);
                }
            }
            return permissionAllowed;
        }

        public bool HasRole(string role)
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            if (user != null)
            {
                var claims = user.Claims?.ToList();
                var rolesString = claims?.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
                if (rolesString != null && rolesString == role)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

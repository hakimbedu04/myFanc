using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Contracts.BLL
{
    public interface IAuthenticationHelper
    {
        int? GetConnectedUserId();
        bool HasPermission(string action);
        bool HasRole(string role);
    }
}

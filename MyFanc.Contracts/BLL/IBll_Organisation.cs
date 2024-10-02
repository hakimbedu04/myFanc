using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BLL
{
    public partial interface IBll
    {
        Task StoreUserOrganisation(string userId);
    }
}

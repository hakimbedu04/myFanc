using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    public class UpdateOrganisationLinkInfoRequest
    {
        public string FancOrganisationId { get; set; } = string.Empty;
        //this commented because in current RAD API swager now EnterpriceCBENumber & BusinessUnitCBENumber not needed again
        //public string EnterpriceCBENumber { get; set; } = string.Empty;
        //public string BusinessUnitCBENumber { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
        public string RequestingUserId { get; set; } = string.Empty;
    }
}

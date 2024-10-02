using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    public class OrganisationLinkEstablishment: OrganisationInfoBase
    {
        public string BusinessUnitCBENumber { get; set; } = string.Empty;
        public IEnumerable<OrganisationRole> Roles { get; set; } = Enumerable.Empty<OrganisationRole>();
    }
}

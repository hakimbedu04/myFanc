using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    public class OrganisationLink: OrganisationInfoBase
    {
        public IEnumerable<OrganisationRole> Roles { get; set; } = Enumerable.Empty<OrganisationRole>();
        public IEnumerable<OrganisationLinkEstablishment> Establishments { get; set; } = Enumerable.Empty<OrganisationLinkEstablishment>();

    }
}

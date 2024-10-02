using MyFanc.DTO.External.RADApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Data
{
    public class OrganisationDTO
    {
        public int OrganisationId { get; set; }
        public UserInfoAddress? MainAddress { get; set; } = new UserInfoAddress();
        public UserInfoAddress? InvoiceAddress { get; set; } = new UserInfoAddress();
        public IEnumerable<string> Nacebel2008Codes { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<OrganisationEstablishment> Establishments { get; set; } = Enumerable.Empty<OrganisationEstablishment>();
    }
}

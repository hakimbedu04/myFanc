using MyFanc.DTO.Internal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    /*public class OrganisationInfo: OrganisationInfoBase
    {  
       /public bool Activated { get; set; }
    }*/
    public class OrganisationInfoBase
    {
        public string DisplayName { get; set; } = string.Empty;
        public bool Activated { get; set; }
        public string FancOrganisationId { get; set; } = string.Empty;
        public string FancNumber { get; set; } = string.Empty;
        public string EnterpriseCBENumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DefaultLanguageCode { get; set; } = string.Empty;
        public IEnumerable<string> Nacabels { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<NacabelsCodeDTO> Sectors { get; set; } = Enumerable.Empty<NacabelsCodeDTO>();
    }
}

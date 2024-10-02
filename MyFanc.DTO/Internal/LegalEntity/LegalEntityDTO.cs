using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.LegalEntity
{
    public class LegalEntityDTO
    {
        public string Id { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string VATNumber { get; set; } = string.Empty;
        public bool? Activated { get; set; }
        public string DefaultLanguageCode { get; set; } = string.Empty;
        public IEnumerable<string> Nacabels { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<NacabelsCodeDTO> Sectors { get; set; } = Enumerable.Empty<NacabelsCodeDTO>();
    }
}

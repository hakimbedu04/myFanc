using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Data
{
    public class CityDTO
    {
        public int NisCode { get; set; }
        public int PostCode { get; set; }
        public string OfficialLangCode1 { get; set; } = string.Empty;
        public string OfficialLangCode2 { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public IEnumerable<CityNameInfoDTO> Names { get; set; } = Enumerable.Empty<CityNameInfoDTO>();

    }

    public class CityNameInfoDTO
    {
        public string LangCode { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}

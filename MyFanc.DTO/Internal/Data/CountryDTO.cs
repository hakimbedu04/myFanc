using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Data
{
    public class CountryDTO
    {
        public string CountryCode { get; set; } = string.Empty;
        public string IsoAlpha3Code { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string ContinentCode { get; set; } = string.Empty;
        public string ContinentName { get; set;} = string.Empty;
    }
}

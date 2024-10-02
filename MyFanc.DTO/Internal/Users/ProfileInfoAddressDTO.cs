using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Users
{
    public class ProfileInfoAddressDTO
    {
        public string? CountryCode { get; set; } = string.Empty;
        public string? StreetName { get; set; } = string.Empty;
        public string? HouseNumber { get; set; } = string.Empty;
        public string? PostalCode { get; set; } = string.Empty;
        public string? CityName { get; set; } = string.Empty;
    }
}

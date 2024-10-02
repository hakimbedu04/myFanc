using MyFanc.DTO.Internal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    public class UpdateOrganisationEnterpriseInfoRequest
    {
        public string? FancOrganisationId { get; set; } = string.Empty;
        public string EnterpriseCBENumber { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;  
        public bool MainAddressIsInvoiceAddress { get; set; }
        public UserInfoAddress? InvoiceAddress { get; set;} = new UserInfoAddress();
        public IEnumerable<int> Tags { get; set; } = Enumerable.Empty<int>();
        public string DefaultLanguageCode { get; set; } = string.Empty;
    }
}

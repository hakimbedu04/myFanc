using MyFanc.DTO.Internal.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.OperationalEntity
{
    public class OrganisationEstablishmentsDTO
    {
        public string FancOrganisationId { get; set; } = string.Empty;
        public string FancNumber { get; set; } = string.Empty;
        public string EnterpriseCBENumber { get; set; } = string.Empty;
        public string BusinessUnitCBENumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ProfileInfoAddressDTO? MainAddress { get; set; } = new ProfileInfoAddressDTO();
        public ProfileInfoAddressDTO? InvoiceAddress { get; set; } = new ProfileInfoAddressDTO();
        public bool Activated { get; set; }
        public IEnumerable<OrganisationEstablishmentsUserDTO> Users { get; set; } = Enumerable.Empty<OrganisationEstablishmentsUserDTO>();
        public bool MainAddressIsInvoiceAddress { get; set; }
        public string DefaultLanguageCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public IEnumerable<string> Nacebel2008Codes { get; set; } = Enumerable.Empty<string>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Users
{
    public class SelectorUserDTO
    {
        public Guid? OrganisationId { get; set; }
        public Guid? EstablishmentId { get; set; }
    }
}

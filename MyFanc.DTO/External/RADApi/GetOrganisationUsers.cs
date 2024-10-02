using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    public class GetOrganisationUsers
    {
        public int OrganisationId { get; set; }
        public int? EstabishmentId { get; set; }
        public string UserId { get; set; }
    }
}

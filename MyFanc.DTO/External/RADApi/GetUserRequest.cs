using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    public class GetUserRequest
    {
        public string UserId { get; set; } = string.Empty;
        public bool ForceUpdate { get; set; } = false;
    }
}

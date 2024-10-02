using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.Authentication
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string ExpiresIn { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public string TokenType { get; set; } = string.Empty;
    }
}

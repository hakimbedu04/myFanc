using MyFanc.Core;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyFanc.Core.Enums;

namespace MyFanc.DTO.Internal.Forms
{
    public class PublishFormDTO
    {
        [SwaggerSchema("Fill with form externalid")]
        public int ExternalId { get; set; }
        [SwaggerSchema("Fill with form current status (Draft, Online, Offline)")]
        public string Status { get; set; } = Enums.FormStatus.Online.ToString();
    }
}

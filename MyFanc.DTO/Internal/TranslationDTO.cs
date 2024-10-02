using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Translation
{
    public class TranslationDTO
    {
        [SwaggerSchema("For add a new translation please fill Id with 'null', and for update tranlation fill Id with Id of edited translation")]
        public int? Id { get; set; }
        [SwaggerSchema("Fill with language code, ex: en, fr, nl, etc")]
        public string? LanguageCode { get; set; }
        [SwaggerSchema("Fill with text translation")]
        public string? Text { get; set; }
    }
}
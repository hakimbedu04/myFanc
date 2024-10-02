using MyFanc.DTO.Internal.Translation;
using Swashbuckle.AspNetCore.Annotations;
using static MyFanc.Core.Enums;

namespace MyFanc.DTO.Internal.Forms
{
    public class CreateFormBlockDTO
    {
        [SwaggerSchema("Fill with label of form block")]
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        [SwaggerSchema("For form block please ignore field version but for standarisation please use 1 as version, since for form block is not version able")]
        public int Version { get; set; }
        public bool IsActive { get; set; }
    }
}

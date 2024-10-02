using MyFanc.Core;
using MyFanc.DTO.Internal.Translation;
using Swashbuckle.AspNetCore.Annotations;

namespace MyFanc.DTO.Internal.Forms
{
    public class AddFormBlockToFormDTO
    {
        [SwaggerSchema("Fill with FormBLockId that will added to the form")]
        public Guid FormBlockId { get; set; }
        [SwaggerSchema("Fill with SectionId on which the form block will added")]
        public Guid SectionId { get; set; }
        [SwaggerSchema("Fill with FormId on which new formblock will added")]
        public Guid FormId { get; set; }
    }
}

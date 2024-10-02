

using MyFanc.DTO.Internal.Translation;
using static MyFanc.Core.Enums;

namespace MyFanc.DTO.Internal.Forms
{
    public class FormBlockDTO
    {
        public Guid Id { get; set; }
        public int ExternalId { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; } = FormType.Block.ToString();
        public string Status { get; set; } = FormStatus.Online.ToString();
    }
}

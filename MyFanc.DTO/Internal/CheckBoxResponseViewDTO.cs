
using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal
{
    public class CheckBoxResponseViewDto
    {
        public Guid Id { get; set; }
        public string? Value { get; set; }
        public ICollection<TranslationDTO>? Labels { get; set; }
    }
}

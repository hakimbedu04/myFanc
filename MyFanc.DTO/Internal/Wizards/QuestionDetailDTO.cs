using MyFanc.DTO.Internal.Translation;
using MyFanc.DTO.Internal.Wizard;

namespace MyFanc.DTO.Internal.Wizards
{
    public class QuestionDetailDTO : QuestionDTO
    {
        public IEnumerable<TranslationDTO> Texts { get; set; } = Enumerable.Empty<TranslationDTO>();
    }
}

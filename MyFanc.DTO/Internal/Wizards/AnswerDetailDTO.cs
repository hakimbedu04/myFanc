using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Wizards
{
    public class AnswerDetailDTO : AnswerDTO
    {
        public IEnumerable<TranslationDTO> Details { get; set; } = Enumerable.Empty<TranslationDTO>();
    }
}

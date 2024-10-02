using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Wizard
{
    public class CreateQuestionDTO
    {
        public int WizardId { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = Enumerable.Empty<TranslationDTO>();
        public IEnumerable<TranslationDTO> Texts { get; set; } = Enumerable.Empty<TranslationDTO>();
        public bool IsFirstQuestion { get; set; }
    }
}

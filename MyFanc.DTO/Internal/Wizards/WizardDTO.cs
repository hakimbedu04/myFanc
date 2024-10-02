using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Wizards
{
    public class WizardDTO
    {
        public int Id { get; set; }
        public IEnumerable<TranslationDTO> Titles { get; set; } = Enumerable.Empty<TranslationDTO>();
        public IEnumerable<TranslationDTO> IntroductionTexts { get; set; } = Enumerable.Empty<TranslationDTO>();
        public bool HasFirstQuestion { get; set; }
    }
}

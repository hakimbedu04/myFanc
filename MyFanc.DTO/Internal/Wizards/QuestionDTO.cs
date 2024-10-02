using MyFanc.DTO.Internal.Translation;
using MyFanc.DTO.Internal.Wizard;

namespace MyFanc.DTO.Internal.Wizards
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public int WizardId { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = Enumerable.Empty<TranslationDTO>();
        public bool IsFirstQuestion { get; set; }
        public bool IsActive { get; set; }
        public int AnswersCount { get; set; }
    }
}

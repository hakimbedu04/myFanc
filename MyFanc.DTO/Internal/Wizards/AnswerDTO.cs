using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Wizards
{
    public class AnswerDTO
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int Order { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = Enumerable.Empty<TranslationDTO>();
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        public int? LinkedQuestionId { get; set; }
        public IEnumerable<TranslationDTO> Links { get; set; } = Enumerable.Empty<TranslationDTO>();
        public bool IsFinalAnswer { get; set; }
        public IEnumerable<TranslationDTO> FinalAnswerTexts { get; set; } = Enumerable.Empty<TranslationDTO>();
    }
}

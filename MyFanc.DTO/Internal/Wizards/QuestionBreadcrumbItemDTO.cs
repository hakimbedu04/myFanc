using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Wizard
{
    public class QuestionBreadcrumbItemDTO
    {
        public int QuestionId { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = Enumerable.Empty<TranslationDTO>();
        public bool IsALoop { get; set; }
    }
}

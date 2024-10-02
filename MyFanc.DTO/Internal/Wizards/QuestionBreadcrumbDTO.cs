using MyFanc.DTO.Internal.Wizards;

namespace MyFanc.DTO.Internal.Wizard
{
    public class QuestionBreadcrumbDTO
    {
        public IEnumerable<QuestionBreadcrumbItemDTO> Items { get; set; } = Enumerable.Empty<QuestionBreadcrumbItemDTO>();
    }
}

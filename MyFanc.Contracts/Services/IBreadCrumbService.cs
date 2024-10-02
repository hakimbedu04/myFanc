using MyFanc.BusinessObjects;
using MyFanc.Contracts.DAL;
using MyFanc.DTO.Internal.Wizard;

namespace MyFanc.Contracts.Services
{
    public interface IBreadCrumbService
    {
        Task<IEnumerable<QuestionBreadcrumbDTO>> ProcessBreadcrumbs(IUnitOfWork unitOfWork, Question question, HashSet<int> visitedQuestion = default);
        Task<bool> CheckQuestionPath(IUnitOfWork unitOfWork, Question question, HashSet<int> visitedQuestion = default);
    }
}

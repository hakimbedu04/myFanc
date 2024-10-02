using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.DTO.Internal.Wizard;

namespace MyFanc.Services
{
    public class BreadCrumbService : IBreadCrumbService
    {
        private readonly IMapper _mapper;

        public BreadCrumbService(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public virtual async Task<IEnumerable<QuestionBreadcrumbDTO>> ProcessBreadcrumbs(IUnitOfWork unitOfWork, Question question, HashSet<int> visitedQuestion = default)
        {
            if (visitedQuestion == default)
                visitedQuestion = new HashSet<int>();

            IEnumerable<Question> questionsWithLinkedAnswers = GetQuestionsWithLinkedAnswers(unitOfWork, question);

            visitedQuestion.Add(question.Id);

            List<QuestionBreadcrumbDTO> allQuestionBreadcrumbs = new List<QuestionBreadcrumbDTO>();

            foreach (var linkedQuestion in questionsWithLinkedAnswers)
            {
                var parents = questionsWithLinkedAnswers = GetQuestionsWithLinkedAnswers(unitOfWork, linkedQuestion);
                bool isLoop = IsLoop(linkedQuestion, visitedQuestion);
                var breadcrumbItem = new QuestionBreadcrumbItem
                {
                    QuestionId = linkedQuestion.Id,
                    IsALoop = isLoop,
                    Question = linkedQuestion
                };
                var newBreadcrumb = new QuestionBreadcrumb()
                {
                    QuestionId = linkedQuestion.Id,
                    Items = new List<QuestionBreadcrumbItem> { breadcrumbItem },
                    Question = linkedQuestion
                };
                var output = _mapper.Map<QuestionBreadcrumbDTO>(newBreadcrumb);
                if (linkedQuestion.IsFirstQuestion)
                {
                    allQuestionBreadcrumbs.Add(output);
                }
                else
                {
                    if (parents.Any())
                    {
                        if (visitedQuestion.Contains(linkedQuestion.Id))
                        {
                            AddQuestionHasVisited(allQuestionBreadcrumbs, output);
                        }
                        else
                        {
                            var resultBreadcrumb = await ProcessBreadcrumbs(unitOfWork, linkedQuestion, visitedQuestion);
                            ReArrangeNewBreadcrumb(allQuestionBreadcrumbs, newBreadcrumb, output, resultBreadcrumb);
                        }
                    }
                }
            }
            return allQuestionBreadcrumbs;
        }

        private QuestionBreadcrumbDTO ReArrangeNewBreadcrumb(List<QuestionBreadcrumbDTO> allQuestionBreadcrumbs, QuestionBreadcrumb newBreadcrumb, QuestionBreadcrumbDTO output, IEnumerable<QuestionBreadcrumbDTO> resultBreadcrumb)
        {
            foreach (var bradcrumb in resultBreadcrumb)
            {
                output = _mapper.Map<QuestionBreadcrumbDTO>(newBreadcrumb);
                var newBradcrumb = new List<QuestionBreadcrumbItemDTO>();
                newBradcrumb.AddRange(bradcrumb.Items);
                newBradcrumb.AddRange(output.Items);
                output.Items = newBradcrumb;
                allQuestionBreadcrumbs.Add(output);
            }

            return output;
        }

        private static void AddQuestionHasVisited(List<QuestionBreadcrumbDTO> allQuestionBreadcrumbs, QuestionBreadcrumbDTO output)
        {
            allQuestionBreadcrumbs.Add(output);
        }

        private static bool IsLoop(Question question, HashSet<int> visitedQuestions)
        {
            foreach (var breadcrumbItem in question.Breadcrumbs)
            {
                if (visitedQuestions.Contains(breadcrumbItem.QuestionId))
                {
                    return true;
                }
            }
            return visitedQuestions.Contains(question.Id);
            //return false;
        }

        private IQueryable<Question> GetQuestionsWithLinkedAnswers(IUnitOfWork unitOfWork, Question question)
        {
            var _questionRepository = unitOfWork.GetGenericRepository<Question>();
            var linkedQuestion = _questionRepository.Find(q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == question.Id))
                .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                .Include(q => q.Titles)
                .Include(q => q.Breadcrumbs)
                .Include(q => q.BreadcrumbsItems)
                .AsTracking();
            return linkedQuestion;
        }

        public async Task<bool> CheckQuestionPath(IUnitOfWork unitOfWork, Question question, HashSet<int> visitedQuestion = default)
        {
            if (visitedQuestion == default)
                visitedQuestion = new HashSet<int>();
            IEnumerable<Question> questionsWithLinkedAnswers = GetQuestionsWithLinkedAnswers(unitOfWork, question);

            foreach (var linkedQuestion in questionsWithLinkedAnswers)
            {
                if (linkedQuestion.IsFirstQuestion) return true;
                var parents = questionsWithLinkedAnswers = GetQuestionsWithLinkedAnswers(unitOfWork, linkedQuestion);
                if (parents.Any())
                {
                    if (!visitedQuestion.Contains(linkedQuestion.Id))
                    {
                        var checkResult = await CheckQuestionPath(unitOfWork, linkedQuestion, visitedQuestion);
                        return checkResult;
                    }
                }
            }
            return false;
        }
    }
}

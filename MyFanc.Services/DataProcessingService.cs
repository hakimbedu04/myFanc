
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.Core.Utility;
using MyFanc.DTO.Internal.Wizard;
using MyFanc.DTO.Internal.Wizards;
using System.Threading.Tasks;
using static MyFanc.Core.Enums;

namespace MyFanc.Services
{
    public class DataProcessingService : IDataProcessingService
    {
        private readonly ILogger<DataProcessingService> _logger;
        private readonly IBreadCrumbService _breadCrumbService;

        public DataProcessingService(ILogger<DataProcessingService> logger, IBreadCrumbService breadCrumbService)
        {
            _logger = logger;
            _breadCrumbService = breadCrumbService;
        }

        public IUnitOfWork UnitOfWork { get; set; }

        public async Task<int> Process(Dictionary<EntityAttributeKey, List<DataProcessingType>> calculationsByEntity)
        {
            foreach (var calculations in calculationsByEntity)
            {
                foreach (var calcul in calculations.Value.Distinct().OrderBy(c => c))
                {
                    
                    if(calcul == DataProcessingType.Breadcrumb){
                        if (calculations.Key.Entity is Question)
                            CalculateBreadcrumb((int)calculations.Key.Id);
                        if (calculations.Key.Entity is Answer answer)
                            CalculateBreadcrumb((int)answer.QuestionId);
                      
                    }
                }
            }
            await UnitOfWork.SaveChangesAsync();
            return 1;
        }

        public async Task<int> ApplyLink(Dictionary<EntityAttributeKey, List<Type>> typesByEntity)
        {
            foreach (var links in typesByEntity)
            {
                foreach (var linkedType in links.Value)
                {
                    if (linkedType == typeof(Question))
                    {
                        Question question = (Question) links.Key.Entity;
                        if (links.Key.Entity is Question && links.Key.State == EntityState.Modified)
                        {
                            CalculateBreadcrumb((int)links.Key.Id != 0 ? (int)links.Key.Id : question.Id);
                        }
                        else if (links.Key.Entity is Question && links.Key.State == EntityState.Deleted)
                        {
                            var repoAnswer = UnitOfWork.GetGenericRepository<Answer>();
                            var answers = repoAnswer.Find(a => a.LinkedQuestionId == question.Id);
                            if (answers != null && answers.Count() > 0)
                            {
                                foreach (var answer in answers)
                                {
                                    CalculateBreadcrumb(answer.QuestionId);
                                }
                            }
                        }
                    }
                    if (linkedType == typeof(Answer))
                    {
                        if (links.Key.Entity is Answer answer)
                        {
                            if(answer.LinkedQuestionId.HasValue)
                                CalculateBreadcrumb(answer.LinkedQuestionId??0);
                        }
                    }
                }
            }

            await UnitOfWork.SaveChangesAsync();
            return 1;
        }

        private void CalculateBreadcrumb(int questionId)
        {
            RecalculateBreadCrumb(questionId).GetAwaiter().GetResult();
        }

        private async Task RecalculateBreadCrumb(int questionId)
        {
            var questionRepo = UnitOfWork.GetGenericRepository<Question>();
            var question = questionRepo.Find(x => x.Id == questionId).FirstOrDefault();
            if (question != null)
            {
                var wizardRepo = UnitOfWork.GetGenericRepository<Wizard>();
                var wizard = wizardRepo.Find(wizard => wizard.Id == question.WizardId)
                    .Include(wizard => wizard.Questions)
                        .ThenInclude(question => question.Answers)
                     .FirstOrDefault();
                await _breadCrumbService.ProcessBreadcrumbs(UnitOfWork, question, new HashSet<int>());
            }
            else
            {
                _logger.LogDebug($"Question with ID {questionId} not found in the wizard.");
                return;
            }
            
        }
    }
}

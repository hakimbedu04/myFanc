using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFanc.BusinessObjects;
using MyFanc.DTO.Internal.Translation;
using MyFanc.DTO.Internal.Wizard;
using MyFanc.DTO.Internal.Wizards;
using MyFanc.Services;
using System.Linq;
using Azure.Identity;
using MyFanc.DAL;

namespace MyFanc.BLL
{
    public partial class Bll : IBll
    {
        public async Task<WizardDTO> EditWizardAsync(WizardDTO wizardDto)
        {
            var wizardEntity = await GetWizardOrFirstAsync(wizardDto.Id).ConfigureAwait(false);
            _mapper.Map(wizardDto, wizardEntity);
            try
            {
                ApplyChangeOnTranslation(wizardDto.Titles, wizardEntity.Titles);
            }catch(Exception ex)
            {
                _logger.LogError("Apply change of the Wizard title translation has failed for an unknown reason {0}",ex);
                throw new Exception("no change has been commited, Wizard not written");
            }
            try
            {
                ApplyChangeOnTranslation(wizardDto.IntroductionTexts, wizardEntity.IntroductionTexts);
            }
            catch (Exception ex)
            {
                _logger.LogError("Apply change of the Wizard Introduction text translation has failed for an unknown reason {0}", ex);
                throw new Exception("no change has been commited, Wizard not written");
            }
            var savedEntries = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            if (savedEntries <= 0)
            {
                _logger.LogError($"The insert of the Wizard has failed for an unknown reason {wizardDto}");
                throw new Exception("no change has been commited, Wizard not written");
            }
            return _mapper.Map<WizardDTO>(wizardEntity);
        }

        private void ApplyChangeOnTranslation(IEnumerable<TranslationDTO> colTranslationDto, ICollection<Translation> colTranslationEntity)
        {
            var toDelete = colTranslationEntity.Where(t => !colTranslationDto.Any(s => s.Id == t.Id)).ToList();
            foreach (var item in toDelete.ToList())
            {
                var deletedItem = colTranslationEntity.Single(t => t.Id == item.Id);
                colTranslationEntity.Remove(deletedItem);

                _translation.Delete(item);
            }
            foreach (var item in colTranslationDto)
            {
                var existing = colTranslationEntity.FirstOrDefault(t => t.Id == item.Id && t.Id > 0);
                if (existing != null)
                {
                    _mapper.Map(item, existing);
                }
                else
                {
                    colTranslationEntity.Add(_mapper.Map<Translation>(item));
                }
            }
        }

        public async Task<WizardDTO> GetWizardDetailAsync(int wizardId)
        {
            var wizard = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);
            return _mapper.Map<WizardDTO>(wizard);
        }

        public async Task<IEnumerable<QuestionDTO>> ListQuestionAsync(int id, bool isFirstQuestion = false)
        {
            var wizard = await GetWizardOrFirstAsync(id).ConfigureAwait(false);
            var questions = _questionRepository.Find(q => q.WizardId == wizard.Id
                                                       && (!isFirstQuestion || q.IsFirstQuestion)
                                                       && !q.DeletedTime.HasValue)
                                               .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                                               .Include(q => q.Titles)
                                               .AsSplitQuery()
                                               .ToList();

            var questionMap = _mapper.Map<List<QuestionDTO>>(questions);
            return questionMap;
        }

        private async Task<bool> ValidateQuestion(int wizardId, int questionId)
        {
            var wizard = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);
            var question = wizard.Questions.FirstOrDefault(x => x.Id == questionId) ?? throw new KeyNotFoundException($"Question with ID {questionId} not found in the wizard.");
            if (!question.IsFirstQuestion)
            {
                // TODO : if not first question, then check for its path
                var validate = await _breadCrumbService.CheckQuestionPath(_unitOfWork, question, new HashSet<int>());
                return validate;
            }
            
            return true;
        }

        public async Task<AnswerDetailDTO> CreateAnswerAsync(int wizardId, int questionId, CreateAnswerDTO answerDto)
        {
            if(questionId != answerDto.LinkedQuestionId)
            {
                try
                {
                    // TODO : Validate this question to have path to first question
                    var isQuestionHasPathToFirstQuestion = await ValidateQuestion(wizardId, questionId);
                    if(!isQuestionHasPathToFirstQuestion) throw new Exception("Can't create answer because this question doesn't have link to first question.");
                    // TODO : process insert answer
                    var wizardEntity = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);

                    var question = _questionRepository.Find(q => q.WizardId == wizardEntity.Id && q.Id == questionId && !q.DeletedTime.HasValue)
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Labels)
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Details)
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Links)
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.FinalAnswerTexts)
                        .AsTracking()
                        .FirstOrDefault();

                    if (question != null)
                    {
                        var answerEntity = _mapper.Map<Answer>(answerDto);
                        answerEntity.Order = question.Answers.Any() ? question.Answers.Max(x => x.Order) + 1 : 1;

                        question.Answers.Add(answerEntity);

                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                        return _mapper.Map<AnswerDetailDTO>(answerEntity);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Create answer tried on unexisting question id {questionId}");
                    }

                }
                catch (Exception e)
                {

                    throw new KeyNotFoundException(e.Message);
                }
            }
            _logger.LogError("The linked question for current answer is linked to current question questionId = {questionId} answerDto.LinkedQuestionId = {updateAnswerDTO.LinkedQuestionId}", questionId, answerDto.LinkedQuestionId);
            throw new Exception("Can't linking an answer of a question to that same question, this causing the loop.");
        }

        public async Task<AnswerDetailDTO> GetAnswerAsync(int wizardId, int questionId, int answerId)
        {
            try
            {
                var wizardEntity = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);
                
                var question = _questionRepository.Find(q => q.WizardId == wizardEntity.Id && q.Id == questionId && !q.DeletedTime.HasValue)
                    .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Labels)
                    .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Details)
                    .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Links)
                    .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.FinalAnswerTexts)
                    .AsSplitQuery()
                    .AsTracking()
                    .FirstOrDefault();
                if (question != null)
                {
                    var answer = question.Answers.FirstOrDefault(x => x.Id == answerId);
                    if (answer != null)
                    {
                        var mapAnswer = _mapper.Map<AnswerDetailDTO>(answer);
                        return mapAnswer;
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Get answer tried on unexisting answer id {answerId}");
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"Get answer tried on unexisting question id {questionId}");
                }
                
            }
            catch (Exception e)
            {

                throw new KeyNotFoundException(e.Message);
            }
        }

        public async Task<AnswerDetailDTO> UpdateAnswerAsync(int wizardId, int questionId, int answerId, UpdateAnswerDTO updateAnswerDTO)
        {
            if(questionId != updateAnswerDTO.LinkedQuestionId)
            {
                try
                {
                    var wizardEntity = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);
                    var questionEntity = _questionRepository.Find(q => q.WizardId == wizardEntity.Id && q.Id == questionId && !q.DeletedTime.HasValue)
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Labels)
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Details)
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Links)
                        .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.FinalAnswerTexts)
                        .AsSplitQuery()
                        .AsTracking()
                        .FirstOrDefault();
                    if (questionEntity != null)
                    {
                        var answerEntity = questionEntity.Answers.FirstOrDefault(x => x.Id == answerId);
                        if (answerEntity != null)
                        {
                            answerEntity = _mapper.Map(updateAnswerDTO, answerEntity);
                            ApplyChangeOnTranslation(updateAnswerDTO.Labels, answerEntity.Labels);
                            ApplyChangeOnTranslation(updateAnswerDTO.Details, answerEntity.Details);
                            ApplyChangeOnTranslation(updateAnswerDTO.Links, answerEntity.Links);
                            ApplyChangeOnTranslation(updateAnswerDTO.FinalAnswerTexts, answerEntity.FinalAnswerTexts);

                            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                            var mapAnswer = _mapper.Map<AnswerDetailDTO>(answerEntity);
                            return mapAnswer;
                        }
                        else
                        {
                            throw new KeyNotFoundException($"Update answer tried on unexisting answer id {answerId}");
                        }

                    }
                    else
                    {
                        throw new KeyNotFoundException($"Update answer tried on unexisting question id {questionId}");
                    }

                }
                catch (Exception e)
                {

                    throw new KeyNotFoundException(e.Message);
                }
            }
            _logger.LogError("The linked question for current answer is linked to current question questionId = {questionId} updateAnswerDTO.LinkedQuestionId = {updateAnswerDTO.LinkedQuestionId}", questionId, updateAnswerDTO.LinkedQuestionId);
            throw new Exception("Can't linking an answer of a question to that same question, this causing the loop.");

        }

        public List<AnswerDTO> ListAnswers(int wizardId, int questionId)
        {
            var wizardEntity = _wizardRepository.Find(w => w.Id == wizardId && !w.DeletedTime.HasValue)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue))
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Labels)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Links)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.FinalAnswerTexts)
                .FirstOrDefault();
            if (wizardEntity != null)
            {
                var questionEntity = wizardEntity.Questions.FirstOrDefault(x => x.Id == questionId);
                if (questionEntity != null)
                {
                    return _mapper.Map<List<AnswerDTO>>(questionEntity.Answers);
                }
                else
                {
                    throw new KeyNotFoundException($"Get list of answer tried on unexisting question id {questionId}");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Get list of answer tried on unexisting wizard id {wizardId}");
            }
        }

        public async Task DeleteQuestionAsync(int wizardId, int questionId)
        {
            var wizardEntity = _wizardRepository.Find(w => w.Id == wizardId && !w.DeletedTime.HasValue)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).FirstOrDefault();
            if (wizardEntity != null)
            {
                var questionEntity = wizardEntity.Questions.FirstOrDefault(x => x.Id == questionId);
                if (questionEntity != null)
                {
                    wizardEntity.Questions.Remove(questionEntity);
                    _wizardRepository.Update(wizardEntity);
                    var savedEntries = await _unitOfWork.SaveChangesAsync();
                    if (savedEntries <= 0)
                    {
                        _logger.LogError("The deletion of the Question has failed for an unknown reason question id {questionId}", questionId);
                        throw new Exception("no change has been commited, Wizard not written");
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"Delete question tried on unexisting question id {questionId}");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Delete question tried on unexisting wizard id {wizardId}");
            }
        }

        public async Task<QuestionDetailDTO> CreateQuestionAsync(int wizardId, CreateQuestionDTO questionDTO)
        {
            if (questionDTO != null)
            {
                var wizard = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);
                if(questionDTO.IsFirstQuestion)
                {
                    var existingFirstQuestions = wizard.Questions.Where(q => q.IsFirstQuestion).ToList();
                    if(existingFirstQuestions.Any())
                    {
                        existingFirstQuestions.ForEach(i => i.IsFirstQuestion = false);
                    }
                }
                var createQuestion = _mapper.Map<Question>(questionDTO);
                wizard.Questions.Add(createQuestion);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<QuestionDetailDTO>(createQuestion);
            }
            else
            {
                throw new ArgumentException($"Error on getting question information. {Constants.CreateQuestion}. UpdateQuestionDTO cannot be {questionDTO}");
            }
        }

        public async Task<QuestionDetailDTO> GetQuestionAsync(int wizardId, int questionId)
        {
            var wizard = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);
            var question = wizard.Questions.FirstOrDefault(x => x.Id == questionId);
            if (question == null)
            {
                throw new KeyNotFoundException($"Question with ID {questionId} not found");
            }
            return _mapper.Map<QuestionDetailDTO>(question);
        }

        public async Task<QuestionDetailDTO> UpdateQuestionAsync(int wizardId, int questionId, UpdateQuestionDTO updateQuestionDTO)
        {
            var wizard = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);
            var existingQuestion = _questionRepository.Find(q => q.WizardId == wizard.Id && q.Id == questionId && !q.DeletedTime.HasValue)
                .Include(q => q.Titles)
                .Include(q => q.Texts)
                .AsSplitQuery()
                .AsTracking()
                .FirstOrDefault();

            if (existingQuestion == null)
            {
                throw new KeyNotFoundException($"Question with ID {questionId} not found");
            }

            if (updateQuestionDTO == null)
            {
                throw new ArgumentNullException(nameof(updateQuestionDTO), "UpdateQuestionDTO cannot be null");
            }

            if(updateQuestionDTO.IsFirstQuestion)
            {
               var prevFirstQuestions = wizard.Questions.Where(q => q.IsFirstQuestion).ToList();
                if (prevFirstQuestions.Any())
                {
                    prevFirstQuestions.ForEach(q => q.IsFirstQuestion = false);
                }
            }

            _mapper.Map(updateQuestionDTO, existingQuestion);
            ApplyChangeOnTranslation(updateQuestionDTO.Labels, existingQuestion.Titles);
            ApplyChangeOnTranslation(updateQuestionDTO.Texts, existingQuestion.Texts);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<QuestionDetailDTO>(existingQuestion);

        }

        private async Task<Wizard> GetWizardOrFirstAsync(int id)
        {
            var wizard = _wizardRepository.Find(w => w.Id == id && !w.DeletedTime.HasValue)
                .Include(w => w.Titles)
                .Include(w => w.IntroductionTexts)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue))
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Titles)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Texts)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Breadcrumbs)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.BreadcrumbsItems)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Labels)
                .AsSplitQuery()
                .FirstOrDefault();
            if (wizard != null)
            {
                return wizard;
            }
            else
            {
                var firstWizardEntity = _wizardRepository.Find(wizard => !wizard.DeletedTime.HasValue)
                    .Include(w => w.Titles)
                    .Include(w => w.IntroductionTexts)
                    .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue))
                    .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Titles)
                    .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Texts)
                    .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                    .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue)).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue)).ThenInclude(a => a.Labels)
                    .AsSplitQuery()
                    .FirstOrDefault();
                if (firstWizardEntity != null)
                {
                    return firstWizardEntity;
                }
                else
                {
                    firstWizardEntity = new Wizard();
                    _wizardRepository.Add(firstWizardEntity);
                    var savedEntries = await _unitOfWork.SaveChangesAsync();
                    if (savedEntries <= 0)
                    {
                        _logger.LogError("The insert of the Wizard has failed for an unknown reason");
                        throw new Exception("no change has been commited, Wizard not written");
                    }
                    return firstWizardEntity;
                }
            }
        }

        public async Task<bool> ReorderAnswersAsync(int wizardId, int questionId, List<AnswerOrderItemDTO> answerOrders)
        {
            var answers = _questionRepository.Find(q => q.WizardId == wizardId && q.Id == questionId && !q.DeletedTime.HasValue)
                .Include(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                .SelectMany(q => q.Answers)
                .AsTracking();

            if (!answerOrders.All(ao => answers.Any(a => a.Id == ao.AnswerId)))
            {
                throw new KeyNotFoundException($"Received data contains answers not linked to received question or wizard!");
            }
            if (answerOrders.Select(o => o.Order).Distinct().Count() != answerOrders.Count
                || answerOrders.Select(o => o.AnswerId).Distinct().Count() != answerOrders.Count)
            {
                throw new ApplicationException("Duplicate key detected in received data!");
            }

            foreach (var answerOrder in answerOrders)
            {
                var answerToUpdate = answers.First(x => x.Id == answerOrder.AnswerId);
                answerToUpdate.Order = answerOrder.Order;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<ICollection<QuestionBreadcrumbDTO>> GetBreadcrumbAsync(int wizardId, int questionId)
        {
            var wizard = await GetWizardOrFirstAsync(wizardId).ConfigureAwait(false);
            var question = wizard.Questions.FirstOrDefault(x => x.Id == questionId) ?? throw new KeyNotFoundException($"Question with ID {questionId} not found in the wizard.");
            if (question.IsFirstQuestion)
            {
                var breadcrumbDTOs = new List<QuestionBreadcrumbDTO>();
                return breadcrumbDTOs;
            }
            if (question.Breadcrumbs != null && question.Breadcrumbs.Any())
            {
                return _mapper.Map<ICollection<QuestionBreadcrumbDTO>>(question.Breadcrumbs);
            }
            var resultBreadcrumb = await _breadCrumbService.ProcessBreadcrumbs(_unitOfWork, question, new HashSet<int>());

            await _unitOfWork.SaveChangesAsync();
            return (ICollection<QuestionBreadcrumbDTO>)resultBreadcrumb;
        }

        public async Task DeleteAnswerAsync(int wizardId, int questionId, int answerId)
        {
            var wizardEntity = _wizardRepository.Find(w => w.Id == wizardId && !w.DeletedTime.HasValue)
                .Include(w => w.Questions.Where(q => !q.DeletedTime.HasValue))
                .Include(w => w.Questions).ThenInclude(q => q.Answers.Where(a => !a.DeletedTime.HasValue))
                .AsTracking()
                .FirstOrDefault();
            if (wizardEntity != null)
            {
                var questionEntity = wizardEntity.Questions.FirstOrDefault(x => x.Id == questionId);
                if (questionEntity != null)
                {
                    var answerEntity = questionEntity.Answers.FirstOrDefault(x => x.Id == answerId);
                    if (answerEntity != null)
                    {
                        questionEntity.Answers.Remove(answerEntity);
                        var savedEntries = await _unitOfWork.SaveChangesAsync();
                        if (savedEntries <= 0)
                        {
                            _logger.LogError("The deletion of the Answer has failed for an unknown reason answer id {answerId}", answerId);
                            throw new Exception("no change has been commited, Wizard not written");
                        }
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Delete answer tried on unexisting answer id {answerId}");

                    }
                }
                else
                {
                    throw new KeyNotFoundException($"Question with ID {questionId} not found");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Delete question tried on unexisting wizard id {wizardId}");
            }
        }
    }
}
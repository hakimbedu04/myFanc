using Microsoft.AspNetCore.Mvc;
using MyFanc.BusinessObjects;
using MyFanc.DTO.Internal.Wizard;
using MyFanc.DTO.Internal.Wizards;

namespace MyFanc.BLL
{
    public partial interface IBll
    {
        Task<WizardDTO> EditWizardAsync(WizardDTO wizardDto);
        Task<WizardDTO> GetWizardDetailAsync(int id);
        Task<IEnumerable<QuestionDTO>> ListQuestionAsync(int wizardId, bool isFirstQuestion = false);
        Task<AnswerDetailDTO> CreateAnswerAsync(int wizardId, int questionId, CreateAnswerDTO answerDto);
        Task<AnswerDetailDTO> GetAnswerAsync(int wizardId, int questionId, int answerId);
        Task<AnswerDetailDTO> UpdateAnswerAsync(int wizardId, int questionId, int answerId, UpdateAnswerDTO updateAnswerDTO);
        List<AnswerDTO> ListAnswers(int wizardId, int questionId);
        Task DeleteQuestionAsync(int wizardId, int questionId);
        Task<QuestionDetailDTO> CreateQuestionAsync(int wizardId, CreateQuestionDTO questionDTO);
        Task<QuestionDetailDTO> GetQuestionAsync(int wizardId, int questionId);
        Task<QuestionDetailDTO> UpdateQuestionAsync(int wizardId, int questionId, UpdateQuestionDTO questionDTO);
        Task<ICollection<QuestionBreadcrumbDTO>> GetBreadcrumbAsync(int wizardId, int questionId);
        Task<bool> ReorderAnswersAsync(int wizardId, int questionId, List<AnswerOrderItemDTO> answerOrders);
        Task DeleteAnswerAsync(int wizardId, int questionId, int answerId);
    }
}


using AutoMapper;
using MyFanc.BusinessObjects;
using MyFanc.DTO.Internal.Translation;
using MyFanc.DTO.Internal.Wizard;
using MyFanc.DTO.Internal.Wizards;

namespace MyFanc.Api.Mapper
{
    public class WizardProfile : Profile
    {
        public WizardProfile()
        {
            CreateMap<TranslationDTO, Translation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionsTitles, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionsTexts, opt => opt.Ignore())
                .ForMember(dest => dest.AnswersLabels, opt => opt.Ignore())
                .ForMember(dest => dest.WizardsTexts, opt => opt.Ignore())
                .ForMember(dest => dest.WizardsTitles, opt => opt.Ignore())
                .ForMember(dest => dest.AnswersDetails, opt => opt.Ignore())
                .ForMember(dest => dest.AnswersLinks, opt => opt.Ignore())
                .ForMember(dest => dest.AnswersFinalAnswerTexts, opt => opt.Ignore())
                .ForMember(dest => dest.FormsLabels, opt => opt.Ignore())
                .ForMember(dest => dest.FormsDescriptions, opt => opt.Ignore())
                .ForMember(dest => dest.FormsUrls, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodesLabels, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodeFieldsLabels, opt => opt.Ignore())
                .ForMember(dest => dest.FormValueFieldsLabels, opt => opt.Ignore())
                .ForMember(dest => dest.PersonaCategoriesLabel, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<WizardDTO, Wizard>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Titles, opt => opt.Ignore())
                .ForMember(dest => dest.IntroductionTexts, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.HasFirstQuestion,
                opt => opt.MapFrom(src => src.Questions.Any(q => q.IsFirstQuestion && q.IsActive)));

            CreateMap<Question, QuestionDTO>()
                .ForMember(dest => dest.AnswersCount, opt => opt.MapFrom(src => src.Answers.Count))
                .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.Titles));

            CreateMap<Question, QuestionDetailDTO>()
                .ForMember(dest => dest.AnswersCount, opt => opt.MapFrom(src => src.Answers.Count))
                .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.Titles));
            
            CreateMap<CreateAnswerDTO, Answer>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.LinkedQuestion, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => string.Join("#", src.Tags)));

            CreateMap<UpdateAnswerDTO, Answer>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.LinkedQuestion, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => string.Join("#", src.Tags)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.Labels, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.Ignore())
                .ForMember(dest => dest.FinalAnswerTexts, opt => opt.Ignore());

            CreateMap<Answer, AnswerDTO>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => (string.IsNullOrEmpty(src.Tags) ? new List<string>() : src.Tags.Split("#", StringSplitOptions.RemoveEmptyEntries).ToList())));

            CreateMap<Answer, AnswerDetailDTO>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => (string.IsNullOrEmpty(src.Tags) ? new List<string>() : src.Tags.Split("#", StringSplitOptions.RemoveEmptyEntries).ToList())));

            CreateMap<CreateQuestionDTO, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src=> true))
                .ForMember(dest => dest.Titles, opt => opt.MapFrom(src => src.Labels))
                .ForMember(dest => dest.Texts, opt => opt.MapFrom(src => src.Texts))
                .ForMember(dest => dest.Answers, opt => opt.Ignore())
                .ForMember(dest => dest.Breadcrumbs, opt => opt.Ignore())
                .ForMember(dest => dest.BreadcrumbsItems, opt => opt.Ignore())
                .ForMember(dest => dest.Wizard, opt => opt.Ignore())
                .ForMember(dest => dest.LinkedAnswer, opt => opt.Ignore());

            CreateMap<UpdateQuestionDTO, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Titles, opt => opt.Ignore())
                .ForMember(dest => dest.Texts, opt => opt.Ignore())
                .ForMember(dest => dest.Answers, opt => opt.Ignore())
                .ForMember(dest => dest.Breadcrumbs, opt => opt.Ignore())
                .ForMember(dest => dest.BreadcrumbsItems, opt => opt.Ignore())
                .ForMember(dest => dest.Wizard, opt => opt.Ignore())
                .ForMember(dest => dest.WizardId, opt => opt.Ignore())
                .ForMember(dest => dest.LinkedAnswer, opt => opt.Ignore());

            CreateMap<QuestionBreadcrumb, QuestionBreadcrumbDTO>()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore());

            CreateMap<QuestionBreadcrumbItemDTO, QuestionBreadcrumbItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Breadcrumb, opt => opt.Ignore())
                .ForMember(dest => dest.BreadcrumbId, opt => opt.Ignore())
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Labels,
                opt => opt.MapFrom(src => src.Question.Titles));
        }

    }
}


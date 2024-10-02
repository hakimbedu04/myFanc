using AutoMapper;
using MyFanc.Api.Common;
using MyFanc.BusinessObjects;
using MyFanc.DTO.Internal.Forms;
using MyFanc.DTO.Internal.Translation;

namespace MyFanc.Api.Mapper
{
    public class FormProfile : Profile
    {
        public FormProfile()
        {
            CreateMap<Form, FormBlockDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateFormBlockDTO, Form>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
                .ForMember(dest => dest.Descriptions, opt => opt.Ignore())
                .ForMember(dest => dest.Urls, opt => opt.Ignore())
                .ForMember(dest => dest.FormCategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodes, opt => opt.Ignore())
                .ForMember(dest => dest.FormDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.Nacabels, opt => opt.Ignore())
                .ForMember(dest => dest.FormCategory, opt => opt.Ignore())
                .ForMember(dest => dest.FormSubmissions, opt => opt.Ignore());

            CreateMap<UpdateFormBlockDTO, Form>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Labels, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
                .ForMember(dest => dest.Descriptions, opt => opt.Ignore())
                .ForMember(dest => dest.Urls, opt => opt.Ignore())
                .ForMember(dest => dest.FormCategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodes, opt => opt.Ignore())
                .ForMember(dest => dest.FormDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.Nacabels, opt => opt.Ignore())
                .ForMember(dest => dest.FormCategory, opt => opt.Ignore())
                .ForMember(dest => dest.FormSubmissions, opt => opt.Ignore())
                ;

            CreateMap<Form, ListFormBlockDTO>()
               .ForMember(dest => dest.OriginalId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
               .ForMember(dest => dest.IsEmptyForm, opt => opt.MapFrom(src => !src.FormNodes.Any()))
               .ForMember(dest => dest.IsUsed, opt => opt.MapFrom<IsUsedResolver>())
               .ForMember(dest => dest.IsEditContentAllowed, opt => opt.MapFrom<IsLinkedToNonDraftFormResolver>());

            CreateMap<Form, ListFormDTO>()
               .ForMember(dest => dest.OriginalId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
               .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.FormCategory != null ? src.FormCategory.Code : ""))
               .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Nacabels != null ? src.Nacabels.Select(n => n.NacabelTranslation).Select(t => t.FirstOrDefault().Description??"") : new List<string>()));

            CreateMap<CreateUpdateFormNodeDto, FormNodes>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Labels, opt => opt.Ignore())
                .ForMember(dest => dest.Form, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore())
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodeFields, opt => opt.Ignore())
                .ForMember(dest => dest.FormConditionals, opt => opt.Ignore());

            CreateMap<CreateUpdateFormNodeFieldDto, FormNodeFields>()
               .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
               .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Labels, opt => opt.Ignore())
               .ForMember(dest => dest.FormNodes, opt => opt.Ignore())
               .ForMember(dest => dest.FormValueFields, opt => opt.Ignore())
               .ForMember(dest => dest.FormConditionals, opt => opt.Ignore())
               .ForMember(dest => dest.Value, opt => opt.Ignore());

            
            CreateMap<FormConditionalDTO, FormConditionals>()
               .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
               .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
               .ForMember(dest => dest.FormNodeFieldId, opt => opt.Ignore())
               .ForMember(dest => dest.FormNodeField, opt => opt.Ignore())
               .ForMember(dest => dest.FormNode, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
               .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Form, FormContentDTO>()
                .ForMember(dest => dest.FormNodes, opt => opt.MapFrom(src => src.FormNodes.Where(n => !n.ParentId.HasValue)));
            CreateMap<FormNodes, FormNodesDTO>()
               .ForMember(dest => dest.IsEditContentAllowed, opt => opt.Ignore())
               .ForMember(dest => dest.FormNodes, opt => opt.Ignore());
            CreateMap<FormNodeFields, FormNodeFieldDTO>();

            CreateMap<FormValueFields, CustomListDTO>()
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Labels));

            CreateMap<FormConditionals, FormConditionalDTO>();

            CreateMap<CreateFormDTO, Form>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
                .ForMember(dest => dest.Nacabels, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodes, opt => opt.Ignore())
                .ForMember(dest => dest.FormCategory, opt => opt.Ignore())
                .ForMember(dest => dest.FormSubmissions, opt => opt.Ignore());

            CreateMap<CreateFormDocumentDTO, FormDocument>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FormId, opt => opt.Ignore())
                .ForMember(dest => dest.Form, opt => opt.Ignore());

            CreateMap<CreateDocumentDTO, Document>()
               .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
               .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.FormDocumentId, opt => opt.Ignore())
               .ForMember(dest => dest.FormDocument, opt => opt.Ignore());

            CreateMap<UpdateFormDTO, Form>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
                .ForMember(dest => dest.Nacabels, opt => opt.Ignore())
                .ForMember(dest => dest.Labels, opt => opt.Ignore())
                .ForMember(dest => dest.Descriptions, opt => opt.Ignore())
                .ForMember(dest => dest.Urls, opt => opt.Ignore())
                .ForMember(dest => dest.FormDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodes, opt => opt.Ignore())
                .ForMember(dest => dest.FormCategory, opt => opt.Ignore())
                .ForMember(dest => dest.FormSubmissions, opt => opt.Ignore());

            CreateMap<UpdateFormDocumentDTO, FormDocument>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Documents, opt => opt.Ignore())
                .ForMember(dest => dest.FormId, opt => opt.Ignore())
                .ForMember(dest => dest.Form, opt => opt.Ignore());

            CreateMap<UpdateDocumentDTO, Document>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
               .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
               .ForMember(dest => dest.FormDocumentId, opt => opt.Ignore())
               .ForMember(dest => dest.Path, opt => opt.Ignore())
               .ForMember(dest => dest.FormDocument, opt => opt.Ignore());

            CreateMap<FormCategory, FormCategoryDTO>();
            CreateMap<Nacabel, NacabelDTO>();
            CreateMap<Document, DocumentDTO>()
                .ForMember(dest => dest.Path, opt => opt.MapFrom<DocumentPathResolver>());
            CreateMap<FormDocument, FormDocumentDTO>();
            CreateMap<Form, FormDTO>()
                 .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Nacabels != null ? src.Nacabels.Select(t => t.Id).ToList(): new List<int>())); 
            CreateMap<SearchParamFormDTO, SearchParamFormDTOExten>()
                .ForMember(dest => dest.CategoriesList, opt => opt.Ignore())
                .ForMember(dest => dest.StatusList, opt => opt.Ignore())
                .ForMember(dest => dest.TagList, opt => opt.Ignore())
                .ForMember(dest => dest.TypeList, opt => opt.Ignore());
            CreateMap<FormSubmission, ListFormSubmissionDTO>();

            CreateMap<Form, Form>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version + 1));

            CreateMap<Translation, Translation>()
                .IgnoreAllMembers()
                .ForMember(x => x.LanguageCode, opt => opt.MapFrom(y => y.LanguageCode))
                .ForMember(x => x.Text, opt => opt.MapFrom(y => y.Text));

            CreateMap<FormNodes, FormNodes>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Labels, opt => opt.Ignore())
                .ForMember(dest => dest.FormConditionals, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Form.Version + 1));

            CreateMap<FormNodeFields, FormNodeFields>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodeId, opt => opt.Ignore());

            CreateMap<FormValueFields, FormValueFields>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FormNodeFieldId, opt => opt.Ignore());

            CreateMap<FormConditionals, FormConditionals>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.FormNodeFieldId, opt => opt.Ignore());

            CreateMap<FormDocument, FormDocument>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.FormId, opt => opt.Ignore());

            CreateMap<Document, Document>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.FormDocumentId, opt => opt.Ignore());

            CreateMap<Form, PreviewFormDTO>()
               .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Details, opt => opt.Ignore())
               .ForMember(dest => dest.Nodes, opt => opt.Ignore());

            CreateMap<CreateFormRequestDTO ,FormSubmission>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserFullName))
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.SubmissionDate, opt => opt.Ignore())
               .ForMember(dest => dest.Form, opt => opt.Ignore())
               .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
               .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
               .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
               .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
               .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore());

            CreateMap<Form, ListViewFormsDTO>()
                .ForMember(dest => dest.OriginalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .AfterMap((src, dest) => MapTags(src, dest));


            CreateMap<FormNodes, NodesDTO>()
               .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
               .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.FormNodeFields))
               .ForMember(dest => dest.Conditionals, opt => opt.MapFrom(src => src.FormNodeFields.SelectMany(x => x.FormConditionals)));

            CreateMap<FormNodeFields, PropertiesDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Property));

            CreateMap<Form, PreviewDetailsDTO>()
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version))
                .ForMember(dest => dest.CaregoryId, opt => opt.MapFrom(src => src.FormCategoryId))
                .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.Labels))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descriptions))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Nacabels, opt => opt.MapFrom(src => src.Nacabels))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.FormDocuments))
                .ForMember(dest => dest.Urls, opt => opt.MapFrom(src => src.Urls));

            CreateMap<Nacabel, PreviewDetailsNacabelDTO>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.NacabelCode))
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.NacabelTranslation));

            CreateMap<NacabelTranslation, TranslationDTO>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.LanguageCode))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Description));

            CreateMap<Translation, FormUrlDTO>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.LanguageCode))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Text));

            CreateMap<FormDocument, PreviewDetailsDocumentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Documents.Select(x => x.Path).FirstOrDefault()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Documents.Select(x => x.Type).FirstOrDefault()));

            CreateMap<CreateUpdateFormNodesItemDto, CreateUpdateFormNodesDto>()
                .ForMember(dest => dest.FormNodes, opt => opt.Ignore());
        }

        private ListViewFormsDTO MapTags(Form src, ListViewFormsDTO dest)
        {
            if(src.Nacabels.Any())
            {
                var tags = src.Nacabels.Select(x => x.NacabelTranslation.FirstOrDefault());
                if(tags.Any(x => x != null))
                    dest.Tags = tags.Where(x => x != null).Select(x => x?.Description??"");
            }
            return dest;
        }
	}

    public class IsUsedResolver : IValueResolver<Form, ListFormBlockDTO, bool>
    {
        public bool Resolve(Form source, ListFormBlockDTO destination, bool member, ResolutionContext context)
        {
            if (context.Items.TryGetValue("AdditionalParam", out var additionalParam) && additionalParam is List<string> usedFormBlockIds)
            {
                return usedFormBlockIds.Contains(source.Id.ToString());
            }

            return false;
        }
    }

    public class IsLinkedToNonDraftFormResolver : IValueResolver<Form, ListFormBlockDTO, bool>
    {
        public bool Resolve(Form source, ListFormBlockDTO destination, bool member, ResolutionContext context)
        {
            if (context.Items.TryGetValue("AdditionalParam2", out var additionalParam) && additionalParam is List<string> usedFormBlockAllowEditIds)
            {
                return !usedFormBlockAllowEditIds.Contains(source.Id.ToString());
            }

            return true;
        }
    }

	public class DocumentPathResolver : IValueResolver<Document, DocumentDTO, string>
	{
		private const string DOWNLOAD_PATH_FORMAT = "api/form/documents/{0}/download";

		private readonly IHttpContextAccessor _httpContextAccessor;

		public DocumentPathResolver(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		}

		public string Resolve(Document source, DocumentDTO destination, string member, ResolutionContext context)
		{
			if (!string.IsNullOrEmpty(source.Path))
            {
				var request = _httpContextAccessor?.HttpContext?.Request;

				var baseUrl = $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
                var downloadUrl = string.Format(DOWNLOAD_PATH_FORMAT, source.Id);

                return $"{baseUrl}/{downloadUrl}";
			}

            return string.Empty;
		}
	}
}

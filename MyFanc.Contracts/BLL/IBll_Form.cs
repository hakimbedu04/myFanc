
using MyFanc.BusinessObjects;
using MyFanc.DTO.Internal.Forms;
using static MyFanc.Core.Enums;

namespace MyFanc.BLL
{
    public partial interface IBll
    {
        Task<FormBlockDTO> CreateFormBlockAsync(CreateFormBlockDTO createFormBlockDTO);
        IEnumerable<ListFormBlockDTO> GetListFormBlock(int? id, string? label, IsUsedParam isUsed);
        Task<FormBlockDTO> UpdateFormBlockAsync(Guid formBlockId, UpdateFormBlockDTO updateFormBlockDTO);
        Task<int> UnpublisForm(Guid formId, PublishFormDTO param);
        List<string> GetFormObjectList();
        Task DeleteFormBlockAsync(Guid formBlockId);
        Task<Guid> CreateUpdateFormNodeAsync(CreateUpdateFormNodeDto createUpdateFormNodeDTO);
        Task<Guid> CreateUpdateFormNodeFieldAsync(CreateUpdateFormNodeFieldDto createUpdateFormNodeFieldDTO);
        FormContentDTO GetFormContent(Guid formId, bool isFormBlock = false);
        Task<Guid> AddFormBlockToForm(AddFormBlockToFormDTO addFormBlockToFormDTO);
        Task<FormDTO> CreateFormAsync(CreateFormDTO createFormDTO);
        IEnumerable<FormCategoryDTO> GetListFormCategory();
        bool IsShortUrlAlreadyUsed(string shortUrl, string languageCode);
        IEnumerable<ListFormDTO> GetListForm(SearchParamFormDTO searchParam);
        Task<Guid> PublisForm(Guid formId, PublishFormDTO param);
        Task<FormDTO> UpdateFormAsync(Guid formId, UpdateFormDTO updateFormDTO);
        IEnumerable<ListFormSubmissionDTO> GetListFormResponses(Guid formId, string? userName, string? companyName, string? email);
        Task<Guid> DuplicateForm(Guid formId);
        OnlineVersionCheckDTO IsOfflineFormHaveOnlineVersion(int offlineFormExternalId);
        Task DeleteFormNodeAsync(Guid formNodeId);
        PreviewFormDTO PreviewAsync(Guid FormId);
        Task DeleteFormNodeSectionAsync(Guid formId, Guid formNodeId);
        Task<Guid> DuplicateFormBlock(Guid formBlockId);
        Task<Guid> SubmitFormRequestAsync(CreateFormRequestDTO createFormRequestDTO);
        Task<PagingResultDTO<ListViewFormsDTO>> ViewAllForms(int pageNumber, int pageSize, string languageCode, string? searchLabel, string? cbeNumber, bool includeHomeTag = false);
        FormDTO GetFormDetails(Guid formId);
        FormDTO GetFormDetailsByFormUrl(string shortUrl);

        Task<FileDataDTO> DownloadAsync(Guid formId, string languageCode);
        Task<List<Guid>> CreateUpdateFormContentAsync(Guid formId, CreateUpdateFormContentDto createUpdateFormContentDTO);
        List<FormResponseDetailDto> GetDetailFormResponse(Guid formSubmissionId);
        Task<PagingResultDTO<ListViewFormsDTO>> ViewFormsWithTag(int pageNumber, int pageSize, string userExternalId, string languageCode, string? searchLabel, string? cbeNumber, bool includeHomeTag = false);
		Task<FileDataDTO> DownloadDocumentAsync(Guid documentId);
	}
}

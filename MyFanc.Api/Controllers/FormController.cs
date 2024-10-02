using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFanc.Api.Common;
using MyFanc.BLL;
using MyFanc.Core;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Forms;
using Swashbuckle.AspNetCore.Annotations;
using static MyFanc.Core.Enums;

namespace MyFanc.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly IBll _bll;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FormController(IBll bll, IHttpContextAccessor httpContextAccessor)
        {
            _bll = bll;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [SwaggerOperation("Get Forms", "Get list of forms base on search parameter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ListFormDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetForms([FromQuery]SearchParamFormDTO searchParamFormDTO)
        {
            try
            {
                var formBlocks = _bll.GetListForm(searchParamFormDTO);
                return Ok(formBlocks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{formId}")]
        [SwaggerOperation("Get Detail Forms", "Get details forms")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FormDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetDetailForm(Guid formId)
        {
            try
            {
                var form = _bll.GetFormDetails(formId);
                return Ok(form);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FormByFormUrl")]
        [SwaggerOperation("Get Detail Forms", "Get details forms by shorturl")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FormDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetDetailFormByFormUrl(string shortUrl)
        {
            try
            {
                var form = _bll.GetFormDetailsByFormUrl(shortUrl);
                return Ok(form);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation("Create Form", "Create new form")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateForm(CreateFormDTO createFormDTO)
        {
            try
            {
                var result = await _bll.CreateFormAsync(createFormDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPut("{formId}")]
        [SwaggerOperation("Update Form", "Update a form")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateForm(Guid formId, UpdateFormDTO updateFormDto)
        {
            string errors;
            try
            {
                if (formId != Guid.Empty)
                {
                    if (updateFormDto == null)
                    {
                        errors = Services.Constants.UpdateForm;
                        return BadRequest(errors);
                    }
                    var result = await _bll.UpdateFormAsync(formId, updateFormDto);
                    return Ok(result);
                }
                errors = Services.Constants.InputMadantoryValues;
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("{formId}/Content")]
        [SwaggerOperation("Get Form Content", "Get form content")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormContentDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetFormContent(Guid formId)
        {
            try
            {
                if (formId != Guid.Empty)
                {
                    var formContent = _bll.GetFormContent(formId);
                    return Ok(formContent);
                }

                return BadRequest("Formid can't not be null or guid empty");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{formId}/Content")]
        [SwaggerOperation("Update a Form Content", "To update a Form Content (FormField)")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Guid>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUpdateFormContent(Guid formId, CreateUpdateFormContentDto createUpdateFormContentDTO)
        {
            try
            {
                var res = await _bll.CreateUpdateFormContentAsync(formId, createUpdateFormContentDTO);
                return Ok(res);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("{formId}/FormResponses")]
        [SwaggerOperation("Get list of form responses", "Return list of form responses base on search parameter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ListFormSubmissionDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetFormResponses(Guid formId, [FromQuery, SwaggerParameter("Filter on 'UserName' value", Required = false)] string? userName, [FromQuery, SwaggerParameter("Filter on 'Company Name' value", Required = false)] string? companyName, [FromQuery, SwaggerParameter("Filter on 'Email' value", Required = false)] string? email)
        {
            try
            {
                var formResponse = _bll.GetListFormResponses(formId, userName, companyName, email);
                return Ok(formResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddFormBlockToForm")]
        [SwaggerOperation("Add Form Block to a Form", "Add Form Block to a Form")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFormBlockToForm(AddFormBlockToFormDTO addFormBlockToFormDTO)
        {
            try
            {
                var result = await _bll.AddFormBlockToForm(addFormBlockToFormDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("IsShortUrlAlreadyUsed")]
        [SwaggerOperation("To check if a short url already used or not", "To check if a short url already used or not, true = already used, false = not used")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult IsShortUrlAlreadyUsed(string shortUrl, string languageCode)
        {
            try
            {
                var result = _bll.IsShortUrlAlreadyUsed(shortUrl, languageCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FormCategories")]
        [SwaggerOperation("Get list of form categories", "To get list of form categories ussually used in input dropdown")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormCategoryDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetFormCategories()
        {
            try
            {
                var result = _bll.GetListFormCategory();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("FormBlocks")]
        [SwaggerOperation("Create new form block", "For create new form block")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormBlockDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFormBlock(CreateFormBlockDTO createFormBlockDTO)
        {
            try
            {
                var result = await _bll.CreateFormBlockAsync(createFormBlockDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("FormBlocks")]
        [SwaggerOperation("Get list of form blocks", "To get list of form blocks base on search parameter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormBlockDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetFormBlocks([FromQuery, SwaggerParameter("Filter on 'ID' value", Required = false)] int? id, [FromQuery, SwaggerParameter("Filter on 'Label' value", Required = false)] string? label, [FromQuery, SwaggerParameter("Posible value [0 = All, 1 = Used, 2 = NotUsed]", Required = true)] IsUsedParam isUsed)
        {
            try
            {
                var formBlocks = _bll.GetListFormBlock(id, label, isUsed);
                return Ok(formBlocks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("FormBlocks/{formBlockId}")]
        [SwaggerOperation("Update a form block", "To update a form block")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormBlockDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFormBlock(Guid formBlockId, UpdateFormBlockDTO updateFormBlockDto)
        {
            string errors;
            try
            {
                if (formBlockId != Guid.Empty)
                {
                    if (updateFormBlockDto == null)
                    {
                        errors = Services.Constants.UpdateFormBlock;
                        return BadRequest(errors);
                    }
                    var result = await _bll.UpdateFormBlockAsync(formBlockId, updateFormBlockDto);
                    return Ok(result);
                }
                errors = Services.Constants.InputMadantoryValues;
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }
        
        [HttpPost("FormUnpublish")]
        [SwaggerOperation("Form Unpublish", "To unpublish a form")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FormUnpublish(Guid formId, PublishFormDTO updateFormDto)
        {
            try
            {
                var result = await _bll.UnpublisForm(formId, updateFormDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }


        [HttpDelete("FormBlocks/{formBlockId}")]
        [SwaggerOperation("Delete form block", "To delete a form block")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFormBlock(Guid formBlockId)
        {
            try
            {
                if (formBlockId != Guid.Empty)
                {
                    await _bll.DeleteFormBlockAsync(formBlockId);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }     
        
        [HttpGet("FormObjectList")]
        [SwaggerOperation("List of form object for edit form fields", "Returns list of form object for edit form fields")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetFormObjectList()
        {
            try
            {
                var result = _bll.GetFormObjectList();
                return Ok(new { Objects = result});

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FormFieldTypes")]
        [SwaggerOperation("Get list of available form field types", "Returns list of form form field types")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<KeyValuePairsDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetFormFieldTypes()
        {
            try
            {
                var dictionaryFormFieldType = Enums.GetEnumDescriptions<Enums.FormNodeFieldType>();
                var result = dictionaryFormFieldType.Select(kvp => new KeyValuePairsDTO { Key = kvp.Key, Value = kvp.Value }).ToArray();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("FormNodes")]
        [SwaggerOperation("Add or update a FormNode (FormField / FormBlock / Section) to the Form ", "To add or update a FormNode (FormField / FormBlock / Section)")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUpdateFormFormNode(CreateUpdateFormNodeDto createUpdateFormNodeDTO)
        {   
            try
            {
                var result = await _bll.CreateUpdateFormNodeAsync(createUpdateFormNodeDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpDelete("FormNodes/{formNodeId}")]
        [SwaggerOperation("Delete FormNode (FormField / FormBlock / Section)", "To delete a FormNode (FormField / FormBlock / Section)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFormNode(Guid formNodeId)
        {
            try
            {
                if (formNodeId != Guid.Empty)
                {
                    await _bll.DeleteFormNodeAsync(formNodeId);
                    return Ok();
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPost("FormNodeFields")]
        [SwaggerOperation("Add or Update a FormNodeFields", "The main function of this endpoint is to add or update FormNodeField of a FormNode, FormNodeFields here mean setting/property of a FormNode, for example: FormNode with type 'CheckBox' has property of 'checkbox label' with value 'Label', you can save thoose property and it value in FormNodeField.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUpdateFormFormNodeField(CreateUpdateFormNodeFieldDto createUpdateFormNodeFieldDTO)
        {
            try
            {
                var result = await _bll.CreateUpdateFormNodeFieldAsync(createUpdateFormNodeFieldDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }
        
        [HttpPost("FormPublish")]
        [SwaggerOperation("Form Publish", "To publish a form")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PublishForm(Guid formId, PublishFormDTO updateFormDto)
        {
            try
            {
                var result = await _bll.PublisForm(formId, updateFormDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        

        [HttpGet("FormResponses/{formResponsesId}")]
        [SwaggerOperation("Get Detail Form Submission", "Get details form submission")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FormResponseDetailDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetDetailFormSubmission(Guid formResponsesId)
        {
            try
            {
                var formResponse = _bll.GetDetailFormResponse(formResponsesId);
                return Ok(formResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DuplicateForm")]
        [SwaggerOperation("Duplicate a Form", "This endpoint will duplicationg form base on gived formid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DuplicateForm(Guid formId)
        {
            try
            {
                var result = await _bll.DuplicateForm(formId);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPost("DuplicateFormBlock")]
        [SwaggerOperation("Duplicate a FormBlock", "This endpoint will duplicationg form block base on gived formBlockid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DuplicateFormBlock(Guid formBlockId)
        {
            try
            {
                var result = await _bll.DuplicateFormBlock(formBlockId);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("GetOnlineVersion")]
        [SwaggerOperation("To check wheater the offline version form is have online version or not", "To check wheater the offline version form is have online version or not")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OnlineVersionCheckDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult IsOfflineFormHaveOnlineVersion(int formID)
        {
            try
            {
                var result = _bll.IsOfflineFormHaveOnlineVersion(formID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("FormSubmission")]
        [SwaggerOperation("To create new form submission", "To create new form submission")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitFormRequestAsync(CreateFormRequestDTO createFormRequestDTO)
        {
            try
            {
                var result = await _bll.SubmitFormRequestAsync(createFormRequestDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }
        
        [HttpDelete("{formId}/FormNodes/{formNodeId}")]
        [SwaggerOperation("Delete a FormNode", "To delete a FormNode from a Form")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFormNodeSection(Guid formId, Guid formNodeId)
        {
            try
            {
                if (formId != Guid.Empty && formNodeId != Guid.Empty)
                {
                    await _bll.DeleteFormNodeSectionAsync(formId, formNodeId);
                    return Ok();
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }
        
        [HttpGet("ViewForms")]
        [SwaggerOperation("Get list of available form for user side base on search parameter and user's LE/OE tags", "Get list avaiable form for user side base on search parameter and user's LE/OE tags")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagingResultDTO<ListViewFormsDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetViewForms([FromQuery, SwaggerParameter("paging pagenumber, start from 1", Required = true)] int pageNumber, [FromQuery, SwaggerParameter("paging pagesize, the number of records showed in each page", Required = true)] int pageSize, [FromQuery, SwaggerParameter("current selected ui language possible values [en, fr, nl, ...etc]", Required = true)] string languageCode, [FromQuery, SwaggerParameter("form label to search", Required = false)] string? searchLabel, [FromQuery, SwaggerParameter("current selected LE/OE Cbenumber", Required = false)] string? cbeNumber)
        {
            try
            {
                var forms = await _bll.ViewAllForms(pageNumber, pageSize, languageCode, searchLabel, cbeNumber);
                return Ok(forms);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDashboardIntroduceRequestFormList")]
        [SwaggerOperation("Get list of available form for user side base on user's LE/OE match tags for use in dashboard widget", "Get list of available form for user side base on user's LE/OE match tags for use in dashboard widget")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagingResultDTO<ListViewFormsDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDashboardIntroduceRequestFormList([FromQuery, SwaggerParameter("paging pagenumber, start from 1", Required = true)] int pageNumber, [FromQuery, SwaggerParameter("paging pagesize, the number of records showed in each page", Required = true)] int pageSize, [FromQuery, SwaggerParameter("current selected ui language possible values [en, fr, nl, ...etc]", Required = true)] string languageCode, [FromQuery, SwaggerParameter("current selected LE/OE Cbenumber", Required = false)] string? cbeNumber)
        {
            try
            {
				var userExternalId = HttpContext?.User?.Identity?.AsClaimIdentity().GetUserExternalId();

				var forms = await _bll.ViewFormsWithTag(pageNumber, pageSize, userExternalId ?? string.Empty, languageCode, null, cbeNumber, true);
                
                return Ok(forms);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("{formId}/Preview")]
        [SwaggerOperation("Preview a form", "Preview a form")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PreviewFormDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult PreviewForm(Guid formId)
        {
            try
            {
                var result = _bll.PreviewAsync(formId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
		[HttpGet("{formId}/Download")]
		public async Task<IActionResult> DownloadForm(Guid formId, [FromQuery]string languageCode)
		{
			try
			{
				var result = await _bll.DownloadAsync(formId, languageCode);

				return File(result.Content, result.MimeType, result.Name);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[AllowAnonymous]
		[HttpGet("documents/{docId}/download")]
		public async Task<IActionResult> DownloadDocumentForm(Guid docId)
		{
			try
			{
				var result = await _bll.DownloadDocumentAsync(docId);

				return File(result.Content, result.MimeType, result.Name);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}

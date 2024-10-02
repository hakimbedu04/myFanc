using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using MyFanc.BusinessObjects;
using MyFanc.Core;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Forms;
using MyFanc.DTO.Internal.OperationalEntity;
using MyFanc.DTO.Internal.Translation;
using MyFanc.DTO.Internal.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Transactions;
using static MyFanc.Core.Enums;

namespace MyFanc.BLL
{
    public partial class Bll : IBll
    {
        public async Task<FormBlockDTO> CreateFormBlockAsync(CreateFormBlockDTO createFormBlockDTO)
        {
            if (createFormBlockDTO != null)
            {
                var createFormEntity = _mapper.Map<Form>(createFormBlockDTO);
                var newEksternalId = _formRepository.Find(f => f.Type == FormType.Block.ToString()).Count() + 1;
                createFormEntity.ExternalId = newEksternalId;
                _formRepository.Add(createFormEntity);
                var savedEntries = await _unitOfWork.SaveChangesAsync();
                if (savedEntries <= 0)
                {
                    _logger.LogError("The insert of the Formblock has failed for an unknown reason");
                    throw new Exception("no change has been commited, Formblock not written");
                }
                return _mapper.Map<FormBlockDTO>(createFormEntity);
            }
            throw new ArgumentException("CreateFormBlockDTO cannot be null");
        }

        public IEnumerable<ListFormBlockDTO> GetListFormBlock(int? id, string? label, IsUsedParam isUsed)
        {
            var usedFormBlock = _formNodeFieldsRepository.Find(n => n.Type == Enums.FormNodeFieldEncodeType.Container.ToString()
                                                                && n.FormNodes.Type == Enums.FormNodeType.FormBlock.ToString()
                                                                && n.Property == "Value"
                                                                && !n.FormNodes.Form.DeletedTime.HasValue);
            var usedFormBlockAllowEditIds = usedFormBlock.Where(x => x.FormNodes.Form.Status == FormStatus.Online || x.FormNodes.Form.Status == FormStatus.Offline)
                                            .Select(n => n.Value).ToList();
            var usedFormBlockIds = usedFormBlock.Select(n => n.Value).ToList();

            var formBlocks = _formRepository.Find(f => !f.DeletedTime.HasValue
                                                    && f.Type.ToLower() == FormType.Block.ToString().ToLower()
                                                    && (f.ExternalId == id || id == null)
                                                    && (string.IsNullOrEmpty(label) || f.Labels.Any(l => l.Text.Contains(label)))
                                                    && (isUsed == IsUsedParam.All
                                                        || (isUsed == IsUsedParam.Used && usedFormBlockIds.Contains(f.Id.ToString()))
                                                        || (isUsed == IsUsedParam.NotUsed && !usedFormBlockIds.Any(i => i == f.Id.ToString())))
                                                )
                                               .Include(f => f.Labels)
                                               .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue))
                                               .OrderBy(f => f.ExternalId)
                                               .AsSplitQuery()
                                               .ToList();

            var listFormBlockDTO = _mapper.Map<List<ListFormBlockDTO>>(formBlocks, opt =>
            {
                opt.Items["AdditionalParam"] = usedFormBlockIds;
                opt.Items["AdditionalParam2"] = usedFormBlockAllowEditIds;
            });

            return listFormBlockDTO;
        }

        public async Task<FormBlockDTO> UpdateFormBlockAsync(Guid formBlockId, UpdateFormBlockDTO updateFormBlockDTO)
        {
            if (updateFormBlockDTO == null)
            {
                throw new ArgumentNullException(nameof(updateFormBlockDTO), "UpdateFormBlockDTO cannot be null");
            }

            var existingFromBlock = _formRepository.Find(q => q.Id == formBlockId && !q.DeletedTime.HasValue)
                .Include(q => q.Labels)
                .AsSplitQuery()
                .AsTracking()
                .FirstOrDefault() ?? throw new KeyNotFoundException($"Formblock with ID {formBlockId} not found");
			_mapper.Map(updateFormBlockDTO, existingFromBlock);
            ApplyChangeOnTranslation(updateFormBlockDTO.Labels, existingFromBlock.Labels);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<FormBlockDTO>(existingFromBlock);
        }

        private bool IsFormBlockUsedByForm(Guid formBlockId)
        {
            return _formNodeFieldsRepository.Find(f => f.Property == "Value"
                && f.Value == formBlockId.ToString()
                && !f.DeletedTime.HasValue
                && !f.FormNodes.Form.DeletedTime.HasValue).Any();        
        }

        public async Task DeleteFormBlockAsync(Guid formBlockId)
        {
            if (IsFormBlockUsedByForm(formBlockId))
                throw new InvalidOperationException($"Can't delete form block with id {formBlockId}, the form block is used by other Form");

            var formBlockEntity = _formRepository.Find(f => f.Id == formBlockId && !f.DeletedTime.HasValue)
                .Include(f => f.FormNodes).ThenInclude(n => n.FormNodeFields).ThenInclude(f => f.FormValueFields)
                .Include(f => f.FormNodes).ThenInclude(n => n.FormNodeFields).ThenInclude(f => f.FormConditionals)
                .AsSplitQuery()
                .AsTracking()
                .FirstOrDefault();

            if (formBlockEntity != null)
            {
                _formRepository.Delete(formBlockEntity);
                var savedEntries = await _unitOfWork.SaveChangesAsync();
                if (savedEntries <= 0)
                {
                    _logger.LogError("The deletion of the Formblock has failed for an unknown reason formblock id {formBlockId}", formBlockId);
                    throw new Exception("no change has been commited, FormBlock not written");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Delete formblock tried on unexisting formblock id {formBlockId}");
            }
        }

        private void ValidateCreateUpdateFormNodeAsync(CreateUpdateFormNodeDto createUpdateFormNodeDTO)
        {
            if (IsFormIsPdf(createUpdateFormNodeDTO.FormId)) throw new ArgumentException($"Form with id {createUpdateFormNodeDTO.FormId} is a PDF type and cannot have form content!");
            if (createUpdateFormNodeDTO == null) throw new ArgumentException("CreateUpdateFormNodeDto cannot be null");
            var form = _formRepository.Find(f => f.Id == createUpdateFormNodeDTO.FormId).FirstOrDefault();

            if (form != null && (form.Status != FormStatus.Draft) && (form.Type != FormType.Block.ToString()))
                throw new ArgumentException("Form with status Online or Offline can't be modified.");

            if (form != null && form.Type == FormType.Block.ToString() && IsFormBlockLinkedToNonDraftForm(form.Id))
                throw new ArgumentException("Form block linked to form with status Online or Offline can't be modified.");

            if (createUpdateFormNodeDTO.ParentId == null && form != null && form.Type != FormType.Block.ToString() && createUpdateFormNodeDTO.Type != FormNodeType.Section.ToString())
                throw new ArgumentException("Cannot add/update FormField/FormBlock directly to the Form");
        }

        public async Task<Guid> CreateUpdateFormNodeAsync(CreateUpdateFormNodeDto createUpdateFormNodeDTO)
        {
            ValidateCreateUpdateFormNodeAsync(createUpdateFormNodeDTO);
            if (createUpdateFormNodeDTO.Id == null || createUpdateFormNodeDTO.Id == Guid.Empty)
            {
                var createFormNodeEntity = _mapper.Map<FormNodes>(createUpdateFormNodeDTO);
                createFormNodeEntity.Labels = _mapper.Map<List<Translation>>(createUpdateFormNodeDTO.Labels);
                var order = _formNodesRepository.Find(f => !f.DeletedTime.HasValue && f.FormId == createUpdateFormNodeDTO.FormId && f.ParentId == createUpdateFormNodeDTO.ParentId).Any() ? _formNodesRepository.Find(f => !f.DeletedTime.HasValue && f.FormId == createUpdateFormNodeDTO.FormId && f.ParentId == createUpdateFormNodeDTO.ParentId).Max(f => f.Order) + 1 : 1;
                createFormNodeEntity.Order = order;
                _formNodesRepository.Add(createFormNodeEntity);
                await _unitOfWork.SaveChangesAsync();
                return createFormNodeEntity.Id;
            }
            else
            {
                if (createUpdateFormNodeDTO.Type == FormNodeType.Section.ToString())
                {
                    var param = _mapper.Map<List<CreateUpdateFormNodesDto>>(createUpdateFormNodeDTO.FormNodes);
                    var item = new CreateUpdateFormContentDto { FormNodes = param };
                    await ProcessCreateUpdateFormContentAsync(item);
                }

                var editFormNodeEntity = _formNodesRepository.Find(n => n.Id == createUpdateFormNodeDTO.Id).Include(n => n.Labels).AsTracking().FirstOrDefault();
                if (editFormNodeEntity != null)
                {
                    _mapper.Map(createUpdateFormNodeDTO, editFormNodeEntity);
                    ApplyChangeOnTranslation(createUpdateFormNodeDTO.Labels, editFormNodeEntity.Labels);

                    await _unitOfWork.SaveChangesAsync();
                    return editFormNodeEntity.Id;
                }
                throw new ArgumentException("Cannot find FormNode");
            }
        }

        private static bool IsFormContentValid(CreateUpdateFormContentDto createUpdateFormContentDTO)
        {
            var formNodeFIelds = createUpdateFormContentDTO.FormNodes.SelectMany(f => f.FormNodeFields);
            return !formNodeFIelds.Any(f => f.Property.ToLower() == "id" && string.IsNullOrEmpty(f.Value));
        }

        private bool IsFormIsPdf(Guid formId)
        {
            var formType = _formRepository.Find(f => f.Id == formId && !f.DeletedTime.HasValue).Select(f => f.Type).FirstOrDefault();
            if (formType == FormType.Pdf.ToString()) return true;
            return false;
        }

        private void ValidateCreateUpdateFormContent(Guid formId, CreateUpdateFormContentDto createUpdateFormContentDTO)
        {
            if (IsFormIsPdf(formId)) throw new ArgumentException($"Form with id {formId} is a PDF type and cannot have form content!");
            if (!IsFormContentValid(createUpdateFormContentDTO)) throw new ArgumentException($"Form with id {formId} contain node that have null value for field ID, please fill ID value (ID is required)");
        }

        public async Task<List<Guid>> CreateUpdateFormContentAsync(Guid formId, CreateUpdateFormContentDto createUpdateFormContentDTO)
        {
            ValidateCreateUpdateFormContent(formId, createUpdateFormContentDTO);
            try
            {
                _unitOfWork.BeginTransaction();
                var result = await ProcessCreateUpdateFormContentAsync(createUpdateFormContentDTO);
                _unitOfWork.CommitTransaction();
                return result;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
				throw;
			}
        }

        private async Task<List<Guid>> ProcessCreateUpdateFormContentAsync(CreateUpdateFormContentDto createUpdateFormContentDTO)
        {
            var result = new List<Guid>();
            foreach (var formNode in createUpdateFormContentDTO.FormNodes)
            {
                var formNodeId = await CreateUpdateFormNodeAsync(formNode);
                result.Add(formNodeId);

                foreach (var formNodeField in formNode.FormNodeFields)
                {
                    formNodeField.FormNodeId = formNodeId;
                    await CreateUpdateFormNodeFieldAsync(formNodeField);
                }
            }
            return result;
        }

        public List<string> GetFormObjectList()
        {
            var result = new List<string>();

            var userProperties = GetProperties("ConnectedUser", typeof(ProfileDTO));
            var organisationProperties = GetProperties("Organisation", typeof(OrganisationEnterpriseDTO));
            var structuredAddressProperties = GetProperties("ConnectedUser.StructuredAddress", typeof(ProfileInfoAddressDTO));
            var unstructuredAddressProperties = GetProperties("ConnectedUser.UnstructuredAddress", typeof(ProfileInfoAddressUnstructuredDTO));
            var mainAddressProperties = GetProperties("Organisation.MainAddress", typeof(ProfileInfoAddressDTO));
            var invoiceAddressProperties = GetProperties("Organisation.InvoiceAddress", typeof(ProfileInfoAddressDTO));
            var sectorsProperties = GetProperties("Organisation.Sectors", typeof(NacabelsCodeDTO));

            result.AddRange(userProperties);
            result.AddRange(organisationProperties);
            result.AddRange(structuredAddressProperties);
            result.AddRange(unstructuredAddressProperties);
            result.AddRange(mainAddressProperties);
            result.AddRange(invoiceAddressProperties);
            result.AddRange(sectorsProperties);

            return result;
        }
        private List<string> GetProperties(string prefix, Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => $"{prefix}.{p.Name}")
                .ToList();
        }

        private void ValidateFormNodeField(CreateUpdateFormNodeFieldDto createUpdateFormNodeFieldDTO)
        {
            if (createUpdateFormNodeFieldDTO.Property.ToLower() == "id")
            {
                if (string.IsNullOrEmpty(createUpdateFormNodeFieldDTO.Value))
                    throw new ArgumentException("Field/Property named 'ID' can't be null/empty");

                var formNode = _formNodesRepository.Find(n => n.Id == createUpdateFormNodeFieldDTO.FormNodeId)
                    .FirstOrDefault();
                if (formNode != null)
                {
                    var form = _formRepository.Find(n => n.Id == formNode.FormId)
                    .Include(n => n.FormNodes.Where(f => !f.DeletedTime.HasValue))
                    .ThenInclude(f => f.FormNodeFields.Where(n => !n.DeletedTime.HasValue))
                    .FirstOrDefault();
                    var allIDProperty = form?.FormNodes?.SelectMany(x => x.FormNodeFields)?.Where(f => f.Property.ToLower() == "id" && f.Id != createUpdateFormNodeFieldDTO.Id);
                    if (allIDProperty != null && allIDProperty.Select(p => p.Value.ToLower()).Contains(createUpdateFormNodeFieldDTO.Value.ToLower()))
                        throw new ArgumentException("The 'ID' field of each form element (form field, form block, section) should be unique in a form");
                }
            }
        }

        public async Task<Guid> CreateUpdateFormNodeFieldAsync(CreateUpdateFormNodeFieldDto createUpdateFormNodeFieldDTO)
        {
            if (createUpdateFormNodeFieldDTO != null)
            {
                ValidateFormNodeField(createUpdateFormNodeFieldDTO);

                if (createUpdateFormNodeFieldDTO.Id == null || createUpdateFormNodeFieldDTO.Id == Guid.Empty)
                {
                    FormNodeFields formNodeFieldEntity = _mapper.Map<FormNodeFields>(createUpdateFormNodeFieldDTO);
                    FillPropertyValue(formNodeFieldEntity, createUpdateFormNodeFieldDTO);
                    _formNodeFieldsRepository.Add(formNodeFieldEntity);
                    await _unitOfWork.SaveChangesAsync();

                    return formNodeFieldEntity.Id;
                }
                else
                {
                    var editFormNodeFieldEntity = _formNodeFieldsRepository.Find(f => f.Id == createUpdateFormNodeFieldDTO.Id)
                            .Include(f => f.FormValueFields.Where(v => !v.DeletedTime.HasValue)).ThenInclude(v => v.Labels)
                            .Include(f => f.FormConditionals.Where(c => !c.DeletedTime.HasValue))
                            .Include(f => f.Labels)
                            .AsSplitQuery()
                            .AsTracking()
                            .FirstOrDefault();
                    if (editFormNodeFieldEntity != null)
                    {
                        _mapper.Map(createUpdateFormNodeFieldDTO, editFormNodeFieldEntity);
                        FillPropertyValue(editFormNodeFieldEntity, createUpdateFormNodeFieldDTO);
                        
                        await _unitOfWork.SaveChangesAsync();
                        return editFormNodeFieldEntity.Id;
                    }
                    throw new ArgumentException("Cannot find FormNodeField");
                }
            }
            throw new ArgumentException("CreateUpdateFormNodeFieldDto cannot be null");
        }

        private void FillPropertyValue(FormNodeFields field, CreateUpdateFormNodeFieldDto property)
        {
            if (IsTextType(property.Type))
            {
                field.Value = property.Value;
                return;
            }

            if (property.Type == Enums.FormNodeFieldEncodeType.Translation.ToString())
            {
                HandleTranslationProperty(field, property);
                return;
            }

            if (property.Type == Enums.FormNodeFieldEncodeType.CustomList.ToString())
            {
                HandleCustomListProperty(field, property);
                return;
            }

            if (property.Type == Enums.FormNodeFieldEncodeType.Conditional.ToString())
            {
                HandleConditionalProperty(field, property);
            }
        }

        private static bool IsTextType(string type)
        {
            return type == Enums.FormNodeFieldEncodeType.Text.ToString()
                || type == Enums.FormNodeFieldEncodeType.File.ToString()
                || type == Enums.FormNodeFieldEncodeType.ArrayText.ToString()
                || type == Enums.FormNodeFieldEncodeType.PredefineList.ToString();
        }

        private void HandleTranslationProperty(FormNodeFields field, CreateUpdateFormNodeFieldDto property)
        {
            if (field.Id == Guid.Empty)
            {
                var tranlationValue = JsonConvert.DeserializeObject<List<TranslationDTO>>(property.Value);
                field.Labels = tranlationValue != null ? _mapper.Map<List<Translation>>(tranlationValue) : new List<Translation>();
            }
            else
            {
                var tranlationValue = JsonConvert.DeserializeObject<List<TranslationDTO>>(property.Value);
                if(tranlationValue != null)
                    ApplyChangeOnTranslation(tranlationValue, field.Labels);
            }
        }

        private void HandleCustomListProperty(FormNodeFields field, CreateUpdateFormNodeFieldDto property)
        {
            var customListValue = JsonConvert.DeserializeObject<List<CustomListDTO>>(property.Value);
            if (customListValue != null)
            {
                if (field.Id == Guid.Empty)
                {

                    field.Value = CreateNewFormDataSource();
                    field.FormValueFields = CreateFormValueFields(customListValue);

                }
                else
                {
                    ApplyChangeOnFormValueFields(customListValue, field.FormValueFields, field);
                }
            }
        }
        private void HandleConditionalProperty(FormNodeFields field, CreateUpdateFormNodeFieldDto property)
        {
            var conditionalsValue = JsonConvert.DeserializeObject<List<FormConditionalDTO>>(property.Value);

            if (field.Id == Guid.Empty)
            {
                field.FormConditionals = _mapper.Map<List<FormConditionals>>(conditionalsValue);
            }
            else
            {
                if(conditionalsValue != null)
                    ApplyChangeOnFormConditionals(conditionalsValue, field.FormConditionals);
            }
        }

        private string CreateNewFormDataSource()
        {
            FormDataSource formDataSourceEntity = new FormDataSource()
            {
                Name = GenerateRandomNameForDataSource()
            };

            _formDataSourceRepository.Add(formDataSourceEntity);
            _unitOfWork.SaveChangesAsync();

            return formDataSourceEntity.Id.ToString();
        }

        private List<FormValueFields> CreateFormValueFields(List<CustomListDTO> customListValue)
        {
            var formValueFields = new List<FormValueFields>();

            foreach (var item in customListValue)
            {
                var formValueField = new FormValueFields()
                {
                    Labels = _mapper.Map<List<Translation>>(item.Translations),
                    Value = item.Value,
                    Order = item.Order
                };

                formValueFields.Add(formValueField);
            }

            return formValueFields;
        }

        private void ApplyChangeOnFormValueFields(ICollection<CustomListDTO> customListDTO, ICollection<FormValueFields> formValueFieldsEntity, FormNodeFields field)
        {
            var toDelete = formValueFieldsEntity.Where(e => !customListDTO.Any(d => d.Id == e.Id)).ToList();
            foreach (var item in toDelete.ToList())
            {
                var deletedItem = formValueFieldsEntity.Single(t => t.Id == item.Id);
                formValueFieldsEntity.Remove(deletedItem);
            }
            foreach (var item in customListDTO)
            {
                var existing = formValueFieldsEntity.FirstOrDefault(t => t.Id == item.Id && (item.Id != Guid.Empty && item.Id != null));
                if (existing != null)
                {
                    ApplyChangeOnTranslation(item.Translations, existing.Labels);
                    existing.Value = item.Value;
                    existing.Order = item.Order;
                }
                else
                {
                    var formValueField = new FormValueFields()
                    {
                        Labels = _mapper.Map<List<Translation>>(item.Translations),
                        Value = item.Value,
                        Order = item.Order
                    };

                    formValueFieldsEntity.Add(formValueField);
                }
            }
        }

        private void ApplyChangeOnFormConditionals(ICollection<FormConditionalDTO> fomConditionalDTO, ICollection<FormConditionals> formConditionalEntity)
        {
            var toDelete = formConditionalEntity.Where(e => !fomConditionalDTO.Any(d => d.Id == e.Id)).ToList();
            foreach (var item in toDelete.ToList())
            {
                var deletedItem = formConditionalEntity.Single(t => t.Id == item.Id);
                formConditionalEntity.Remove(deletedItem);
            }
            foreach (var item in fomConditionalDTO)
            {
                var existing = formConditionalEntity.FirstOrDefault(t => t.Id == item.Id);
                if (existing != null)
                {
                    _mapper.Map(item, existing);
                }
                else
                {
                    formConditionalEntity.Add(_mapper.Map<FormConditionals>(item));
                }
            }
        }
        private string GenerateRandomNameForDataSource()
        {
            string prefix = "CUSTOM_DATASOURCE";
            long ticks = DateTime.Now.Ticks;
            string randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
            string randomName = $"{prefix}_{ticks}_{randomSuffix}";

            return randomName;
        }

        public FormContentDTO GetFormContent(Guid formId, bool isFormBlock = false)
        {
            if (formId != Guid.Empty)
            {
                var form = _formRepository.Find(f => f.Id == formId && !f.DeletedTime.HasValue)
                    .Include(f => f.Labels)
                    .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue))
                    .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue)).ThenInclude(f => f.Labels)
                    .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue)).ThenInclude(f => f.Children.Where(c => !c.DeletedTime.HasValue))
                    .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue)).ThenInclude(f => f.FormNodeFields.Where(d => !d.DeletedTime.HasValue))
                    .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue)).ThenInclude(f => f.FormNodeFields.Where(d => !d.DeletedTime.HasValue)).ThenInclude(n => n.Labels)
                    .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue)).ThenInclude(f => f.FormNodeFields.Where(d => !d.DeletedTime.HasValue)).ThenInclude(n => n.FormValueFields.Where(v => !v.DeletedTime.HasValue))
                    .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue)).ThenInclude(f => f.FormNodeFields.Where(d => !d.DeletedTime.HasValue)).ThenInclude(n => n.FormValueFields.Where(v => !v.DeletedTime.HasValue)).ThenInclude(v => v.Labels)
                    .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue)).ThenInclude(f => f.FormNodeFields.Where(d => !d.DeletedTime.HasValue)).ThenInclude(n => n.FormConditionals.Where(c => !c.DeletedTime.HasValue))
                    .FirstOrDefault();

                if (form != null || isFormBlock)
                {
                    var result = form != null ? _mapper.Map<FormContentDTO>(form) : new FormContentDTO();
                    if (form != null)
                    {
                        FillCorrectValue(form, result.FormNodes);
                        ProcessFormBlockContent(form, result.FormNodes);
                        ProcessSectionContent(form, result.FormNodes);
                        result.FormNodes = result.FormNodes.OrderBy(n => n.Order).ToList();
                    }
                    return result;

                }
                throw new KeyNotFoundException($"Cannot get form {formId}. {MyFanc.Services.Constants.FormNotFound}");
                
            }
            throw new ArgumentException($"Parameter is invalid (Value was: {formId}");
        }
        private void ProcessFormBlockContent(Form form, List<FormNodesDTO> result)
        {
            var formBLocks = form.FormNodes.Where(n => n.Type == Enums.FormNodeType.FormBlock.ToString() && !n.DeletedTime.HasValue);
            foreach (var formBLock in formBLocks)
            {
                var formBLockProperty = formBLock.FormNodeFields.FirstOrDefault(p => p.Property == "Value");
                if (formBLockProperty != null)
                {
                    var matchingItems = result.Where(x => x.Type == Enums.FormNodeType.FormBlock.ToString() && x.FormNodeFields.Any(f => f.Value == formBLockProperty.Value));
                    foreach (var item in matchingItems)
                    {
                        item.IsEditContentAllowed = !IsFormBlockLinkedToNonDraftForm(new Guid(item.FormNodeFields.First(x => x.Property == "Value").Value));
                    }

                    var formBlockResult = result.FirstOrDefault(n => n.Id == formBLock.Id);
                    if (formBlockResult != null)
                    {
                        var formBlockContent = GetFormContent(Guid.Parse(formBLockProperty.Value), true);
                        formBlockResult.FormNodes = formBlockContent.FormNodes.OrderBy(n => n.Order).ToList();
                    }
                }
            }
        }

        private void ProcessSectionContent(Form form, List<FormNodesDTO> resultDto, Guid? parentId = null)
        {
            var sectionEntities = form.FormNodes.Where(n => n.Type == Enums.FormNodeType.Section.ToString() && n.ParentId == parentId && !n.DeletedTime.HasValue);
            foreach (var sectionEntity in sectionEntities)
            {
                var sectionDto = resultDto.First(n => n.Id == sectionEntity.Id);
                if(sectionDto != null)
                {
                    sectionDto.FormNodes = _mapper.Map<List<FormNodesDTO>>(sectionEntity.Children).OrderBy(n => n.Order).ToList();
                    FillCorrectValue(form, sectionDto.FormNodes);
                    ProcessFormBlockContent(form, sectionDto.FormNodes);
                    ProcessSectionContent(form, sectionDto.FormNodes, sectionDto.Id);
                }
            }
        }

        private void FillCorrectValue(Form form, List<FormNodesDTO> result)
        {
            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            foreach (var formNode in form.FormNodes)
            {
                foreach (var field in formNode.FormNodeFields)
                {
                    var resultNode = result.Find(x => x.Id == formNode.Id);
                    var resultField = resultNode?.FormNodeFields.Find(x => x.Id == field.Id);
                    if (resultField != null)
                    {
                        string val = field.Value;
                        if (field.Type == FormNodeFieldEncodeType.Translation.ToString())
                        {
                            var tranlationDto = _mapper.Map<List<TranslationDTO>>(field.Labels);
                            val = JsonConvert.SerializeObject(tranlationDto, options);
                        }
                        else if (field.Type == FormNodeFieldEncodeType.CustomList.ToString())
                        {
                            var custList = _mapper.Map<List<CustomListDTO>>(field.FormValueFields.OrderBy(x => x.Order));
                            val = JsonConvert.SerializeObject(custList, options);

                        }
                        else if (field.Type == FormNodeFieldEncodeType.Conditional.ToString())
                        {
                            var conditionals = _mapper.Map<List<FormConditionalDTO>>(field.FormConditionals.OrderBy(x => x.CreationTime));
                            val = JsonConvert.SerializeObject(conditionals, options);

                        }
                    
                        resultField.Value = val;
                    }
                }
            }
        }

        private void ValidateAddFormBlockToForm(Form? form, Form? formBlockToAdd)
        {
            var formType =  form?.Type; 
            if (formType != null && formType == FormType.Pdf.ToString())
                throw new ArgumentException($"Form with id {form?.Id} is a PDF type and cannot have form content!");

            if (form != null && (form.Status != FormStatus.Draft))
                throw new ArgumentException("Form with status Online or Offline can't be modified.");

            var allFormIDProperties = form?.FormNodes?.SelectMany(x => x.FormNodeFields.Where(f => f.Property.ToLower() == "id"))?.Select(x => x.Value);
            var allFormBlockIDProperties = formBlockToAdd?.FormNodes?.SelectMany(x => x.FormNodeFields.Where(f => f.Property.ToLower() == "id"))?.Select(x => x.Value);
            if (allFormIDProperties != null && allFormBlockIDProperties != null)
            {
                foreach (var idPropety in allFormBlockIDProperties)
                {
                    if (allFormIDProperties.Any(x => x.ToLower() == idPropety.ToLower()))
                        throw new ArgumentException("The 'ID' field of each form element (form field, form block, section) should be unique in a form");
                }
            }
        }

        public async Task<Guid> AddFormBlockToForm(AddFormBlockToFormDTO addFormBlockToFormDTO)
        {
            if (addFormBlockToFormDTO != null)
            {
                var form = _formRepository.Find(f => f.Id == addFormBlockToFormDTO.FormId)
                    .Include(f => f.FormNodes.Where(x => !x.DeletedTime.HasValue))
                    .ThenInclude(f => f.FormNodeFields.Where(x => !x.DeletedTime.HasValue))
                    .FirstOrDefault();

                var formBlockToAdd = _formRepository.Find(f => f.Id == addFormBlockToFormDTO.FormBlockId)
                    .Include(f => f.Labels)
                    .Include(f => f.FormNodes.Where(x => !x.DeletedTime.HasValue))
                    .ThenInclude(n => n.FormNodeFields.Where(x => !x.DeletedTime.HasValue))
                    .FirstOrDefault();

                ValidateAddFormBlockToForm(form, formBlockToAdd);

                var section = _formNodesRepository.Find(n => n.Id == addFormBlockToFormDTO.SectionId && n.FormId == addFormBlockToFormDTO.FormId)
                    .Include(f => f.Children)
                    .FirstOrDefault();
                
                if (formBlockToAdd != null && section != null)
                {
                    try
                    {
                        _unitOfWork.BeginTransaction();
                        var createFormNodeEntity = new FormNodes()
                        {
                            FormId = addFormBlockToFormDTO.FormId,
                            ParentId = addFormBlockToFormDTO.SectionId,
                            IsActive = true,
                            Labels = _mapper.Map<List<Translation>>(formBlockToAdd.Labels),
                            Version = section.Version,
                            Type = Enums.FormNodeType.FormBlock.ToString(),
                            Order = section.Children != null && section.Children.Count > 0 ? section.Children.Max(x => x.Order) + 1 : 1,
                            FieldType = ""
                        };
                        _formNodesRepository.Add(createFormNodeEntity);
                        await _unitOfWork.SaveChangesAsync();
                        var formNodeFieldEntity = new FormNodeFields()
                        {
                            FormNodeId = createFormNodeEntity.Id,
                            Property = "Value",
                            Value = formBlockToAdd.Id.ToString(),
                            Type = Enums.FormNodeFieldEncodeType.Container.ToString()
                        };
                        _formNodeFieldsRepository.Add(formNodeFieldEntity);
                        await _unitOfWork.SaveChangesAsync();
                        _unitOfWork.CommitTransaction();
                        return createFormNodeEntity.Id;
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackTransaction();

                        throw;
                    }
                }
                throw new ArgumentException($"Cannot find FormBlock/Section id: {addFormBlockToFormDTO.FormId}/{addFormBlockToFormDTO.SectionId}");
            }
            throw new ArgumentException("CreateUpdateFormNodeDto cannot be null");
        }

        public async Task<FormDTO> CreateFormAsync(CreateFormDTO createFormDTO)
        {
            if (createFormDTO != null)
            {
                var queryForm = _formRepository.Find(f => (f.Type == FormType.Webform.ToString() || f.Type == FormType.Pdf.ToString()));
                var newEksternalId = queryForm.Any() ? queryForm.Max(f => f.ExternalId) + 1 : 1;

				if (createFormDTO.IsFormPdf)
				{
					await this.UploadFormDocumentAsync(createFormDTO, newEksternalId);
				}

				var createFormEntity = _mapper.Map<Form>(createFormDTO);
                createFormEntity.ExternalId = newEksternalId;
                createFormEntity.Status = FormStatus.Draft;
                createFormEntity.Type = createFormDTO.IsFormPdf ? FormType.Pdf.ToString() : FormType.Webform.ToString();

				var nacabelEntities = new List<Nacabel>();
                foreach (var nacabel in createFormDTO.Tags)
                {
                    var nacabelEntity = await _nacabelRepository.GetByIdAsync(nacabel);
                    if (nacabelEntity != null)
                        nacabelEntities.Add(nacabelEntity);
                }
                
                createFormEntity.Nacabels = nacabelEntities;
                _formRepository.Add(createFormEntity);
                var savedEntries = await _unitOfWork.SaveChangesAsync();
                if (savedEntries <= 0)
                {
                    _logger.LogError("The insert of the Form has failed for an unknown reason");
                    throw new Exception("no change has been commited, Form not written");
                }

				return _mapper.Map<FormDTO>(createFormEntity);
            }
            throw new ArgumentException("createFormDTO cannot be null");
        }

        public IEnumerable<FormCategoryDTO> GetListFormCategory()
        {
            var formCategories = _formCategoryRepository.Find(f => (!f.DeletedTime.HasValue))
                                               .OrderBy(f => f.Code)
                                               .AsSplitQuery()
                                               .ToList();

            var listFormCategoryDTO = _mapper.Map<List<FormCategoryDTO>>(formCategories);
            return listFormCategoryDTO;
        }

        public FormDTO GetFormDetails(Guid formId)
        {
            var formEntity = _formRepository.Find(q => q.Id == formId && !q.DeletedTime.HasValue)
                .Include(f => f.Labels)
                .Include(f => f.Urls)
                .Include(f => f.Descriptions)
                .Include(f => f.FormCategory)
                .Include(f => f.Nacabels).ThenInclude(n => n.NacabelTranslation)
                .Include(f => f.FormDocuments.Where(x => !x.DeletedTime.HasValue).OrderBy(y => y.LanguageCode))
                    .ThenInclude(d => d.Documents.Where(y => !y.DeletedTime.HasValue))
                .AsSplitQuery()
                .FirstOrDefault();
            
            return _mapper.Map<FormDTO>(formEntity);
        }

        public FormDTO GetFormDetailsByFormUrl(string shortUrl)
        {
            var formEntity = _formRepository.Find(q => q.Urls.Any(u => u.Text.ToLower() == shortUrl.ToLower()) && !q.DeletedTime.HasValue)
                .Include(f => f.Labels)
                .Include(f => f.Urls)
                .Include(f => f.Descriptions)
                .Include(f => f.FormCategory)
                .Include(f => f.Nacabels).ThenInclude(n => n.NacabelTranslation)
                .Include(f => f.FormDocuments.Where(x => !x.DeletedTime.HasValue).OrderBy(y => y.LanguageCode))
                    .ThenInclude(d => d.Documents.Where(y => !y.DeletedTime.HasValue))
                .AsSplitQuery()
                .FirstOrDefault();

            return _mapper.Map<FormDTO>(formEntity);
        }

        public bool IsShortUrlAlreadyUsed(string shortUrl, string languageCode)
        {
            var form = _formRepository.Find(f => f.Urls.Any(u => u.Text.ToLower() == shortUrl.ToLower() && u.LanguageCode == languageCode)).Any();
            return form;
        }

        public IEnumerable<ListFormDTO> GetListForm(SearchParamFormDTO searchDto)
        {
            SearchParamFormDTOExten searchParam = _mapper.Map<SearchParamFormDTOExten>(searchDto);
            var forms = _formRepository.Find(f => !f.DeletedTime.HasValue
                                                && (f.Type == "Webform" || f.Type == "Pdf")
                                                && (!searchParam.Id.HasValue || f.ExternalId.ToString().Contains(searchParam.Id.ToString()))
                                                && (string.IsNullOrEmpty(searchParam.Label) || f.Labels.Any(l => l.Text.Contains(searchParam.Label)))
                                                && (string.IsNullOrEmpty(searchParam.Category) || searchParam.CategoriesList.Contains(f.FormCategory != null ? f.FormCategory.Id : 0))
                                                && ((string.IsNullOrEmpty(searchParam.Status) && f.Status != FormStatus.Offline) || searchParam.StatusList.Contains(((int)f.Status)))
                                                && (string.IsNullOrEmpty(searchParam.Tag) || f.Nacabels.Any(n => searchParam.TagList.Contains(n.NacabelTranslation.Where(t => t.LanguageCode == searchParam.LanguageCode).FirstOrDefault().Description.ToLower())))
                                                && (string.IsNullOrEmpty(searchParam.Type) || searchParam.TypeList.Contains(f.Type.ToLower())))
                                               .Include(f => f.Labels)
                                               .Include(f => f.Urls)
                                               .Include(f => f.Nacabels).ThenInclude(n => n.NacabelTranslation.Where(t => t.LanguageCode == searchParam.LanguageCode))
                                               .Include(f => f.FormCategory)
                                               .OrderBy(f => f.ExternalId)
                                               .AsSplitQuery()
                                               .ToList();

            var listFormDTO = _mapper.Map<List<ListFormDTO>>(forms);
            return listFormDTO;
        }

        public async Task<FormDTO> UpdateFormAsync(Guid formId, UpdateFormDTO updateFormDTO)
        {
            if (updateFormDTO == null)
            {
                throw new ArgumentNullException(nameof(updateFormDTO), "UpdateFormDTO cannot be null");
            }

            var entityFrom = _formRepository.Find(q => q.Id == formId && !q.DeletedTime.HasValue)
                .Include(f => f.Labels)
                .Include(f => f.Urls)
                .Include(f => f.Descriptions)
                .Include(f => f.FormCategory)
                .Include(f => f.Nacabels)
                .Include(f => f.FormDocuments.Where(d => !d.DeletedTime.HasValue)).ThenInclude(d => d.Documents.Where(x => !x.DeletedTime.HasValue))
                .AsSplitQuery()
                .AsTracking()
                .FirstOrDefault() ?? throw new KeyNotFoundException($"Form with ID {formId} not found");
			_mapper.Map(updateFormDTO, entityFrom);
            ApplyChangeOnTranslation(updateFormDTO.Labels, entityFrom.Labels);
            ApplyChangeOnTranslation(updateFormDTO.Urls, entityFrom.Urls);
            ApplyChangeOnTranslation(updateFormDTO.Descriptions, entityFrom.Descriptions);
            ApplyChangeOnFormNacabels(updateFormDTO.Tags, entityFrom.Nacabels);
            await ApplyChangeOnFormDocumentsAsync(updateFormDTO.FormDocuments??new List<UpdateFormDocumentDTO>(), entityFrom.FormDocuments, formId, updateFormDTO.Version.ToString());
            entityFrom.Status = FormStatus.Draft;
            entityFrom.Type = updateFormDTO.IsFormPdf ? FormType.Pdf.ToString() : FormType.Webform.ToString();
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<FormDTO>(entityFrom);
        }

        private void ApplyChangeOnFormNacabels(IEnumerable<int> nacabelsDto, ICollection<Nacabel> nacabelsEntity)
        {
            var toDelete = nacabelsEntity.Where(t => !nacabelsDto.Any(s => s == t.Id)).ToList();
            foreach (var item in toDelete.ToList())
            {
                nacabelsEntity.Remove(item);
            }
            foreach (var item in nacabelsDto)
            {
                var existing = nacabelsEntity.FirstOrDefault(t => t.Id == item);
                if (existing == null)
                {
                    var nacabel = _nacabelRepository.Find(x => x.Id == item).First();
                    if (nacabel != null)
                        nacabelsEntity.Add(nacabel);
                }
            }
        }

        private async Task ApplyChangeOnFormDocumentsAsync(IEnumerable<UpdateFormDocumentDTO> documentsDto, ICollection<FormDocument> documentsEntity, Guid formId, string version)
        {
            var toDelete = documentsEntity.Where(t => !documentsDto.Any(s => s.Id == t.Id)).ToList();
            foreach (var item in toDelete.ToList())
            {
                documentsEntity.Remove(item);
            }
            foreach (var item in documentsDto)
            {
                var existing = documentsEntity.FirstOrDefault(t => t.Id == item.Id && item.Id != Guid.Empty);
                if (existing != null)
                {
                    _mapper.Map(item, existing);
                    await ApplyChangeOnDocumentsAsync(item.Documents, existing.Documents, formId, version);
                }
                else
                {
                    var newFormDocument = _mapper.Map<FormDocument>(item);
                    await ApplyChangeOnDocumentsAsync(item.Documents, newFormDocument.Documents, formId, version);
                    documentsEntity.Add(newFormDocument);
                }
            }
        }

        private async Task ApplyChangeOnDocumentsAsync(IEnumerable<UpdateDocumentDTO> documentsDto, ICollection<Document> documentsEntity, Guid formId, string version)
        {
            if (documentsEntity == null)
                documentsEntity = new List<Document>();

            var toDelete = documentsEntity.Where(t => !documentsDto.Any(s => s.Id == t.Id)).ToList();
            foreach (var item in toDelete.ToList())
            {
                await DeleteFileAsync(item);
                documentsEntity.Remove(item);
            }
            foreach (var item in documentsDto)
            {
                var existing = documentsEntity.FirstOrDefault(t => t.Id == item.Id && item.Id != Guid.Empty);
                if (existing != null)
                {
                    //for update document, if base64 is empty do not update the file, since it mean FE not want update the file
                    if (!string.IsNullOrEmpty(item.Base64))
                    {
                        await CreateUpdateFile(existing, item.Base64, formId, version);
                        _mapper.Map(item, existing);
                    }
                }
                else
                {
                    var docEntity = _mapper.Map<Document>(item);
                    await CreateUpdateFile(docEntity, item.Base64, formId, version);
                    documentsEntity.Add(docEntity);
                }
            }
        }
        private static byte[] CheckBase64AndConvertToArrayByte(string base64Data)
        {
            if (string.IsNullOrEmpty(base64Data))
                throw new ArgumentException("Pdf content is empty");
            
            byte[] fileData;
            
            try
            {
                fileData = Convert.FromBase64String(base64Data);
            }
            catch
            {
                throw new FormatException("Invalid base64 format");
            }
            return fileData;
        }
        private async Task CreateUpdateFile(Document documentEntity, string base64Data, Guid formId, string version)
        {
            var fileData = CheckBase64AndConvertToArrayByte(base64Data);

            var isExist = await _fileStorage.ExistsAsync(documentEntity.Path);
            if (isExist)
            {
                await _fileStorage.DeleteAsync(documentEntity.Path);
            }

            documentEntity.Path = await _fileStorage.SaveAsync(
                fileData,
                $"{formId}_{version}",
                Guid.NewGuid().ToString().ToLower());

        }

        private async Task DeleteFileAsync(Document? documentEntity)
        {
            if(documentEntity != null)
            {
                var isExist = await _fileStorage.ExistsAsync(documentEntity.Path);
                if (isExist)
                {
                    await _fileStorage.DeleteAsync(documentEntity.Path);
                }
            }
        } 

        public IEnumerable<ListFormSubmissionDTO> GetListFormResponses(Guid formId, string? userName, string? companyName, string? email)
        {
            var formSubmissions = _formSubmissionRepository.Find(s => !s.DeletedTime.HasValue 
                                                    && s.FormId == formId
                                                    && (string.IsNullOrEmpty(userName) || s.UserName.ToLower().Contains(userName.ToLower()))
                                                    && (string.IsNullOrEmpty(companyName) || s.CompanyName.ToLower().Contains(companyName.ToLower()))
                                                    && (string.IsNullOrEmpty(email) || s.Email.ToLower().Contains(email.ToLower()))
                                                )
                                               .Include(f => f.Form)
                                               .AsSplitQuery()
                                               .ToList();

            var listFormBlockDTO = _mapper.Map<List<ListFormSubmissionDTO>>(formSubmissions);

            return listFormBlockDTO;
        }

        public async Task<Guid> DuplicateForm(Guid formId)
        {
            if (formId != Guid.Empty)
            {
                var formToDuplicate = _formRepository.Find(f => f.Id == formId && !f.DeletedTime.HasValue)
                    .Include(f => f.Labels)
                    .Include(f => f.Urls)
                    .Include(f => f.Descriptions)
                    .Include(f => f.Nacabels)
                    .Include(f => f.FormDocuments).ThenInclude(d => d.Documents)
                    .Include(f => f.FormNodes).ThenInclude(f => f.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormValueFields)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormValueFields).ThenInclude(v => v.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormConditionals)
                    .FirstOrDefault();
                if (formToDuplicate != null)
                {
                    Form duplicateForm = _mapper.Map<Form>(formToDuplicate);
                    duplicateForm.Status = FormStatus.Draft;
                    _formRepository.Add(duplicateForm);
                    await _unitOfWork.SaveChangesAsync();
                    FixDuplicateFormConditional(formToDuplicate, duplicateForm);
                    return duplicateForm.Id;
                }
                throw new ArgumentException($"Cannot find Form with id: {formId}");
            }
            throw new ArgumentException($"FormId can't be GUID empty");
        }

        public async Task<Guid> DuplicateFormBlock(Guid formBlockId)
        {
            if (formBlockId != Guid.Empty)
            {
                var formToDuplicate = _formRepository.Find(f => f.Id == formBlockId && !f.DeletedTime.HasValue && f.Type == FormType.Block.ToString())
                    .Include(f => f.Labels)
                    .Include(f => f.Urls)
                    .Include(f => f.Descriptions)
                    .Include(f => f.Nacabels)
                    .Include(f => f.FormDocuments).ThenInclude(d => d.Documents)
                    .Include(f => f.FormNodes).ThenInclude(f => f.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormValueFields)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormValueFields).ThenInclude(v => v.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormConditionals)
                    .FirstOrDefault();
                if (formToDuplicate != null)
                {
                    Form duplicateForm = _mapper.Map<Form>(formToDuplicate);
                    duplicateForm.Status = FormStatus.Online;
                    var queryForm = _formRepository.Find(f => f.Type == FormType.Block.ToString());
                    var newEksternalId = queryForm.Any() ? queryForm.Max(f => f.ExternalId) + 1 : 1;
                    duplicateForm.ExternalId = newEksternalId;
                    duplicateForm.Version = 0;
                    duplicateForm.Type = FormType.Block.ToString();
                    foreach (var label in duplicateForm.Labels)
                    {
                        label.Text = $"{label.Text}_{newEksternalId}";
                    }

                    _formRepository.Add(duplicateForm);
                    await _unitOfWork.SaveChangesAsync();
                    FixDuplicateFormConditional(formToDuplicate, duplicateForm);
                    return duplicateForm.Id;
                }
                throw new ArgumentException($"Cannot find Form Block with id: {formBlockId}");
            }
            throw new ArgumentException($"FormBlockId can't be GUID empty");
        }

        private async void FixDuplicateFormConditional(Form src, Form des)
        {
            foreach (var node in des.FormNodes)
            {
                foreach (var formNodeField in node.FormNodeFields)
                {
                    foreach (var conditional in formNodeField.FormConditionals)
                    {
                        var srcNode = src.FormNodes.First(n => n.Id == conditional.FormNodeId);
                        if (srcNode != null)
                        {
                            var destNode = des.FormNodes.First(n => n.Order == srcNode.Order && n.Type == srcNode.Type && n.FieldType == srcNode.FieldType);
                            if (destNode != null)
                            {
                                conditional.FormNodeId = destNode.Id;
                                _formConditionalsRepository.Update(conditional);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

        public OnlineVersionCheckDTO IsOfflineFormHaveOnlineVersion(int offlineFormExternalId)
        {
            if (offlineFormExternalId > 0)
            {
                var onlineVersionForm = _formRepository.Find(f =>
                       f.ExternalId == offlineFormExternalId
                    && f.Status == FormStatus.Online
                    && (f.Type == FormType.Webform.ToString() || f.Type == FormType.Pdf.ToString())).FirstOrDefault();
                if (onlineVersionForm != null)
                    return new OnlineVersionCheckDTO() { IsHaveOnlineVersionForm = true, OnlineFormId = onlineVersionForm.Id };
                return new OnlineVersionCheckDTO() { IsHaveOnlineVersionForm = false, OnlineFormId = Guid.Empty };
            }
            throw new ArgumentException($"FormId can't be GUID empty");
        }
        
        public async Task DeleteFormNodeAsync(Guid formNodeId)
        {
            var formNodeEntity = _formNodesRepository.Find(n => n.Id == formNodeId)
                                    .Include(n => n.FormNodeFields)
                                    .Include(n => n.FormNodeFields).ThenInclude(f => f.FormValueFields)
                                    .Include(n => n.FormNodeFields).ThenInclude(f => f.FormConditionals)
                                    .AsSplitQuery()
                                    .AsTracking()
                                    .FirstOrDefault();

            if (formNodeEntity != null)
            {
                _formNodesRepository.Delete(formNodeEntity);
                var savedEntries = await _unitOfWork.SaveChangesAsync();
                if (savedEntries <= 0)
                {
                    _logger.LogError("The deletion of the Form Field has failed for an unknown reason formFieldId {formFieldId}", formNodeId);
                    throw new Exception("no change has been commited, Form Field not written");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Delete form field tried on unexisting form field id {formNodeId}");
            }
        }

        public async Task DeleteFormNodeSectionAsync(Guid formId, Guid formNodeId)
        {
            var formSectionEntity = _formNodesRepository.Find(n => n.FormId == formId && n.Type == FormNodeType.Section.ToString() && !n.DeletedTime.HasValue)
                                    .Include(n => n.FormNodeFields)
                                    .Include(n => n.FormNodeFields).ThenInclude(f => f.FormValueFields)
                                    .Include(n => n.FormNodeFields).ThenInclude(f => f.FormConditionals)
                                    .AsSplitQuery()
                                    .AsTracking()
                                    .ToList();

            if (formSectionEntity.Any())
            {
                if(formSectionEntity.Count <= 1) throw new Exception("You cannot delete a section when it is the only section of a form.");

                var formNodeEntity = formSectionEntity.Where(x => x.Id == formNodeId).FirstOrDefault() ?? throw new KeyNotFoundException($"Delete form field tried on unexisting form field id {formNodeId}");
                _formNodesRepository.Delete(formNodeEntity);
                var savedEntries = await _unitOfWork.SaveChangesAsync();
                if (savedEntries <= 0)
                {
                    _logger.LogError("The deletion of the Form Field has failed for an unknown reason formFieldId {formFieldId}", formNodeId);
                    throw new Exception("no change has been commited, Form Field not written");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Delete form field tried on unexisting form id {formId}");
            }
        }

        public async Task<Guid> PublisForm(Guid formId, PublishFormDTO param)
        {
            Form? formvalidation = _formRepository
                .Find(f => !f.DeletedTime.HasValue && f.Id == formId).Include(x => x.FormNodes)
                .FirstOrDefault() ?? throw new ArgumentException($"Cannot find Form with id: {formId}");
            
            if (formvalidation.Type != FormType.Pdf.ToString() && formvalidation?.FormNodes.Any() == false) 
                throw new ArgumentException("This form is empty. Please make sure to add at least one form field or form block to the form.");

            // TODO : check same externalId with status online
            var existingForms = _formRepository.Find(f => !f.DeletedTime.HasValue && f.ExternalId == param.ExternalId && f.Status == FormStatus.Online).FirstOrDefault();

            // TODO : if current form status is offline then
            if (param.Status == FormStatus.Offline.ToString())
            {
                // TODO : if exist throw error
                if (existingForms != null) throw new ArgumentException("There is already an updated version of this form online, please use this version if you want to change the form.");
                // TODO : else publish ( change status to online )
                ProcessPublishForm(formId);
            }
            // TODO : if current form status is Draft then
            else if (param.Status == FormStatus.Draft.ToString())
            {
                // TODO : if exist update to offline
                if (existingForms != null)
                {
                    existingForms.Status = FormStatus.Offline;
                    existingForms.LatestUpdateTime = DateTime.Now;
                    _formRepository.Update(existingForms);
                }
                // TODO : publish ( change status to online )
                ProcessPublishForm(formId);
            }
            var savedEntries = await _unitOfWork.SaveChangesAsync();
            if (savedEntries <= 0)
            {
                _logger.LogError("The publish of form has failed for an unknown reason");
                throw new Exception("no change has been commited, publish fail");
            }
            return formId;
        }

        private void ProcessPublishForm(Guid formId)
        {
            var formOfline = _formRepository.Find(f => !f.DeletedTime.HasValue && f.Id == formId).FirstOrDefault();
            if (formOfline != null)
            {
                formOfline.Status = FormStatus.Online;
                formOfline.LatestUpdateTime = DateTime.Now;
                _formRepository.Update(formOfline);
            }
        }
        public async Task<int> UnpublisForm(Guid formId, PublishFormDTO param)
        {
            if (param.Status == FormStatus.Offline.ToString())
            {
                var forms = _formRepository.Find(f => !f.DeletedTime.HasValue && f.ExternalId == param.ExternalId).ToList();
                if (forms.Any())
                {
                    var onlineForm = forms.Where(f => f.Status == FormStatus.Online).FirstOrDefault();
                    if (onlineForm != null) throw new Exception($"This form has been updated, please click ${onlineForm.Id} to go to the most recent version.");
                }
                else
                {
                    throw new KeyNotFoundException($"This form is not available anymore, please use our wizard to find the form you need");
                }
            }
            var form = _formRepository.Find(f => !f.DeletedTime.HasValue && f.Id == formId).FirstOrDefault() ?? throw new KeyNotFoundException($"Form with ID {formId} not found");
            form.IsActive = false;
            form.Status = FormStatus.Offline;
            _formRepository.Update(form);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("The unpublish form has failed for an unknown reason");
                throw new Exception("no change has been commited, Unpublish form not affected");
            }
            return result;
        }

        private async Task UploadFormDocumentAsync(CreateFormDTO dto, int formId)
        {
            var documentDtos = dto.FormDocuments?.SelectMany(x => x.Documents).ToList();

            if (documentDtos != null && documentDtos.Any())
            {
                foreach (var documentDto in documentDtos)
                {
                    byte[] fileData;

					if (string.IsNullOrEmpty(documentDto.Base64))
                    {
						throw new ArgumentException("Document content is empty");
					}

                    try
                    {
                        fileData = Convert.FromBase64String(documentDto.Base64);
                    }
                    catch
                    {
						throw new FormatException("Invalid base64 format");
					}

					documentDto.Path = await this._fileStorage. SaveAsync(
					    fileData,
						$"{formId}_{dto.Version}",
						Guid.NewGuid().ToString().ToLower());
                }
			}
        }

        public async Task<Guid> SubmitFormRequestAsync(CreateFormRequestDTO createFormRequestDTO)
        {
            if (createFormRequestDTO != null)
            {
                var createFormSubmissionEntity = _mapper.Map<FormSubmission>(createFormRequestDTO);
                if (createFormRequestDTO.FormSubmissionType == FormSubmissionType.Webform)
                {
                    createFormSubmissionEntity.Value = _aESEncryptService.EncryptString(createFormRequestDTO.Value);
                }
                else
                {
                    createFormSubmissionEntity.Value = await _fileStorage.SaveAsync(Convert.FromBase64String(createFormRequestDTO.Value), createFormRequestDTO.FormId.ToString(), Guid.NewGuid().ToString());
                }
                createFormSubmissionEntity.SubmissionDate = DateTime.Now;
                _formSubmissionRepository.Add(createFormSubmissionEntity);
                var savedEntries = await _unitOfWork.SaveChangesAsync();
                if (savedEntries <= 0)
                {
                    _logger.LogError("The insert of the Form Submission has failed for an unknown reason");
                    throw new Exception("no change has been commited, Form submisson not written");
                }
                return createFormSubmissionEntity.Id;
            }
            throw new ArgumentException("CreateFormRequestDTO cannot be null");
        }

        public List<FormResponseDetailDto> GetDetailFormResponse(Guid formSubmissionId)
        {
            var formSubmission = _formSubmissionRepository.Find(s => s.Id == formSubmissionId && !s.DeletedTime.HasValue).FirstOrDefault();
            if (formSubmission != null)
            {
                if (formSubmission.FormSubmissionType == FormSubmissionType.Pdf) throw new ArgumentException($"Form submission with id {formSubmissionId} is a PDF type");
                var submissionDataJson = _aESEncryptService.DecryptString(formSubmission.Value);
                var submissionData = JsonConvert.DeserializeObject<SubmitResponseDto>(submissionDataJson);
                if(submissionData != null)
                {
                    return ProcessGetDetailFormResponse(submissionData, formSubmission.FormId);
                }
            }

            throw new KeyNotFoundException($"Cannot Get Form Submission with id {formSubmissionId}");
        }

        private List<FormResponseDetailDto> ProcessGetDetailFormResponse(SubmitResponseDto submissionData, Guid formId, Guid? parentId = null)
        {
            var result = new List<FormResponseDetailDto>();
            var form = _formRepository.Find(f => f.Id == formId && !f.DeletedTime.HasValue)
                .Include(f => f.Labels)
                .Include(f => f.FormNodes.Where(n => !n.DeletedTime.HasValue))
                .Include(f => f.FormNodes).ThenInclude(f => f.Labels)
                .Include(f => f.FormNodes).ThenInclude(f => f.Children)
                .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields.Where(d => !d.DeletedTime.HasValue))
                .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.Labels)
                .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormValueFields.Where(v => !v.DeletedTime.HasValue))
                .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormValueFields).ThenInclude(v => v.Labels)
                .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormConditionals.Where(c => !c.DeletedTime.HasValue))
                .FirstOrDefault();
            if (form != null && submissionData != null)
            {
                var formNodesIteration = !parentId.HasValue ? form.FormNodes.Where(n => !n.ParentId.HasValue).OrderBy(n => n.Order)
                    : form.FormNodes.Where(n => n.ParentId == parentId).OrderBy(n => n.Order);
                foreach (var node in formNodesIteration)
                {
                    ProcessDetailResponse(submissionData, node, result,  formId);
                }
            }
            return result;
        }

        private void ProcessDetailResponse(SubmitResponseDto submissionData, FormNodes? node, List<FormResponseDetailDto> result, Guid formId)
        {
            if (node?.Type == FormNodeType.FormField.ToString())
            {
                var responseData = submissionData.Fields.FirstOrDefault(d => d.NodeId == node.Id);
                if (responseData != null)
                {
                    var formResponseDetailDTO = new FormResponseDetailDto()
                    {
                        FormNodeId = node.Id,
                        Type = node.Type,
                        FieldType = node.FieldType,
                        Labels = _mapper.Map<List<TranslationDTO>>(node.Labels),
                        Value = ProcessResponseDataValue(responseData.Value, node.FieldType??"", node)
                    };
                    result.Add(formResponseDetailDTO);
                }
            }
            else if (node?.Type == FormNodeType.FormBlock.ToString())
            {
                var formNodeField = node.FormNodeFields.FirstOrDefault(f => f.Property == "Value");
                if (formNodeField != null)
                {
                    var formResponseDetailDTO = new FormResponseDetailDto()
                    {
                        FormNodeId = node.Id,
                        Type = node.Type,
                        FieldType = node.FieldType,
                        Labels = _mapper.Map<List<TranslationDTO>>(node.Labels),
                        Children = ProcessGetDetailFormResponse(submissionData, Guid.Parse(formNodeField.Value))
                    };
                    result.Add(formResponseDetailDTO);
                }

            }
            else if (node?.Type == FormNodeType.Section.ToString())
            {
                var formResponseDetailDTO = new FormResponseDetailDto()
                {
                    FormNodeId = node.Id,
                    Type = node.Type,
                    FieldType = node.FieldType,
                    Labels = _mapper.Map<List<TranslationDTO>>(node.Labels),
                    Children = ProcessGetDetailFormResponse(submissionData, formId, node.Id)
                };
                result.Add(formResponseDetailDTO);
            }
        }

        private string ProcessResponseDataValue(string value, string fieldType, FormNodes node)
        {
            if (fieldType != FormNodeFieldType.List.ToString() && fieldType != FormNodeFieldType.CheckBox.ToString())
            {
                return value;
            }

            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var isCustomList = node.FormNodeFields.Any(f => f.Type == FormNodeFieldEncodeType.CustomList.ToString() && f.Property == "List");
            var formNodeField = node.FormNodeFields.FirstOrDefault(f => f.Type == (isCustomList ? FormNodeFieldEncodeType.CustomList.ToString() : FormNodeFieldEncodeType.PredefineList.ToString()) && f.Property == "List");
            var dataSource = formNodeField?.FormValueFields?.ToList() ?? new List<FormValueFields>();

            if (!isCustomList)
            {
                var defaultDataSource = _formNodeFieldsRepository.Find(f => f.Type == FormNodeFieldEncodeType.CustomList.ToString() && f.Property == "List" && f.Value == (formNodeField != null ? formNodeField.Value:""))
                        .Include(n => n.FormValueFields.Where(v => !v.DeletedTime.HasValue))
                        .Include(n => n.FormValueFields).ThenInclude(v => v.Labels)
                        .FirstOrDefault();

                dataSource = defaultDataSource?.FormValueFields?.ToList() ?? new List<FormValueFields>();
            }
            var listCheckBox = GetRelatedCheckboxItem(value, dataSource);
            return JsonConvert.SerializeObject(listCheckBox, options);
        }

        private List<CheckBoxResponseViewDto> GetRelatedCheckboxItem(string value, List<FormValueFields> formValueFields)
        {
            var listCheckBox = new List<CheckBoxResponseViewDto>();
            var selectedList = value.Split(",");
            if (selectedList != null)
            {   
                foreach (var item in selectedList)
                {
                    var fValueFields = formValueFields.FirstOrDefault(v => v.Value == item);
                    if (fValueFields != null)
                    {
                        var checkboxitem = new CheckBoxResponseViewDto()
                        {
                            Id = fValueFields.Id,
                            Value = fValueFields.Value,
                            Labels = _mapper.Map<List<TranslationDTO>>(fValueFields.Labels)
                        };
                        listCheckBox.Add(checkboxitem);
                    }
                }
            }
            return listCheckBox;
        }
        
        public async Task<PagingResultDTO<ListViewFormsDTO>> ViewAllForms(int pageNumber, int pageSize, string languageCode, string? searchLabel, string? cbeNumber, bool includeHomeTag  = false)
        {
            var nacabels = new List<int>();

            if (includeHomeTag)
            {
				PopulateHomeTagId(nacabels);
			}

			nacabels.AddRange(await GetCompanyTag(cbeNumber ?? string.Empty, languageCode));

			return GetAllFormViews(pageNumber, pageSize, languageCode, searchLabel ?? string.Empty, nacabels);
		}

		public async Task<PagingResultDTO<ListViewFormsDTO>> ViewFormsWithTag(int pageNumber, int pageSize, string userExternalId, string languageCode, string? searchLabel, string? cbeNumber, bool includeHomeTag = false)
		{
			var nacabels = new List<int>();

			if (includeHomeTag)
			{
				PopulateHomeTagId(nacabels);
			}

            nacabels.AddRange(await GetUserAssociatedNacabelTag(userExternalId, cbeNumber ?? string.Empty, languageCode));

            return GetAllFormViews(pageNumber, pageSize, languageCode, searchLabel ?? string.Empty, nacabels, true);
		}

		public PreviewFormDTO PreviewAsync(Guid FormId)
        {
            var res = new PreviewFormDTO();
            var form = _formRepository.Find(f => f.Id == FormId && !f.DeletedTime.HasValue)
                    .Include(f => f.Labels)
                    .Include(f => f.Urls)
                    .Include(f => f.Descriptions)
                    .Include(f => f.Nacabels).ThenInclude(n => n.NacabelTranslation)
                    .Include(f => f.FormDocuments).ThenInclude(d => d.Documents)
                    .Include(f => f.FormNodes).ThenInclude(f => f.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormValueFields)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormValueFields).ThenInclude(v => v.Labels)
                    .Include(f => f.FormNodes).ThenInclude(f => f.FormNodeFields).ThenInclude(n => n.FormConditionals)
                    .FirstOrDefault() ?? throw new KeyNotFoundException($"The form is not found.");
            var nodes = _mapper.Map<List<NodesDTO>>(form.FormNodes);
            res.Details = _mapper.Map<PreviewDetailsDTO>(form);
            res.Nodes.AddRange(nodes);
            res.FormId = form.Id;
            return res;
        }

        public async Task<FileDataDTO> DownloadAsync(Guid formId, string languageCode)
        {
            var form = this._formRepository
                .Find(f => f.Id == formId && !f.DeletedTime.HasValue)
                .Include(f => f.FormDocuments).ThenInclude(d => d.Documents)
                .SingleOrDefault() ?? throw new ArgumentException($"Cannot find Form with id: {formId}");
			
            if (form.Type != FormType.Pdf.ToString())
			{
				throw new ArgumentException($"Form with id: {formId} is not a PDF Form");
			}

            var document = form.FormDocuments
                .Where(x => x.LanguageCode == languageCode)
                .SelectMany(s => s.Documents)
                .FirstOrDefault();

			if (document == null && form.FormDocuments.Count == 1)
            {
				document = form.FormDocuments
                    .First()
                    .Documents
                    .FirstOrDefault();
			}

			return await this.GetDocument(document);
		}

		public async Task<FileDataDTO> DownloadDocumentAsync(Guid documentId)
		{
            var document = await _documentRepository.Find(x => x.Id == documentId).SingleOrDefaultAsync();

			return await this.GetDocument(document);
		}

		private async Task<IEnumerable<int>> GetUserAssociatedNacabelTag(string userExternalId, string cbeNumber, string languageCode)
        {
            var nacabelIds = new List<int>();

            var userPersona = await _userPersonasRepository
                .Find(x => x.User!.ExternalId == userExternalId)
                .FirstOrDefaultAsync();

            if (userPersona != null)
            {
                var userPersonaNacebelIds = await _userPersonaCategoriesRepository
                    .Find(x => x.UserPersonaId == userPersona.Id)
                    .Select(x => x.PersonaCategory!.NacabelId ?? 0)
                    .ToListAsync();

                if (userPersonaNacebelIds.Any())
                {
                    nacabelIds.AddRange(userPersonaNacebelIds);
                }
            }

            nacabelIds.AddRange(string.IsNullOrEmpty(cbeNumber) ? 
                await GetCompanyPersonaTag(userExternalId)
                : await GetCompanyTag(cbeNumber, languageCode));

            return nacabelIds;
		}

        private async Task<IEnumerable<int>> GetCompanyPersonaTag(string userExternalId)
        {
			var nacabelIds = new List<int>();

			var user = _userRepository
                .Find(x => x.ExternalId == userExternalId)
                .FirstOrDefault();

            if (user != null)
            {
                var organizationId = user.LatestEstablishment ?? Guid.Empty;

                if (organizationId == Guid.Empty)
                {
                    organizationId = user.LatestOrganisation ?? Guid.Empty;
				}

                if (organizationId != Guid.Empty)
                {
					var companyPersona = await _companyPersonasRepository
						.Find(x => x.OrganisationFancId == organizationId)
						.FirstOrDefaultAsync();

					if (companyPersona != null)
					{
						var companyPersonaNacebelIds = await _companyPersonaCategoriesRepository
							.Find(x => x.CompanyPersonaId == companyPersona.Id)
							.Select(x => x.PersonaCategory!.NacabelId ?? 0)
							.ToListAsync();

						if (companyPersonaNacebelIds.Any())
						{
							nacabelIds.AddRange(companyPersonaNacebelIds);
						}
					}
				}
            }

            return nacabelIds;
		}

        private async Task<IEnumerable<int>> GetCompanyTag(string cbeNumber, string languageCode)
        {
			if (!string.IsNullOrEmpty(cbeNumber))
			{
				var organisation = await _fancRADApi.GetOrganisationEnterpriseReference(new GetOrganisationEnterpriseRequest()
				{
					LanguageCode = languageCode,
					Reference = cbeNumber,
					IncludeMissingCbeBusinessUnits = true
				});

				if (organisation.Nacebel2008Codes.Any())
				{
					var nacabelForSelectedOrganisation = _nacabelHelper.GetMappedSectors(organisation.Nacebel2008Codes, languageCode, organisation.EnterpriseCBENumber);

					if (nacabelForSelectedOrganisation.Any())
						return nacabelForSelectedOrganisation.Select(x => x.Id).ToList();
				}
			}

            return Enumerable.Empty<int>();
		}

        private void PopulateHomeTagId(ICollection<int> nacabelIds)
        {
			var nacabelCodeForHomeTag = _nacabelTranslation.Find(t => t.LanguageCode == Core.Constants.LangauageCodeEN && t.Description == Core.Constants.HomeTagEn)
					.Include(t => t.Nacabel)
					.FirstOrDefault();

			if (nacabelCodeForHomeTag != null)
			{
				nacabelIds.Add(nacabelCodeForHomeTag.Nacabel.Id);
			}
		}

        private bool IsFormBlockLinkedToNonDraftForm(Guid formBlockId)
		{
            var form = _formNodeFieldsRepository.Find(x => x.Value == formBlockId.ToString() 
            && (x.FormNodes.Form.Status == FormStatus.Online || x.FormNodes.Form.Status == FormStatus.Offline));

            return form.Any();
		}

		private PagingResultDTO<ListViewFormsDTO> GetAllFormViews(int pageNumber, int pageSize, 
            string languageCode, string searchLabel, IEnumerable<int> nacabels, bool searchOnTag = false)
        {
			int itemsToSkip = (pageNumber - 1) * pageSize;
            var forms = _formRepository.Find(f => !f.DeletedTime.HasValue
                                                && (f.Type == "Webform" || f.Type == "Pdf")
                                                && f.Status == FormStatus.Online
                                                && (string.IsNullOrEmpty(searchLabel) || f.Labels.Any(l => l.Text.Contains(searchLabel))));

			if (searchOnTag)
			{
                forms = forms.Where(f => f.Nacabels.Any(n => nacabels.Contains(n.Id)));
			}

			var formSelect = forms.Include(f => f.Labels)
                .Include(f => f.Descriptions)
                .Include(f => f.Urls)
                .Include(f => f.Nacabels).ThenInclude(n => n.NacabelTranslation.Where(t => t.LanguageCode == languageCode))
				.Select(p => new
				{
					Parent = p,
					LabelSelected = p.Labels.First(l => l.LanguageCode == languageCode).Text,
					ChildCount = p.Nacabels.Any(n => nacabels.Contains(n.Id)) ? 1 : 0
				});

			var totalRecords = formSelect.Count();

			formSelect = formSelect.OrderByDescending(f => f.ChildCount).ThenBy(f => f.LabelSelected).Skip(itemsToSkip).Take(pageSize);

			var listViewFormsDTO = _mapper.Map<List<ListViewFormsDTO>>(formSelect.Select(f => f.Parent).ToList());

			return new PagingResultDTO<ListViewFormsDTO>()
			{
				Records = listViewFormsDTO,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalRecords = totalRecords,
				TotalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)pageSize)
			};
		}

		private async Task<FileDataDTO> GetDocument(Document? document)
        {
			if (document == null
				|| string.IsNullOrEmpty(document.Path)
				|| !(await this._fileStorage.ExistsAsync(document.Path)))
			{
				throw new ArgumentException($"Document does not exists");
			}

			var mimeProvider = new FileExtensionContentTypeProvider();
			var extensionData = mimeProvider.Mappings
				.FirstOrDefault(x => x.Value == document.Type);

			var filename = document.Name;

			if (!string.IsNullOrEmpty(extensionData.Key) && !filename.EndsWith(extensionData.Key))
			{
				filename += extensionData.Key;
			}

			var fileDto = new FileDataDTO
			{
				Content = await this._fileStorage.GetAsync(document.Path),
				MimeType = document.Type,
				Name = filename
			};

			return fileDto;
		}
	}
}

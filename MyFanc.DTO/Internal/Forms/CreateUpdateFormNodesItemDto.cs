using MyFanc.Core;
using MyFanc.DTO.Internal.Translation;
using Swashbuckle.AspNetCore.Annotations;

namespace MyFanc.DTO.Internal.Forms
{
    public class CreateUpdateFormNodesItemDto
    {
        [SwaggerSchema("For add a new FormNode to the form please fill Id with 'null', and for update FormNode fill Id with Id of edited FormNode")]
        public Guid? Id { get; set; }
        public bool IsActive { get; set; } = true;
        public IEnumerable<TranslationDTO> Labels { get; set; } = Enumerable.Empty<TranslationDTO>();

        [SwaggerSchema("Please fill with version of current Form version")]
        public int Version { get; set; } = 1;

        [SwaggerSchema("Fill with Id of a Form on which FormNode is added")]
        public Guid FormId { get; set; }

        [SwaggerSchema("WARNING!: Only filled when added FormNode to the SECTION, for others fill it with 'null'. Fill parentId with the id of section on which the formnode will added.")]
        public Guid? ParentId { get; set; }

        [SwaggerSchema("Available possible value [FormField, FormBlock, Section], note: case sensitif (use pascal case), please fill ex. Correct: 'FormField' , Incorrect: 'formfield'")]
        public string Type { get; set; } = Enums.FormNodeType.FormField.ToString();

        [SwaggerSchema("For FormNodeType = 'FormBlock' or 'Section' fill the FieldType with string empty, for type = 'FormField', available possible value are [Text, CheckBox, List, UploadFile, Paragraph, HiddenValue], note: case sensitif (use pascal case), please fill ex. Correct: 'CheckBox' , Incorrect: 'checkBox'")]
        public string? FieldType { get; set; } = Enums.FormNodeFieldType.Text.ToString();

        [SwaggerSchema("Fill with the relative position of FormNode from it parent (form/form block/section)")]
        public int Order { get; set; }
        public List<CreateUpdateFormNodeFieldDto> FormNodeFields { get; set; } = new List<CreateUpdateFormNodeFieldDto>();
    }
}

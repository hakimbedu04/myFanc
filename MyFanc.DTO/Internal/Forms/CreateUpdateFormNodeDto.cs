using MyFanc.Core;
using MyFanc.DTO.Internal.Translation;
using Swashbuckle.AspNetCore.Annotations;
using static MyFanc.Core.Enums;

namespace MyFanc.DTO.Internal.Forms
{
    public class CreateUpdateFormContentDto
    {
        [SwaggerSchema("Fill with array of form nodes")]
        public List<CreateUpdateFormNodesDto> FormNodes { get; set; } = new List<CreateUpdateFormNodesDto>();
    }

    public class CreateUpdateFormNodesDto: CreateUpdateFormNodeDto
    {
        [SwaggerSchema("Fill with array of form nodes fields")]
        public List<CreateUpdateFormNodeFieldDto> FormNodeFields { get; set; } = new List<CreateUpdateFormNodeFieldDto>();
    }

    public class CreateUpdateFormNodeDto
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
        public List<CreateUpdateFormNodesItemDto> FormNodes { get; set; } = new List<CreateUpdateFormNodesItemDto>();
    }


    public class CreateUpdateFormNodeFieldDto
    {
        [SwaggerSchema("For add a new FormNodeField to the form please fill Id with 'null', and for update FormNodeField fill Id with Id of edited FormNodeField")]
        public Guid? Id { get; set; }

        [SwaggerSchema("Fill with Id of a FormNode on which FormNodeField is added")]
        public Guid? FormNodeId { get; set; }

        [SwaggerSchema("Fill with property name that want to save, property name here is free text (no rule), what you fill here that is what you will get when get content of form on related property of related formnode. Example porperty name: 'Mandatory' , 'Hide by default, etc'")]
        public string Property { get; set; } = String.Empty;

        [SwaggerSchema("Fill with FormNodeFieldEncodeType, possible values: [Text, Translation, ArrayText, File, CustomList, PredefineList, Conditional, Container], note: case sensitif (use pascal case)!, please fill ex. Correct: 'PredefineList' , Incorrect: 'predefineList'")]
        public string Type { get; set; } = FormNodeFieldEncodeType.Text.ToString();

        [SwaggerSchema("Fill with property value, the value here can be the direct value if the encode type is 'Text', for others encode type (Translation, ArrayText, File, CustomList, PredefineList, Conditional, Container) the value is must in JSON Stringify format, For more detail about the posible value field, please see the example in MyFanc Postman Collection")]
        public string Value { get; set; } = String.Empty;
    }

    public class FileDTO
    {
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
        public string MimeType { get; set;} = string.Empty;
    }
    
    public class CustomListDTO
    {
        public Guid? Id { get; set; }
        public string Value { get; set; }
        public IEnumerable<TranslationDTO> Translations { get; set; } = Enumerable.Empty<TranslationDTO>();
        public int Order { get; set; }
    }

    public class FormConditionalDTO
    {
        public Guid? Id { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public Guid FormNodeId { get; set; }
    }
}

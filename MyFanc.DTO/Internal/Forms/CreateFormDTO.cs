using MyFanc.DTO.Internal.Translation;
using Swashbuckle.AspNetCore.Annotations;

namespace MyFanc.DTO.Internal.Forms
{
    [SwaggerSchema(Required = new[] { "Labels", "Urls", "FormCategoryId" })]
    public class CreateFormDTO
    {
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<TranslationDTO> Urls { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<TranslationDTO> Descriptions { get; set; } = new HashSet<TranslationDTO>();

        [SwaggerSchema("Fill with form category id, to get list of categories use endpoint: /Form/FormCategories")]
        public int FormCategoryId { get; set; }

        [SwaggerSchema("Fill with tags id, to get list of tags/sectors use endpoint: /Data/Sectors/{LanguageCode}")]
        public IEnumerable<int> Tags { get; set; } = new HashSet<int>();
        
        [SwaggerSchema("Form version, for new form please fill with '1'")]
        public int Version { get; set; }
        public bool IsActive { get; set; }

        [SwaggerSchema("true if Pdf form, and false if Webform")]
        public bool IsFormPdf { get; set; } = false;

        [SwaggerSchema("Fill only if IsFormPdf = true, otherwise just fill with array empty")]
        public IEnumerable<CreateFormDocumentDTO>? FormDocuments { get; set; } = new HashSet<CreateFormDocumentDTO>();
    }

    public class CreateFormDocumentDTO
    {
        public string LanguageCode { get; set; } = string.Empty;
        public IEnumerable<CreateDocumentDTO> Documents { get; set; } = new HashSet<CreateDocumentDTO>();

    }

    public class FormDocumentDTO
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public IEnumerable<DocumentDTO> Documents { get; set; } = new HashSet<DocumentDTO>();

    }

    public class CreateDocumentDTO
    {
        [SwaggerSchema("Fill with mime type, ex: application/pdf")]
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [SwaggerSchema("Fill with file path if any")]
        public string Path { get; set; } = string.Empty;
		public string Base64 { get; set; } = string.Empty;
    }

    public class DocumentDTO
    {
        public Guid Id { get; set; }
        public Guid FormDocumentId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}

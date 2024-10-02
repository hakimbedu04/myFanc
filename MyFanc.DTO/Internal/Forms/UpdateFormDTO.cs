using MyFanc.DTO.Internal.Translation;
using Swashbuckle.AspNetCore.Annotations;

namespace MyFanc.DTO.Internal.Forms
{
    public class UpdateFormDTO
    {
        [SwaggerSchema(Required = new[] { "Labels", "Urls", "FormCategoryId" })]
        public Guid Id { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<TranslationDTO> Urls { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<TranslationDTO> Descriptions { get; set; } = new HashSet<TranslationDTO>();

        [SwaggerSchema("Fill with form category id, to get list of categories use endpoint: /Form/FormCategories")]
        public int FormCategoryId { get; set; }

        [SwaggerSchema("Fill with tags id, to get list of tags/sectors use endpoint: /Data/Sectors/{LanguageCode}")]
        public IEnumerable<int> Tags { get; set; } = new HashSet<int>();
        public int Version { get; set; }
        public bool IsActive { get; set; }

        [SwaggerSchema("true if Pdf form, and false if Webform")]
        public bool IsFormPdf { get; set; } = false;
        public IEnumerable<UpdateFormDocumentDTO>? FormDocuments { get; set; } = new HashSet<UpdateFormDocumentDTO>();
    }
    public class UpdateFormDocumentDTO
    {
        public Guid Id { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public IEnumerable<UpdateDocumentDTO> Documents { get; set; } = new HashSet<UpdateDocumentDTO>();

    }

    public class UpdateDocumentDTO
    {
        public Guid Id { get; set; }
        
        [SwaggerSchema("Fill with mime type, ex: application/pdf")]
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
    }
}

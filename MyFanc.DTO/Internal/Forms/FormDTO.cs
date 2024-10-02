using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Forms
{
    public class FormDTO : FormBlockDTO
    {
        public int? FormCategoryId { get; set; }
        public IEnumerable<TranslationDTO> Descriptions { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<TranslationDTO> Urls { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<int> Tags { get; set; } = new HashSet<int>();
        public IEnumerable<FormDocumentDTO> FormDocuments { get; set; } = new HashSet<FormDocumentDTO>();
    }

    public class NacabelDTO
    {
        public int Id { get; set; }
        public string NacabelCode { get; set; } = string.Empty;
    }
}

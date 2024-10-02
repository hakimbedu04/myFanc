using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Forms
{
    public class ListFormDTO
    {
        public Guid OriginalId { get; set; }
        public int Id { get; set; }
        public int Version { get; set; }
        public string Category { get; set; } = string.Empty;
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<string> Tags { get; set; } = new HashSet<string>();
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public IEnumerable<TranslationDTO> Urls { get; set; } = new HashSet<TranslationDTO>();
    }
}

using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Forms
{
    public class ListViewFormsDTO
    {
        public Guid OriginalId { get; set; }
        public int Id { get; set; }
        public IEnumerable<string> Tags { get; set; } = new HashSet<string>();
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<TranslationDTO> Descriptions { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<TranslationDTO> Urls { get; set; } = new HashSet<TranslationDTO>();
    }
}

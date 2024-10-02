using MyFanc.DTO.Internal.Translation;
namespace MyFanc.DTO.Internal.Forms
{
    public class ListFormBlockDTO
    {
        public Guid OriginalId { get; set; }
        public int Id { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public bool IsUsed { get; set; }
        public bool IsEmptyForm { get; set; }
        public bool IsEditContentAllowed { get; set; }
    }
}

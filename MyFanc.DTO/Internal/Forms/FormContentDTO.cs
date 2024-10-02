using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Forms
{
    public class FormContentDTO
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public List<FormNodesDTO> FormNodes { get; set; } = new List<FormNodesDTO>();
    }

    public class FormNodesDTO
    {
        public Guid Id { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public string Type { get; set; }
        public string FieldType { get; set; }
        public int Order { get; set; }
        public int Version { get; set; }
        public bool IsEditContentAllowed { get; set; } = true;
        public List<FormNodeFieldDTO> FormNodeFields { get; set; }
        public List<FormNodesDTO> FormNodes { get; set; }
    }

    public class FormNodeFieldDTO
    {
        public Guid Id { get; set; }
        public string Property { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}

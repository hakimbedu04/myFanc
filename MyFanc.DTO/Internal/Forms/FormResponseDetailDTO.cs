using MyFanc.DTO.Internal.Translation;

namespace MyFanc.DTO.Internal.Forms
{
    public class FormResponseDetailDto
    {
        public Guid FormNodeId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? FieldType { get; set; } = string.Empty;
        public IEnumerable<TranslationDTO> Labels { get; set; } = new List<TranslationDTO>();
        public string Value { get; set; } = string.Empty;
        public List<FormResponseDetailDto> Children { get; set; } = new List<FormResponseDetailDto>();
    }

    public class SubmitResponseDto
    {
        public FormInformationDto? FormInformation { get; set; }
        public UserInformationDto? UserInformation { get; set; }
        public List<FieldInformationDto> Fields { get; set; } = new List<FieldInformationDto>();
    }

    public class FieldInformationDto
    {
        public Guid NodeId { get; set; }
        public string Value { get; set;} = string.Empty;
        private IEnumerable<TranslationDTO> _labels = new List<TranslationDTO>();
        public IEnumerable<TranslationDTO> Labels
        {
            get { return _labels; }
        }
        public string ExternalId { get; set; } = string.Empty;
        public string Id { get; set;} = string.Empty;
    }

    public class FormInformationDto
    {
        public Guid Formid { get; set; }
        public string Version { get; set; } = string.Empty;
        private IEnumerable<TranslationDTO> _labels = new List<TranslationDTO>();
        public IEnumerable<TranslationDTO> Labels
        {
            get { return _labels; }
        }
        public DateTime SubmissionDate { get; set; }
    }

    public class UserInformationDto
    {
        public string Userid { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;   
        public string Email { get; set;} = string.Empty;
        public string FancOrganisationId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}

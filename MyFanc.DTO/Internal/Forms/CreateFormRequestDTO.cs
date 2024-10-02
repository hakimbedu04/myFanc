using Swashbuckle.AspNetCore.Annotations;
using static MyFanc.Core.Enums;

namespace MyFanc.DTO.Internal.Forms
{
    public class CreateFormRequestDTO
    {
        [SwaggerSchema("Fill with formId that submitted")]
        public Guid FormId { get; set; }

        [SwaggerSchema("Fill with form version")]
        public int Version { get; set; }

        [SwaggerSchema("Fill with JSON Stringify of form value if FormSubmissionType = 'Webform', fill with base64 if FormSubmissionType = 'Pdf', For more detail about the posible value, please see the example in MYFANC.API Postman Collection")]
        public string Value { get; set; } = string.Empty;
        [SwaggerSchema("Fill with user FancOrganisationId")]
        public string FancOrganisationId { get; set; } = string.Empty;
        [SwaggerSchema("Fill with user CompanyName")]
        public string CompanyName { get; set; } = string.Empty;

        [SwaggerSchema("Fill with company type, possible values: [LE, OE], note: case sensitif")]
        public string CompanyType { get; set; } = string.Empty;
        [SwaggerSchema("Fill with user Email")]
        public string Email { get; set; } = string.Empty;
        [SwaggerSchema("Fill with user UserId")]
        public string UserId { get; set; } = string.Empty;
        [SwaggerSchema("Fill with user FullName")]
        public string UserFullName { get; set; } = string.Empty;

        [SwaggerSchema("Fill with form submission type Ex: 1, possible values: [0 = Webform, 1 = Pdf]")]
        public FormSubmissionType FormSubmissionType { get; set; }
    }
}

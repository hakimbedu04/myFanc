namespace MyFanc.DTO.Internal.OperationalEntity
{
    public class SendInvitationDTO
    {
        public string EmailTo { get; set; } = string.Empty;
        public IEnumerable<OeAndRoleDTO> ListOe { get; set; } = new List<OeAndRoleDTO>();
    }

    public class OeAndRoleDTO
    {
        public string Role { get; set; } = string.Empty;
        public string FancOrganisationId { get; set; } = string.Empty;
    }
}

namespace MyFanc.DTO.Internal.OperationalEntity
{
    public class OeUserDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public IEnumerable<OrganisationRoleDTO> OrganisationalRoles { get; set; } = Enumerable.Empty<OrganisationRoleDTO>();

    }
}

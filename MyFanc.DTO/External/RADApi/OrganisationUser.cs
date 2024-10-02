﻿namespace MyFanc.DTO.External.RADApi
{
    public class OrganisationUser
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<OrganisationRole> OrganisationalRoles = Enumerable.Empty<OrganisationRole>();
    }
}

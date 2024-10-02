namespace MyFanc.DTO.External.RADApi
{
    public  class UpdateOrganisationBusinessUnitInfoRequest: UpdateOrganisationEnterpriseInfoRequest
    {
        public bool DeclaredToCBE { get; set; }
        public string ReasonForRegistrationBeforeCBEFinalization { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; 
        public string DisplayName { get; set; } = string.Empty;
        public UserInfoAddress? MainAddress { get; set; } = new UserInfoAddress();
        public string BusinessUnitCBENumber { get; set; } = string.Empty;
    }
}

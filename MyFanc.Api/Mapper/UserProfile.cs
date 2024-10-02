using AutoMapper;
using MyFanc.BusinessObjects;
using MyFanc.Core;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.OperationalEntity;
using MyFanc.DTO.Internal.Persona;
using MyFanc.DTO.Internal.Users;

namespace MyFanc.Api.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<GetUserInfoResult, User>()
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestConnection, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsCSAMUser, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.NrNumber)))
                .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestSynchronization, opt => opt.MapFrom(src => src.LastAuthenticSourceRefreshDate))
                .ForMember(dest => dest.LatestOrganisation, opt => opt.Ignore())
                .ForMember(dest => dest.LatestEstablishment, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.UserPersonas, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.LatestConnection = DateTime.Now;
                });

            CreateMap<ProfileInfoAddressDTO, UserInfoAddress>().ReverseMap();

            CreateMap<UpdateProfileDTO, UpdateUserInfoRequest>()
                .ForMember(dest => dest.StructuredAddress, opt => opt.MapFrom(src=>src.Address))
				.ForMember(dest => dest.ForeignIdentityOrPassportNumber, opt => opt.MapFrom(src => src.ForeignIdentityOrPassportNumber ?? string.Empty))
				.ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.PrivacyDeclatationAccepted, opt => opt.Ignore())
                .ForMember(dest => dest.UserIsValidated, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.StructuredAddress))
                .ForMember(dest => dest.Id, opt=>opt.Ignore());

            CreateMap<UpdateUserInfoRequest, GetUserInfoResult>()
                .ForMember(dest => dest.NrNumber, opt => opt.Ignore())
                .ForMember(dest => dest.UnstructuredAddress, opt => opt.Ignore())
                .ForMember(dest => dest.LastAuthenticSourceRefreshDate, opt => opt.Ignore())
                .ForMember(dest => dest.ManualUpdateAllowed, opt => opt.Ignore())
                .ForMember(dest => dest.AllowedLanguageCodes, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.PrivacyDeclatationAccepted, opt => opt.Ignore())
                .ForMember(dest => dest.UserIsValidated, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<GetUserInfoResult,ProfileDTO>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ValidationStatus, opt => opt.MapFrom(src=>src.UserIsValidated? Enums.ValidationStatus.Approved:Enums.ValidationStatus.Pending))
                .ReverseMap();

            CreateMap<GetUserInfoResult, UserIdentityDTO>()
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".TrimStart().TrimEnd()))
               .ForMember(dest => dest.IsValidated, opt => opt.MapFrom(src => src.UserIsValidated))
               .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
               .ForMember(dest => dest.CurrentOrganisation, opt => opt.Ignore())
               .ForMember(dest => dest.CurrentEstablishment, opt => opt.Ignore())
               .ForMember(dest => dest.UserOrganisations, opt => opt.Ignore())
               .ForMember(dest => dest.GlobalRoles, opt => opt.Ignore());

            CreateMap<UserInfoAddress, ProfileInfoAddressDTO>()
                .ReverseMap();

            CreateMap<UserInfoAddressUnstructured, ProfileInfoAddressUnstructuredDTO>()
                .ReverseMap();

            CreateMap<OrganisationLinkEstablishment, OperationalEntityDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.FancOrganisationId) ? src.FancOrganisationId: src.BusinessUnitCBENumber))
                .ForMember(dest => dest.IntracommunityNumber, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.BusinessUnitCBENumber) ? src.BusinessUnitCBENumber : "-"))
                .ForMember(dest => dest.DataOrigin, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.BusinessUnitCBENumber) ? Enums.OeDataOrigin.CBE : Enums.OeDataOrigin.Manual))
                .ForMember(dest => dest.Activated, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.FancNumber) ? true : false));
            
            CreateMap<OrganisationRole, OrganisationRoleDTO>();
            CreateMap<OrganisationUser, OeUserDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.LastName) ? src.FirstName + " " + src.LastName: src.FirstName));

            CreateMap<OrganisationLink, UserOrganisationsDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.FancOrganisationId) ? new Guid(src.FancOrganisationId): Guid.Empty))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.Establishment, opt => opt.MapFrom(src => src.Establishments));

            CreateMap<OrganisationLinkEstablishment, EstablishmentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FancOrganisationId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles));

            CreateMap<GetOrganisationEnterpriseInfoResult, UserOrganisationsDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Guid(src.FancOrganisationId)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Establishment, opt => opt.MapFrom(src => src.Establishments))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<OrganisationEstablishment, EstablishmentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Guid(src.FancOrganisationId)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<CreateOeDTO, UpdateOrganisationBusinessUnitInfoRequest>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .AfterMap((src, dest) => MapAddress(src, dest));

            CreateMap<ActivateOeDTO, UpdateOrganisationBusinessUnitInfoRequest>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .AfterMap((src, dest) => MapAddress(src, dest));

            CreateMap<User, SelectorUserDTO>()
                .ForMember(dest => dest.OrganisationId, opt => opt.MapFrom(src => src.LatestOrganisation))
                .ForMember(dest => dest.EstablishmentId, opt => opt.MapFrom(src => src.LatestEstablishment))
                .ReverseMap();
            CreateMap<UserPersonaDTO, UserPersonas>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PersonaCategories, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore());

            CreateMap<UserPersonas, UserPersonaDTO>()
                .ForMember(dest => dest.UserPersonaId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User != null ? src.User.ExternalId : string.Empty))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.PersonaCategories));

            CreateMap<CompanyPersonas, CompanyPersonaDTO>()
                .ForMember(dest => dest.OrganisationId, opt => opt.MapFrom(src => src.OrganisationFancId))
                .ForMember(dest => dest.NacebelCode, opt => opt.MapFrom(src => src.NacabelCode))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.PersonaCategories));


            CreateMap<AddUserPersonaDTO, UserPersonas>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.PersonaCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore());

            CreateMap<AddUserPersonaCategoriesDTO, UserPersonaCategories>()
               .ForMember(dest => dest.PersonaCategory, opt => opt.Ignore())
               .ForMember(dest => dest.UserPersonas, opt => opt.Ignore());

            CreateMap<AddCompanyPersonaDTO, CompanyPersonas>()
                .ForMember(dest => dest.PersonaCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorUserId, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterUserId, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateTime, opt => opt.Ignore())
                .ForMember(dest => dest.LatestUpdateUserId, opt => opt.Ignore());

            CreateMap<AddCompanyPersonaCategoriesDTO, CompanyPersonaCategories>()
                .ForMember(dest => dest.CompanyPersonaId, opt => opt.MapFrom(src => src.CompanyPersonaId))
                .ForMember(dest => dest.PersonaCategoryId, opt => opt.MapFrom(src => src.PersonaCategoryId))
                .ForMember(dest => dest.PersonaCategory, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyPersonas, opt => opt.Ignore());
        }
        private UpdateOrganisationBusinessUnitInfoRequest MapAddress(CreateOeDTO src, UpdateOrganisationBusinessUnitInfoRequest dest)
        {
            UserInfoAddress invoiceAddress = new UserInfoAddress();
            invoiceAddress.StreetName = src.MainAddressIsInvoiceAddress ? src.MainAddress.StreetName : src.InvoiceAddress.StreetName;
            invoiceAddress.HouseNumber = src.MainAddressIsInvoiceAddress ? src.MainAddress.HouseNumber : src.InvoiceAddress.HouseNumber;
            invoiceAddress.CityName = src.MainAddressIsInvoiceAddress ? src.MainAddress.CityName : src.InvoiceAddress.CityName;
            invoiceAddress.CountryCode = src.MainAddressIsInvoiceAddress ? src.MainAddress.CountryCode: src.InvoiceAddress.CountryCode;
            invoiceAddress.PostalCode = src.MainAddressIsInvoiceAddress ? src.MainAddress.PostalCode : src.InvoiceAddress.PostalCode;
            dest.InvoiceAddress = invoiceAddress;

            return dest;
        }
    }
}

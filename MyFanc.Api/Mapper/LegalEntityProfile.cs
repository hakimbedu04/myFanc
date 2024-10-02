using AutoMapper;
using MyFanc.BusinessObjects;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.LegalEntity;
using MyFanc.DTO.Internal.OperationalEntity;
using MyFanc.DTO.Internal.Translation;
using MyFanc.DTO.Internal.Users;
using MyFanc.DTO.Internal.Wizards;

namespace MyFanc.Api.Mapper
{
    public class LegalEntityProfile : Profile
    {
        public LegalEntityProfile()
        {
            CreateMap<OrganisationLink, LegalEntityDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.FancOrganisationId) ? src.EnterpriseCBENumber : src.FancOrganisationId))
                .ForMember(dest => dest.VATNumber, opt => opt.MapFrom(src => src.EnterpriseCBENumber))
                .ForMember(dest => dest.LegalName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Activated, opt => opt.MapFrom(src => src.Activated));

            CreateMap<OrganisationUser, OrganisationEstablishmentsUserDTO>().ReverseMap();
            CreateMap<OrganisationEstablishment, OrganisationEstablishmentsDTO>()
				.ForMember(dest => dest.MainAddressIsInvoiceAddress, opt => opt.Ignore());
			CreateMap<GetOrganisationEnterpriseInfoResult, OrganisationEnterpriseDTO>()
                .ForMember(dest => dest.MainAddressIsInvoiceAddress, opt => opt.Ignore());

            CreateMap<ActivateLeDTO, UpdateOrganisationEnterpriseInfoRequest>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Sectors));
        }
    }
}

using AutoMapper;
using MyFanc.BusinessObjects;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Persona;

namespace MyFanc.Api.Mapper
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<CountryDTO, GetCountryInfoResult>()
                .ReverseMap();

            CreateMap<OrganisationDTO, GetOrganisation>()
                .ReverseMap();

            CreateMap<OrganisationUsersDTO, GetOrganisationUsers>()
                .ReverseMap();

            CreateMap<Nacabel, SectorDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NacabelCode, opt => opt.MapFrom(src => src.NacabelCode))
                .ForMember(dest => dest.Sector, opt => opt.MapFrom(src => src.NacabelTranslation.First() != null ? src.NacabelTranslation.First().Description : ""));

            CreateMap<Nacabel, NacabelsCodeDTO>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.NacabelCode))
                .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.NacabelTranslation.Any() ? src.NacabelTranslation.First().Description : ""))
                .ReverseMap();

            CreateMap<NacabelsEntityMap, NacabelsCodeDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.NacabelId))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Nacabel.NacabelCode))
                .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Nacabel.NacabelTranslation.Any() ? src.Nacabel.NacabelTranslation.First().Description : ""))
                .ReverseMap();

            CreateMap<CityNameInfoDTO, CityNameInfo>()
                .ReverseMap();

            CreateMap<CityDTO, GetCityInfoResult>()
                .ReverseMap();

            CreateMap<LegalFormDTO, GetLegalFormInfoResult>()
                .ReverseMap();

            CreateMap<GetCityInfoResult, OrganisationDefaultLanguangeDTO>();

            CreateMap<PersonaCategories, CategoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Labels.Any() ? src.Labels.First().Text : string.Empty))
                .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Nacebel != null ? src.Nacebel.NacabelTranslation.First().Id : 0))
                .ForMember(dest => dest.TagTitle, opt => opt.MapFrom(src => src.Nacebel != null ? src.Nacebel.NacabelTranslation.First().Description : string.Empty))
                .ForMember(dest => dest.SubItems, opt => opt.MapFrom(src => src.Children))
                .ForMember(dest => dest.SelectedEnable, opt => opt.MapFrom(src => src.Children.Count == 0));

            CreateMap<Nacabel, CategoryDTO>()
                .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TagTitle, opt => opt.MapFrom(src => src.NacabelCode))
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.SelectedEnable, opt => opt.Ignore())
                .ForMember(dest => dest.SubItems, opt => opt.Ignore());
        }       
    }
}

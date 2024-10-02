using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.Services.FancRadApi;
using MyFanc.Services;
using NSubstitute;
using MyFanc.Api.Mapper;
using MyFanc.DTO.External.RADApi;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MyFanc.DTO.Internal.Data;
using System;
using MyFanc.Contracts.BLL;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_Organisation_Test
    {
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IMapper mapper;
        private ILogger<Bll> logger;
        private IFancRADApi fancRADApi;
        private ISharedDataCache sharedDataCache;
        private IBreadCrumbService breadCrumbService;
        private IAESEncryptService aESEncryptService;
        private ITokenConfiguration tokenConfiguration;
        private IEmailService emailService;
        private INacabelHelper nacabelHelper;
        private IFileStorage fileStorage;
        private IIdentityProviderConfiguration identityProviderConfiguration;

        [TestInitialize()]
        public void Initialize()
        {
            this.unitOfWork = Substitute.For<IUnitOfWork>();
            this.sharedDataCache = Substitute.For<ISharedDataCache>();
            this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();

            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new DataProfile());
                cfg.AddProfile(new UserProfile());
            });

            this.mapper = new Mapper(mapperConfig);
            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();
            this.breadCrumbService = new BreadCrumbService(mapper);
            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod()]
        public async Task StoreUserOrganisation()
        {
            string reference = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            GetOrganisationEnterpriseInfoResult getOrganisationEnterpriseInfoResultNew = new GetOrganisationEnterpriseInfoResult()
            {
                FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                FancNumber = "FancNumber Update",
                EnterpriseCBENumber = "EnterpriseCBENumber Update",
                Name = "Organitation 1 Update",
                Activated = true,
                Email = "emailupdate@email.com",
                Phone = "666666666",
                MainAddress = new UserInfoAddress() { 
                    CityName = "CityName Update",
                    StreetName = "StreetName Update",
                    HouseNumber = "HouseNumber Update",
                    PostalCode = "PostalCode Update",
                    CountryCode = "CountryCode Update"
                },
                InvoiceAddress = new UserInfoAddress() {
                    CityName = "Invoice CityName Update",
                    StreetName = "Invoice StreetName Update",
                    HouseNumber = "Invoice HouseNumber Update",
                    PostalCode = "Invoice PostalCode Update",
                    CountryCode = "Invoice CountryCode Update"
                },
                Nacebel2008Codes = new List<string>() { "Nacebel2008Codes Update" },
                Establishments = new List<OrganisationEstablishment>()
                {
                    new OrganisationEstablishment()
                    {
                        FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66aaaa",
                        FancNumber = "FancNumber OrganisationEstablishment Update",
                        Name = "OrganisationEstablishment Update"
                    }
                }
            };

            GetOrganisationEnterpriseInfoResult getOrganisationEnterpriseInfoResultOld = new GetOrganisationEnterpriseInfoResult()
            {
                FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                FancNumber = "FancNumber Old",
                EnterpriseCBENumber = "EnterpriseCBENumber Old",
                Name = "Organitation 1 Old",
                Activated = true,
                Email = "email@email.com",
                Phone = "123456789",
                MainAddress = new UserInfoAddress()
                {
                    CityName = "CityName Old",
                    StreetName = "StreetName Old",
                    HouseNumber = "HouseNumber Old",
                    PostalCode = "PostalCode Old",
                    CountryCode = "CountryCode Old"
                },
                InvoiceAddress = new UserInfoAddress()
                {
                    CityName = "Invoice CityName Old",
                    StreetName = "Invoice StreetName Old",
                    HouseNumber = "Invoice HouseNumber Old",
                    PostalCode = "Invoice PostalCode Old",
                    CountryCode = "Invoice CountryCode Old"
                },
                Nacebel2008Codes = new List<string>() { "Nacebel2008Codes Old" },
                Establishments = new List<OrganisationEstablishment>()
                {
                    new OrganisationEstablishment()
                    {
                        FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66aaaa",
                        FancNumber = "FancNumber OrganisationEstablishment Old",
                        Name = "OrganisationEstablishment Old"
                    }
                }
            };

            OrganisationLink organisationLink = new OrganisationLink()
            {
                FancOrganisationId = reference,
                FancNumber = "FancNumber",
                EnterpriseCBENumber = "EnterpriseCBENumber",
                Name = "Organitation 1 Update",
                Establishments = new List<OrganisationLinkEstablishment>()
                        {
                            new OrganisationLinkEstablishment()
                            {
                                Name = "OrganisationEstablishment Update",
                                FancNumber = "FancNumber"
                            }
                        }

            };

            fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(new GetOrganisationLinkInfoResult()
            {
                OrganisationLinks = new List<OrganisationLink>()
                {
                    organisationLink
                }
            });
            fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(getOrganisationEnterpriseInfoResultNew);
            sharedDataCache.GetData<List<GetOrganisationEnterpriseInfoResult>>("Organisations").Returns(new List<GetOrganisationEnterpriseInfoResult>()
            {
                getOrganisationEnterpriseInfoResultOld
            });
            sharedDataCache.GetData<List<OrganisationUsersDTO>>(Arg.Is<string>("OrganisationUsers")).Returns(new List<OrganisationUsersDTO>()
            {
                new OrganisationUsersDTO()
                {
                    OrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    EstabishmentId = "FancNumber",
                    UserId = "25"
                }
            });


            await bll.StoreUserOrganisation("25");
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
            await fancRADApi.Received(1).GetOrganisationEnterpriseReference(Arg.Is<GetOrganisationEnterpriseRequest>(x =>
                   x.Reference == reference
            ));
            sharedDataCache.Received(1).GetData<List<GetOrganisationEnterpriseInfoResult>>("Organisations");
            sharedDataCache.Received(1).SetData("Organisations", Arg.Is<List<GetOrganisationEnterpriseInfoResult>>(x => 
                (x[0].Name == "Organitation 1 Update")
                && (x[0].MainAddress.CityName == "CityName Update")
                && (x[0].MainAddress.StreetName == "StreetName Update")
                && (x[0].MainAddress.HouseNumber == "HouseNumber Update")
                && (x[0].MainAddress.PostalCode == "PostalCode Update")
                && (x[0].MainAddress.CountryCode == "CountryCode Update")
            ));
            sharedDataCache.Received(1).GetData<List<OrganisationUsersDTO>>("OrganisationUsers");
            sharedDataCache.Received(1).RemoveData<List<OrganisationUsersDTO>>("User-25-Organisations");
            sharedDataCache.Received(1).SetData("OrganisationUsers", Arg.Is<List<OrganisationUsersDTO>>(x => x.Count() == 0));
            sharedDataCache.Received(1).SetData("User-25-Organisations", Arg.Is<List<OrganisationUsersDTO>>(x =>
                x[0].OrganisationId == "3fa85f64-5717-4562-b3fc-2c963f66afa6"
                && x[0].EstabishmentId == "3fa85f64-5717-4562-b3fc-2c963f66aaaa"
                && x[0].UserId == "25"
            ));
        }

        [TestMethod()]
        public async Task StoreUserOrganisationStoreOrganisationReturnError()
        {
            string reference = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            OrganisationLink organisationLink = new OrganisationLink()
            {
                FancOrganisationId = reference,
                FancNumber = "FancNumber",
                EnterpriseCBENumber = "EnterpriseCBENumber",
                Name = "Organitation 1 Update",
                Establishments = new List<OrganisationLinkEstablishment>()
                        {
                            new OrganisationLinkEstablishment()
                            {
                                Name = "OrganisationEstablishment Update",
                                FancNumber = "FancNumber"
                            }
                        }

            };

            fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(new GetOrganisationLinkInfoResult()
            {
                OrganisationLinks = new List<OrganisationLink>()
                {
                    organisationLink
                }
            });
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await bll.StoreUserOrganisation("25");
            });
            await fancRADApi.Received(1).GetUserOrganisationLinks(Arg.Is<GetUserOrganisationLinksRequest>(x => x.UserId == "25"));
        }

        [TestMethod()]
        public async Task StoreUserOrganisationCacheContainNoData()
        {
            string reference = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            GetOrganisationEnterpriseInfoResult getOrganisationEnterpriseInfoResultNew = new GetOrganisationEnterpriseInfoResult()
            {
                FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                FancNumber = "FancNumber Update",
                EnterpriseCBENumber = "EnterpriseCBENumber Update",
                Name = "Organitation 1 Update",
                Activated = true,
                Email = "emailupdate@email.com",
                Phone = "666666666",
                MainAddress = new UserInfoAddress()
                {
                    CityName = "CityName Update",
                    StreetName = "StreetName Update",
                    HouseNumber = "HouseNumber Update",
                    PostalCode = "PostalCode Update",
                    CountryCode = "CountryCode Update"
                },
                InvoiceAddress = new UserInfoAddress()
                {
                    CityName = "Invoice CityName Update",
                    StreetName = "Invoice StreetName Update",
                    HouseNumber = "Invoice HouseNumber Update",
                    PostalCode = "Invoice PostalCode Update",
                    CountryCode = "Invoice CountryCode Update"
                },
                Nacebel2008Codes = new List<string>() { "Nacebel2008Codes Update" },
                Establishments = new List<OrganisationEstablishment>()
                {
                    new OrganisationEstablishment()
                    {
                        FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66aaaa",
                        FancNumber = "FancNumber OrganisationEstablishment Update",
                        Name = "OrganisationEstablishment Update"
                    }
                }
            };

            OrganisationLink organisationLink = new OrganisationLink()
            {
                FancOrganisationId = reference,
                FancNumber = "FancNumber",
                EnterpriseCBENumber = "EnterpriseCBENumber",
                Name = "Organitation 1 Update",
                Establishments = new List<OrganisationLinkEstablishment>()
                        {
                            new OrganisationLinkEstablishment()
                            {
                                Name = "OrganisationEstablishment Update",
                                FancNumber = "FancNumber"
                            }
                        }

            };

            fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(new GetOrganisationLinkInfoResult()
            {
                OrganisationLinks = new List<OrganisationLink>()
                {
                    organisationLink
                }
            });
            fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(getOrganisationEnterpriseInfoResultNew);
            
            await bll.StoreUserOrganisation("25");
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
            await fancRADApi.Received(1).GetOrganisationEnterpriseReference(Arg.Is<GetOrganisationEnterpriseRequest>(x =>
                   x.Reference == reference
            ));
            sharedDataCache.Received(1).GetData<List<GetOrganisationEnterpriseInfoResult>>("Organisations");
            sharedDataCache.Received(1).SetData("Organisations", Arg.Is<List<GetOrganisationEnterpriseInfoResult>>(x =>
                (x[0].Name == "Organitation 1 Update")
                && (x[0].MainAddress.CityName == "CityName Update")
                && (x[0].MainAddress.StreetName == "StreetName Update")
                && (x[0].MainAddress.HouseNumber == "HouseNumber Update")
                && (x[0].MainAddress.PostalCode == "PostalCode Update")
                && (x[0].MainAddress.CountryCode == "CountryCode Update")
            ));
            sharedDataCache.Received(1).GetData<List<OrganisationUsersDTO>>("OrganisationUsers");
            sharedDataCache.Received(0).RemoveData<List<OrganisationUsersDTO>>("User-25-Organisations");
            sharedDataCache.Received(0).SetData("OrganisationUsers", Arg.Any<List<OrganisationUsersDTO>>());

            sharedDataCache.Received(1).SetData("User-25-Organisations", Arg.Is<List<OrganisationUsersDTO>>(x =>
                x[0].OrganisationId == "3fa85f64-5717-4562-b3fc-2c963f66afa6"
                && x[0].EstabishmentId == "3fa85f64-5717-4562-b3fc-2c963f66aaaa"
                && x[0].UserId == "25"
            ));
        }

    }
}

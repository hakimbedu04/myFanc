using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.Api.Mapper;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.LegalEntity;
using MyFanc.DTO.Internal.Users;
using MyFanc.Services.FancRadApi;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_LegalEntity_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable

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
                cfg.AddProfile(new LegalEntityProfile());
            });

            this.mapper = new Mapper(mapperConfig);

            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();
            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod()]
        public async Task GetLegalEntitiesTest()
        {
            // arrange
            var mockUserOrganitationLinks = GetLegalEntityMockOrigin();
            var mockOrganisationReferences = GetOrganisationEnterpriseInfoResultMock();
            var mockCityByCode = GetCityByCodeMock();

            // act
            var userOrganitationLinks = fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(mockUserOrganitationLinks);
            var organisationReferences = fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(mockOrganisationReferences);
            var cityByCodes = fancRADApi.GetCityByCode(Arg.Any<GetCityRequest>()).ReturnsForAnyArgs(mockCityByCode);

            var result = await bll.GetLegalEntityListAsync(new GetUserOrganisationLinksRequest() { UserId = "2"});

            // assert
            Assert.AreEqual(mockUserOrganitationLinks.OrganisationLinks.SelectMany(x => x.Establishments).ToList().Count, result.ToList().Count);
            Assert.AreEqual(mockUserOrganitationLinks.OrganisationLinks.ElementAt(0).FancOrganisationId, result.ElementAt(0).Id);
            Assert.AreEqual(mockUserOrganitationLinks.OrganisationLinks.ElementAt(0).EnterpriseCBENumber, result.ElementAt(0).VATNumber);
            Assert.AreEqual(mockUserOrganitationLinks.OrganisationLinks.ElementAt(0).Name, result.ElementAt(0).LegalName);
            Assert.AreEqual(mockUserOrganitationLinks.OrganisationLinks.ElementAt(0).Activated, result.ElementAt(0).Activated);
            Assert.AreEqual(mockUserOrganitationLinks.OrganisationLinks.ElementAt(0).DefaultLanguageCode, result.ElementAt(0).DefaultLanguageCode);
            Assert.AreEqual(mockUserOrganitationLinks.OrganisationLinks.ElementAt(0).Nacabels.Count(), result.ElementAt(0).Nacabels.Count());
            Assert.AreEqual(mockUserOrganitationLinks.OrganisationLinks.ElementAt(0).Sectors.Count(), result.ElementAt(0).Sectors.Count());
        }

        private static List<GetCityInfoResult> GetCityByCodeMock()
        {
            var result = new List<GetCityInfoResult>()
            {
                new GetCityInfoResult()
                {
                    NisCode = 25112,
                    PostCode = 1300,
                    OfficialLangCode1 = "fr",
                    DisplayName = "Wavre"
                }
            };
            return result;
        }

        private static GetOrganisationEnterpriseInfoResult GetOrganisationEnterpriseInfoResultMock()
        {
            var result = new GetOrganisationEnterpriseInfoResult()
            {
                FancOrganisationId = "75153fc0-b230-4785-b7f3-df005258729b",
                MainAddress = new UserInfoAddress()
                {
                    PostalCode = "6141"
                },
                Nacebel2008Codes = new List<string>() { "62010", "62090", "62030" }
            };

            return result;
        }

        private static GetOrganisationLinkInfoResult GetLegalEntityMockOrigin()
        {
            var OrganisationLinkList = new List<OrganisationLink>();

            var organisationRole = new OrganisationRole()
            {
                Role = "MyFANC_Manager",
                LinkSource = "Local",
            };
            var organisationLinkEstablishment = new OrganisationLinkEstablishment()
            {
                FancOrganisationId = "75153fc0-b230-4785-b7f3-df005258729b",
                FancNumber = "fancNumber",
                EnterpriseCBENumber = "enterpriseCBENumber",
                BusinessUnitCBENumber = "businessUnitCBENumber",
                Name = "Company Name",
                Roles = new List<OrganisationRole>() { organisationRole }
            };
            var organisationLink = new OrganisationLink()
            {
                FancOrganisationId = "75153fc0-b230-4785-b7f3-df005258729b",
                EnterpriseCBENumber = "0761448614",
                Name = "N.F.DEVELOPMENT",
                Roles = new List<OrganisationRole>() { organisationRole },
                Establishments = new List<OrganisationLinkEstablishment>() { organisationLinkEstablishment },
                DefaultLanguageCode = "fr",
                Nacabels = new List<string>() { "62010", "62090", "62030" },
                Sectors = new List<NacabelsCodeDTO>
                {
                    new NacabelsCodeDTO
                    {
                        Code = "62010",
                        Label = "Nucléaire"
                    },
                    new NacabelsCodeDTO
                    {
                        Code = "62090",
                        Label = "Fabrication"
                    },
                    new NacabelsCodeDTO
                    {
                        Code = "62030",
                        Label = "Industries militaires"
                    },
                    new NacabelsCodeDTO
                    {
                        Code = "62031",
                        Label = "Pharmacie"
                    }
                }
            };
            OrganisationLinkList.Add(organisationLink);

            return new GetOrganisationLinkInfoResult()
            {
                OrganisationLinks = OrganisationLinkList
            };
        }

        [TestMethod()]
        public async Task ListLegalEntityTestParameterInvalid()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.GetLegalEntityListAsync(new GetUserOrganisationLinksRequest() { UserId = "" });
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }

        [TestMethod()]
        public async Task ListLegalEntityTestUserOrganitationLinksNotFound()
        {
            var userOrganitationLinks = new GetOrganisationLinkInfoResult();
            fancRADApi.GetUserOrganisationLinks(default).ReturnsForAnyArgs(userOrganitationLinks);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetLegalEntityListAsync(new GetUserOrganisationLinksRequest() { UserId = "1" });
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }

        [TestMethod()]
        public async Task ListOperationalEntityTestCallApiError()
        {
            fancRADApi.When(x => x.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>())).Do(x => { throw new Exception(); });

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.GetLegalEntityListAsync(new GetUserOrganisationLinksRequest() { UserId = "1" });
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }

        private ActivateLeDTO GetRequestActivateLEMock()
        {
            // TODO : create payload
            var request = new ActivateLeDTO()
            {
                FancOrganisationId = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5",
                EnterpriseCBENumber = "0477390844",
                Email = "",
                Phone = "",
                MainAddressIsInvoiceAddress = true,
                InvoiceAddress = new ProfileInfoAddressDTO()
                {
                    CountryCode = "BE",
                    StreetName = "Rue des Mûriers,Spy(V. 02.01.1977 rue d'Ordin)",
                    HouseNumber = "2 / D000",
                    PostalCode = "5190",
                    CityName = "Jemeppe-sur-Sambre"
                },
                DefaultLanguageCode = "fr",
                Sectors = new List<int>
                {
                    1,2,3
                }
            };
            return request;
        }

        private UpdateOrganisationEnterpriseInfoRequest GetRequestActivateLEMockRad()
        {
            // TODO : create payload
            var request = new UpdateOrganisationEnterpriseInfoRequest()
            {
                FancOrganisationId = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5",
                EnterpriseCBENumber = "0477390844",
                Email = "",
                Phone = "",
                MainAddressIsInvoiceAddress = true,
                InvoiceAddress = new UserInfoAddress()
                {
                    CountryCode = "BE",
                    StreetName = "Rue des Mûriers,Spy(V. 02.01.1977 rue d'Ordin)",
                    HouseNumber = "2 / D000",
                    PostalCode = "5190",
                    CityName = "Jemeppe-sur-Sambre"
                },
                DefaultLanguageCode = "fr",
                Tags = new List<int>
                {
                    1,2,3
                }
            };
            return request;
        }

        [TestMethod()]
        public async Task ActivateLegalEntities()
        {
            var paramActivate = GetRequestActivateLEMock();
            await bll.ActivateLegalEntities(paramActivate);
            await fancRADApi.ReceivedWithAnyArgs(1).UpdateOrganisationEnterprise(GetRequestActivateLEMockRad());
            await fancRADApi.Received(1).UpdateOrganisationEnterprise(Arg.Is<UpdateOrganisationEnterpriseInfoRequest>(x =>
                   x.FancOrganisationId == paramActivate.FancOrganisationId
                && x.InvoiceAddress.StreetName == paramActivate.InvoiceAddress.StreetName
                && x.InvoiceAddress.HouseNumber == paramActivate.InvoiceAddress.HouseNumber
                && x.InvoiceAddress.CityName == paramActivate.InvoiceAddress.CityName
                && x.InvoiceAddress.CountryCode == paramActivate.InvoiceAddress.CountryCode
                && x.InvoiceAddress.PostalCode == paramActivate.InvoiceAddress.PostalCode
                && x.DefaultLanguageCode == paramActivate.DefaultLanguageCode
            ));

            //act
            await nacabelHelper.InsertOrUpdateNacabelSector(paramActivate.Sectors, paramActivate.EnterpriseCBENumber);

        }

        //[TestMethod()]
        //public async Task InsertOrUpdateNacabelSector()
        //{
        //    //var paramActivate = GetRequestActivateLEMock();
        //    ////var mockUserOrganitationLinks = new List<E>
        //    ////var userOrganitationLinks = fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(mockUserOrganitationLinks);
        //    //var existingNacabelMap = new List<NacabelsEntityMap>();
        //    //// arrange
        //    //nacabelsEntityMapRepository.Find(Arg.Any<Expression<Func<NacabelsEntityMap, bool>>>()).Returns(existingNacabelMap.AsQueryable());

        //    //await nacabelHelper.InsertOrUpdateNacabelSector(paramActivate.Tags, paramActivate.DefaultLanguageCode, paramActivate.EnterpriseCBENumber);

        //    //nacabelHelper.GetMappedSectors(paramActivate.DefaultLanguageCode,)


        //    //scenario
        //    // buat data awal tabel nacabelsEntityMapRepository dengan return kosong
        //    var existingNacabelMap = new List<NacabelsEntityMap>();

        //    // buat data awal get LE dengan return sectors kosong, nacabel terisi default
        //    var mockListLE = new List<LegalEntityDTO>
        //    {
        //        new LegalEntityDTO
        //        {
        //            Id = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5",
        //            LegalName = "VOX TENEO",
        //            VATNumber = "0477390844",
        //            Activated = true,
        //            DefaultLanguageCode = "fr",
        //            Nacabels = new List<string> {"62020", "63110"},
        //            Sectors = new List<NacabelsCodeDTO> {}
        //        }
        //    };

        //    //act call LE
        //    //var result = await bll.GetLegalEntityListAsync(new GetUserOrganisationLinksRequest() { UserId = "34" });
        //    var sectors = nacabelHelper.GetMappedSectors();

        //    // activate LE

        //    //buat inputan param tags 

        //    //panggil hasil insert dengan cara get LE berisi data yang baru di input


        //}

        [TestMethod()]
        public async Task ActivateLegalEntitiesTestParameterInvalid()
        {
            var request = GetRequestActivateLEMock();
            request.EnterpriseCBENumber = string.Empty;
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                var result = await bll.ActivateLegalEntities(request);
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().UpdateOrganisationEnterprise(Arg.Any<UpdateOrganisationEnterpriseInfoRequest>());
        }

        [TestMethod()]
        public async Task ActivateLegalEntitiesTestCallApiError()
        {
            var request = GetRequestActivateLEMock();
            fancRADApi.When(x => x.UpdateOrganisationEnterprise(Arg.Any<UpdateOrganisationEnterpriseInfoRequest>())).Do(x => { throw new Exception(); });

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                var result = await bll.ActivateLegalEntities(request);
            });
            await fancRADApi.ReceivedWithAnyArgs(1).UpdateOrganisationEnterprise(Arg.Any<UpdateOrganisationEnterpriseInfoRequest>());
        }
    }
}

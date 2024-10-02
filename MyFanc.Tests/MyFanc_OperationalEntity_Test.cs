using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.Api.Mapper;
using MyFanc.BLL;
using MyFanc.BLL.Utility;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.Core;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.OperationalEntity;
using MyFanc.DTO.Internal.Users;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants = MyFanc.Core.Constants;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_OperationalEntity_Test
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
            this.aESEncryptService = Substitute.For<IAESEncryptService>();
            this.tokenConfiguration = Substitute.For<TokenConfiguration>();
            this.emailService = Substitute.For<IEmailService>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();

            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new LegalEntityProfile());
                cfg.AddProfile(new DataProfile());
            });

            this.mapper = new Mapper(mapperConfig);
            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();
            this.breadCrumbService = new BreadCrumbService(mapper);
            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod]
        public async Task ListOperationalEntityTest()
        {
            var userOrganitationLinks = new GetOrganisationLinkInfoResult()
            {
                OrganisationLinks = new List<OrganisationLink>()
                {
                    new OrganisationLink
                    {
                        FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        FancNumber = "FancNumber",
                        EnterpriseCBENumber = "EnterpriseCBENumber",
                        Name = "Organitation Parent",
                        Roles = new List<OrganisationRole>(){ new OrganisationRole() { Role = "Manager", LinkSource = "LinkSource" }},
                        Establishments = new List<OrganisationLinkEstablishment>(){new OrganisationLinkEstablishment() {
                            FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afff",
                            BusinessUnitCBENumber = "BusinessUnitCBENumber",
                            EnterpriseCBENumber = "EnterpriseCBENumber",
                            FancNumber = "FancNumber",
                            Name = "Organisation Child",
                            Roles = new List<OrganisationRole>(){ new OrganisationRole() { Role = "Manager", LinkSource = "LinkSource" }},
                            DefaultLanguageCode = "fr"
                        }}
                    }
                }
            };
            var mockCityByCode = GetCityByCodeMock();
            var mockOrganisationReferences = GetOrganisationEnterpriseInfoResultMock();

            var userOrganisations = fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(userOrganitationLinks);
            var organisationReferences = fancRADApi.GetOrganisationEnterpriseReference(default).ReturnsForAnyArgs(mockOrganisationReferences);
            var cityByCodes = fancRADApi.GetCityByCode(default).ReturnsForAnyArgs(mockCityByCode);

            var result = await bll.ListOperationalEntityAsync("1");
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
            Assert.AreEqual(userOrganitationLinks.OrganisationLinks.SelectMany(x => x.Establishments).ToList().Count, result.ToList().Count);
            Assert.AreEqual(userOrganitationLinks.OrganisationLinks.ElementAt(0).Establishments.ElementAt(0).FancOrganisationId, result.ElementAt(0).Id);
            Assert.AreEqual(userOrganitationLinks.OrganisationLinks.ElementAt(0).Establishments.ElementAt(0).BusinessUnitCBENumber, result.ElementAt(0).IntracommunityNumber);
            Assert.AreEqual(userOrganitationLinks.OrganisationLinks.ElementAt(0).Establishments.ElementAt(0).Name, result.ElementAt(0).Name);
            Assert.AreEqual(userOrganitationLinks.OrganisationLinks.ElementAt(0).Establishments.ElementAt(0).DefaultLanguageCode, result.ElementAt(0).DefaultLanguageCode);
            Assert.AreEqual((!string.IsNullOrEmpty(userOrganitationLinks.OrganisationLinks.ElementAt(0).Establishments.ElementAt(0).BusinessUnitCBENumber) ? Enums.OeDataOrigin.CBE : Enums.OeDataOrigin.Manual), result.ElementAt(0).DataOrigin);
            Assert.AreEqual((!string.IsNullOrEmpty(userOrganitationLinks.OrganisationLinks.ElementAt(0).Establishments.ElementAt(0).FancNumber) ? true : false), result.ElementAt(0).Activated);
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

        [TestMethod()]
        public async Task ListOperationalEntityTestParameterInvalid()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.ListOperationalEntityAsync("");
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }

        [TestMethod()]
        public async Task ListOperationalEntityTestUserOrganitationLinksNotFound()
        {
            var userOrganitationLinks = new GetOrganisationLinkInfoResult();
            fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(userOrganitationLinks);
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.ListOperationalEntityAsync("1");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }

        [TestMethod()]
        public async Task ListOperationalEntityTestCallApiError()
        {
            fancRADApi.When(x => x.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>())).Do(x => { throw new Exception(); });
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.ListOperationalEntityAsync("1");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }


        [TestMethod]
        public async Task DeleteUserFromOeTest()
        {
            var userOrganitationLinks = GetMockDataForOrganisationLinkInfoResult();
            var userResult = new GetUserInfoResult() { Email = "ahn@voxteneo.com" };
            fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(userOrganitationLinks);
            fancRADApi.GetUser(Arg.Any<GetUserRequest>()).Returns(userResult);
            await bll.DeleteUserFromOeAsync("25", "3fa85f64-5717-4562-b3fc-2c963f66afff", "1");
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
            await fancRADApi.ReceivedWithAnyArgs(1).DeleteUserOrganisationLinks(Arg.Any<UpdateOrganisationLinkInfoRequest>(), Arg.Any<UpdateUserRequest>());
            await fancRADApi.ReceivedWithAnyArgs(1).GetUser(Arg.Any<GetUserRequest>());
            await emailService.Received(1).SendDeleteUserFromOeNotificationAsync(Arg.Is<string>(emailTo => emailTo == "ahn@voxteneo.com"), Arg.Is<string>(fancOrgId => fancOrgId == "3fa85f64-5717-4562-b3fc-2c963f66afff"), Arg.Is<IEnumerable<string>>(roles => roles.Contains("Manager")), Arg.Is<string>(reqUid => reqUid == "1"));
        }

        [TestMethod]
        public async Task DeleteUserFromOeWithoutSendEmailNotificationBacauseUserEmailIsEmptyTest()
        {
            var userOrganitationLinks = GetMockDataForOrganisationLinkInfoResult();
            var userResult = new GetUserInfoResult() { Email = "" };
            fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(userOrganitationLinks);
            fancRADApi.GetUser(Arg.Any<GetUserRequest>()).Returns(userResult);
            await bll.DeleteUserFromOeAsync("25", "3fa85f64-5717-4562-b3fc-2c963f66afff", "1");
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
            await fancRADApi.ReceivedWithAnyArgs(1).DeleteUserOrganisationLinks(Arg.Any<UpdateOrganisationLinkInfoRequest>(), Arg.Any<UpdateUserRequest>());
            await fancRADApi.ReceivedWithAnyArgs(1).GetUser(Arg.Any<GetUserRequest>());
            await emailService.DidNotReceiveWithAnyArgs().SendDeleteUserFromOeNotificationAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>(), Arg.Any<string>());
        }

        private GetOrganisationLinkInfoResult GetMockDataForOrganisationLinkInfoResult()
        {
            return new GetOrganisationLinkInfoResult()
            {
                OrganisationLinks = new List<OrganisationLink>()
                {
                    new OrganisationLink
                    {
                        FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        FancNumber = "FancNumber",
                        EnterpriseCBENumber = "EnterpriseCBENumber",
                        Name = "Organitation Parent",
                        Roles = new List<OrganisationRole>(){ new OrganisationRole() { Role = "Manager", LinkSource = "LinkSource" }},
                        Establishments = new List<OrganisationLinkEstablishment>(){new OrganisationLinkEstablishment() {
                            FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afff",
                            BusinessUnitCBENumber = "BusinessUnitCBENumber",
                            EnterpriseCBENumber = "EnterpriseCBENumber",
                            FancNumber = "FancNumber",
                            Name = "Organisation Child",
                            Roles = new List<OrganisationRole>(){ new OrganisationRole() { Role = "Manager", LinkSource = "LinkSource" }}
                        }}
                    }
                }
            };
        }

        [TestMethod()]
        public async Task DeleteUserFromOeTestErrorCanotFoundUserRoles()
        {
            var userOrganitationLinks = GetMockDataForOrganisationLinkInfoResult();
            fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(userOrganitationLinks);
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.DeleteUserFromOeAsync("25", "3fa85f64-5717-4562-b3fc-ffffffffffff", "1");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
            await fancRADApi.DidNotReceiveWithAnyArgs().DeleteUserOrganisationLinks(Arg.Any<UpdateOrganisationLinkInfoRequest>(), Arg.Any<UpdateUserRequest>());
        }

        [TestMethod()]
        public async Task DeleteUserFromOeTestErrorCanotFoundOrganisations()
        {
            var userOrganitationLinks = new GetOrganisationLinkInfoResult()
            {
                OrganisationLinks = null
            };
            fancRADApi.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>()).ReturnsForAnyArgs(userOrganitationLinks);
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.DeleteUserFromOeAsync("25", "3fa85f64-5717-4562-b3fc-ffffffffffff", "1");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
            await fancRADApi.DidNotReceiveWithAnyArgs().DeleteUserOrganisationLinks(Arg.Any<UpdateOrganisationLinkInfoRequest>(), Arg.Any<UpdateUserRequest>());
        }

        [TestMethod()]
        public async Task DeleteUserFromOeTestErrorParameterInvalid()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.DeleteUserFromOeAsync("", "3fa85f64-5717-4562-b3fc-ffffffffffff", "1");
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
            await fancRADApi.DidNotReceiveWithAnyArgs().DeleteUserOrganisationLinks(Arg.Any<UpdateOrganisationLinkInfoRequest>(), Arg.Any<UpdateUserRequest>());
        }

        [TestMethod]
        public async Task ListUserLinkedToOeTest()
        {
            var getOrganisationEnterpriseInfoResult = new GetOrganisationEnterpriseInfoResult()
            {
                Activated = true,
                Email = "mail@test.com",
                EnterpriseCBENumber = "EnterpriceCBENumber",
                Establishments = new List<OrganisationEstablishment>(){new OrganisationEstablishment() {
                            FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afff",
                            BusinessUnitCBENumber = "BusinessUnitCBENumber",
                            EnterpriseCBENumber = "EnterpriceCBENumber",
                            FancNumber = "FancNumber",
                            Name = "Organisation Child",
                            Activated = true,
                            MainAddress = new UserInfoAddress(){
                                CityName = "MainAddress CityName" ,
                                StreetName = "MainAddress StreetName",
                                CountryCode = "MainAddress CountryCode",
                                HouseNumber = "MainAddress HouseNumber",
                                PostalCode = "MainAddress PostalCode"
                            },
                            InvoiceAddress = new UserInfoAddress()
                            {
                                CityName = "InvoiceAddress CityName" ,
                                StreetName = "InvoiceAddress StreetName",
                                CountryCode = "InvoiceAddress CountryCode",
                                HouseNumber = "InvoiceAddress HouseNumber",
                                PostalCode = "InvoiceAddress PostalCode"
                            },
                            Users = new List<OrganisationUser>()
                            {
                                new OrganisationUser()
                                {
                                    UserId = "1",
                                    Email = "user@mail.com",
                                    FirstName = "",
                                    LastName = "",
                                    OrganisationalRoles = new List<OrganisationRole>()
                                    {
                                        new OrganisationRole()
                                        {
                                            Role = "Manager",
                                            LinkSource = "LinkSource"
                                        }
                                    }
                                }
                            }
                        }}
            };
            var organisationEnterpriseInfoResult = fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(getOrganisationEnterpriseInfoResult);
            var result = await bll.ListUserLinkedToOeAsync("reference", "3fa85f64-5717-4562-b3fc-2c963f66afff", false, "en-US");
            await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
            Assert.AreEqual(getOrganisationEnterpriseInfoResult.Establishments.ElementAt(0).Users.Count(), result.Count());
            Assert.AreEqual(getOrganisationEnterpriseInfoResult.Establishments.ElementAt(0).Users.ElementAt(0).UserId, result.ElementAt(0).UserId);
            Assert.AreEqual(!string.IsNullOrEmpty(getOrganisationEnterpriseInfoResult.Establishments.ElementAt(0).Users.ElementAt(0).LastName) ? getOrganisationEnterpriseInfoResult.Establishments.ElementAt(0).Users.ElementAt(0).FirstName + " " + getOrganisationEnterpriseInfoResult.Establishments.ElementAt(0).Users.ElementAt(0).LastName : getOrganisationEnterpriseInfoResult.Establishments.ElementAt(0).Users.ElementAt(0).FirstName, result.ElementAt(0).FullName);
            Assert.AreEqual(getOrganisationEnterpriseInfoResult.Establishments.ElementAt(0).Users.ElementAt(0).OrganisationalRoles.ElementAt(0).Role, result.ElementAt(0).OrganisationalRoles.ElementAt(0).Role);
        }

        [TestMethod()]
        public async Task ListUserLinkedToOeTestCallApiError()
        {
            fancRADApi.When(x => x.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>())).Do(x => { throw new Exception(); });
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.ListUserLinkedToOeAsync("reference", "3fa85f64-5717-4562-b3fc-2c963f66afff", false, "en-US");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
        }

        [TestMethod()]
        public async Task ListUserLinkedToOeTestParameterInvalid()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.ListUserLinkedToOeAsync("", "", false, "en-US");
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
        }

        [TestMethod()]
        public async Task ListUserLinkedToOeTestOperatinalEntityListNull()
        {
            var getOrganisationEnterpriseInfoResult = new GetOrganisationEnterpriseInfoResult()
            {
                Activated = true,
                Email = "mail@test.com",
                EnterpriseCBENumber = "EnterpriceCBENumber",
                Establishments = null
            };
            var organisationEnterpriseInfoResult = fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(getOrganisationEnterpriseInfoResult);
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.ListUserLinkedToOeAsync("reference", "3fa85f64-5717-4562-b3fc-2c963f66afff", false, "en-US");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
        }

        [TestMethod()]
        public async Task ListUserLinkedToOeTestOperatinalEntityNotFound()
        {
            var getOrganisationEnterpriseInfoResult = new GetOrganisationEnterpriseInfoResult()
            {
                Activated = true,
                Email = "mail@test.com",
                EnterpriseCBENumber = "EnterpriceCBENumber",
                Establishments = new List<OrganisationEstablishment>(){new OrganisationEstablishment() {
                            FancOrganisationId = "3fa85f64-5717-4562-b3fc-ffffffffffff",
                            BusinessUnitCBENumber = "BusinessUnitCBENumber",
                            EnterpriseCBENumber = "EnterpriceCBENumber",
                            FancNumber = "FancNumber",
                            Name = "Organisation Child",
                            Activated = true,
                        }}
            };
            var organisationEnterpriseInfoResult = fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(getOrganisationEnterpriseInfoResult);
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.ListUserLinkedToOeAsync("reference", "3fa85f64-5717-4562-b3fc-2c963f66afff", false, "en-US");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
        }

        [TestMethod()]
        public async Task CreateOeAsyncTest()
        {
            CreateOeDTO createOeDTO = new CreateOeDTO()
            {
                EnterpriseCBENumber = "123456",
                FancOrganisationId = "",
                //temporary comment since need to hide additional fields in creating OE
                //IsBeingValidatedByBceKbo = true,
                //ReasonForDeclaringInMyFancBeforeValidated = "Reason",
                Name = "LegalName",
                DisplayName = "CommercialName",
                MainAddress = new ProfileInfoAddressDTO()
                {
                    StreetName = "MainStreetName",
                    HouseNumber = "MainHouseNumber",
                    PostalCode = "MainPostalCode",
                    CityName = "MainCityName",
                    CountryCode = "MainCountryCode",
                },
                MainAddressIsInvoiceAddress = false,
                InvoiceAddress = new ProfileInfoAddressDTO()
                {
                    StreetName = "InvoiceStreetName",
                    HouseNumber = "InvoiceHouseNumber",
                    PostalCode = "InvoicePostalCode",
                    CityName = "InvoiceCityName",
                    CountryCode = "InvoiceCountryCode",
                },
                Email = "test@test.com",
                Phone = "12345678",
                DefaultLanguageCode = "fr"
            };

            await bll.CreateOeAsync(createOeDTO);

            await fancRADApi.Received(1).UpdateOrganisationBusinessUnit(Arg.Is<UpdateOrganisationBusinessUnitInfoRequest>(x =>
                   x.FancOrganisationId == createOeDTO.FancOrganisationId
                   && x.EnterpriseCBENumber == createOeDTO.EnterpriseCBENumber
                && x.InvoiceAddress.StreetName == createOeDTO.InvoiceAddress.StreetName
                && x.InvoiceAddress.HouseNumber == createOeDTO.InvoiceAddress.HouseNumber
                && x.InvoiceAddress.CityName == createOeDTO.InvoiceAddress.CityName
                && x.InvoiceAddress.CountryCode == createOeDTO.InvoiceAddress.CountryCode
                && x.InvoiceAddress.PostalCode == createOeDTO.InvoiceAddress.PostalCode
                && x.DefaultLanguageCode == createOeDTO.DefaultLanguageCode
            ));
        }

        [TestMethod()]
        public async Task CreateOeMainAddressIsInvoiceAddressTest()
        {
            CreateOeDTO createOeDTO = new CreateOeDTO()
            {
                EnterpriseCBENumber = "123456",
                FancOrganisationId = "",
                //temporary comment since need to hide additional fields in creating OE
                //IsBeingValidatedByBceKbo = true,
                //ReasonForDeclaringInMyFancBeforeValidated = "Reason",
                Name = "LegalName",
                DisplayName = "CommercialName",
                MainAddress = new ProfileInfoAddressDTO()
                {
                    StreetName = "MainStreetName",
                    HouseNumber = "MainHouseNumber",
                    PostalCode = "MainPostalCode",
                    CityName = "MainCityName",
                    CountryCode = "MainCountryCode",
                },
                MainAddressIsInvoiceAddress = true,
                Email = "test@test.com",
                Phone = "12345678"
                //DefaultLanguaage = "en-US"
            };

            await bll.CreateOeAsync(createOeDTO);

            await fancRADApi.Received(1).UpdateOrganisationBusinessUnit(Arg.Is<UpdateOrganisationBusinessUnitInfoRequest>(x =>
                   x.EnterpriseCBENumber == createOeDTO.EnterpriseCBENumber
                && x.FancOrganisationId == createOeDTO.FancOrganisationId
                && x.InvoiceAddress.StreetName == createOeDTO.MainAddress.StreetName
                && x.InvoiceAddress.HouseNumber == createOeDTO.MainAddress.HouseNumber
                && x.InvoiceAddress.CityName == createOeDTO.MainAddress.CityName
                && x.InvoiceAddress.CountryCode == createOeDTO.MainAddress.CountryCode
                && x.InvoiceAddress.PostalCode == createOeDTO.MainAddress.PostalCode
            ));
        }

        [TestMethod()]
        public async Task CreateOeAsyncCreateDtoNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await bll.CreateOeAsync(null);
            });
            await fancRADApi.ReceivedWithAnyArgs(0).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
            await fancRADApi.Received(0).UpdateOrganisationBusinessUnit(Arg.Any<UpdateOrganisationBusinessUnitInfoRequest>());
        }

        [TestMethod()]
        public async Task CreateOeAsyncLeIsNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await bll.CreateOeAsync(new CreateOeDTO() { EnterpriseCBENumber = "" });
            });
            await fancRADApi.Received(0).UpdateOrganisationBusinessUnit(Arg.Any<UpdateOrganisationBusinessUnitInfoRequest>());
        }

        //temporary comment since need to hide additional fields in creating OE

        /* [TestMethod()]
         public async Task CreateOeAsyncIsBeingValidatedByBceKboFalse()
         {
             fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(new GetOrganisationEnterpriseInfoResult()
             {
                 MainAddress = new UserInfoAddress()
                 {
                     CountryCode = "ID"
                 }
             });
             await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
             {
                 await bll.CreateOeAsync(new CreateOeDTO() { IsBeingValidatedByBceKbo = false });
             });
             await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
             await fancRADApi.Received(0).UpdateOrganisationBusinessUnit(Arg.Any<UpdateOrganisationBusinessUnitInfoRequest>());
         }*/

        [TestMethod]
        public async Task SendInvitationMailTest()
        {
            string tokenGenerated = "ItUIKsO9Q1TV/8ykvFS7Fb72to9D5OT9fSjBkDBk5iRqkLJzTtZ2SuMQRR4u5x5yN3HFbQzbX3TSmotgNhuENptgY4qG6kDlIejgermtXVhQJJXX1PodboKEu9v0xHwf";
            string fancOrgId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            string roleManager = "manager";
            string emailTo = "ahn@voxteneo.com";

            aESEncryptService.EncryptString(Arg.Any<string>()).ReturnsForAnyArgs(tokenGenerated);

            await bll.SendInvitationMailAsync(new SendInvitationDTO()
            {
                EmailTo = emailTo,
                ListOe = new List<OeAndRoleDTO> { new OeAndRoleDTO() { FancOrganisationId = fancOrgId, Role = roleManager } }
            });

            aESEncryptService.ReceivedWithAnyArgs(1).EncryptString(Arg.Any<string>());
            await emailService.Received(1).SendInvitationAsync(Arg.Is<string>(toEmail => toEmail == emailTo), Arg.Is<string>(token => token == tokenGenerated), Arg.Is<string>(fid => fid == fancOrgId), Arg.Is<string>(role => role == roleManager));
        }

        [TestMethod]
        public async Task SendInvitationMail_Multiple_User_Test()
        {
            string tokenGenerated = "ItUIKsO9Q1TV/8ykvFS7Fb72to9D5OT9fSjBkDBk5iRqkLJzTtZ2SuMQRR4u5x5yN3HFbQzbX3TSmotgNhuENptgY4qG6kDlIejgermtXVhQJJXX1PodboKEu9v0xHwf";
            string fancOrgId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            string roleManager = "manager";
            string emailTo = "ahn@voxteneo.com";

            aESEncryptService.EncryptString(Arg.Any<string>()).ReturnsForAnyArgs(tokenGenerated);

            await bll.SendInvitationMailAsync(new SendInvitationDTO()
            {
                EmailTo = emailTo,
                ListOe = new List<OeAndRoleDTO> {
                    new OeAndRoleDTO() { FancOrganisationId = fancOrgId, Role = roleManager },
                    new OeAndRoleDTO() { FancOrganisationId = fancOrgId, Role = roleManager }}
            });

            aESEncryptService.ReceivedWithAnyArgs(2).EncryptString(Arg.Any<string>());
            await emailService.Received(2).SendInvitationAsync(Arg.Is<string>(toEmail => toEmail == emailTo), Arg.Is<string>(token => token == tokenGenerated), Arg.Is<string>(fid => fid == fancOrgId), Arg.Is<string>(role => role == roleManager));
        }
        
        [TestMethod()]
        public async Task ActivateOeAsyncTest()
        {
            ActivateOeDTO activateOeDTO = new ActivateOeDTO()
            {
                EnterpriseCBENumber = "123456",
                FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66aaaa",
                //temporary comment since need to hide additional fields in creating OE
                //IsBeingValidatedByBceKbo = true,
                //ReasonForDeclaringInMyFancBeforeValidated = "Reason",
                Name = "LegalName",
                DisplayName = "CommercialName",
                MainAddress = new ProfileInfoAddressDTO()
                {
                    StreetName = "MainStreetName",
                    HouseNumber = "MainHouseNumber",
                    PostalCode = "MainPostalCode",
                    CityName = "MainCityName",
                    CountryCode = "MainCountryCode",
                },
                MainAddressIsInvoiceAddress = false,
                InvoiceAddress = new ProfileInfoAddressDTO()
                {
                    StreetName = "InvoiceStreetName",
                    HouseNumber = "InvoiceHouseNumber",
                    PostalCode = "InvoicePostalCode",
                    CityName = "InvoiceCityName",
                    CountryCode = "InvoiceCountryCode",
                },
                Email = "test@test.com",
                Phone = "12345678",
                DefaultLanguageCode = "fr"
                //DefaultLanguaage = "en-US"
            };

            await bll.ActivateOeAsync(activateOeDTO);

            await fancRADApi.Received(1).UpdateOrganisationBusinessUnit(Arg.Is<UpdateOrganisationBusinessUnitInfoRequest>(x =>
                   x.EnterpriseCBENumber == activateOeDTO.EnterpriseCBENumber
                && x.InvoiceAddress.StreetName == activateOeDTO.InvoiceAddress.StreetName
                && x.InvoiceAddress.HouseNumber == activateOeDTO.InvoiceAddress.HouseNumber
                && x.InvoiceAddress.CityName == activateOeDTO.InvoiceAddress.CityName
                && x.InvoiceAddress.CountryCode == activateOeDTO.InvoiceAddress.CountryCode
                && x.InvoiceAddress.PostalCode == activateOeDTO.InvoiceAddress.PostalCode
                && x.DefaultLanguageCode == activateOeDTO.DefaultLanguageCode
            ));
        }

        [TestMethod()]
        public async Task ActivateOeMainAddressIsInvoiceAddressTest()
        {
            ActivateOeDTO activateOeDTO = new ActivateOeDTO()
            {
                EnterpriseCBENumber = "123456",
                FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66aaaa",
                //temporary comment since need to hide additional fields in creating OE
                //IsBeingValidatedByBceKbo = true,
                //ReasonForDeclaringInMyFancBeforeValidated = "Reason",
                Name = "LegalName",
                DisplayName = "CommercialName",
                MainAddress = new ProfileInfoAddressDTO()
                {
                    StreetName = "MainStreetName",
                    HouseNumber = "MainHouseNumber",
                    PostalCode = "MainPostalCode",
                    CityName = "MainCityName",
                    CountryCode = "MainCountryCode",
                },
                MainAddressIsInvoiceAddress = true,
                Email = "test@test.com",
                Phone = "12345678"
                //DefaultLanguaage = "en-US"
            };

            await bll.ActivateOeAsync(activateOeDTO);

            await fancRADApi.Received(1).UpdateOrganisationBusinessUnit(Arg.Is<UpdateOrganisationBusinessUnitInfoRequest>(x =>
                    x.EnterpriseCBENumber == activateOeDTO.EnterpriseCBENumber
                && x.InvoiceAddress.StreetName == activateOeDTO.MainAddress.StreetName
                && x.InvoiceAddress.HouseNumber == activateOeDTO.MainAddress.HouseNumber
                && x.InvoiceAddress.CityName == activateOeDTO.MainAddress.CityName
                && x.InvoiceAddress.CountryCode == activateOeDTO.MainAddress.CountryCode
                && x.InvoiceAddress.PostalCode == activateOeDTO.MainAddress.PostalCode
            ));
        }

        [TestMethod()]
        public async Task ActivateOeAsyncCreateDtoNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await bll.ActivateOeAsync(null);
            });
            await fancRADApi.Received(0).UpdateOrganisationBusinessUnit(Arg.Any<UpdateOrganisationBusinessUnitInfoRequest>());
        }

        [TestMethod]
        public async Task GetOrganisationEnterpriseDetailAsyncNacabelHelperTest()
        {
            var getOrganisationEnterpriseInfoResult = new GetOrganisationEnterpriseInfoResult()
            {
                FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                Nacebel2008Codes = new List<string>() { "62010", "62090", "62030" },
                Establishments = new List<OrganisationEstablishment>()
            };
            var sectors = new List<NacabelsCodeDTO>() {  
                new NacabelsCodeDTO()
                {
                    Code = "62090",
                    Label = "Fabrication"
                },
                new NacabelsCodeDTO()
                {
                    Code = "62030",
                    Label = "Industries militaires"
                },
                new NacabelsCodeDTO()
                {
                    Code = "62010",
                    Label = "Nucléaire"
                }
            };

            fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(getOrganisationEnterpriseInfoResult);
            nacabelHelper.GetMappedSectors(Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(sectors);
            var result = await bll.GetOrganisationEnterpriseDetailAsync("3fa85f64-5717-4562-b3fc-2c963f66afa6", true, "fr", false);
            await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
            nacabelHelper.ReceivedWithAnyArgs(1).GetMappedSectors(Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<string>());
            
            Assert.AreEqual(getOrganisationEnterpriseInfoResult.FancOrganisationId, result.FancOrganisationId);
            Assert.AreEqual(sectors.Count(), result.Sectors.Count());
            var index = 0;
            foreach(var item in result.Sectors)
            {
                Assert.AreEqual(sectors[index].Code, item.Code);
                Assert.AreEqual(sectors[index].Label, item.Label);
                index++;
            }
            Assert.AreEqual(sectors.ElementAt(1), result.Sectors.ElementAt(1));
            Assert.AreEqual(sectors.ElementAt(2), result.Sectors.ElementAt(2));
        }

		[TestMethod]
		public async Task GetOrganisationEnterpriseDetailAsyncNacabelHelperTest_ForceUpdate()
		{
			var getOrganisationEnterpriseInfoResult = new GetOrganisationEnterpriseInfoResult()
			{
				FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
				Nacebel2008Codes = new List<string>() { "62010", "62090", "62030" },
				Establishments = new List<OrganisationEstablishment>()
			};
			var sectors = new List<NacabelsCodeDTO>() {
				new NacabelsCodeDTO()
				{
					Code = "62090",
					Label = "Fabrication"
				},
				new NacabelsCodeDTO()
				{
					Code = "62030",
					Label = "Industries militaires"
				},
				new NacabelsCodeDTO()
				{
					Code = "62010",
					Label = "Nucléaire"
				}
			};

			fancRADApi.GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>()).ReturnsForAnyArgs(getOrganisationEnterpriseInfoResult);
			nacabelHelper.GetMappedSectors(Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(sectors);
			var result = await bll.GetOrganisationEnterpriseDetailAsync("3fa85f64-5717-4562-b3fc-2c963f66afa6", true, "fr", true);
			await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
			
            nacabelHelper.ReceivedWithAnyArgs(1).GetMappedSectors(Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<string>());
            sharedDataCache.ReceivedWithAnyArgs(1).ClearAllData();

			Assert.AreEqual(getOrganisationEnterpriseInfoResult.FancOrganisationId, result.FancOrganisationId);
			Assert.AreEqual(sectors.Count, result.Sectors.Count());
			var index = 0;
			foreach (var item in result.Sectors)
			{
				Assert.AreEqual(sectors[index].Code, item.Code);
				Assert.AreEqual(sectors[index].Label, item.Label);
				index++;
			}
			Assert.AreEqual(sectors.ElementAt(1), result.Sectors.ElementAt(1));
			Assert.AreEqual(sectors.ElementAt(2), result.Sectors.ElementAt(2));
		}

		[TestMethod()]
        public async Task GetOrganisationEnterpriseDetailAsyncOrganisationNotFound()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetOrganisationEnterpriseDetailAsync("3fa85f64-5717-4562-b3fc-2c963f66afa6", true, "en", false);
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
            nacabelHelper.DidNotReceiveWithAnyArgs().GetDescription(Arg.Any<List<string>>(), Arg.Any<string>());
        }

        [TestMethod()]
        public async Task GetOrganisationEnterpriseDetailAsyncOrganisationNotFoundNacabelHelperNotReceiveACall()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetOrganisationEnterpriseDetailAsync("3fa85f64-5717-4562-b3fc-2c963f66afa6", true, "en", false);
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetOrganisationEnterpriseReference(Arg.Any<GetOrganisationEnterpriseRequest>());
            nacabelHelper.ReceivedWithAnyArgs(0).GetDescription(Arg.Any<List<string>>(), Arg.Any<string>());
        }

        [TestMethod()]
        public async Task GetDefaultLanguageForOrganisationByPostalCodeAsyncTest()
        {
            var citiesInfoResult = new List<GetCityInfoResult>() { new GetCityInfoResult()
            {
                    NisCode =  21004,
                    PostCode = 1000,
                    OfficialLangCode1 = "nl",
                    OfficialLangCode2 = "fr",
                    DisplayName = "Brussel / Bruxelles",
                    Names = new List<CityNameInfo>(){
                        new CityNameInfo()
                    {
                        LangCode = "nl",
                        Value = "Brussel"
                    },
                        new CityNameInfo()
                    {
                        LangCode = "fr",
                        Value = "Bruxelles"
                    }
                }
            } };

            fancRADApi.GetCityByCode(Arg.Any<GetCityRequest>()).ReturnsForAnyArgs(citiesInfoResult);
            var result = await bll.GetDefaultLanguageForOrganisationByPostalCodeAsync("1000");

            await fancRADApi.ReceivedWithAnyArgs(1).GetCityByCode(Arg.Any<GetCityRequest>());
            Assert.AreEqual("nl", result.OfficialLangCode1);
            Assert.AreEqual("fr", result.OfficialLangCode2);
        }

        [TestMethod()]
        public async Task GetDefaultLanguageForOrganisationByPostalCodeAsyncTestInvalidParameter()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await bll.GetDefaultLanguageForOrganisationByPostalCodeAsync("");
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().GetCityByCode(Arg.Any<GetCityRequest>());
        }

        [TestMethod()]
        public async Task GetDefaultLanguageForOrganisationByPostalCodeAsyncTestCitiesNotFound()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetDefaultLanguageForOrganisationByPostalCodeAsync("2000");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetCityByCode(Arg.Any<GetCityRequest>());
        }

        [TestMethod()]
        public async Task GetDefaultLanguageForOrganisationByPostalCodeAsyncTestCityNotFound()
        {
            var citiesInfoResult = new List<GetCityInfoResult>();
            fancRADApi.GetCityByCode(Arg.Any<GetCityRequest>()).ReturnsForAnyArgs(citiesInfoResult);
            
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetDefaultLanguageForOrganisationByPostalCodeAsync("2000");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetCityByCode(Arg.Any<GetCityRequest>());
        }
    }
}

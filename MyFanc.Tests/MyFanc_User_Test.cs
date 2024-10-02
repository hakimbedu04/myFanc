using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.Api.Mapper;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Users;
using MyFanc.DTO.Internal.Wizards;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static MyFanc.Core.Enums;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_User_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IGenericRepository<User> userRepository;
        private IGenericRepository<Roles> rolesRepository;
        private IGenericRepository<Wizard> wizardRepository;
        private IGenericRepository<Question> questionRepository;
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		[TestInitialize()]
        public void Initialize()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            userRepository = Substitute.For<IGenericRepository<User>>();
            rolesRepository = Substitute.For<IGenericRepository<Roles>>();
            wizardRepository = Substitute.For<IGenericRepository<Wizard>>();
            questionRepository = Substitute.For<IGenericRepository<Question>>();
            emailService = Substitute.For<IEmailService>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            sharedDataCache = Substitute.For<ISharedDataCache>();
            this.fileStorage = Substitute.For<IFileStorage>();
            identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();

            logger = Substitute.For<ILogger<Bll>>();
            fancRADApi = Substitute.For<IFancRADApi>();

            unitOfWork.GetGenericRepository<User>().Returns(userRepository);
            unitOfWork.GetGenericRepository<Roles>().Returns(rolesRepository);
            unitOfWork.GetGenericRepository<Wizard>().Returns(wizardRepository);
            unitOfWork.GetGenericRepository<Question>().Returns(questionRepository);

            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new WizardProfile());
                cfg.AddProfile(new DataProfile());
            });

            mapper = new Mapper(mapperConfig);

            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            breadCrumbService = new BreadCrumbService(mapper);

            bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetUserInfoAsyncTestNotFound()
        {
            await bll.GetUserInfoAsync(new DTO.External.RADApi.GetUserRequest { UserId = "2" });
        }


        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdateUserInfoAsyncTestNotFound()
        {
            await bll.UpdateUserAsync("2", new DTO.Internal.Users.UpdateProfileDTO
            {
                BirthDate = new DateTime(2000, 1, 2),
                BirthPlace = "A Place Update",
                Email = "an email Update",
                FirstName = "Firstname Update",
                LastName = "Lastname Update",
                Id = "1",
                GenderCode = "a Update",
                InterfaceLanguageCode = "b Update",
                LanguageCode = "c Update",
                Phone1 = "0444719 Update",
                Phone2 = "123456 Update",
                Address = new ProfileInfoAddressDTO()
                {
                    CityName = "city Update",
                    CountryCode = "b Update",
                    HouseNumber = "5 Update",
                    PostalCode = "2 Update",
                    StreetName = "street Update"
                }
            });
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserAlreadyInDbTest()
        {
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "1" && x.ForceUpdate == false))
                      .Returns(GetFancRADApiUserActiveMock());
            userRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
                          .ReturnsForAnyArgs(new List<User> { GetUserFromDbMock() }.AsQueryable());
            var result = await bll.GetAuthRedirectionAsync("1", "FAS");
            Assert.AreEqual(AuthRedirection.WelcomePage, result.RedirecTo);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserNotInDbTest()
        {
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "1" && x.ForceUpdate == false))
                     .Returns(GetFancRADApiUserMock());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            var result = await bll.GetAuthRedirectionAsync("1", "FAS");
            userRepository.ReceivedWithAnyArgs(1).Add(Arg.Any<User>());
            await unitOfWork.Received(1).SaveChangesAsync();
            Assert.AreEqual(AuthRedirection.WaitingValidationPage, result.RedirecTo);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task GetAuthRedirectionUserNotInDbFailedToSave()
        {
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "1" && x.ForceUpdate == false))
                     .Returns(GetFancRADApiUserMock());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(0);

            var result = await bll.GetAuthRedirectionAsync("1", "FAS");
            userRepository.ReceivedWithAnyArgs(1).Add(Arg.Any<User>());
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetAuthRedirectionTestNotFound()
        {
            GetUserInfoResult result = null;
            Task<GetUserInfoResult> response = Task.FromResult(result);
            fancRADApi.GetUser(Arg.Any<GetUserRequest>()).Returns(response);
            await bll.GetAuthRedirectionAsync("5", "FAS");
            userRepository.DidNotReceive().Find(Arg.Any<Expression<Func<User, bool>>>());
            userRepository.DidNotReceive().Add(Arg.Any<User>());
            userRepository.DidNotReceive().Update(Arg.Any<User>());
            unitOfWork.DidNotReceive();
            await fancRADApi.Received(1).GetUser(Arg.Is<GetUserRequest>(n => n.UserId == "5"));
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserAlreadyInDbButNotValidatedTest()
        {
            var radUser = GetFancRADApiUserMock();
            radUser.NrNumber = string.Empty;
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "1" && x.ForceUpdate == false))
                     .Returns(radUser);
            userRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
                          .ReturnsForAnyArgs(new List<User> { GetUserFromDbMock() }.AsQueryable());
            var result = await bll.GetAuthRedirectionAsync("1", "FANCAdfs");
            Assert.AreEqual(AuthRedirection.WaitingValidationPage, result.RedirecTo);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserButFirstNameNotFilledTest()
        {
            var radUser = GetFancRADApiUserMock();
            radUser.FirstName = string.Empty;
            await TestMandatoryFields(radUser);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserButLastNameNotFilledTest()
        {
            var radUser = GetFancRADApiUserMock();
            radUser.LastName = string.Empty;
            await TestMandatoryFields(radUser);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserButEmailNotFilledTest()
        {
            var radUser = GetFancRADApiUserMock();
            radUser.Email = string.Empty;
            await TestMandatoryFields(radUser);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserButCountryCodeNotFilledTest()
        {
            var radUser = GetFancRADApiUserMock();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            radUser.StructuredAddress.CountryCode = string.Empty;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await TestMandatoryFields(radUser);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserButCityNameNotFilledTest()
        {
            var radUser = GetFancRADApiUserMock();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            radUser.StructuredAddress.CityName = string.Empty;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await TestMandatoryFields(radUser);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserButStreetNameNotFilledTest()
        {
            var radUser = GetFancRADApiUserMock();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            radUser.StructuredAddress.StreetName = string.Empty;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await TestMandatoryFields(radUser);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserButHouseNumberNotFilledTest()
        {
            var radUser = GetFancRADApiUserMock();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            radUser.StructuredAddress.HouseNumber = string.Empty;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await TestMandatoryFields(radUser);
        }

        [TestMethod()]
        public async Task GetAuthRedirectionUserButPostalCOdeNotFilledTest()
        {
            var radUser = GetFancRADApiUserMock();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            radUser.StructuredAddress.PostalCode = string.Empty;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await TestMandatoryFields(radUser);
        }

        private static GetUserInfoResult GetFancRADApiUserActiveMock()
        {
            return new GetUserInfoResult
            {
                FirstName = "Firstname",
                LastName = "Lastname",
                AllowedLanguageCodes = new[] { "abc" }.ToList(),
                BirthDate = new DateTime(2000, 1, 1),
                BirthPlace = "A place",
                Email = "an email",
                GenderCode = "a",
                UserIsValidated=true,
                InterfaceLanguageCode = "b",
                LanguageCode = "c",
                LastAuthenticSourceRefreshDate = new DateTime(2001, 1, 1),
                ManualUpdateAllowed = false,
                NrNumber = "12345678",
                Phone1 = "0444719",
                Phone2 = "123456",
                StructuredAddress = new UserInfoAddress
                {
                    CityName = "city",
                    CountryCode = "b",
                    HouseNumber = "5",
                    PostalCode = "2",
                    StreetName = "street"
                },
                UnstructuredAddress = new UserInfoAddressUnstructured
                {
                    CountryCode = "c",
                    Address = "a sort of address"
                }
            };
        }

        private static GetUserInfoResult GetFancRADApiUserMock()
        {
            return new GetUserInfoResult
            {
                FirstName = "Firstname",
                LastName = "Lastname",
                AllowedLanguageCodes = new[] { "abc" }.ToList(),
                BirthDate = new DateTime(2000, 1, 1),
                BirthPlace = "A place",
                Email = "an email",
                GenderCode = "a",
                InterfaceLanguageCode = "b",
                LanguageCode = "c",
                LastAuthenticSourceRefreshDate = new DateTime(2001, 1, 1),
                ManualUpdateAllowed = false,
                NrNumber = "12345678",
                Phone1 = "0444719",
                Phone2 = "123456",
                StructuredAddress = new UserInfoAddress
                {
                    CityName = "city",
                    CountryCode = "b",
                    HouseNumber = "5",
                    PostalCode = "2",
                    StreetName = "street"
                },
                UnstructuredAddress = new UserInfoAddressUnstructured
                {
                    CountryCode = "c",
                    Address = "a sort of address"
                }
            };
        }

        private static User GetUserFromDbMock()
        {
            return new User()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = 0,
                ExternalId = "36",
                IsCSAMUser = true,
                LatestConnection = DateTime.Now,
                LatestSynchronization = DateTime.Now,
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                UserRoles = new[]
                {
                    new UserRoles {
                        Id = 1,
                        UserId = 1,
                        InternalRole = "Admin"
                    }
                }
            };
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetUserIdentityAsyncTestNotFound()
        {
            try
            {
                GetUserInfoResult? userInfoResult = null;
                fancRADApi.GetUser(Arg.Any<GetUserRequest>()).ReturnsForAnyArgs(userInfoResult);
                // Act
                var result = await bll.GetUserIdentityAsync(new GetUserRequest() { UserId = "" }, "FAS");
            }
            catch (Exception ex)
            {
                await fancRADApi.DidNotReceiveWithAnyArgs().GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
                Assert.AreEqual($"Cannot Get User Identity Info . {Constants.UserNotFound}", ex.Message);
                throw;
            }
        }

        private static GetUserInfoResult GetFancRADApiUserActiveMockNew()
        {
            return new GetUserInfoResult
            {
                FirstName = "Geoffrey",
                LastName = "Jacquet",
                AllowedLanguageCodes = new[] { "fr" }.ToList(),
                InterfaceLanguageCode = "fr",
                StructuredAddress = new UserInfoAddress
                {
                    CountryCode = "BE",
                    StreetName = "Rue des Mûriers,Spy(V. 02.01.1977 rue d'Ordin)",
                    HouseNumber = "2 / D000",
                    PostalCode = "5190",
                    CityName = "Jemeppe-sur-Sambre"
                },
                UnstructuredAddress = new UserInfoAddressUnstructured { }
            };
        }

        [TestMethod()]
        public async Task GetUserIdentityAsync_ValidRequest_ReturnsUserIdentityDTO_CurrentIdFromDb()
        {
            //Arrange
            var userRequest = new GetUserRequest
            {
                UserId = "36",
                ForceUpdate = false
            };
            var getUserInfoResult = GetFancRADApiUserActiveMockNew();
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "36" && x.ForceUpdate == false))
                      .Returns(getUserInfoResult);

            var userOrganisations = GetMockUserOrganisationsDTO();

            var userFromDb = GetUserFromDbMock();
            userFromDb.LatestOrganisation = new Guid("7a7bb72d-8414-ec11-b832-02bfc0a8feb6");
            userFromDb.LatestEstablishment = new Guid("7003541f-d17f-ee11-b843-02bfc0a8feb6");

            userRepository.Find(Arg.Any<Expression<Func<User, bool>>>())
                          .ReturnsForAnyArgs(new List<User> { userFromDb }.AsQueryable());

            var organisationLinks = GetMockOrganisationLinkInfoResult();
            fancRADApi.GetUserOrganisationLinks(Arg.Is<GetUserOrganisationLinksRequest>(x => x.UserId == "36"))
                      .Returns(organisationLinks);

            var organisationEnterpriseInfo = GetMockOrganisationEnterpriseReferenceResult();
            fancRADApi.GetOrganisationEnterpriseReference(Arg.Is<GetOrganisationEnterpriseRequest>(x => x.Reference == organisationLinks.OrganisationLinks.Select(x => x.FancOrganisationId).FirstOrDefault()))
                      .Returns(organisationEnterpriseInfo);

            var organisationUsers = new List<OrganisationUsersDTO>
            {
                new OrganisationUsersDTO
                {
                    OrganisationId = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5"
                }
            };
            string cacheKey = $"User-{userRequest.UserId}-Organisations";
            sharedDataCache.GetData<List<OrganisationUsersDTO>>(Arg.Is<string>(x => x.Equals(cacheKey))).Returns(organisationUsers);

            var organisationEnterpriseInfoList = new List<GetOrganisationEnterpriseInfoResult>
            {
                organisationEnterpriseInfo
            };

            sharedDataCache.GetData<List<GetOrganisationEnterpriseInfoResult>>(Arg.Is<string>(x => x.Equals(cacheKey))).Returns(organisationEnterpriseInfoList);
            var mappedOrganisationDetail = mapper.Map<UserOrganisationsDTO>(organisationEnterpriseInfo);
            var mappedOrganisationDetailList = mapper.Map<List<UserOrganisationsDTO>>(organisationEnterpriseInfoList);

            identityProviderConfiguration.CSAM.Returns("FAS");

            // Act
            var result = await bll.GetUserIdentityAsync(userRequest, "FAS");
            var fullName = $"{getUserInfoResult.FirstName} {getUserInfoResult.LastName}".TrimStart().TrimEnd();
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fullName, result.FullName);
            Assert.AreEqual(getUserInfoResult.UserIsValidated, result.IsValidated);
            Assert.AreEqual(getUserInfoResult.EmailConfirmed, result.EmailConfirmed);
            Assert.AreEqual(getUserInfoResult.InterfaceLanguageCode, result.InterfaceLanguageCode);
            Assert.AreEqual("BasicUser", result.GlobalRoles[0].Role);

            if (userFromDb.LatestOrganisation != null)
            {
                Assert.AreEqual(new Guid("7a7bb72d-8414-ec11-b832-02bfc0a8feb6"), result.CurrentOrganisation);
            }

            if (userFromDb.LatestEstablishment != null)
            {
                //Assert.AreEqual(mappedOrganisationDetail.Establishment.First().Id, result.CurrentEstablishment);
                Assert.AreEqual(new Guid("7003541f-d17f-ee11-b843-02bfc0a8feb6"), result.CurrentEstablishment);
            }
        }

        [TestMethod()]
        public async Task GetUserIdentityAsync_ValidRequest_ReturnsUserIdentityDTO_CurrentIdFromApi()
        {
            //Arrange
            var userRequest = new GetUserRequest
            {
                UserId = "36",
                ForceUpdate = false
            };
            var getUserInfoResult = GetFancRADApiUserActiveMockNew();
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "36" && x.ForceUpdate == false))
                      .Returns(getUserInfoResult);

            var userOrganisations = GetMockUserOrganisationsDTO();

            var userFromDb = GetUserFromDbMock();

            userRepository.Find(Arg.Any<Expression<Func<User, bool>>>())
                          .ReturnsForAnyArgs(new List<User> { userFromDb }.AsQueryable());

            var organisationLinks = GetMockOrganisationLinkInfoResult();
            fancRADApi.GetUserOrganisationLinks(Arg.Is<GetUserOrganisationLinksRequest>(x => x.UserId == "36"))
                      .Returns(organisationLinks);

            var organisationEnterpriseInfo = GetMockOrganisationEnterpriseReferenceResult();
            fancRADApi.GetOrganisationEnterpriseReference(Arg.Is<GetOrganisationEnterpriseRequest>(x => x.Reference == organisationLinks.OrganisationLinks.Select(x => x.FancOrganisationId).FirstOrDefault()))
                      .Returns(organisationEnterpriseInfo);

            var organisationUsers = new List<OrganisationUsersDTO>
            {
                new OrganisationUsersDTO
                {
                    OrganisationId = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5"
                }
            };
            string cacheKey = $"User-{userRequest.UserId}-Organisations";
            sharedDataCache.GetData<List<OrganisationUsersDTO>>(Arg.Is<string>(x => x.Equals(cacheKey))).Returns(organisationUsers);

            var organisationEnterpriseInfoList = new List<GetOrganisationEnterpriseInfoResult>
            {
                organisationEnterpriseInfo
            };

            sharedDataCache.GetData<List<GetOrganisationEnterpriseInfoResult>>(Arg.Is<string>(x => x.Equals(cacheKey))).Returns(organisationEnterpriseInfoList);
            var mappedOrganisationDetail = mapper.Map<UserOrganisationsDTO>(organisationEnterpriseInfo);

            // Act
            var result = await bll.GetUserIdentityAsync(userRequest, "FANCAdfs");
            var fullName = $"{getUserInfoResult.FirstName} {getUserInfoResult.LastName}".TrimStart().TrimEnd();
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fullName, result.FullName);
            Assert.AreEqual(getUserInfoResult.UserIsValidated, result.IsValidated);
            Assert.AreEqual(getUserInfoResult.EmailConfirmed, result.EmailConfirmed);
            Assert.AreEqual("Admin", result.GlobalRoles[0].Role);

            if (userFromDb.LatestOrganisation == null)
            {
                Assert.AreEqual(new Guid("7a7bb72d-8414-ec11-b832-02bfc0a8feb5"), result.CurrentOrganisation);
            }

            if (userFromDb.LatestEstablishment == null)
            {
                Assert.AreEqual(Guid.Empty, result.CurrentEstablishment);
            }
        }

        private static GetOrganisationEnterpriseInfoResult GetMockOrganisationEnterpriseReferenceResult()
        {
            var userInfoAddress = new UserInfoAddress()
            {
                CountryCode = "BE",
                StreetName = "Rue Léon Deladrière",
                HouseNumber = "15",
                PostalCode = "1300",
                CityName = "Wavre"
            };
            
            var result = new GetOrganisationEnterpriseInfoResult()
            {
                FancOrganisationId = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5",
                Name = "VOX TENEO",
                MainAddress = userInfoAddress,
                InvoiceAddress = null,
                Establishments = new List<OrganisationEstablishment>
                {
                    new OrganisationEstablishment
                    {
                        FancOrganisationId = "7003541f-d17f-ee11-b843-02bfc0a8feb5",
                        Name = "VOX TENEO"
                    }
                }
            };

            return result;
        }

        private static GetOrganisationLinkInfoResult GetMockOrganisationLinkInfoResult()
        {
            var result = new GetOrganisationLinkInfoResult();
            var OrganisationLinks = new List<OrganisationLink>
            {
                new OrganisationLink
                {
                    FancOrganisationId = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5",
                }
            };
            result.OrganisationLinks = OrganisationLinks;
            return result;
        }

        private static List<UserOrganisationsDTO> GetMockUserOrganisationsDTO()
        {
            var userOrganisations = new List<UserOrganisationsDTO>
            {
                new UserOrganisationsDTO
                {
                    Id = new Guid("7a7bb72d-8414-ec11-b832-02bfc0a8feb5"),
                    Name = "VOX TENEO",
                    Establishment = new List<EstablishmentDTO>
                    {
                        new EstablishmentDTO
                        {
                            Id= new Guid("7003541f-d17f-ee11-b843-02bfc0a8feb5"),
                            Name = "VOX TENEO"
                        }
                    }
                }
            };
            return userOrganisations;
        }

        private async Task TestMandatoryFields(GetUserInfoResult radUser)
        {
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "1" && x.ForceUpdate == false))
                      .Returns(radUser);
            userRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
                          .ReturnsForAnyArgs(new List<User> { GetUserFromDbMock() }.AsQueryable());
            var result = await bll.GetAuthRedirectionAsync("1", "FAS");
            Assert.AreEqual(AuthRedirection.ProfilePage, result.RedirecTo);
        }

        // user roles dari rad ada di tabel user
        // user mapping ditambahin dari user mapping

        private static List<Roles> GetRolesMockDb()
        {
            var roles = new List<Roles>();
            var role = new Roles()
            {
                Id = 1,
                ExternalRole = "MyFANC_Manager",
                InternalRole = "manager"
            };
            roles.Add(role);
            return roles;
        }

        private static User GetUserRolesMockDb()
        {
            var userRoles = new UserRoles()
            {
                Id = 1,
                UserId = 2,
                InternalRole = "Admin"
            };
            var user = new User()
            {
                Id = 2,
                ExternalId = "36",
                UserRoles = new UserRoles[] { userRoles }
            };
            return user;
        }

        [TestMethod()]
        public async Task GetUserRolesTest()
        {
            var userOrganitationLinks = GetOrganisationLinksMockOrigin();
            var userOrganisations = fancRADApi.GetUserOrganisationLinks(default).ReturnsForAnyArgs(userOrganitationLinks);
            
            userRepository.Find(Arg.Any<Expression<Func<User, bool>>>())
                          .ReturnsForAnyArgs(new List<User> { GetUserFromDbMock() }.AsQueryable());

            rolesRepository.Find(
                Arg.Is<Expression<Func<Roles, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue)))
                .Returns((new[]
                {
                    new Roles {
                        Id = 1,
                        ExternalRole = "MyFANC_Manager",
                        InternalRole = "manager"
                    }
                }).AsQueryable());

            var result = await bll.GetUserRoles(new GetUserOrganisationLinksRequest() { UserId = "36" });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());


            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("manager", result[0]);
            Assert.AreEqual("Admin", result[1]);
        }

        private static GetOrganisationLinkInfoResult GetOrganisationLinksMockOrigin()
        {
            var OrganisationLinkList = new List<OrganisationLink>();

            var organisationRole = new OrganisationRole()
            {
                Role = "MyFANC_Manager",
                LinkSource = "Local"
            };
            var organisationLinkEstablishment = new OrganisationLinkEstablishment()
            {
                FancOrganisationId = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5",
                FancNumber = "LE-0300057",
                EnterpriseCBENumber = "0477390844",
                BusinessUnitCBENumber = "businessUnitCBENumber",
                Name = "Company Name",
                Roles = new List<OrganisationRole>() { organisationRole }
            };
            var organisationLink = new OrganisationLink()
            {
                FancOrganisationId = "7a7bb72d-8414-ec11-b832-02bfc0a8feb5",
                FancNumber = "LE-0300057",
                EnterpriseCBENumber = "0477390844",
                Name = "VOX TENEO",
                Roles = new List<OrganisationRole>() { organisationRole },
                Establishments = new List<OrganisationLinkEstablishment>() { organisationLinkEstablishment }
            };
            OrganisationLinkList.Add(organisationLink);

            return new GetOrganisationLinkInfoResult()
            {
                OrganisationLinks = OrganisationLinkList
            };
        }

        [TestMethod()]
        public async Task UserRolesTestParameterInvalid()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                var result = await bll.GetUserRoles(new GetUserOrganisationLinksRequest() { UserId = "" });
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }

        [TestMethod()]
        public async Task UserRolesTestUserOrganitationLinksNotFound()
        {
            var userOrganitationLinks = new GetOrganisationLinkInfoResult();
            fancRADApi.GetUserOrganisationLinks(default).ReturnsForAnyArgs(userOrganitationLinks);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                var result = await bll.GetUserRoles(new GetUserOrganisationLinksRequest() { UserId = "1" });
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }

        [TestMethod()]
        public async Task UserRolesTestCallApiError()
        {
            fancRADApi.When(x => x.GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>())).Do(x => { throw new Exception(); });

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                var result = await bll.GetUserRoles(new GetUserOrganisationLinksRequest() { UserId = "1" });
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetUserOrganisationLinks(Arg.Any<GetUserOrganisationLinksRequest>());
        }

        [TestMethod]
        public async Task AcceptInvitationTest()
        {
            string emailTo = "ahn@voxteneo.com";
            string fancOrgId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            string roleManager = "manager";

            fancRADApi.GetUser(Arg.Any<GetUserRequest>()).ReturnsForAnyArgs(new GetUserInfoResult() { Email = emailTo});

            var result = await bll.AcceptInvitation("25", new UpdateOrganisationLinkInfoRequest()
            {
                FancOrganisationId = fancOrgId,
                RequestingUserId = "1",
                Roles = new List<string>() { roleManager }
            });

            await fancRADApi.Received(1).UpdateUserOrganisationLinks(Arg.Is<UpdateOrganisationLinkInfoRequest>(x => x.FancOrganisationId == fancOrgId && x.RequestingUserId == "1" && x.Roles.Contains(roleManager)), Arg.Is<UpdateUserRequest>(x => x.UserId == "25"));
            await fancRADApi.ReceivedWithAnyArgs(1).GetUser(Arg.Any<GetUserRequest>());
            await emailService.Received(1).SendInvitationAcceptOrRefuseNotificationAsync(Arg.Is<string>(toEmail => toEmail == emailTo), Arg.Is<string>(uid => uid == "25"), Arg.Is<string>(fid => fid == fancOrgId), Arg.Is<IEnumerable<string>>(role => role.Contains(roleManager)), Arg.Is<bool>(isAccepted => isAccepted == true));
            Assert.AreEqual("You successfully accepted to join the Operational Entity. The manager of the entity will be informed.", result);
        }

        [TestMethod()]
        public async Task AcceptInvitationCallApiError()
        {
            fancRADApi.When(x => x.GetUser(Arg.Any<GetUserRequest>())).Do(x => { throw new Exception(); });
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.AcceptInvitation("25", new UpdateOrganisationLinkInfoRequest()
                {
                    FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    RequestingUserId = "1",
                    Roles = new List<string>() { "manager" }
                });
            });
            await fancRADApi.ReceivedWithAnyArgs(1).UpdateUserOrganisationLinks(Arg.Any<UpdateOrganisationLinkInfoRequest>(), Arg.Any<UpdateUserRequest>());
        }

        [TestMethod]
        public async Task RefuseInvitationTest()
        {
            string emailTo = "ahn@voxteneo.com";
            string fancOrgId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            string roleManager = "manager";

            fancRADApi.GetUser(Arg.Any<GetUserRequest>()).ReturnsForAnyArgs(new GetUserInfoResult() { Email = emailTo });

            var result = await bll.RefuseInvitation("25", new UpdateOrganisationLinkInfoRequest()
            {
                FancOrganisationId = fancOrgId,
                RequestingUserId = "1",
                Roles = new List<string>() { roleManager }
            });

            await fancRADApi.Received(1).DeleteUserOrganisationLinks(Arg.Is<UpdateOrganisationLinkInfoRequest>(x => x.FancOrganisationId == fancOrgId && x.RequestingUserId == "1" && x.Roles.Contains(roleManager)), Arg.Is<UpdateUserRequest>(x => x.UserId == "25"));
            await fancRADApi.ReceivedWithAnyArgs(1).GetUser(Arg.Any<GetUserRequest>());
            await emailService.Received(1).SendInvitationAcceptOrRefuseNotificationAsync(Arg.Is<string>(toEmail => toEmail == emailTo), Arg.Is<string>(uid => uid == "25"), Arg.Is<string>(fid => fid == fancOrgId), Arg.Is<IEnumerable<string>>(role => role.Contains(roleManager)), Arg.Is<bool>(isAccepted => isAccepted == false));
            Assert.AreEqual("You successfully refused to join the Operational Entity. The manager of the entity will be informed.", result);
        }

        [TestMethod()]
        public async Task RefuseInvitationCallApiError()
        {
            fancRADApi.When(x => x.GetUser(Arg.Any<GetUserRequest>())).Do(x => { throw new Exception(); });
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.RefuseInvitation("25", new UpdateOrganisationLinkInfoRequest()
                {
                    FancOrganisationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    RequestingUserId = "1",
                    Roles = new List<string>() { "manager" }
                });
            });
            await fancRADApi.ReceivedWithAnyArgs(1).DeleteUserOrganisationLinks(Arg.Any<UpdateOrganisationLinkInfoRequest>(), Arg.Any<UpdateUserRequest>());
        }

        [TestMethod()]
        public async Task UpdateCurrentSelectorTest()
        {
            //arrange
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    ExternalId = "29"
                },
                new User
                {
                    Id = 1,
                    ExternalId = "30"
                }
            };
            var userIdParam = "29";
            var userRequest = new SelectorUserDTO()
            {
                OrganisationId = new Guid("75153fc0-b230-4785-b7f3-df005258729b"),
                EstablishmentId = new Guid("78e68cdb-b3d7-483a-a4c1-458fc5320545")
            };

            userRepository.Find(Arg.Any<Expression<Func<User, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<User, bool>>>();
                return users.Where(exp.Compile()).AsQueryable();
            });

            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            //act
            var result = await bll.UpdateCurrentSelector(userIdParam, userRequest);

            userRepository.ReceivedWithAnyArgs(1).Update(Arg.Any<User>());
            await unitOfWork.Received(1).SaveChangesAsync();

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userRequest.OrganisationId, result.OrganisationId);
            Assert.AreEqual(userRequest.EstablishmentId, result.EstablishmentId);
        }

        [TestMethod()]
        public async Task UpdateCurrentSelectorTest_EstablishmentNull()
        {
            //arrange
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    ExternalId = "29"
                },
                new User
                {
                    Id = 1,
                    ExternalId = "30"
                }
            };
            var userIdParam = "29";
            var userRequest = new SelectorUserDTO()
            {
                OrganisationId = new Guid("ff215841-ea5d-ee11-b843-02bfc0a8feb5"),
                EstablishmentId = null
            };

            userRepository.Find(Arg.Any<Expression<Func<User, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<User, bool>>>();
                return users.Where(exp.Compile()).AsQueryable();
            });

            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            //act
            var result = await bll.UpdateCurrentSelector(userIdParam, userRequest);

            userRepository.ReceivedWithAnyArgs(1).Update(Arg.Any<User>());
            await unitOfWork.Received(1).SaveChangesAsync();

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userRequest.OrganisationId, result.OrganisationId);
            Assert.AreEqual(userRequest.EstablishmentId, result.EstablishmentId);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task UpdateCurrentSelectorTest_FailedSave()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    ExternalId = "29"
                },
                new User
                {
                    Id = 2,
                    ExternalId = "30"
                }
            };

            var userIdParam = "29";
            var userRequest = new SelectorUserDTO()
            {
                OrganisationId = new Guid("75153fc0-b230-4785-b7f3-df005258729b"),
                EstablishmentId = new Guid("78e68cdb-b3d7-483a-a4c1-458fc5320545")
            };

            userRepository.Find(Arg.Any<Expression<Func<User, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<User, bool>>>();
                return users.Where(exp.Compile()).AsQueryable();
            });

            try
            {
                var result = await bll.UpdateCurrentSelector(userIdParam, userRequest);
            }
            catch (Exception ex)
            {
                unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(0);
                Assert.AreEqual($"no change has been commited, User not updated", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdateCurrentSelectorTest_FailedKeyNotFoundException()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    ExternalId = "29"
                },
                new User
                {
                    Id = 2,
                    ExternalId = "30"
                }
            };

            var userIdParam = "1";
            var userRequest = new SelectorUserDTO()
            {
                OrganisationId = new Guid("75153fc0-b230-4785-b7f3-df005258729b"),
                EstablishmentId = new Guid("78e68cdb-b3d7-483a-a4c1-458fc5320545")
            };

            userRepository.Find(Arg.Any<Expression<Func<User, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<User, bool>>>();
                return users.Where(exp.Compile()).AsQueryable();
            });

            try
            {
                var result = await bll.UpdateCurrentSelector(userIdParam, userRequest);
            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"Invalid user ID. 1", ex.Message);
                throw;
            }
        }
    }
}

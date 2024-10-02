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
using MyFanc.DTO.Internal.Users;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyFanc.Core.Enums;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_Mapping_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IGenericRepository<User> userRepository;
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
            this.unitOfWork = Substitute.For<IUnitOfWork>();
            this.userRepository = Substitute.For<IGenericRepository<User>>();
            this.wizardRepository = Substitute.For<IGenericRepository<Wizard>>();
            this.questionRepository = Substitute.For<IGenericRepository<Question>>();

            this.sharedDataCache = Substitute.For<ISharedDataCache>();

            this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();

            this.unitOfWork.GetGenericRepository<User>().Returns(this.userRepository);
            this.unitOfWork.GetGenericRepository<Wizard>().Returns(this.wizardRepository);
            this.unitOfWork.GetGenericRepository<Question>().Returns(this.questionRepository);
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.identityProviderConfiguration = Substitute.For<IdentityProviderConfiguration>();

            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new WizardProfile());
                cfg.AddProfile(new DataProfile());
            });

            this.mapper = new Mapper(mapperConfig);

            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();

            this.breadCrumbService = new BreadCrumbService(mapper);

            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod()]
        public async Task GetUserInfoAsyncTestMapping()
        {
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "1" && x.ForceUpdate == false))
                      .Returns(new GetUserInfoResult
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
                      });

            var result = await bll.GetUserInfoAsync(new DTO.External.RADApi.GetUserRequest { UserId = "1" });

            //Assert.IsTrue(fancRADApi.ReceivedWithAnyArgs().GetUser(default));
            Assert.AreEqual("Firstname", result.FirstName);
            Assert.AreEqual("Lastname", result.LastName);
            Assert.AreEqual(new DateTime(2001, 1, 1), result.LastAuthenticSourceRefreshDate);
            Assert.AreEqual("an email", result.Email);
            Assert.AreEqual("A place", result.BirthPlace);
            Assert.AreEqual(new DateTime(2000, 1, 1), result.BirthDate);
            Assert.AreEqual("a", result.GenderCode);
            Assert.AreEqual("b", result.InterfaceLanguageCode);
            Assert.AreEqual("c", result.LanguageCode);
            Assert.AreEqual(false, result.ManualUpdateAllowed);
            Assert.AreEqual("12345678", result.NrNumber);
            Assert.AreEqual("0444719", result.Phone1);
            Assert.AreEqual("123456", result.Phone2);
            Assert.AreEqual(ValidationStatus.Pending, result.ValidationStatus);
            Assert.IsNotNull(result.StructuredAddress);
            Assert.AreEqual("city", result.StructuredAddress.CityName);
            Assert.AreEqual("b", result.StructuredAddress.CountryCode);
            Assert.AreEqual("5", result.StructuredAddress.HouseNumber);
            Assert.AreEqual("2", result.StructuredAddress.PostalCode);
            Assert.AreEqual("street", result.StructuredAddress.StreetName);
            Assert.IsNotNull(result.UnstructuredAddress);
            Assert.AreEqual("c", result.UnstructuredAddress.CountryCode);
            Assert.AreEqual("a sort of address", result.UnstructuredAddress.Address);

        }

        [TestMethod()]
        public async Task UpdateUserAsyncTestMapping()
        {
            var getUserInfoResult = new GetUserInfoResult()
            {
                FirstName = "Firstname Update",
                LastName = "Lastname Update",
                AllowedLanguageCodes = new[] { "abc" }.ToList(),
                BirthDate = new DateTime(2000, 1, 2),
                BirthPlace = "A Place Update",
                Email = "an email Update",
                GenderCode = "a Update",
                InterfaceLanguageCode = "b Update",
                LanguageCode = "c Update",
                LastAuthenticSourceRefreshDate = new DateTime(2001, 1, 1),
                ManualUpdateAllowed = false,
                NrNumber = "12345678 Update",
                Phone1 = "0444719 Update",
                Phone2 = "123456 Update",
                StructuredAddress = new UserInfoAddress
                {
                    CityName = "city Update",
                    CountryCode = "b Update",
                    HouseNumber = "5 Update",
                    PostalCode = "2 Update",
                    StreetName = "street Update"
                },
                UnstructuredAddress = new UserInfoAddressUnstructured
                {
                    CountryCode = "c",
                    Address = "a sort of address"
                }
            };
            fancRADApi.GetUser(Arg.Is<GetUserRequest>(x => x.UserId == "1" && x.ForceUpdate == false))
                      .Returns(Task.FromResult(getUserInfoResult));
            var dummyUpdateUserInfoRequest = new UpdateUserInfoRequest()
            {
                BirthDate = new DateTime(2000, 1, 2),
                BirthPlace = "A Place Update",
                Email = "an email Update",
                FirstName = "Firstname Update",
                LastName = "Lastname Update",
                GenderCode = "a Update",
                InterfaceLanguageCode = "b Update",
                LanguageCode = "c Update",
                Phone1 = "0444719 Update",
                Phone2 = "123456 Update",
                StructuredAddress = new UserInfoAddress()
                {
                    CountryCode = "c Update",
                    CityName = "city Update",
                    HouseNumber = "5 Update",
                    PostalCode = "2 Update",
                    StreetName = "street Update"
                }
            };
            fancRADApi.UpdateUser(Arg.Any<UpdateUserInfoRequest>(), Arg.Any<UpdateUserRequest>())
                .Returns(Task.FromResult(dummyUpdateUserInfoRequest));

            var updateProfileDTO = new UpdateProfileDTO()
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
            };
            var result = await bll.UpdateUserAsync("1", updateProfileDTO);

            Assert.AreEqual("Firstname Update", result.FirstName);
            Assert.AreEqual("Lastname Update", result.LastName);
            Assert.AreEqual(new DateTime(2000, 1, 2), result.BirthDate);
            Assert.AreEqual("an email Update", result.Email);
            Assert.AreEqual("A Place Update", result.BirthPlace);
            Assert.AreEqual("a Update", result.GenderCode);
            Assert.AreEqual("b Update", result.InterfaceLanguageCode);
            Assert.AreEqual("c Update", result.LanguageCode);
            Assert.AreEqual("0444719 Update", result.Phone1);
            Assert.AreEqual("123456 Update", result.Phone2);
            Assert.IsNotNull(result.StructuredAddress);
        }
    }
}

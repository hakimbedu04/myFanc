using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.BLL;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.Services.FancRadApi;
using MyFanc.Services;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using MyFanc.Api.Mapper;
using MyFanc.DTO.Internal.Data;
using MyFanc.BusinessObjects;
using MyFanc.DTO.External.RADApi;
using System.Threading.Tasks;
using System;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_BLL_Data_Test
    {
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IGenericRepository<Question> questionRepository;
        private IGenericRepository<User> userRepository;
        private IGenericRepository<Wizard> wizardRepository;
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
            this.userRepository = Substitute.For<IGenericRepository<User>>();
            this.wizardRepository = Substitute.For<IGenericRepository<Wizard>>();
            this.questionRepository = Substitute.For<IGenericRepository<Question>>();
            this.sharedDataCache = Substitute.For<ISharedDataCache>();
            this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();
            this.aESEncryptService = Substitute.For<IAESEncryptService>();
            this.tokenConfiguration = Substitute.For<TokenConfiguration>();
            this.emailService = Substitute.For<IEmailService>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.identityProviderConfiguration = Substitute.For<IdentityProviderConfiguration>();

            this.unitOfWork.GetGenericRepository<User>().Returns(this.userRepository);
            this.unitOfWork.GetGenericRepository<Wizard>().Returns(this.wizardRepository);
            this.unitOfWork.GetGenericRepository<Question>().Returns(this.questionRepository);


            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new WizardProfile());
                cfg.AddProfile(new LegalEntityProfile());
                cfg.AddProfile(new DataProfile());
            });

            this.mapper = new Mapper(mapperConfig);
            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();
            this.breadCrumbService = new BreadCrumbService(mapper);
            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod]
        public void GetSectorsTest()
        {
            var sectors = new List<SectorDTO>()
            {
                new SectorDTO()
                {
                    Id = 1,
                    NacabelCode = "01.01",
                    Sector = "Dentist"
                },
                new SectorDTO()
                {
                    Id = 2,
                    NacabelCode ="01.02",
                    Sector = "Radiopharma"
                }
            };
            nacabelHelper.GetSectors(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).ReturnsForAnyArgs(sectors);
            var result = bll.GetSectors("en", "", 10);
            nacabelHelper.Received(1).GetSectors(Arg.Is<string>(x => x == "en"), Arg.Is<string>(x => x == ""), Arg.Is<int>(x => x == 10));
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.ElementAt(0).Id);
            Assert.AreEqual("01.01", result.ElementAt(0).NacabelCode);
            Assert.AreEqual("Dentist", result.ElementAt(0).Sector);

        }

        [TestMethod()]
        public async Task GetCitiesAsyncTest()
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

            fancRADApi.GetCities().ReturnsForAnyArgs(citiesInfoResult);
            var result = await bll.GetCitiesAsync();

            await fancRADApi.ReceivedWithAnyArgs(1).GetCities();
            Assert.AreEqual(1, result.Count());
            var city = result.First();
            Assert.AreEqual(21004, city.NisCode);
            Assert.AreEqual(1000, city.PostCode);
            Assert.AreEqual("nl", city.OfficialLangCode1);
            Assert.AreEqual("fr", city.OfficialLangCode2);
            Assert.AreEqual(2, city.Names.Count());
        }

        [TestMethod()]
        public async Task GetCitiesAsyncTestCityNotFOund()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetCitiesAsync();
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetCities();
        }

        [TestMethod()]
        public async Task GetCitiesByCodeAsyncTest()
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
            var result = await bll.GetCitiesByCodeAsync("1000");

            await fancRADApi.ReceivedWithAnyArgs(1).GetCityByCode(Arg.Any<GetCityRequest>());
            Assert.AreEqual(1, result.Count());
            var city = result.First();
            Assert.AreEqual(21004, city.NisCode);
            Assert.AreEqual(1000, city.PostCode);
            Assert.AreEqual("nl", city.OfficialLangCode1);
            Assert.AreEqual("fr", city.OfficialLangCode2);
            Assert.AreEqual(2, city.Names.Count());
        }

        [TestMethod()]
        public async Task GetCitiesByCodeAsyncTestInvalidParameter()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await bll.GetCitiesByCodeAsync("");
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().GetCityByCode(Arg.Any<GetCityRequest>());
        }

        [TestMethod()]
        public async Task GetCitiesByCodeAsyncTestCityNotFOund()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetCitiesByCodeAsync("2000");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetCityByCode(Arg.Any<GetCityRequest>());
        }

        [TestMethod()]
        public async Task GetLegalFormsAsyncTest()
        {
            var legalFormsInfoResult = new List<GetLegalFormInfoResult>() { new GetLegalFormInfoResult()
            {
                    Code = "001",
                    DescriptionFR = "Société coopérative européenne",
                    DescriptionNL = "Europese Coöperatieve Vennootschap",
                    DescriptionDE = "Europäische Genossenschaft",
                    DescriptionEN = "European cooperative society",
                    AbbreviationFR = "SCE",
                    AbbreviationNL = "SCE",
                    AbbreviationDE = "SCE",
                    AbbreviationEN = "SCE"
            } };

            fancRADApi.GetLegalForms().ReturnsForAnyArgs(legalFormsInfoResult);
            var result = await bll.GetLegalFormsAsync();

            await fancRADApi.ReceivedWithAnyArgs(1).GetLegalForms();
            Assert.AreEqual(1, result.Count());
            var legalForm = result.First();
            Assert.AreEqual("Société coopérative européenne", legalForm.DescriptionFR);
            Assert.AreEqual("Europese Coöperatieve Vennootschap", legalForm.DescriptionNL);
            Assert.AreEqual("SCE", legalForm.AbbreviationFR);
            Assert.AreEqual("SCE", legalForm.AbbreviationNL);
        }

        [TestMethod()]
        public async Task GetLegalFormsAsyncTestCityNotFOund()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetLegalFormsAsync();
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetLegalForms();
        }

        [TestMethod()]
        public async Task GetLegalFormsByCodeAsyncTest()
        {
            var legalFormsInfoResult = new List<GetLegalFormInfoResult>() { new GetLegalFormInfoResult()
            {
                    Code = "001",
                    DescriptionFR = "Société coopérative européenne",
                    DescriptionNL = "Europese Coöperatieve Vennootschap",
                    DescriptionDE = "Europäische Genossenschaft",
                    DescriptionEN = "European cooperative society",
                    AbbreviationFR = "SCE",
                    AbbreviationNL = "SCE",
                    AbbreviationDE = "SCE",
                    AbbreviationEN = "SCE"
            } };

            fancRADApi.GetLegalFormByCode(Arg.Any<GetLegalFormRequest>()).ReturnsForAnyArgs(legalFormsInfoResult);
            var result = await bll.GetLegalFormsByCodeAsync("001");

            await fancRADApi.ReceivedWithAnyArgs(1).GetLegalFormByCode(Arg.Any<GetLegalFormRequest>());
            Assert.AreEqual(1, result.Count());
            var legalForm = result.First();
            Assert.AreEqual("Société coopérative européenne", legalForm.DescriptionFR);
            Assert.AreEqual("Europese Coöperatieve Vennootschap", legalForm.DescriptionNL);
            Assert.AreEqual("SCE", legalForm.AbbreviationFR);
            Assert.AreEqual("SCE", legalForm.AbbreviationNL);
        }

        [TestMethod()]
        public async Task GetLegalFormsByCodeAsyncTestInvalidParameter()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await bll.GetLegalFormsByCodeAsync("");
            });
            await fancRADApi.DidNotReceiveWithAnyArgs().GetLegalFormByCode(Arg.Any<GetLegalFormRequest>());
        }

        [TestMethod()]
        public async Task GetLegalFormsByCodeAsyncTestCityNotFOund()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.GetLegalFormsByCodeAsync("001");
            });
            await fancRADApi.ReceivedWithAnyArgs(1).GetLegalFormByCode(Arg.Any<GetLegalFormRequest>());
        }
    }
}

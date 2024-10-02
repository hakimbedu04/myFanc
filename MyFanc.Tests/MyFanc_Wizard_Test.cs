using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.Api.Mapper;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.DTO.Internal.Translation;
using MyFanc.DTO.Internal.Wizard;
using MyFanc.DTO.Internal.Wizards;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_Wizard_Test
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
        private BreadCrumbService breadCrumbService;
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
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.sharedDataCache = Substitute.For<ISharedDataCache>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();

            this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();

            this.unitOfWork.GetGenericRepository<User>().Returns(this.userRepository);
            this.unitOfWork.GetGenericRepository<Wizard>().Returns(this.wizardRepository);
            this.unitOfWork.GetGenericRepository<Question>().Returns(this.questionRepository);

            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new WizardProfile());
                cfg.AddProfile(new DataProfile());
            });

            var implMapper = new Mapper(mapperConfig);
            this.mapper = Substitute.For<IMapper>();

            this.mapper.Map<WizardDTO>(Arg.Any<Wizard>())
               .Returns(callinfo => implMapper.Map< WizardDTO> (callinfo.Arg<Wizard>()));
            this.mapper.Map<TranslationDTO>(Arg.Any<Translation>())
               .Returns(callinfo => implMapper.Map<TranslationDTO>(callinfo.Arg<Translation>()));

            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();

            //this.breadCrumbService = new BreadCrumbService(mapper);
            this.breadCrumbService = Substitute.ForPartsOf<BreadCrumbService>(mapper);

            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod]
        public async Task GetWizardIdAlreadyInDbTest()
        {
            int wizarIdToRetrieve = 1;
            var wizardInDb = GetWizardMock();
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizardInDb }).AsQueryable());

            var wizard = await bll.GetWizardDetailAsync(wizarIdToRetrieve);

            Assert.IsNotNull(wizard);
            Assert.AreEqual(wizardInDb.Id, wizard.Id);
            Assert.AreEqual(true, wizard.HasFirstQuestion);
        }

        [TestMethod]
        public async Task GetWizard_HasFirstQuestionIsFalse()
        {
            int wizarIdToRetrieve = 1;
            var wizardInDb = GetWizardMock();
            var question = new List<Question>
            {
                new Question {
                    Id = 1,
                    IsFirstQuestion = false,
                    IsActive = true
                }
            };
            wizardInDb.Questions = question;
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizardInDb }).AsQueryable());

            var wizard = await bll.GetWizardDetailAsync(wizarIdToRetrieve);

            Assert.IsNotNull(wizard);
            Assert.AreEqual(wizardInDb.Id, wizard.Id);
            Assert.AreEqual(false, wizard.HasFirstQuestion);
        }

        [TestMethod]
        public async Task GetWizard_HasFirstQuestionIsFalse_IsactiveFalse()
        {
            int wizarIdToRetrieve = 1;
            var wizardInDb = GetWizardMock();
            var question = new List<Question>
            {
                new Question {
                    Id = 1,
                    IsFirstQuestion = true,
                    IsActive = false
                },
                new Question {
                    Id = 2,
                    IsFirstQuestion = false,
                    IsActive = true
                },
                new Question {
                    Id = 3,
                    IsFirstQuestion = false,
                    IsActive = true
                },
            };
            wizardInDb.Questions = question;
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizardInDb }).AsQueryable());

            var wizard = await bll.GetWizardDetailAsync(wizarIdToRetrieve);

            Assert.IsNotNull(wizard);
            Assert.AreEqual(wizardInDb.Id, wizard.Id);
            Assert.AreEqual(false, wizard.HasFirstQuestion);
        }

        [TestMethod]
        public async Task GetWizardIdNotInDbTest()
        {
            int wizarIdToRetrieve = 0;
            unitOfWork.SaveChangesAsync().Returns(1);
            var wizard = await bll.GetWizardDetailAsync(wizarIdToRetrieve);
            Assert.IsNotNull(wizard);
            wizardRepository.Received(1).Add(Arg.Any<Wizard>());
        }

        private Question GetQuestionMock()
        {
            return new Question
            {
                Id = 1,
                WizardId = 1,
                IsActive = true,
                IsFirstQuestion = true,
                Answers = new List<Answer> { },
                Breadcrumbs = new List<QuestionBreadcrumb> { },
                BreadcrumbsItems = new List<QuestionBreadcrumbItem> { },
                Texts = new List<Translation> { },
                Titles = new List<Translation> { },
                Wizard = new Wizard
                {
                    Id = 1,
                    CreationTime = DateTime.Now,
                    Questions = new List<Question> { },
                    CreatorUserId = 1,
                    DeletedTime = DateTime.Now,
                    DeleterUserId = 1,
                    IntroductionTexts = new List<Translation> { },
                    LatestUpdateTime = DateTime.Now,
                    LatestUpdateUserId = 1,
                    Titles = new List<Translation> { },
                },
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                DeletedTime = DateTime.Now,
                DeleterUserId = 1
            };
        }
        private Wizard GetWizardMock()
        {
            Question question = GetQuestionMock();
            List<Translation> titles = new List<Translation>() { 
                new Translation { Id = 1, LanguageCode = "en", Text = "Number"},
                new Translation { Id = 2, LanguageCode = "fr", Text = "Numbre"},
            };
            List<Translation> introductionTexts = new List<Translation>() {
                new Translation { Id = 1, LanguageCode = "en", Text = "one"},
                new Translation { Id = 2, LanguageCode = "fr", Text = "uno"},
            };
            return new Wizard
            {
                Id = 1,
                IntroductionTexts = introductionTexts,
                Questions = new List<Question>() { question },
                Titles = titles,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                DeletedTime = DateTime.Now,
                DeleterUserId = 1
            };
        }

        [TestMethod]
        public async Task EditWizardTest()
        {
            // Arrange
            var existingWizard = GetWizardMock();
            existingWizard.Titles = new List<Translation> 
            {
                new Translation()
                {
                    Id = 1,
                    LanguageCode = "en",
                    Text = "Number"
                }
            };
            existingWizard.IntroductionTexts = new List<Translation> 
            {
                new Translation(){
                    Id = 1
                }
            };
            var wizardDto = new WizardDTO()
            {
                Id = 1,
                Titles = new List<TranslationDTO>()
                {
                    new TranslationDTO()
                    {
                        Id = 1
                    },
                    new TranslationDTO()
                    {
                        Id = 2
                    }
                },
                IntroductionTexts = new List<TranslationDTO>()
                {
                    new TranslationDTO()
                    {
                        Id=1
                    }
                }
            };

            // Act
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);
            var updatedWizard = await bll.EditWizardAsync(wizardDto);

            // Assert
            wizardRepository.Received(0).Add(Arg.Any<Wizard>());
            await unitOfWork.Received(1).SaveChangesAsync();
            mapper.Received(1).Map(Arg.Any<WizardDTO>(), Arg.Any<Wizard>());
            mapper.Received(2).Map(Arg.Any<TranslationDTO>(), Arg.Any<Translation>());
            Assert.IsNotNull(updatedWizard);
            Assert.AreEqual(existingWizard.Id, updatedWizard.Id);
            Assert.AreEqual(2, updatedWizard.Titles.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task EditWizardTest_Failed()
        {
            try
            {
                var existingWizard = GetWizardMock();
                existingWizard.Titles = new List<Translation>
                {
                    new Translation()
                    {
                        Id = 1
                    }
                };
                existingWizard.IntroductionTexts = new List<Translation>
                {
                    new Translation()
                    {
                        Id = 1
                    }
                };
                var wizardDto = new WizardDTO()
                {
                    Titles = new List<DTO.Internal.Translation.TranslationDTO> { },
                    IntroductionTexts = new List<DTO.Internal.Translation.TranslationDTO> { },
                };
                wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
                unitOfWork.SaveChangesAsync().Returns(0);
                var wizard = await bll.EditWizardAsync(wizardDto);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("no change has been commited, Wizard not written", ex.Message);
                throw;
            }

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task EditWizardTest_TitlesFailed()
        {
            try
            {
                var existingWizard = GetWizardMock();
                existingWizard.Titles = new List<Translation>
                {
                    new Translation()
                    {
                        Id = 1
                    }
                };
                existingWizard.IntroductionTexts = new List<Translation>
                {
                    new Translation()
                    {
                        Id = 1
                    }
                };
                var wizardDto = new WizardDTO()
                {
                    Titles = new List<TranslationDTO>()
                    {
                        new TranslationDTO()
                        {
                            Id = 1
                        },
                         new TranslationDTO()
                        {
                            Id = 2
                        },
                        new TranslationDTO()
                        {
                            Id=3
                        }
                    },
                    IntroductionTexts = new List<TranslationDTO>()
                    {
                        new TranslationDTO()
                        {
                            Id=1
                        },
                        new TranslationDTO()
                        {
                            Id=2
                        }
                    }
                };
                wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
                unitOfWork.SaveChangesAsync().Returns(0);
                var updatedWizard = await bll.EditWizardAsync(wizardDto);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("no change has been commited, Wizard not written", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task EditWizardTest_IntroductionTextsFailed()
        {
            try
            {
                var existingWizard = GetWizardMock();
                existingWizard.Titles = new List<Translation>
                {
                    new Translation()
                    {
                        Id = 1
                    }
                };
                existingWizard.IntroductionTexts = new List<Translation>
                {
                    new Translation()
                    {
                        Id = 1
                    }
                };
                var wizardDto = new WizardDTO()
                {
                    Titles = new List<TranslationDTO>()
                    {
                        new TranslationDTO()
                        {
                            Id = 1
                        },
                         new TranslationDTO()
                        {
                            Id = 2
                        }
                    },
                    IntroductionTexts = new List<TranslationDTO>()
                    {
                        new TranslationDTO()
                        {
                            Id=1
                        },
                        new TranslationDTO()
                        {
                            Id=2
                        },
                        new TranslationDTO()
                        {
                            Id=3
                        }
                    }
                };
                wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
                unitOfWork.SaveChangesAsync().Returns(0);
                var updatedWizard = await bll.EditWizardAsync(wizardDto);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("no change has been commited, Wizard not written", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task EditWizardIdTest()
        {
            var wizardInDb = GetWizardMock();
            WizardDTO updateDto = new WizardDTO()
            {
                Id = 1,
                Titles = new List<TranslationDTO>() {
                    new TranslationDTO()
                    {
                        Id= 1,
                        LanguageCode = "en",
                        Text ="Update"
                    },
                    new TranslationDTO()
                    {
                        Id= 2,
                        LanguageCode = "fr",
                        Text ="mise à jour"
                    }
                },
                IntroductionTexts = new List<TranslationDTO>() {
                    new TranslationDTO()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text= "Update"
                    },
                    new TranslationDTO()
                    {
                        Id = 2,
                        LanguageCode = "fr",
                        Text ="mise à jour"
                    }
                }
            };
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizardInDb }.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);
            var updateWizard = await bll.EditWizardAsync(updateDto);
            mapper.Received(1).Map(updateDto, wizardInDb);
            mapper.Received(4).Map(Arg.Any<TranslationDTO>(), Arg.Any<Translation>());
            Assert.IsNotNull(updateWizard);
            Assert.AreEqual(updateWizard.Id, updateDto.Id);
            Assert.AreEqual(2, updateDto.Titles.Count());
            Assert.AreEqual(2, updateDto.IntroductionTexts.Count());
        }

        [TestMethod]
        public async Task EditWizard_Deleted_Translations_Test()
        {
            // Arrange
            var existingWizard = GetWizardMock();
            existingWizard.Titles = new List<Translation> 
            {
                new Translation(){ Id = 1 }
            };
            existingWizard.IntroductionTexts = new List<Translation> 
            {
                new Translation(){ Id = 1 },
                new Translation(){ Id = 2 }
            };
            var wizardDto = new WizardDTO()
            {
                Titles = new List<TranslationDTO>()
                {
                    new TranslationDTO(){ Id = 1 }
                },
                IntroductionTexts = new List<TranslationDTO>()
                {
                    new TranslationDTO(){ Id = 1 }
                }
            };
            // Act
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);
            var wizard = await bll.EditWizardAsync(wizardDto);
            Assert.IsNotNull(wizard);
            wizardRepository.Received(0).Add(Arg.Any<Wizard>());
            await unitOfWork.Received(1).SaveChangesAsync();
            //mapper.Received(0).Map(Arg.Any<WizardDTO>(), Arg.Any<Wizard>());
            //mapper.Received(1).Map(wizardDto, existingWizard);
            //mapper.Received().Map<WizardDTO>(Arg.Any<Wizard>());
            //mapper.Received(2).Map(Arg.Any<TranslationDTO>(), Arg.Any<Translation>());
            Assert.AreEqual(1, wizard.Titles.Count());
            Assert.AreEqual(1, wizard.IntroductionTexts.Count());
        }

        //[TestMethod]
        //[ExpectedException(typeof(Exception))]
        //public async Task EditWizardIdTestException()
        //{
        //    var wizardInDb = GetWizardMock();
        //    WizardDTO updateDto = new WizardDTO();
        //    wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizardInDb }.AsQueryable());
        //    unitOfWork.SaveChangesAsync().Returns(0);
        //    var wizardId = await bll.EditWizardAsync(updateDto);
        //}
    }
}

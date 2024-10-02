using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.Api.Mapper;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.DTO.Internal.Wizard;
using MyFanc.DTO.Internal.Wizards;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_Question_Test
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

             this.mapper.Map<List<QuestionDTO>>(Arg.Any<List<Question>>())
                .Returns(callinfo => implMapper.Map<List<QuestionDTO>>(callinfo.Arg<List<Question>>()));
             this.mapper.Map<Question>(Arg.Any<CreateQuestionDTO>())
                .Returns(callinfo => implMapper.Map<Question>(callinfo.Arg<CreateQuestionDTO>()));
             this.mapper.Map<QuestionDetailDTO>(Arg.Any<Question>())
                .Returns(callinfo => implMapper.Map<QuestionDetailDTO>(callinfo.Arg<Question>()));
             this.mapper.Map<QuestionBreadcrumbDTO>(Arg.Any<QuestionBreadcrumb>())
                .Returns(callinfo => implMapper.Map<QuestionBreadcrumbDTO>(callinfo.Arg<QuestionBreadcrumb>()));
             this.mapper.Map<ICollection<QuestionBreadcrumbDTO>>(Arg.Any<ICollection<QuestionBreadcrumb>>())
                .Returns(callinfo => implMapper.Map<ICollection<QuestionBreadcrumbDTO>>(callinfo.Arg<ICollection<QuestionBreadcrumb>>()));
             this.mapper.Map<Question>(Arg.Any<UpdateQuestionDTO>())
                .Returns(callinfo => implMapper.Map<Question>(callinfo.Arg<UpdateQuestionDTO>()));
             this.mapper.ConfigurationProvider.AssertConfigurationIsValid();

            this.breadCrumbService = Substitute.ForPartsOf<BreadCrumbService>(mapper);

            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod]
        public async Task ListFirstQuestionFromDb()
        {
            var existingWizard = GetWizardMock();
            foreach (var item in existingWizard.Questions)
            {
                item.IsFirstQuestion = true;
            }
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(existingWizard.Questions.AsQueryable());

            var isFirstQuestion = true;
            var questionList = await bll.ListQuestionAsync(existingWizard.Id, isFirstQuestion);
            foreach (var quest in questionList)
            {
                Assert.AreEqual(1, quest.Id);
                Assert.AreEqual(1, quest.WizardId);
                Assert.AreEqual(true, quest.IsActive);
                Assert.AreEqual(true, quest.IsFirstQuestion);
                Assert.AreEqual(0, quest.Labels.Count());
                Assert.AreEqual(0, quest.AnswersCount);
            }
            Assert.AreEqual(1, questionList.Count());
            wizardRepository.Received().Find(Arg.Any<Expression<Func<Wizard, bool>>>());
            questionRepository.Received().Find(Arg.Any<Expression<Func<Question, bool>>>());
            mapper.Received().Map<List<QuestionDTO>>(Arg.Any<List<Question>>());
        }

        [TestMethod]
        public async Task ListDontHaveFirstQuestionFromDb_Case2()
        {
            var existingWizard = GetWizardMock();
            foreach (var item in existingWizard.Questions)
            {
                item.IsFirstQuestion = true;
            }
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Question, bool>>>();
                var items = existingWizard.Questions.Where(exp.Compile()).AsQueryable();
                return items;
            });

            var isFirstQuestion = true;
            var questionList = await bll.ListQuestionAsync(existingWizard.Id, isFirstQuestion);
            Assert.AreEqual(1, questionList.Count());
            wizardRepository.Received().Find(Arg.Any<Expression<Func<Wizard, bool>>>());
            questionRepository.Received().Find(Arg.Any<Expression<Func<Question, bool>>>());
            mapper.Received().Map<List<QuestionDTO>>(Arg.Any<List<Question>>());
        }

        [TestMethod]
        public async Task ListDontHaveFirstQuestionFromDb()
        {
            var existingWizard = GetWizardMock();
            foreach (var item in existingWizard.Questions)
            {
                item.IsFirstQuestion = false;
            }
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Question, bool>>>();
                return existingWizard.Questions.Where(exp.Compile()).AsQueryable();
            });

            var isFirstQuestion = true;
            var questionList = await bll.ListQuestionAsync(existingWizard.Id, isFirstQuestion);
            Assert.AreEqual(0, questionList.Count());
            Assert.AreEqual(0, questionList.Sum(n => n.AnswersCount));

            wizardRepository.Received().Find(Arg.Any<Expression<Func<Wizard, bool>>>());
            questionRepository.Received().Find(Arg.Any<Expression<Func<Question, bool>>>());
            mapper.Received().Map<List<QuestionDTO>>(Arg.Any<List<Question>>());
        }
        [TestMethod]
        public async Task ListQuestionFromDb()
        {
            var existingWizard = GetWizardMockWithAnswer();
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Question, bool>>>();
                return existingWizard.Questions.Where(exp.Compile()).AsQueryable();
            });

            var questionList = await bll.ListQuestionAsync(existingWizard.Id);
            foreach (var quest in questionList)
            {
                Assert.AreEqual(1, quest.Id);
                Assert.AreEqual(1, quest.WizardId);
                Assert.AreEqual(true, quest.IsActive);
                Assert.AreEqual(true, quest.IsFirstQuestion);
                Assert.AreEqual(0, quest.Labels.Count());
                Assert.AreEqual(1, quest.AnswersCount);
            }
            Assert.AreEqual(1, questionList.Count());
            Assert.AreEqual(1, questionList.Sum(n => n.AnswersCount));
            wizardRepository.Received().Find(Arg.Any<Expression<Func<Wizard, bool>>>());
            questionRepository.Received().Find(Arg.Any<Expression<Func<Question, bool>>>());
            mapper.Received().Map<List<QuestionDTO>>(Arg.Any<List<Question>>());
        }

        [TestMethod]
        public async Task ListDontHaveAnswerQuestionFromDb()
        {
            var existingWizard = GetWizardMockWithAnswer();
            foreach (var question in existingWizard.Questions)
            {
                question.Answers = new List<Answer>();
            }
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Question, bool>>>();
                return existingWizard.Questions.Where(exp.Compile()).AsQueryable();
            });

            var questionList = await bll.ListQuestionAsync(existingWizard.Id);
            foreach (var quest in questionList)
            {
                Assert.AreEqual(1, quest.Id);
                Assert.AreEqual(1, quest.WizardId);
                Assert.AreEqual(true, quest.IsActive);
                Assert.AreEqual(true, quest.IsFirstQuestion);
                Assert.AreEqual(0, quest.Labels.Count());
                Assert.AreEqual(0, quest.AnswersCount);
            }
            Assert.AreEqual(1, questionList.Count());
            Assert.AreEqual(0, questionList.Sum(n => n.AnswersCount));
            wizardRepository.Received().Find(Arg.Any<Expression<Func<Wizard, bool>>>());
            questionRepository.Received().Find(Arg.Any<Expression<Func<Question, bool>>>());
            mapper.Received().Map<List<QuestionDTO>>(Arg.Any<List<Question>>());
        }

        [TestMethod]
        public async Task ListQuestionFromDb_Deleted()
        {
            var existingWizard = GetWizardMock();
            foreach (var item in existingWizard.Questions)
            {
                item.DeletedTime = DateTime.Now;
            }
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Question, bool>>>();
                return existingWizard.Questions.Where(exp.Compile()).AsQueryable();
            });

            var questionList = await bll.ListQuestionAsync(2);
            Assert.AreEqual(0, questionList.Count());
            wizardRepository.Received().Find(Arg.Any<Expression<Func<Wizard, bool>>>());
            questionRepository.Received().Find(Arg.Any<Expression<Func<Question, bool>>>());
            mapper.Received().Map<List<QuestionDTO>>(Arg.Any<List<Question>>());
        }

        [TestMethod]
        public async Task ListQuestionFromDbButFirstWizardEntityIs0()
        {
            var existingWizard = GetWizardMock();
            var query = new List<Wizard> { existingWizard }.AsQueryable();
            var wizard = wizardRepository.GetByIdAsync(existingWizard.Id).ReturnsNull();
            var repository = Substitute.For<IGenericRepository<Wizard>>();
            repository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).
                ReturnsNull();
            wizardRepository.Add(existingWizard);
            unitOfWork.SaveChangesAsync().Returns(1);
            mapper.Map<List<QuestionDetailDTO>>(existingWizard.Questions);
            var questionList = await bll.ListQuestionAsync(existingWizard.Id);
            Assert.AreEqual(0, questionList.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ListQuestionFromDbButSaveFailed()
        {
            var existingWizard = GetWizardMock();
            var query = new List<Wizard> { existingWizard }.AsQueryable();
            var wizard = wizardRepository.GetByIdAsync(existingWizard.Id).ReturnsNull();
            var repository = Substitute.For<IGenericRepository<Wizard>>();
            repository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).
                ReturnsNull();
            wizardRepository.Add(existingWizard);
            var countSave = unitOfWork.SaveChangesAsync().Returns(0);
            var questionMap = mapper.Map<List<QuestionDetailDTO>>(existingWizard.Questions);
            var questionList = await bll.ListQuestionAsync(existingWizard.Id);
        }

        [TestMethod]
        public async Task GetListQuestion()
        {
            var existingWizard = GetWizardMockWithAnswer();
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Question, bool>>>();
                return existingWizard.Questions.Where(exp.Compile()).AsQueryable();
            });

            var questionList = await bll.ListQuestionAsync(existingWizard.Id);
            foreach (var quest in questionList)
            {
                Assert.AreEqual(1, quest.Id);
                Assert.AreEqual(1, quest.WizardId);
                Assert.AreEqual(true, quest.IsActive);
                Assert.AreEqual(true, quest.IsFirstQuestion);
                Assert.AreEqual(0, quest.Labels.Count());
                Assert.AreEqual(1, quest.AnswersCount);
            }
        }

        [TestMethod]
        public async Task CreateQuestionAsync_WhenWizardExist_ShouldCreateAndReturnQuestion()
        {
            int wizardId = 1;
            int questionId = 1;
            var existingWizard = GetWizardMock();

            var createQuestionDTO = new DTO.Internal.Wizard.CreateQuestionDTO
            {
                WizardId = wizardId,
                IsFirstQuestion = existingWizard.Questions.FirstOrDefault(x => x.Id == questionId).IsFirstQuestion,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> { },
                Texts = new List<DTO.Internal.Translation.TranslationDTO>(),
            };
            var wizards = wizardRepository.GetByIdAsync(wizardId).Returns(existingWizard);
            var countSave = unitOfWork.SaveChangesAsync().Returns(1);
            var createQuestions = mapper.Map<Question>(createQuestionDTO);
            var createQuestion = await bll.CreateQuestionAsync(questionId, createQuestionDTO);
            Assert.AreEqual(wizardId, createQuestion.WizardId);
            Assert.AreEqual(true, createQuestion.IsActive);
            Assert.AreEqual(true, createQuestion.IsFirstQuestion);
            Assert.AreEqual(0, createQuestion.Labels.Count());
            Assert.AreEqual(0, createQuestion.Texts.Count());
            Assert.AreEqual(0, createQuestion.AnswersCount);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CreateQuestionAsync_WhenWizardExist_FailedSave()
        {
            int wizardId = 1;
            int questionId = 1;
            var existingWizard = GetWizardMock();

            var createQuestionDTO = new DTO.Internal.Wizard.CreateQuestionDTO
            {
                WizardId = wizardId,
                IsFirstQuestion = existingWizard.Questions.FirstOrDefault(x => x.Id == questionId).IsFirstQuestion,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> { },
                Texts = new List<DTO.Internal.Translation.TranslationDTO>(),
            };
            var wizards = wizardRepository.GetByIdAsync(wizardId).Returns(existingWizard);
            var countSave = unitOfWork.SaveChangesAsync().Returns(0);
            var createQuestion = await bll.CreateQuestionAsync(questionId, createQuestionDTO);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateQuestionTestFoundArgumentException()
        {
            var existingWizard = new Wizard
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
            };
            var wizards = wizardRepository.GetByIdAsync(1).Returns(existingWizard);
            var createQuestion = await bll.CreateQuestionAsync(1, null);
        }

        [TestMethod]
        public async Task UpdateQuestionAsync_WhenWizardExist_ShouldUpdateAndReturnQuestion()
        {
            int wizardId = 1;
            int questionId = 1;
            var existingWizard = GetWizardMock();
            var existingQUestion = existingWizard.Questions.FirstOrDefault();
            var updateQuestionDTO = new DTO.Internal.Wizard.UpdateQuestionDTO
            {
                Id = 1,
                IsActive = true,
                IsFirstQuestion = existingWizard.Questions.FirstOrDefault(x => x.Id == questionId)?.IsFirstQuestion ?? false,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> { },
                Texts = new List<DTO.Internal.Translation.TranslationDTO>(),
            };
            Question question = new Question
            {
                Answers = new List<Answer> { },
                Breadcrumbs = new List<QuestionBreadcrumb> { },
                BreadcrumbsItems = new List<QuestionBreadcrumbItem> { },
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = DateTime.Now,
                DeleterUserId = 1,
                Id = 1,
                IsActive = true,
                IsFirstQuestion = true,
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
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
                WizardId = 1
            };

            var wizards = wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());
            var questions = questionRepository.Find(default).ReturnsForAnyArgs((new[] { existingQUestion }).AsQueryable());
            var updateQuestionMap = mapper.Map(updateQuestionDTO, question);
            unitOfWork.SaveChangesAsync().Returns(1);
            var updateQuestion = await bll.UpdateQuestionAsync(wizardId, questionId, updateQuestionDTO);

            Assert.AreEqual(questionId, updateQuestion.Id);
            Assert.AreEqual(wizardId, updateQuestion.WizardId);
            Assert.AreEqual(0, updateQuestion.Labels.Count());
            Assert.AreEqual(0, updateQuestion.Texts.Count());
            Assert.AreEqual(0, updateQuestion.AnswersCount);

        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdateQuestionAsync_WhenWizardExist_QuestionIDNotFound()
        {
            int wizardId = 1;
            int questionId = 1;
            var existingWizard = GetWizardMock();
            var updateQuestionDTO = new DTO.Internal.Wizard.UpdateQuestionDTO
            {
                IsActive = true,
                IsFirstQuestion = existingWizard.Questions.FirstOrDefault(x => x.Id == questionId).IsFirstQuestion,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> { },
                Texts = new List<DTO.Internal.Translation.TranslationDTO>(),
            };
            var wizards = wizardRepository.GetByIdAsync(wizardId).Returns(existingWizard);
            var countSave = unitOfWork.SaveChangesAsync().Returns(1);
            var updateQuestion = await bll.UpdateQuestionAsync(wizardId, 2, updateQuestionDTO);
        }

        //this not required anymore since change on bll
        /*[TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task UpdateQuestionAsync_WhenWizardExist_FailedSaveUpdate()
        {
            int wizardId = 1;
            int questionId = 1;
            var existingWizard = GetWizardMock();
            var updateQuestionDTO = new DTO.Internal.Wizard.UpdateQuestionDTO
            {
                IsActive = true,
                IsFirstQuestion = existingWizard.Questions.FirstOrDefault(x => x.Id == questionId).IsFirstQuestion,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> { },
                Texts = new List<DTO.Internal.Translation.TranslationDTO>(),
            };
            var wizards = wizardRepository.GetByIdAsync(wizardId).Returns(existingWizard);
            var countSave = unitOfWork.SaveChangesAsync().Returns(0);
            var updateQuestion = await bll.UpdateQuestionAsync(wizardId, questionId, updateQuestionDTO);
        }*/

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateQuestionTestFoundArgumentException()
        {
            int wizardId = 1;
            int questionId = 1;
            var existingWizard = GetWizardMock();
            var existingQuestion = existingWizard.Questions.FirstOrDefault();
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).Returns(new List<Wizard>() { existingWizard }.AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(new List<Question>() { existingQuestion }.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);
            await bll.UpdateQuestionAsync(wizardId, questionId, null);
            var wizards = wizardRepository.Find(default).ReturnsForAnyArgs((new[] { existingWizard }).AsQueryable());

        }
        private Wizard GetSecondWizardMock()
        {
            var wizard = GetWizardMock();
            foreach (var question in wizard.Questions)
            {
                question.IsFirstQuestion = false;
            }
            return wizard;
        }
        private Wizard GetWizardMockWithAnswer()
        {
            var result = GetWizardMock();
            foreach (var question in result.Questions)
            {
                question.Answers.Add(new Answer()
                {
                    Id = 1
                });
            }
            return result;
        }
        private Wizard GetWizardMock()
        {
            Question question = new Question
            {
                Answers = new List<Answer> { },
                Breadcrumbs = new List<QuestionBreadcrumb> { },
                BreadcrumbsItems = new List<QuestionBreadcrumbItem> { },
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                Id = 1,
                IsActive = true,
                IsFirstQuestion = true,
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
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
                WizardId = 1
            };
            return new Wizard
            {
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = DateTime.Now,
                DeleterUserId = 1,
                Id = 1,
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                IntroductionTexts = new List<Translation>(),
                Questions = new List<Question>() { question },
                Titles = new List<Translation>(),
            };
        }

        [TestMethod()]
        public async Task DeleteQuestionAsyncTest()
        {
            Wizard wizard = new Wizard()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                IntroductionTexts = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1
                    }
                }
            };

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            await bll.DeleteQuestionAsync(1, 1);

            wizardRepository.ReceivedWithAnyArgs(1).Update(Arg.Any<Wizard>());
            unitOfWork.Received(1).SaveChangesAsync();
            Assert.AreEqual(0, wizard.Questions.Count);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task DeleteQuestionAsyncTestRetrunException()
        {
            Wizard wizard = new Wizard()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                IntroductionTexts = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1
                    }
                }
            };

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(0);
            await bll.DeleteQuestionAsync(1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteQuestionAsyncTestQuestionNotFound()
        {
            Wizard wizard = new Wizard()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                IntroductionTexts = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            await bll.DeleteQuestionAsync(1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteQuestionAsyncTestWizardNotFound()
        {
            await bll.DeleteQuestionAsync(1, 1);
        }

        [TestMethod()]
        public async Task GetQuestionAsync()
        {
            Question question = new Question()
            {
                Id = 1
            };
            Wizard wizard = new Wizard()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                IntroductionTexts = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Questions = new List<Question>() { question }
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            var result = await bll.GetQuestionAsync(1, 1);
            wizardRepository.ReceivedWithAnyArgs(1).Find(Arg.Any<Expression<Func<Wizard, bool>>>());
            Assert.AreEqual(result.Id, question.Id);
        }

        //this not required anymore since change on bll
        /*[TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task UpdateQuestionAsync_WhenQuestionExist_FailedSaveUpdate()
        {
            int wizardId = 1;
            int questionId = 1;
            var existingWizard = GetWizardMock();
            var updateQuestionDTO = new DTO.Internal.Wizard.UpdateQuestionDTO
            {
                IsActive = true,
                IsFirstQuestion = existingWizard.Questions.FirstOrDefault(x => x.Id == questionId).IsFirstQuestion,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> { },
                Texts = new List<DTO.Internal.Translation.TranslationDTO>(),
            };
            var wizards = wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard> { existingWizard }.AsQueryable());
            var countSave = unitOfWork.SaveChangesAsync().Returns(0);
            var updateQuestion = await bll.UpdateQuestionAsync(wizardId, questionId, updateQuestionDTO);
        }*/

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetQuestionAsync_WhenQuestionNotExist()
        {
            Wizard wizard = new Wizard()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                IntroductionTexts = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Questions = new List<Question>()
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            var result = await bll.GetQuestionAsync(1, 1);
        }

        [TestMethod]
        public async Task GetBreadcrumbAsyncTest()
        {
            var existingWizard = GetSecondWizardMock();
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>()
            {
                existingWizard
            }.AsQueryable());
            questionRepository.Find(default).ReturnsForAnyArgs(existingWizard.Questions.AsQueryable());
            var breadCrumpList = await bll.GetBreadcrumbAsync(existingWizard.Id, 1);
            Assert.IsNotNull(breadCrumpList);
            Assert.AreEqual(1, breadCrumpList.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetBreadcrumbAsyncTest_WhenQuestionNotFound()
        {
            var existingWizard = GetWizardMock();
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            var breadCrumpList = await bll.GetBreadcrumbAsync(existingWizard.Id, 2);
        }

        [TestMethod]
        public async Task GetBreadcrumbAsyncTest_WhenQuestionIsNotFirstQuestion_And_BreadcrumpFieldInQuestionNotNull()
        {
            var existingWizard = GetWizardMock();
            Question question = new Question
            {
                Id = 2,
                Answers = new List<Answer> { },
                Breadcrumbs = new List<QuestionBreadcrumb> { new QuestionBreadcrumb() { Id = 1 } },
                BreadcrumbsItems = new List<QuestionBreadcrumbItem> { },
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = DateTime.Now,
                DeleterUserId = 1,
                IsActive = true,
                IsFirstQuestion = false,
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Texts = new List<Translation> { },
                Titles = new List<Translation> { },
                WizardId = 1
            };
            existingWizard.Questions.Add(question);
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            var breadCrumpList = await bll.GetBreadcrumbAsync(existingWizard.Id, 2);
            Assert.IsNotNull(breadCrumpList);
            Assert.AreEqual(1, breadCrumpList.Count);
        }

        [TestMethod]
        public async Task GetBreadcrumbAsyncTest_WhenQuestionIsNotFirstQuestion_And_BreadcrumpFieldInQuestionIsNullOrEmpty()
        {
            var existingWizard = GetWizardMock();
            Question question = new Question
            {
                Id = 2,
                Answers = new List<Answer> { },
                Breadcrumbs = new List<QuestionBreadcrumb>(),
                BreadcrumbsItems = new List<QuestionBreadcrumbItem>(),
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = DateTime.Now,
                DeleterUserId = 1,
                IsActive = true,
                IsFirstQuestion = false,
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Texts = new List<Translation> { },
                Titles = new List<Translation> { },
                WizardId = 1
            };
            existingWizard.Questions.Add(question);
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            breadCrumbService.ProcessBreadcrumbs(unitOfWork, Arg.Any<Question>(), Arg.Any<HashSet<int>>()).ReturnsForAnyArgs(new List<QuestionBreadcrumbDTO>() { new QuestionBreadcrumbDTO() });
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            var breadCrumpList = await bll.GetBreadcrumbAsync(existingWizard.Id, 2);
            Assert.IsNotNull(breadCrumpList);

        }
        [TestMethod()]
        public async Task GetBreadcrumbExisting()
        {
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { new Wizard{
                Id = 1,
                Questions = new [] { new Question {
                    Id = 1,
                    Breadcrumbs = new [] { new QuestionBreadcrumb {
                        Id = 1,
                        QuestionId = 1,
                        Items = new [] { new QuestionBreadcrumbItem {
                            Id = 1,
                            BreadcrumbId = 1,
                            IsALoop = false,
                            QuestionId = 2,
                            Question = new Question {
                                Id = 2,
                                IsFirstQuestion = true,
                                Titles = new []
                                {
                                    new Translation{ Id = 1, LanguageCode = "en", Text = "test"}
                                }
                            }
                        } }
                    } }
                } }
            } }).AsQueryable());

            var result = await bll.GetBreadcrumbAsync(1, 1).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().Items.Count());
            Assert.AreEqual(2, result.First().Items.First().QuestionId);
            Assert.AreEqual("test", result.First().Items.First().Labels.FirstOrDefault(l => l.LanguageCode == "en").Text);
        }

        [TestMethod()]
        public async Task GetBreadcrumbFirstQuestion()
        {
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { new Wizard{
                Id = 1,
                Questions = new [] { new Question {
                    Id = 1,
                    IsFirstQuestion = true
                } }
            } }).AsQueryable());

            var result = await bll.GetBreadcrumbAsync(1, 1).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public async Task GetBreadcrumbProcessSimple()
        {
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { new Wizard{
                Id = 1,
                Questions = new []
                {
                    new Question {
                        Id = 4,
                        IsFirstQuestion = false
                    }
                }
            } }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 4))))
                .Returns((new[]
                {
                    new Question {
                        Id = 1,
                        IsFirstQuestion = true,
                        Titles = new [] {
                            new Translation {
                                Id = 1,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    }
                }).AsQueryable());

            var result = await bll.GetBreadcrumbAsync(1, 4).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().Items.Count());
            Assert.AreEqual(1, result.First().Items.First().QuestionId);
            Assert.AreEqual("question 1", result.First().Items.First().Labels.FirstOrDefault(l => l.LanguageCode == "en").Text);
        }

        [TestMethod()]
        public async Task GetBreadcrumbProcessMultiple()
        {
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { new Wizard{
                Id = 1,
                Questions = new []
                {
                    new Question {
                        Id = 4,
                        IsFirstQuestion = false
                    }
                }
            } }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 4))))
                .Returns((new[]
                {
                    new Question {
                        Id = 1,
                        IsFirstQuestion = true,
                        Titles = new [] {
                            new Translation {
                                Id = 1,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    },
                    new Question {
                        Id = 2,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 2,
                                LanguageCode = "en",
                                Text = "question 2"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 2))))
                .Returns((new[]
                {
                    new Question {
                        Id = 1,
                        IsFirstQuestion = true,
                        Titles = new [] {
                            new Translation {
                                Id = 1,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    }
                }).AsQueryable());

            var result = await bll.GetBreadcrumbAsync(1, 4).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(1, result.ToArray()[0].Items.Count());
            Assert.AreEqual(2, result.ToArray()[1].Items.Count());

            Assert.AreEqual(1, result.ToArray()[0].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(false, result.ToArray()[0].Items.ToArray()[0].IsALoop);

            Assert.AreEqual(1, result.ToArray()[1].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(false, result.ToArray()[1].Items.ToArray()[0].IsALoop);
            Assert.AreEqual(2, result.ToArray()[1].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(false, result.ToArray()[1].Items.ToArray()[0].IsALoop);
        }

        [TestMethod()]
        public async Task GetBreadcrumbProcessMultiple_LinkedToOtherRootQuestion()
        {
            var wizard = MockWizardWithQuestionsForBreadcrumbWithNotFirstQuestion();

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());

            ArrangeQuestion();

            var result = await bll.GetBreadcrumbAsync(1, 10).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());

            Assert.AreEqual(3, result.ToArray()[0].Items.Count());

            Assert.AreEqual(7, result.ToArray()[0].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(8, result.ToArray()[0].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(9, result.ToArray()[0].Items.ToArray()[2].QuestionId);
        }

        [TestMethod()]
        public async Task GetBreadcrumbProcessMultiple_ReturnsMultipleRoot()
        {
            var wizard = MockWizardWithQuestionsForBreadcrumbWithNotFirstQuestion();

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());

            ArrangeQuestion();

            var result = await bll.GetBreadcrumbAsync(1, 6).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(5, result.ToArray()[0].Items.Count());
            Assert.AreEqual(3, result.ToArray()[1].Items.Count());

            Assert.AreEqual(7, result.ToArray()[0].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(8, result.ToArray()[0].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(9, result.ToArray()[0].Items.ToArray()[2].QuestionId);
            Assert.AreEqual(10, result.ToArray()[0].Items.ToArray()[3].QuestionId);
            Assert.AreEqual(5, result.ToArray()[0].Items.ToArray()[4].QuestionId);

            Assert.AreEqual(13, result.ToArray()[1].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(14, result.ToArray()[1].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(5, result.ToArray()[1].Items.ToArray()[2].QuestionId);
        }
        
        [TestMethod()]
        public async Task GetBreadcrumbProcessMultiple_ReturnsMultipleRoot_FirstRowOfQuestionDontHaveFirstQuestion()
        {
            var wizard = MockWizardWithQuestionsForBreadcrumbWithNotFirstQuestion();

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());

            ArrangeQuestion();

            var result = await bll.GetBreadcrumbAsync(1, 5).ConfigureAwait(false);

            //Assert.IsNull(result);
            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(4, result.ToArray()[0].Items.Count());
            Assert.AreEqual(2, result.ToArray()[1].Items.Count());

            Assert.AreEqual(7, result.ToArray()[0].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(8, result.ToArray()[0].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(9, result.ToArray()[0].Items.ToArray()[2].QuestionId);
            Assert.AreEqual(10, result.ToArray()[0].Items.ToArray()[3].QuestionId);

            Assert.AreEqual(13, result.ToArray()[1].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(14, result.ToArray()[1].Items.ToArray()[1].QuestionId);
        }

        [TestMethod()]
        public async Task GetBreadcrumbProcessLoop()
        {
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { new Wizard{
                Id = 1,
                Questions = new []
                {
                    new Question {
                        Id = 4,
                        IsFirstQuestion = false
                    }
                }
            } }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 4))))
                .Returns((new[]
                {
                    new Question {
                        Id = 1,
                        IsFirstQuestion = true,
                        Titles = new [] {
                            new Translation {
                                Id = 1,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    },
                    new Question {
                        Id = 2,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 2,
                                LanguageCode = "en",
                                Text = "question 2"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 2))))
                .Returns((new[]
                {
                    new Question {
                        Id = 1,
                        IsFirstQuestion = true,
                        Titles = new [] {
                            new Translation {
                                Id = 1,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    },
                    new Question {
                        Id = 4,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 1,
                                LanguageCode = "en",
                                Text = "question 4"
                            }
                        }
                    }
                }).AsQueryable());

            var result = await bll.GetBreadcrumbAsync(1, 4).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, result.ToArray()[0].Items.Count());
            Assert.AreEqual(2, result.ToArray()[1].Items.Count());
            Assert.AreEqual(2, result.ToArray()[2].Items.Count());

            Assert.AreEqual(1, result.ToArray()[0].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(false, result.ToArray()[0].Items.ToArray()[0].IsALoop);

            Assert.AreEqual(1, result.ToArray()[1].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(false, result.ToArray()[1].Items.ToArray()[0].IsALoop);
            Assert.AreEqual(2, result.ToArray()[1].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(false, result.ToArray()[1].Items.ToArray()[1].IsALoop);

            Assert.AreEqual(4, result.ToArray()[2].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(true, result.ToArray()[2].Items.ToArray()[0].IsALoop);
            Assert.AreEqual(2, result.ToArray()[2].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(false, result.ToArray()[2].Items.ToArray()[1].IsALoop);

        }

        private Wizard MockWizardWithQuestionsForBreadcrumbWithNotFirstQuestion()
        {
            var firstQuestionList = new List<Question>
            {
                new Question {
                    Id = 1,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 1, QuestionId = 1, LinkedQuestionId = 2, Order = 1 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 1,
                            LanguageCode = "en",
                            Text = "question 1"
                        }
                    }
                },
                new Question {
                    Id = 2,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 2, QuestionId = 2, LinkedQuestionId = 1, Order = 2 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 2,
                            LanguageCode = "en",
                            Text = "question 2"
                        }
                    }
                },
                new Question {
                    Id = 3,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 3, QuestionId = 3, LinkedQuestionId = 4, Order = 3 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 3,
                            LanguageCode = "en",
                            Text = "question 3"
                        }
                    }
                },
                new Question {
                    Id = 4,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 4, QuestionId = 4, LinkedQuestionId = 5, Order = 4 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 5,
                            LanguageCode = "en",
                            Text = "question 4"
                        }
                    }
                },
                new Question {
                    Id = 5,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 5, QuestionId = 5, LinkedQuestionId = 6, Order = 5 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 5,
                            LanguageCode = "en",
                            Text = "question 5"
                        }
                    }
                },
                new Question {
                    Id = 6,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 6, QuestionId = 6, LinkedQuestionId = 7, Order = 6 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 6,
                            LanguageCode = "en",
                            Text = "question 6"
                        }
                    }
                }
            };
            var secondQuestionList = new List<Question>
            {
                new Question {
                    Id = 7,
                    IsFirstQuestion = true,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 7, QuestionId = 7, LinkedQuestionId = 8, Order = 7 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 7,
                            LanguageCode = "en",
                            Text = "question 7"
                        }
                    }
                },
                new Question {
                    Id = 8,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 8, QuestionId = 8, LinkedQuestionId = 9, Order = 8 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 8,
                            LanguageCode = "en",
                            Text = "question 8"
                        }
                    }
                },
                new Question {
                    Id = 8,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 8, QuestionId = 8, LinkedQuestionId = 9, Order = 8 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 8,
                            LanguageCode = "en",
                            Text = "question 8"
                        }
                    }
                },
                new Question {
                    Id = 9,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 9, QuestionId = 9, LinkedQuestionId = 10, Order = 9 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 9,
                            LanguageCode = "en",
                            Text = "question 9"
                        }
                    }
                },
                new Question {
                    Id = 10,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 10, QuestionId = 10, LinkedQuestionId = 11, Order = 10 },
                        new Answer { Id = 13, QuestionId = 10, LinkedQuestionId = 5, Order = 13 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 10,
                            LanguageCode = "en",
                            Text = "question 10"
                        }
                    }
                },
                new Question {
                    Id = 11,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 11, QuestionId = 11, LinkedQuestionId = 12, Order = 11 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 11,
                            LanguageCode = "en",
                            Text = "question 11"
                        }
                    }
                },
                new Question {
                    Id = 12,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 12, QuestionId = 12, LinkedQuestionId = null, Order = 12 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 12,
                            LanguageCode = "en",
                            Text = "question 12"
                        }
                    }
                }
            };
            var thirdQuestionList = new List<Question>
            {
                new Question {
                    Id = 13,
                    IsFirstQuestion = true,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 20, QuestionId = 13, LinkedQuestionId = 14, Order = 13 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 20,
                            LanguageCode = "en",
                            Text = "question 13"
                        }
                    }
                },
                new Question {
                    Id = 14,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 21, QuestionId = 14, LinkedQuestionId = 5, Order = 14 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 21,
                            LanguageCode = "en",
                            Text = "question 21"
                        }
                    }
                }
            };
            var questionList = new List<Question>();
            questionList.AddRange(firstQuestionList);
            questionList.AddRange(secondQuestionList);
            questionList.AddRange(thirdQuestionList);
            var wizard = new Wizard
            {
                Id = 1,
                Questions = questionList
            };

            return wizard;
        }

        private void ArrangeQuestion()
        {
            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 2))))
                .Returns((new[]
                {
                    new Question {
                        Id = 1,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 1,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    }
                        }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 3))))
                .Returns((new[]
                {
                    new Question {
                        Id = 2,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 2,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 4))))
                .Returns((new[]
                {
                    new Question {
                        Id = 3,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 3,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 5))))
                .Returns((new[]
                {
                    new Question {
                        Id = 4,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 4,
                                LanguageCode = "en",
                                Text = "question 4"
                            }
                        }
                    },
                    new Question {
                        Id = 10,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 10,
                                LanguageCode = "en",
                                Text = "question 10"
                            }
                        }
                    },
                    new Question {
                        Id = 14,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 14,
                                LanguageCode = "en",
                                Text = "question 14"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 6))))
                .Returns((new[]
                {
                    new Question {
                        Id = 5,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 5,
                                LanguageCode = "en",
                                Text = "question 5"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 8))))
                .Returns((new[]
                {
                    new Question {
                        Id = 7,
                        IsFirstQuestion = true,
                        Titles = new [] {
                            new Translation {
                                Id = 7,
                                LanguageCode = "en",
                                Text = "question 7"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 9))))
                .Returns((new[]
                {
                    new Question {
                        Id = 8,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 8,
                                LanguageCode = "en",
                                Text = "question 8"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 10))))
                .Returns((new[]
                {
                    new Question {
                        Id = 9,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 9,
                                LanguageCode = "en",
                                Text = "question 9"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 11))))
                .Returns((new[]
                {
                    new Question {
                        Id = 10,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 10,
                                LanguageCode = "en",
                                Text = "question 10"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 12))))
                .Returns((new[]
                {
                    new Question {
                        Id = 11,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 11,
                                LanguageCode = "en",
                                Text = "question 11"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 14))))
                .Returns((new[]
                {
                    new Question {
                        Id = 13,
                        IsFirstQuestion = true,
                        Titles = new [] {
                            new Translation {
                                Id = 13,
                                LanguageCode = "en",
                                Text = "question 13"
                            }
                        }
                    }
        }).AsQueryable());
        }

        private Wizard MockWizardWithQuestionsForBreadcrumb_ReturnsMultipleRoot_SecondRowOfQuestionDontHaveFirstQuestion_LinkedFirstToSecondQuestionRow()
        {
            var firstQuestionList = new List<Question>
            {
                new Question {
                    Id = 1,
                    IsFirstQuestion = true,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 1, QuestionId = 1, LinkedQuestionId = 2, Order = 1 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 1,
                            LanguageCode = "en",
                            Text = "question 1"
                        }
                    }
                },
                new Question {
                    Id = 2,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 2, QuestionId = 2, LinkedQuestionId = 1, Order = 2 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 2,
                            LanguageCode = "en",
                            Text = "question 2"
                        }
                    }
                },
                new Question {
                    Id = 3,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 3, QuestionId = 3, LinkedQuestionId = 4, Order = 3 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 3,
                            LanguageCode = "en",
                            Text = "question 3"
                        }
                    }
                },
                new Question {
                    Id = 4,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 4, QuestionId = 4, LinkedQuestionId = 5, Order = 4 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 5,
                            LanguageCode = "en",
                            Text = "question 4"
                        }
                    }
                },
                new Question {
                    Id = 5,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 5, QuestionId = 5, LinkedQuestionId = 6, Order = 5 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 5,
                            LanguageCode = "en",
                            Text = "question 5"
                        }
                    }
                },
                new Question {
                    Id = 6,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 6, QuestionId = 6, LinkedQuestionId = 7, Order = 6 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 6,
                            LanguageCode = "en",
                            Text = "question 6"
                        }
                    }
                }
            };
            var secondQuestionList = new List<Question>
            {
                new Question {
                    Id = 7,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 7, QuestionId = 7, LinkedQuestionId = 8, Order = 7 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 7,
                            LanguageCode = "en",
                            Text = "question 7"
                        }
                    }
                },
                new Question {
                    Id = 8,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 8, QuestionId = 8, LinkedQuestionId = 9, Order = 8 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 8,
                            LanguageCode = "en",
                            Text = "question 8"
                        }
                    }
                },
                new Question {
                    Id = 8,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 8, QuestionId = 8, LinkedQuestionId = 9, Order = 8 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 8,
                            LanguageCode = "en",
                            Text = "question 8"
                        }
                    }
                },
                new Question {
                    Id = 9,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 9, QuestionId = 9, LinkedQuestionId = 10, Order = 9 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 9,
                            LanguageCode = "en",
                            Text = "question 9"
                        }
                    }
                },
                new Question {
                    Id = 10,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 10, QuestionId = 10, LinkedQuestionId = 11, Order = 10 },
                        new Answer { Id = 13, QuestionId = 10, LinkedQuestionId = 5, Order = 13 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 10,
                            LanguageCode = "en",
                            Text = "question 10"
                        }
                    }
                },
                new Question {
                    Id = 11,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 11, QuestionId = 11, LinkedQuestionId = 12, Order = 11 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 11,
                            LanguageCode = "en",
                            Text = "question 11"
                        }
                    }
                },
                new Question {
                    Id = 12,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 12, QuestionId = 12, LinkedQuestionId = null, Order = 12 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 12,
                            LanguageCode = "en",
                            Text = "question 12"
                        }
                    }
                }
            };
            var thirdQuestionList = new List<Question>
            {
                new Question {
                    Id = 13,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 20, QuestionId = 13, LinkedQuestionId = 14, Order = 13 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 20,
                            LanguageCode = "en",
                            Text = "question 13"
                        }
                    }
                },
                new Question {
                    Id = 14,
                    IsFirstQuestion = false,
                    Answers = new List<Answer>
                    {
                        new Answer { Id = 21, QuestionId = 14, LinkedQuestionId = 5, Order = 14 }
                    },
                    Titles = new [] {
                        new Translation {
                            Id = 21,
                            LanguageCode = "en",
                            Text = "question 21"
                        }
                    }
                }
            };
            var questionList = new List<Question>();
            questionList.AddRange(firstQuestionList);
            questionList.AddRange(secondQuestionList);
            questionList.AddRange(thirdQuestionList);
            var wizard = new Wizard
            {
                Id = 1,
                Questions = questionList
            };

            return wizard;
        }

        private void ArrangeQuestion_ReturnsMultipleRoot_SecondRowOfQuestionDontHaveFirstQuestion_LinkedFirstToSecondQuestionRow()
        {
            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 2))))
                .Returns((new[]
                {
                    new Question {
                        Id = 1,
                        IsFirstQuestion = true,
                        Titles = new [] {
                            new Translation {
                                Id = 1,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    }
                        }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 3))))
                .Returns((new[]
                {
                    new Question {
                        Id = 2,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 2,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 4))))
                .Returns((new[]
                {
                    new Question {
                        Id = 3,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 3,
                                LanguageCode = "en",
                                Text = "question 1"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 5))))
                .Returns((new[]
                {
                    new Question {
                        Id = 4,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 4,
                                LanguageCode = "en",
                                Text = "question 4"
                            }
                        }
                    },
                    new Question {
                        Id = 10,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 10,
                                LanguageCode = "en",
                                Text = "question 10"
                            }
                        }
                    },
                    new Question {
                        Id = 14,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 14,
                                LanguageCode = "en",
                                Text = "question 14"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 6))))
                .Returns((new[]
                {
                    new Question {
                        Id = 5,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 5,
                                LanguageCode = "en",
                                Text = "question 5"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 8))))
                .Returns((new[]
                {
                    new Question {
                        Id = 7,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 7,
                                LanguageCode = "en",
                                Text = "question 7"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 9))))
                .Returns((new[]
                {
                    new Question {
                        Id = 8,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 8,
                                LanguageCode = "en",
                                Text = "question 8"
                            }
                        }
                    },
                    new Question {
                        Id = 3,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 3,
                                LanguageCode = "en",
                                Text = "question 3"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 10))))
                .Returns((new[]
                {
                    new Question {
                        Id = 9,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 9,
                                LanguageCode = "en",
                                Text = "question 9"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 11))))
                .Returns((new[]
                {
                    new Question {
                        Id = 10,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 10,
                                LanguageCode = "en",
                                Text = "question 10"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 12))))
                .Returns((new[]
                {
                    new Question {
                        Id = 11,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 11,
                                LanguageCode = "en",
                                Text = "question 11"
                            }
                        }
                    }
                }).AsQueryable());

            questionRepository.Find(
                Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                        q => !q.DeletedTime.HasValue && q.Answers.Any(a => !a.DeletedTime.HasValue && a.LinkedQuestionId == 14))))
                .Returns((new[]
                {
                    new Question {
                        Id = 13,
                        IsFirstQuestion = false,
                        Titles = new [] {
                            new Translation {
                                Id = 13,
                                LanguageCode = "en",
                                Text = "question 13"
                            }
                        }
                    }
        }).AsQueryable());
        }
        
        [TestMethod()]
        public async Task GetBreadcrumbProcessMultiple_ReturnsMultipleRoot_SecondRowOfQuestionDontHaveFirstQuestion_LinkedFirstToSecondQuestionRow()
        {
            var wizard = MockWizardWithQuestionsForBreadcrumb_ReturnsMultipleRoot_SecondRowOfQuestionDontHaveFirstQuestion_LinkedFirstToSecondQuestionRow();

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());

            ArrangeQuestion_ReturnsMultipleRoot_SecondRowOfQuestionDontHaveFirstQuestion_LinkedFirstToSecondQuestionRow();

            var result = await bll.GetBreadcrumbAsync(1, 5).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(4, result.ToArray()[0].Items.Count());
            Assert.AreEqual(3, result.ToArray()[1].Items.Count());

            Assert.AreEqual(1, result.ToArray()[0].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(2, result.ToArray()[0].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(3, result.ToArray()[0].Items.ToArray()[2].QuestionId);
            Assert.AreEqual(4, result.ToArray()[0].Items.ToArray()[3].QuestionId);

            Assert.AreEqual(3, result.ToArray()[1].Items.ToArray()[0].QuestionId);
            Assert.AreEqual(9, result.ToArray()[1].Items.ToArray()[1].QuestionId);
            Assert.AreEqual(10, result.ToArray()[1].Items.ToArray()[2].QuestionId);
        }
    }
}

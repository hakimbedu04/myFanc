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
    public class MyFanc_BLL_Answer_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IAESEncryptService aESEncryptService;
        private IBll bll;

        private IBreadCrumbService breadCrumbService;
        private IEmailService emailService;
        private IFancRADApi fancRADApi;
        private ILogger<Bll> logger;
        private IMapper mapper;
        private IGenericRepository<Question> questionRepository;
        private ISharedDataCache sharedDataCache;
        private ITokenConfiguration tokenConfiguration;
        private IUnitOfWork unitOfWork;
        private IGenericRepository<User> userRepository;
        private IGenericRepository<Wizard> wizardRepository;
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
            this.aESEncryptService = Substitute.For<IAESEncryptService>();
            this.tokenConfiguration = Substitute.For<ITokenConfiguration>();
            this.emailService = Substitute.For<IEmailService>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
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

            this.mapper = new Mapper(mapperConfig);

            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();

            this.breadCrumbService = new BreadCrumbService(mapper);

            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }
        [TestMethod()]
        public async Task CreateAnswerAsyncTest()
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
                        Id = 1,
                        IsFirstQuestion = true
                    }
                }
            };

            var anwerDto = new CreateAnswerDTO()
            {
                Labels = new List<TranslationDTO>()
            };

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            questionRepository.Find(Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                         q => q.WizardId == 1 && q.Id == 1 && !q.DeletedTime.HasValue))
                ).Returns(wizard.Questions.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            var result = await bll.CreateAnswerAsync(1, 1, anwerDto);

            await unitOfWork.Received(1).SaveChangesAsync();
            Assert.AreEqual(1, result.Order);
            Assert.AreEqual(1, wizard.Questions.First().Answers.Count);
        }

        [TestMethod()]
        public async Task CreateAnswerIsFinalAswerAsyncTest()
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
                        Id = 1,
                        IsFirstQuestion = true,
                        Answers = new List<Answer>() {
                            new Answer()
                            {
                                Id= 1,
                                Order = 1,
                                IsFinalAnswer = true,
                                FinalAnswerTexts = new List<Translation>()
                                {
                                    new Translation()
                                    {
                                        Id = 1,
                                        LanguageCode = "en",
                                        Text = "Final Answer Old"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var anwerDto = new CreateAnswerDTO()
            {
                Labels = new List<TranslationDTO>(),
                IsFinalAnswer = true,
                FinalAnswerTexts = new List<TranslationDTO>()
                                {
                                    new TranslationDTO()
                                    {
                                        Id = 0,
                                        LanguageCode = "en",
                                        Text = "Final Answer Update"
                                    }
                                }
            };

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            questionRepository.Find(Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                         q => q.WizardId == 1 && q.Id == 1 && !q.DeletedTime.HasValue))
                ).Returns(wizard.Questions.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            var result = await bll.CreateAnswerAsync(1, 1, anwerDto);

            await unitOfWork.Received(1).SaveChangesAsync();
            Assert.AreEqual(2, result.Order);
            Assert.AreEqual(2, wizard.Questions.First().Answers.Count);
            Assert.AreEqual(true, wizard.Questions.First().Answers.ElementAt(0).IsFinalAnswer);
            Assert.AreEqual(1, wizard.Questions.First().Answers.ElementAt(0).FinalAnswerTexts.Count());
            Assert.AreEqual(true, wizard.Questions.First().Answers.ElementAt(1).IsFinalAnswer);
            Assert.AreEqual(1, wizard.Questions.First().Answers.ElementAt(1).FinalAnswerTexts.Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetAnswerAsyncTest_Failed_KeyNotFoundException()
        {
            try
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

                var anwerDto = new CreateAnswerDTO()
                {
                    Labels = new List<TranslationDTO>()
                };

                wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
                questionRepository.Find(default).ReturnsForAnyArgs(wizard.Questions.AsQueryable());
                unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

                var result = await bll.GetAnswerAsync(1, 1, 1);
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"Get answer tried on unexisting answer id {1}", ex.Message);
                throw;
            }

        }


        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task CreateAnswerAsyncTestDontHaveQuestion()
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
                        Id = 1,
                        IsFirstQuestion = true
                    }
                }
            };

            var anwerDto = new CreateAnswerDTO()
            {
                Labels = new List<TranslationDTO>()
            };

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Question, bool>>>();
                return wizard.Questions.Where(exp.Compile()).AsQueryable();
            });
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            try
            {
                var result = await bll.CreateAnswerAsync(1, 1, anwerDto);
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"Create answer tried on unexisting question id {1}", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task CreateAnswerAsyncTestQuestionNotFound()
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
            var anwerDto = new CreateAnswerDTO()
            {
                Labels = new List<TranslationDTO>()
            };
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            await bll.CreateAnswerAsync(1, 1, anwerDto);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task CreateAnswerAsyncTestQuestionNotFound_Case2()
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
                Questions = new List<Question>(){
                    new Question()
                    {
                        Id=2
                    }
                }
            };
            var anwerDto = new CreateAnswerDTO()
            {
                Labels = new List<TranslationDTO>()
            };
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Question, bool>>>();
                return wizard.Questions.Where(exp.Compile()).AsQueryable();
            });
            await bll.CreateAnswerAsync(1, 1, anwerDto);
        }

        private static List<Answer> GetAnswerMock()
        {
            var answers = new List<Answer>()
            {
                new Answer()
                {
                    Id = 1,
                    QuestionId = 1,
                    Order = 1,
                    Labels = new List<Translation>
                    {
                        new Translation { Id = 1, Text = "Answer one", LanguageCode = "en" },
                        new Translation { Id = 2, Text = "Répondez à une", LanguageCode = "fr" }
                    },
                    LinkedQuestionId = null,
                    Links = new List<Translation>
                    {
                        new Translation { Id = 3, Text = "Link one", LanguageCode = "en" },
                        new Translation { Id = 4, Text = "Lien un", LanguageCode = "fr" }
                    },
                    IsFinalAnswer = false,
                    FinalAnswerTexts = new List<Translation>()
                },
                new Answer()
                {
                    Id = 2,
                    QuestionId = 1,
                    Order = 2,
                    Labels = new List<Translation>
                    {
                        new Translation { Id = 5, Text = "Answer two", LanguageCode = "en" },
                        new Translation { Id = 3, Text = "Réponse deux", LanguageCode = "fr" }
                    },
                    LinkedQuestionId = null,
                    Links = new List<Translation>
                    {
                        new Translation { Id = 7, Text = "Link two", LanguageCode = "en" },
                        new Translation { Id = 8, Text = "Lien deux", LanguageCode = "fr" }
                    },
                    IsFinalAnswer = true,
                    FinalAnswerTexts = new List<Translation>
                    {
                        new Translation { Id = 9, Text = "Final Answer EN", LanguageCode = "en" },
                        new Translation { Id = 10, Text = "Final Answer FR", LanguageCode = "fr" }
                    }
                }
            };
            return answers;
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task CreateAnswerAsyncTest_CanNotLinkedToSameQuestion()
        {
            var anwerDto = new CreateAnswerDTO()
            {
                LinkedQuestionId = 1
            };

            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            try
            {
                var result = await bll.CreateAnswerAsync(1, 1, anwerDto);
            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"Can't linking an answer of a question to that same question, this causing the loop.", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        public async Task ListAnswersAsyncTest()
        {
            var answers = GetAnswerMock();
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
                        Id = 1,
                        WizardId = 1,
                        IsFirstQuestion = true,
                        IsActive = true,
                        Answers = answers
                    }
                }
            };

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            var result = bll.ListAnswers(1, 1);
            var countOrder = 1;

            wizardRepository.ReceivedWithAnyArgs(1).Find(default);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(false, result.ElementAt(0).IsFinalAnswer);
            Assert.AreEqual(true, result.ElementAt(1).IsFinalAnswer);
            Assert.AreEqual(0, result.ElementAt(0).FinalAnswerTexts.Count());
            Assert.AreEqual(2, result.ElementAt(1).FinalAnswerTexts.Count());
            foreach (var answer in result)
            {
                Assert.AreEqual(1, answer.QuestionId);
                Assert.AreEqual(countOrder, answer.Order);
                Assert.AreEqual(2, answer.Labels.Count());
                Assert.AreEqual(2, answer.Links.Count());
                Assert.AreEqual(null, answer.LinkedQuestionId);
                countOrder++;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ListAnswersAsyncTestQuestionNotFound()
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
            var result = bll.ListAnswers(1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ListAnswersAsyncTestWizardNotFound()
        {
            var result = bll.ListAnswers(1, 1);
        }

        [TestMethod()]
        public async Task GetAnswerAsyncTest()
        {
            Answer answer = new Answer()
            {
                Id = 1
            };
            List<Question> questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1,
                        Answers = new List<Answer>()
                        {
                            answer
                        }
                    }
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
                Questions = questions
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).ReturnsForAnyArgs(questions.AsQueryable());
            var result = await bll.GetAnswerAsync(1, 1, 1);
            wizardRepository.ReceivedWithAnyArgs(1).Find(Arg.Any<Expression<Func<Wizard, bool>>>());
            Assert.AreEqual(answer.Id, result.Id);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetAnswerAsyncTestExceptionAnswerNotFound()
        {
            List<Question> questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1,
                        Answers = new List<Answer>()
                    }
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
                Questions = questions
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).ReturnsForAnyArgs(questions.AsQueryable());
            await bll.GetAnswerAsync(1, 1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetAnswerAsyncTestExceptionQuestionNotFound()
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
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).ReturnsForAnyArgs(new List<Question>().AsQueryable());
            await bll.GetAnswerAsync(1, 1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetAnswerAsyncTestExceptionWizardsNotFound()
        {
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>().AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(0);
            await bll.GetAnswerAsync(1, 1, 1);
        }

        [TestMethod()]
        public async Task DeleteAnswernAsyncTest()
        {
            Answer answer = new Answer()
            {
                Id = 1
            };
            List<Question> questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1,
                        Answers = new List<Answer>()
                        {
                            answer
                        }
                    }
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
                Questions = questions
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            await bll.DeleteAnswerAsync(1, 1, 1);
            unitOfWork.Received(1).SaveChangesAsync();
            Assert.AreEqual(0, wizard.Questions.FirstOrDefault().Answers.Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task DeleteAnswernAsyncTest_WhenFailedSaveChange()
        {
            Answer answer = new Answer()
            {
                Id = 1
            };
            List<Question> questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1,
                        Answers = new List<Answer>()
                        {
                            answer
                        }
                    }
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
                Questions = questions
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(0);
            await bll.DeleteAnswerAsync(1, 1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteAnswernAsyncTest_WhenAnswerNotFoun()
        {
            List<Question> questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1,
                        Answers = new List<Answer>()
                    }
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
                Questions = questions
            };

            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { wizard }.AsQueryable());
            await bll.DeleteAnswerAsync(1, 1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteAnswernAsyncTest_WhenQuestionNotFoun()
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
            await bll.DeleteAnswerAsync(1, 1, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteAnswernAsyncTest_WhenWizardNotFoun()
        {
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>().AsQueryable());
            await bll.DeleteAnswerAsync(1, 1, 1);
        }

        [TestMethod]
        public async Task UpdateAnswerAsyncTest()
        {
            int wizardId = 1;
            int questionId = 1;
            int answerId = 1;
            Answer answer = new Answer()
            {
                Id = 1,
                QuestionId = 1
            };
            List<Question> questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1,
                        Answers = new List<Answer>()
                        {
                            answer
                        }
                    }
                };
            Wizard existingWizard = new Wizard()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                IntroductionTexts = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Questions = questions
            };
            var updateAnswerDTO = new MyFanc.DTO.Internal.Wizards.UpdateAnswerDTO
            {
                Id = 1,
                Labels = new List<DTO.Internal.Translation.TranslationDTO>(),
                Tags = new List<string>(),
            };
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).ReturnsForAnyArgs(questions.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);
            var updateAnswer = await bll.UpdateAnswerAsync(wizardId, questionId, answerId, updateAnswerDTO);
            Assert.AreEqual(answer.Id, updateAnswer.Id);
            Assert.AreEqual(questionId, updateAnswer.QuestionId);
        }


        [TestMethod]
        public async Task UpdateAnswerWithFinalAsswerTrueAsyncTest()
        {
            int wizardId = 1;
            int questionId = 1;
            int answerId = 1;
            Answer answer1 = new Answer()
            {
                Id = 1,
                Order = 1,
                QuestionId = 1,
                IsFinalAnswer = true,
                FinalAnswerTexts = new List<Translation>() { 
                    new Translation()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Old Final Aswer"
                    }
                }
            };
            Answer answer2 = new Answer()
            {
                Id = 2,
                Order = 2,
                QuestionId = 1,
                IsFinalAnswer = false
            };
            List<Question> questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1,
                        Answers = new List<Answer>()
                        {
                            answer1,
                            answer2
                        }
                    }
                };
            Wizard existingWizard = new Wizard()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                IntroductionTexts = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Questions = questions
            };
            var updateAnswerDTO = new MyFanc.DTO.Internal.Wizards.UpdateAnswerDTO
            {
                Id = 2,
                Labels = new List<DTO.Internal.Translation.TranslationDTO>(),
                Tags = new List<string>(),
                IsFinalAnswer = true,
                FinalAnswerTexts = new List<TranslationDTO>() {
                    new TranslationDTO()
                    {
                        Id = 0,
                        LanguageCode = "en",
                        Text = "New Final Aswer"
                    }
                }
            };
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).ReturnsForAnyArgs(questions.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);
            var updateAnswer = await bll.UpdateAnswerAsync(wizardId, questionId, 2, updateAnswerDTO);
            Assert.AreEqual(answer2.Id, updateAnswer.Id);
            Assert.AreEqual(questionId, updateAnswer.QuestionId);
            Assert.AreEqual(true, updateAnswer.IsFinalAnswer);
            Assert.AreEqual(true, answer1.IsFinalAnswer);
            Assert.AreEqual(1, updateAnswer.FinalAnswerTexts.Count());
            Assert.AreEqual(1, answer1.FinalAnswerTexts.Count());
            Assert.AreEqual("New Final Aswer", updateAnswer.FinalAnswerTexts.ElementAt(0).Text);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdateAnswerAsyncTest_WhenAnswerNotFound()
        {
            int wizardId = 1;
            int questionId = 1;
            int answerId = 1;
            List<Question> questions = new List<Question>()
                {
                    new Question()
                    {
                        Id=1,
                        Answers = new List<Answer>()
                    }
                };
            Wizard existingWizard = new Wizard()
            {
                Id = 1,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                IntroductionTexts = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1,
                Questions = questions
            };
            var updateAnswerDTO = new MyFanc.DTO.Internal.Wizards.UpdateAnswerDTO
            {
                Id = 1,
                Labels = new List<DTO.Internal.Translation.TranslationDTO>(),
                Tags = new List<string>(),
            };
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).ReturnsForAnyArgs(questions.AsQueryable());
            await bll.UpdateAnswerAsync(wizardId, questionId, answerId, updateAnswerDTO);

        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdateAnswerAsyncTest_WhenQuestionNotFound()
        {
            int wizardId = 1;
            int questionId = 1;
            int answerId = 1;
            Wizard existingWizard = new Wizard() { Id = 1 };
            var updateAnswerDTO = new MyFanc.DTO.Internal.Wizards.UpdateAnswerDTO();
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>() { existingWizard }.AsQueryable());
            questionRepository.Find(Arg.Any<Expression<Func<Question, bool>>>()).ReturnsForAnyArgs(new List<Question>().AsQueryable());
            await bll.UpdateAnswerAsync(wizardId, questionId, answerId, updateAnswerDTO);

        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdateAnswerAsyncTest_WhenWizardNotFound()
        {
            int wizardId = 1;
            int questionId = 1;
            int answerId = 1;
            Wizard existingWizard = new Wizard() { Id = 1 };
            var updateAnswerDTO = new MyFanc.DTO.Internal.Wizards.UpdateAnswerDTO();
            wizardRepository.Find(Arg.Any<Expression<Func<Wizard, bool>>>()).ReturnsForAnyArgs(new List<Wizard>().AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(0);
            await bll.UpdateAnswerAsync(wizardId, questionId, answerId, updateAnswerDTO);

        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task CreateAnswerAsync_QuestionNotLinkedToAnyFirstQuestion()
        {
            var wizard = MockWizard_ForCreateAnswerAsync();

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());

            ArrangeQuestion_ForCreateAnswerAsync();

            var createAnswerDTO = new CreateAnswerDTO
            {
                QuestionId = 6,
                LinkedQuestionId = null,
            };
            try
            {
                var answer = await bll.CreateAnswerAsync(1, 6, createAnswerDTO);
            }catch(KeyNotFoundException ex)
            {
                Assert.AreEqual("Can't create answer because this question doesn't have link to first question.", ex.Message);
                throw;
            }
        }

        private Wizard MockWizard_ForCreateAnswerAsync()
        {
            var questionList = new List<Question>
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
                            Id = 4,
                            LanguageCode = "en",
                            Text = "question 4"
                        }
                    }
                },
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
                },
                new Question {
                    Id = 6,
                    IsFirstQuestion = false,
                    Titles = new [] {
                        new Translation {
                            Id = 6,
                            LanguageCode = "en",
                            Text = "question 6"
                        }
                    }
                }
            };

            var wizard = new Wizard
            {
                Id = 1,
                Questions = questionList
            };

            return wizard;
        }

        [TestMethod]
        public async Task CreateAnswerAsync_QuestionLinkedToAnyFirstQuestion()
        {
            var wizard = MockWizard_ForCreateAnswerAsync();

            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());

            ArrangeQuestion_ForCreateAnswerAsync();

            questionRepository.Find(Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                         q => q.WizardId == 1 && q.Id == 5 && !q.DeletedTime.HasValue))
                ).Returns(wizard.Questions.AsQueryable());

            var createAnswerDTO = new CreateAnswerDTO
            {
                QuestionId = 5,
                LinkedQuestionId = 6,
            };
            
            var answer = await bll.CreateAnswerAsync(1, 5, createAnswerDTO);
        }

        private void ArrangeQuestion_ForCreateAnswerAsync()
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
                                Text = "question 2"
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
                                Text = "question 3"
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
                    }
                }).AsQueryable());
        }

        [TestMethod]
        public async Task ReorderAnswersAsyncTest()
        {
            // Arrange
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
                        Id = 1,
                        Answers = new List<Answer>
                        {
                            new Answer { Id = 1, QuestionId = 1, Order = 5},
                            new Answer { Id = 2, QuestionId = 1, Order = 4},
                            new Answer { Id = 3, QuestionId = 1, Order = 3},
                            new Answer { Id = 4, QuestionId = 1, Order = 2},
                            new Answer { Id = 5, QuestionId = 1, Order = 1},
                        }
                    }
                }
            };

            var answerOrderItemDTO = new List<AnswerOrderItemDTO>
            {
                new AnswerOrderItemDTO { AnswerId = 1, Order = 1 },
                new AnswerOrderItemDTO { AnswerId = 2, Order = 2 },
                new AnswerOrderItemDTO { AnswerId = 3, Order = 3 },
                new AnswerOrderItemDTO { AnswerId = 4, Order = 4 },
                new AnswerOrderItemDTO { AnswerId = 5, Order = 5 }
            };

            // Act
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            questionRepository.Find(Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                         q => q.WizardId == 1 && q.Id == 1 && !q.DeletedTime.HasValue))
                ).Returns(wizard.Questions.AsQueryable());

            // Assert
            var result = await bll.ReorderAnswersAsync(1, 1, answerOrderItemDTO);
            var answers = bll.ListAnswers(1, 1);

            // Assert
            Assert.AreEqual(true, result);

            Assert.AreEqual(answerOrderItemDTO.ToArray()[0].AnswerId, answers.ToArray()[0].Id);
            Assert.AreEqual(1, answers.ToArray()[0].Order);

            Assert.AreEqual(answerOrderItemDTO.ToArray()[1].AnswerId, answers.ToArray()[1].Id);
            Assert.AreEqual(2, answers.ToArray()[1].Order);

            Assert.AreEqual(answerOrderItemDTO.ToArray()[2].AnswerId, answers.ToArray()[2].Id);
            Assert.AreEqual(3, answers.ToArray()[2].Order);

            Assert.AreEqual(answerOrderItemDTO.ToArray()[3].AnswerId, answers.ToArray()[3].Id);
            Assert.AreEqual(4, answers.ToArray()[3].Order);

            Assert.AreEqual(answerOrderItemDTO.ToArray()[4].AnswerId, answers.ToArray()[4].Id);
            Assert.AreEqual(5, answers.ToArray()[4].Order);
        }
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task UpdateAnswerAsyncTest_CanNotLinkedToSameQuestion()
        {
            var updateAnswerDTO = new UpdateAnswerDTO
            {
                LinkedQuestionId = 1
            };

            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            try
            {
                var result = await bll.UpdateAnswerAsync(1, 1, 1, updateAnswerDTO);
            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"Can't linking an answer of a question to that same question, this causing the loop.", ex.Message);
                throw;
            }
        }
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ReorderAnswersAsyncTest_WhenKeyNotFound()
        {
            // Arrange
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
                        Id = 1,
                        Answers = new List<Answer>
                        {
                            new Answer { Id = 1, QuestionId = 1, Order = 5},
                            new Answer { Id = 2, QuestionId = 1, Order = 4},
                            new Answer { Id = 3, QuestionId = 1, Order = 3},
                            new Answer { Id = 4, QuestionId = 1, Order = 2},
                            //new Answer { Id = 5, QuestionId = 1, Order = 1},
                        }
                    }
                }
            };

            var answerOrderItemDTO = new List<AnswerOrderItemDTO>
            {
                new AnswerOrderItemDTO { AnswerId = 1, Order = 1 },
                new AnswerOrderItemDTO { AnswerId = 2, Order = 2 },
                new AnswerOrderItemDTO { AnswerId = 3, Order = 3 },
                new AnswerOrderItemDTO { AnswerId = 4, Order = 4 },
                new AnswerOrderItemDTO { AnswerId = 5, Order = 5 }
            };

            // Act
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            questionRepository.Find(Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                         q => q.WizardId == 1 && q.Id == 1 && !q.DeletedTime.HasValue))
                ).Returns(wizard.Questions.AsQueryable());

            try
            {
                await bll.ReorderAnswersAsync(1, 1, answerOrderItemDTO);
            }
            catch (KeyNotFoundException ex)
            {
                // Assert
                Assert.AreEqual("Received data contains answers not linked to received question or wizard!", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public async Task ReorderAnswersAsyncTest_WhenDuplicateKey()
        {
            // Arrange
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
                        Id = 1,
                        Answers = new List<Answer>
                        {
                            new Answer { Id = 1, QuestionId = 1, Order = 5},
                            new Answer { Id = 2, QuestionId = 1, Order = 4},
                            new Answer { Id = 3, QuestionId = 1, Order = 3},
                            new Answer { Id = 4, QuestionId = 1, Order = 2},
                            new Answer { Id = 5, QuestionId = 1, Order = 1},
                        }
                    }
                }
            };

            var answerOrderItemDTO = new List<AnswerOrderItemDTO>
            {
                new AnswerOrderItemDTO { AnswerId = 1, Order = 1 },
                new AnswerOrderItemDTO { AnswerId = 1, Order = 2 },
                new AnswerOrderItemDTO { AnswerId = 3, Order = 3 },
                new AnswerOrderItemDTO { AnswerId = 4, Order = 4 },
                new AnswerOrderItemDTO { AnswerId = 5, Order = 5 }
            };

            // Act
            wizardRepository.Find(default).ReturnsForAnyArgs((new[] { wizard }).AsQueryable());
            questionRepository.Find(Arg.Is<Expression<Func<Question, bool>>>(
                    expr => Neleus.LambdaCompare.Lambda.Eq(expr,
                         q => q.WizardId == 1 && q.Id == 1 && !q.DeletedTime.HasValue))
                ).Returns(wizard.Questions.AsQueryable());

            try
            {
                await bll.ReorderAnswersAsync(1, 1, answerOrderItemDTO);
            }
            catch (ApplicationException ex)
            {
                // Assert
                Assert.AreEqual("Duplicate key detected in received data!", ex.Message);
                throw;
            }
        }
    }
}



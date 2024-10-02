using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.Api.Mapper;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.DTO.Internal.Forms;
using MyFanc.DTO.Internal.Translation;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MyFanc.Core.Enums;

namespace MyFanc.Test
{
	[TestClass()]
    public class MyFanc_Form_Form_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IHttpContextAccessor httpContextAccessor;
        private IGenericRepository<Form> formRepository;
        private IGenericRepository<Nacabel> nacabelRepository;
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
            this.httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            this.unitOfWork = Substitute.For<IUnitOfWork>();
            this.formRepository = Substitute.For<IGenericRepository<Form>>();
            this.nacabelRepository = Substitute.For<IGenericRepository<Nacabel>>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.sharedDataCache = Substitute.For<ISharedDataCache>();
			this.identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();
            this.aESEncryptService = Substitute.For<IAESEncryptService>();

			this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();

            this.unitOfWork.GetGenericRepository<Form>().Returns(this.formRepository);
            this.unitOfWork.GetGenericRepository<Nacabel>().Returns(this.nacabelRepository);

            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new WizardProfile());
                cfg.AddProfile(new FormProfile());
                cfg.ConstructServicesUsing(svc =>
                {
					if (svc.UnderlyingSystemType == typeof(DocumentPathResolver))
                    {
                        return new DocumentPathResolver(this.httpContextAccessor);
                    }

                    return new IsUsedResolver();
				});
            });

            this.mapper = new Mapper(mapperConfig);

            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();
            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod]
        public void GetListFormTest()
        {
            var listFormMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form 1 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form 1 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 2,
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form 2 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form 2 fr",
                            LanguageCode = "fr"
                        }
                    }
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var formList = bll.GetListForm(new SearchParamFormDTO());
            for (int i = 0; i < formList.Count(); i++)
            {
                var result = formList.ElementAt(i);
                var source = listFormMock.ElementAt(i);
                Assert.AreEqual(source.ExternalId, result.Id);
                Assert.AreEqual(source.Id, result.OriginalId);
                Assert.AreEqual(source.Labels.Count, result.Labels.Count());
            }
        }

        [TestMethod]
        public void GetListFormTestFilterById()
        {
            var listFormMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Type = "Webform",
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 1 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 1 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 2,
                    Type = "Pdf",
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 2 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 2 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 10,
                    Type = "Webform",
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 10 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 10 fr",
                            LanguageCode = "fr"
                        }
                    }
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var searchParam = new SearchParamFormDTO() { Id = 1 };
            var formList = bll.GetListForm(searchParam);

            Assert.AreEqual(formList.Count(), 2);

            var result = formList.ElementAt(0);
            var source = listFormMock.ElementAt(0);
            Assert.IsTrue(result.Id.ToString().Contains(searchParam.Id.ToString()));
            Assert.AreEqual(source.Id, result.OriginalId);
            Assert.AreEqual(source.Labels.Count, result.Labels.Count());

            var result2 = formList.ElementAt(1);
            var source2 = listFormMock.ElementAt(2);
            Assert.IsTrue(result2.Id.ToString().Contains(searchParam.Id.ToString()));
            Assert.AreEqual(source2.Id, result2.OriginalId);
            Assert.AreEqual(source2.Labels.Count, result2.Labels.Count());

        }

        [TestMethod]
        public void GetListFormTestFilterByLabel()
        {
            var listFormMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Type = "Webform",
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 1 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 1 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 2,
                    Type = "Pdf",
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 2 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 2 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 10,
                    Type = "Webform",
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 10 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 10 fr",
                            LanguageCode = "fr"
                        }
                    }
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var searchParam = new SearchParamFormDTO() { Label = "Block 1", LanguageCode = "en" };
            var formList = bll.GetListForm(searchParam);

            Assert.AreEqual(formList.Count(), 2);

            var result = formList.ElementAt(0);
            var source = listFormMock.ElementAt(0);
            Assert.IsTrue(result.Labels.Where(l => l.LanguageCode == "en").First()?.Text?.Contains("Block 1"));
            Assert.AreEqual(source.ExternalId, result.Id);
            Assert.AreEqual(source.Id, result.OriginalId);
           
            var result2 = formList.ElementAt(1);
            var source2 = listFormMock.ElementAt(2);
            Assert.IsTrue(result2.Labels.Where(l => l.LanguageCode == "en").First()?.Text?.Contains("Block 1"));
            Assert.AreEqual(source2.ExternalId, result2.Id);
            Assert.AreEqual(source2.Id, result2.OriginalId);

        }

        [TestMethod]
        public void GetListFormTestFilterByCategory()
        {
            var listFormMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Type = "Webform",
                    FormCategoryId = 1,
                    FormCategory = new FormCategory()
                    {
                        Id = 1,
                        Code = "IAII"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 1 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 1 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 2,
                    Type = "Pdf",
                    FormCategoryId = 2,
                    FormCategory = new FormCategory()
                    {
                        Id = 2,
                        Code = "GLMI"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 2 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 2 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 10,
                    Type = "Webform",
                    FormCategoryId = 3,
                    FormCategory = new FormCategory()
                    {
                        Id = 3,
                        Code = "GLBEG"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 10 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 10 fr",
                            LanguageCode = "fr"
                        }
                    }
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var searchParam = new SearchParamFormDTO() { Category = "1,3" };
            var formList = bll.GetListForm(searchParam);

            Assert.AreEqual(formList.Count(), 2);

            SearchParamFormDTOExten searchParamList = mapper.Map<SearchParamFormDTOExten>(searchParam);
            var result = formList.ElementAt(0);
            var source = listFormMock.ElementAt(0);
            Assert.AreEqual(source.Id, result.OriginalId);
            Assert.IsTrue(searchParamList.CategoriesList.Contains(source.FormCategory.Id));
            Assert.AreEqual(source.ExternalId, result.Id);
            

            var result2 = formList.ElementAt(1);
            var source2 = listFormMock.ElementAt(2);
            Assert.AreEqual(source2.Id, result2.OriginalId);
            Assert.IsTrue(searchParamList.CategoriesList.Contains(source2.FormCategory.Id));
            Assert.AreEqual(source2.ExternalId, result2.Id);
        }

        [TestMethod]
        public void GetListFormTestFilterByStatus()
        {
            var listFormMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Type = "Webform",
                    FormCategoryId = 1,
                    Status = FormStatus.Draft,
                    FormCategory = new FormCategory()
                    {
                        Id = 1,
                        Code = "IAII"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 1 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 1 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 2,
                    Type = "Pdf",
                    FormCategoryId = 2,
                    Status = FormStatus.Online,
                    FormCategory = new FormCategory()
                    {
                        Id = 2,
                        Code = "GLMI"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 2 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 2 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 10,
                    Type = "Webform",
                    FormCategoryId = 3,
                    Status = FormStatus.Offline,
                    FormCategory = new FormCategory()
                    {
                        Id = 3,
                        Code = "GLBEG"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 10 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 10 fr",
                            LanguageCode = "fr"
                        }
                    }
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var searchParam = new SearchParamFormDTO() { Status = "1,2" };
            var formList = bll.GetListForm(searchParam);

            Assert.AreEqual(formList.Count(), 2);

            SearchParamFormDTOExten searchParamList = mapper.Map<SearchParamFormDTOExten>(searchParam);
            var result = formList.ElementAt(0);
            var source = listFormMock.ElementAt(1);
            Assert.AreEqual(source.Id, result.OriginalId);
            Assert.IsTrue(searchParamList.StatusList.Contains((int)source.Status));
            Assert.AreEqual(source.ExternalId, result.Id);


            var result2 = formList.ElementAt(1);
            var source2 = listFormMock.ElementAt(2);
            Assert.AreEqual(source2.Id, result2.OriginalId);
            Assert.IsTrue(searchParamList.StatusList.Contains((int)source2.Status));
            Assert.AreEqual(source2.ExternalId, result2.Id);

        }

        [TestMethod]
        public void GetListFormTestFilterByTag()
        {
            var listFormMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Type = "Webform",
                    FormCategoryId = 1,
                    Status = FormStatus.Draft,
                    Nacabels = new List<Nacabel>()
                    {
                        new Nacabel()
                        {
                            Id = 1,
                            NacabelTranslation = new List<NacabelTranslation>()
                            {
                                new NacabelTranslation()
                                {
                                    Id = 1,
                                    NacabelId = 1,
                                    LanguageCode = "en",
                                    Description = "Transport"
                                }
                            }
                        }
                    },
                    FormCategory = new FormCategory()
                    {
                        Id = 1,
                        Code = "IAII"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 1 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 1 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 2,
                    Type = "Pdf",
                    FormCategoryId = 2,
                    Status = FormStatus.Online,
                    Nacabels = new List<Nacabel>()
                    {
                        new Nacabel()
                        {
                            Id = 2,
                            NacabelTranslation = new List<NacabelTranslation>()
                            {
                                new NacabelTranslation()
                                {
                                    Id = 2,
                                    NacabelId = 2,
                                    LanguageCode = "en",
                                    Description = "Dentist"
                                }
                            }
                        }
                    },
                    FormCategory = new FormCategory()
                    {
                        Id = 2,
                        Code = "GLMI"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 2 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 2 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 10,
                    Type = "Webform",
                    FormCategoryId = 3,
                    Status = FormStatus.Offline,
                    Nacabels = new List<Nacabel>()
                    {
                        new Nacabel()
                        {
                            Id = 3,
                            NacabelTranslation = new List<NacabelTranslation>()
                            {
                                new NacabelTranslation()
                                {
                                    Id = 3,
                                    NacabelId = 3,
                                    LanguageCode = "en",
                                    Description = "Radiopharma"
                                }
                            }
                        }
                    },
                    FormCategory = new FormCategory()
                    {
                        Id = 3,
                        Code = "GLBEG"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 10 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 10 fr",
                            LanguageCode = "fr"
                        }
                    }
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var searchParam = new SearchParamFormDTO() { Tag = "Transport,Dentist", LanguageCode = "en" };
            var formList = bll.GetListForm(searchParam);

            Assert.AreEqual(formList.Count(), 2);

            SearchParamFormDTOExten searchParamList = mapper.Map<SearchParamFormDTOExten>(searchParam);
            var result = formList.ElementAt(0);
            var source = listFormMock.ElementAt(0);
            Assert.IsTrue(searchParamList.TagList.Contains(result.Tags.First().ToLower()));
            Assert.AreEqual(source.Id, result.OriginalId);
            Assert.AreEqual(source.ExternalId, result.Id);


            var result2 = formList.ElementAt(1);
            var source2 = listFormMock.ElementAt(1);
            Assert.IsTrue(searchParamList.TagList.Contains(result2.Tags.First().ToLower()));
            Assert.AreEqual(source2.Id, result2.OriginalId);
            Assert.AreEqual(source2.ExternalId, result2.Id);

        }

        [TestMethod]
        public void GetListFormTestFilterByType()
        {
            var listFormMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Type = "Webform",
                    FormCategoryId = 1,
                    Status = FormStatus.Draft,
                    Nacabels = new List<Nacabel>()
                    {
                        new Nacabel()
                        {
                            Id = 1,
                            NacabelTranslation = new List<NacabelTranslation>()
                            {
                                new NacabelTranslation()
                                {
                                    Id = 1,
                                    LanguageCode = "en",
                                    Description = "Transport"
                                }
                            }
                        }
                    },
                    FormCategory = new FormCategory()
                    {
                        Id = 1,
                        Code = "IAII"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 1 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 1 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 2,
                    Type = "Pdf",
                    FormCategoryId = 2,
                    Status = FormStatus.Online,
                    Nacabels = new List<Nacabel>()
                    {
                        new Nacabel()
                        {
                            Id = 2,
                            NacabelTranslation = new List<NacabelTranslation>()
                            {
                                new NacabelTranslation()
                                {
                                    Id = 2,
                                    LanguageCode = "en",
                                    Description = "Dentist"
                                }
                            }
                        }
                    },
                    FormCategory = new FormCategory()
                    {
                        Id = 2,
                        Code = "GLMI"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 2 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 2 fr",
                            LanguageCode = "fr"
                        }
                    }
                },
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 10,
                    Type = "Webform",
                    FormCategoryId = 3,
                    Status = FormStatus.Offline,
                    Nacabels = new List<Nacabel>()
                    {
                        new Nacabel()
                        {
                            Id = 3,
                            NacabelTranslation = new List<NacabelTranslation>()
                            {
                                new NacabelTranslation()
                                {
                                    Id = 3,
                                    LanguageCode = "en",
                                    Description = "Radiopharma"
                                }
                            }
                        }
                    },
                    FormCategory = new FormCategory()
                    {
                        Id = 3,
                        Code = "GLBEG"
                    },
                    Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                            Text = "Form Block 10 en",
                            LanguageCode = "en"
                        },
                        new Translation()
                        {
                            Text = "Form Block 10 fr",
                            LanguageCode = "fr"
                        }
                    }
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var searchParam = new SearchParamFormDTO() { Type = "Webform,Pdf" };
            var formList = bll.GetListForm(searchParam);

            Assert.AreEqual(formList.Count(), 2);

            SearchParamFormDTOExten searchParamList = mapper.Map<SearchParamFormDTOExten>(searchParam);
            var result = formList.ElementAt(0);
            var source = listFormMock.ElementAt(0);
            Assert.IsTrue(searchParamList.TypeList.Contains(result.Type.ToLower()));
            Assert.AreEqual(source.Id, result.OriginalId);
            Assert.AreEqual(source.ExternalId, result.Id);


            var result2 = formList.ElementAt(1);
            var source2 = listFormMock.ElementAt(1);
            Assert.IsTrue(searchParamList.TypeList.Contains(result2.Type.ToLower()));
            Assert.AreEqual(source2.Id, result2.OriginalId);
            Assert.AreEqual(source2.ExternalId, result2.Id);

        }

        [TestMethod]
        public async Task CreateFormAsyncTestWebform()
        {
            var createFormDTO = new CreateFormDTO()
            {
                IsActive = true,
                Labels = new List<TranslationDTO>()
                {
                    new TranslationDTO(){LanguageCode = "en", Text = "Form 2 en"},
                    new TranslationDTO(){LanguageCode = "fr", Text = "Form 2 fr"}

                },
                Urls = new List<TranslationDTO>()
                {
                    new TranslationDTO(){LanguageCode = "en", Text = "form_2_en"},
                    new TranslationDTO(){LanguageCode = "fr", Text = "form_2_fr"}
                },
                FormCategoryId = 2,
                Tags = new List<int>() { 1,2 },
                IsFormPdf = false,
                Version = 1
            };
            var nacabelMock = new Nacabel()
            {
                Id = 1,
                NacabelCode = "",
                NacabelTranslation = new List<NacabelTranslation>() { 
                    new NacabelTranslation() { 
                        Id = 1, 
                        NacabelId = 1, 
                        LanguageCode = "en", 
                        Description = "Transport" }
                }
            };
            var mockListFormInDb = new List<Form>() { new Form()
                {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Version = 1,
                    Status = FormStatus.Draft,
                    FormCategoryId = 1,
                    Labels = new List<Translation>()
                    {
                        new Translation(){LanguageCode = "en", Text = "Form 1 en"},
                        new Translation(){LanguageCode = "fr", Text = "Form 1 fr"}

                    },
                    Urls= new List<Translation>()
                    {
                        new Translation(){LanguageCode = "en", Text = "form_1_en"},
                        new Translation(){LanguageCode = "fr", Text = "form_1_fr"}
                    },
                    Type = FormType.Webform.ToString()
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(mockListFormInDb.AsQueryable());
            nacabelRepository.GetByIdAsync(Arg.Any<int>()).ReturnsForAnyArgs(nacabelMock);
            unitOfWork.SaveChangesAsync().Returns(1);

            var createForm = await bll.CreateFormAsync(createFormDTO);
            formRepository.Received(1).Add(Arg.Any<Form>());
            await unitOfWork.Received(1).SaveChangesAsync();

            Assert.AreEqual(2, createForm.ExternalId);
            Assert.AreEqual(true, createForm.IsActive);
            Assert.AreEqual(Core.Enums.FormStatus.Draft.ToString(), createForm.Status);
            Assert.AreEqual(2, createForm.Labels.Count());
            Assert.AreEqual(createFormDTO.Labels.ElementAt(0).Text, createForm.Labels.ElementAt(0).Text);
            Assert.AreEqual(createFormDTO.Labels.ElementAt(1).Text, createForm.Labels.ElementAt(1).Text);
            Assert.AreEqual(2, createForm.Urls.Count());
            Assert.AreEqual(createFormDTO.Urls.ElementAt(0).Text, createForm.Urls.ElementAt(0).Text);
            Assert.AreEqual(createFormDTO.Urls.ElementAt(1).Text, createForm.Urls.ElementAt(1).Text);
        }

        [TestMethod]
        public async Task CreateFormAsyncTestPdf()
        {
            var createFormDTO = new CreateFormDTO()
            {
                IsActive = true,
                Labels = new List<TranslationDTO>()
                {
                    new TranslationDTO(){LanguageCode = "en", Text = "Form 2 en"},
                    new TranslationDTO(){LanguageCode = "fr", Text = "Form 2 fr"}

                },
                Urls = new List<TranslationDTO>()
                {
                    new TranslationDTO(){LanguageCode = "en", Text = "form_2_en"},
                    new TranslationDTO(){LanguageCode = "fr", Text = "form_2_fr"}
                },
                FormCategoryId = 2,
                Tags = new List<int>() { 1, 2 },
                IsFormPdf = true,
                Version = 1,
                FormDocuments = new List<CreateFormDocumentDTO>()
                {
                    new CreateFormDocumentDTO()
                    {
                        LanguageCode = "en",
                        Documents = new List<CreateDocumentDTO>(){
                            new CreateDocumentDTO() {
                                Type = "application/pdf",
                                Name = "FileName",
                                Base64 = "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"
                            }
                        }
                    }
                }
            };
            var nacabelMock = new Nacabel()
            {
                Id = 1,
                NacabelCode = "",
                NacabelTranslation = new List<NacabelTranslation>() {
                    new NacabelTranslation() {
                        Id = 1,
                        NacabelId = 1,
                        LanguageCode = "en",
                        Description = "Transport" }
                }
            };
            var mockListFormInDb = new List<Form>() { new Form()
                {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
                    Version = 1,
                    Status = FormStatus.Draft,
                    FormCategoryId = 1,
                    Labels = new List<Translation>()
                    {
                        new Translation(){LanguageCode = "en", Text = "Form 1 en"},
                        new Translation(){LanguageCode = "fr", Text = "Form 1 fr"}

                    },
                    Urls= new List<Translation>()
                    {
                        new Translation(){LanguageCode = "en", Text = "form_1_en"},
                        new Translation(){LanguageCode = "fr", Text = "form_1_fr"}
                    },
                    Type = FormType.Webform.ToString()
                }
            };

            var formIdGUid = Guid.NewGuid();
            var guidFile = Guid.NewGuid();
            var docPath = $"{formIdGUid}_{createFormDTO}/{guidFile}";

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(mockListFormInDb.AsQueryable());
            nacabelRepository.GetByIdAsync(Arg.Any<int>()).ReturnsForAnyArgs(nacabelMock);
            fileStorage.SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(docPath);
            unitOfWork.SaveChangesAsync().Returns(1);

            var createForm = await bll.CreateFormAsync(createFormDTO);
            formRepository.Received(1).Add(Arg.Any<Form>());
            await unitOfWork.Received(1).SaveChangesAsync();

            Assert.AreEqual(2, createForm.ExternalId);
            Assert.AreEqual(true, createForm.IsActive);
            Assert.AreEqual("Pdf", createForm.Type);
            Assert.AreEqual(Core.Enums.FormStatus.Draft.ToString(), createForm.Status);
            Assert.AreEqual(2, createForm.Labels.Count());
            Assert.AreEqual(createFormDTO.Labels.ElementAt(0).Text, createForm.Labels.ElementAt(0).Text);
            Assert.AreEqual(createFormDTO.Labels.ElementAt(1).Text, createForm.Labels.ElementAt(1).Text);
            Assert.AreEqual(2, createForm.Urls.Count());
            Assert.AreEqual(createFormDTO.Urls.ElementAt(0).Text, createForm.Urls.ElementAt(0).Text);
            Assert.AreEqual(createFormDTO.Urls.ElementAt(1).Text, createForm.Urls.ElementAt(1).Text);
        }

        [TestMethod()]
        public async Task CreateFormAsyncTestArgumentExeption()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.CreateFormAsync(default);
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task CreateFormAsyncTestExceptionOnSaveChange()
        {
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs((new[] { new Form() }).AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(0);
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.CreateFormAsync(new CreateFormDTO());
            });
            await unitOfWork.ReceivedWithAnyArgs(1).SaveChangesAsync();
        }

        [TestMethod]
        public async Task UpdateFormAsyncTestPdf()
        {
            Guid formId = Guid.NewGuid();
            Guid formDelDocId = Guid.NewGuid();
            Guid formEdDocId = Guid.NewGuid();
            Guid formKeepDocId = Guid.NewGuid();
            string pathEn = "http://127.0.0.1:10001/fileen";
            string pathFr = "http://127.0.0.1:10001/filefr";
            string pathNl = "http://127.0.0.1:10001/filenl";
            var updateFormDTO = new DTO.Internal.Forms.UpdateFormDTO
            {
                Id = formId,
                IsActive = true,
                Version = 1,
                Tags = new[] { 2,3 },
                FormCategoryId = 2,
                IsFormPdf = true,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> {
                    new TranslationDTO()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label Form 1 Update",
                    }
                },
                Urls = new List<DTO.Internal.Translation.TranslationDTO> {
                    new TranslationDTO()
                    {
                        Id = 2,
                        LanguageCode = "en",
                        Text = "url_form_1_update",
                    }
                },
                Descriptions = new List<DTO.Internal.Translation.TranslationDTO> {
                    new TranslationDTO()
                    {
                        Id = 3,
                        LanguageCode = "en",
                        Text = "Description Form 1 Update",
                    }
                },
                FormDocuments = new List<UpdateFormDocumentDTO>()
                {
                    new UpdateFormDocumentDTO()
                    {
                        Id = formEdDocId,
                        LanguageCode = "en",
                        Documents = new List<UpdateDocumentDTO>(){
                            new UpdateDocumentDTO() {
                                Id = formEdDocId,
                                Type = "application/pdf",
                                Name = "FileNameEn_update",
                                Base64 = "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"
                            }
                        }
                    },
                    new UpdateFormDocumentDTO()
                    {
                        Id = formKeepDocId,
                        LanguageCode = "fr",
                        Documents = new List<UpdateDocumentDTO>(){
                            new UpdateDocumentDTO() {
                                Id = formKeepDocId
                            }
                        }
                    },
                    new UpdateFormDocumentDTO()
                    {
                        Id = formDelDocId,
                        LanguageCode = "nl",
                        Documents = new List<UpdateDocumentDTO>()
                    },
                    new UpdateFormDocumentDTO()
                    {
                        LanguageCode = "tw",
                        Documents = new List<UpdateDocumentDTO>(){
                            new UpdateDocumentDTO() {
                                Type = "application/pdf",
                                Name = "FileNameEn_update",
                                Base64 = "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"
                            }
                        }
                    }
                }
            };
            var existingForm = new Form()
            {
                Id = formId,
                IsActive = true,
                Version = 1,
                FormCategoryId = 1,
                FormCategory = new FormCategory()
                {
                    Id = 1,
                    Code = "IAII"
                },
                Nacabels =  new List<Nacabel>() { 
                    new Nacabel()
                    {
                        Id = 1
                    },
                    new Nacabel()
                    {
                        Id = 2
                    }
                },
                Labels = new List<Translation> {
                    new Translation()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label Form 1",
                    }
                },
                Urls = new List<Translation> {
                    new Translation()
                    {
                        Id = 2,
                        LanguageCode = "en",
                        Text = "url_form_1",
                    }
                },
                Descriptions = new List<Translation> {
                    new Translation()
                    {
                        Id = 3,
                        LanguageCode = "en",
                        Text = "Description Form 1",
                    }
                },
                FormDocuments = new List<FormDocument>()
                {
                    new FormDocument()
                    {
                        Id = formEdDocId,
                        LanguageCode = "en",
                        Documents = new List<Document>(){
                            new Document() {
                                Id = formEdDocId,
                                Type = "application/pdf",
                                Name = "FileNameEn",
                                Path = pathEn
                            }
                        }
                    },
                    new FormDocument()
                    {
                        Id = formKeepDocId,
                        LanguageCode = "fr",
                        Documents = new List<Document>(){
                            new Document() {
                                Id = formKeepDocId,
                                Type = "application/pdf",
                                Name = "FileNameFr",
                                Path = pathFr
                            }
                        }
                    },
                    new FormDocument()
                    {
                        Id = formDelDocId,
                        LanguageCode = "nl",
                        Documents = new List<Document>(){
                            new Document() {
                                Id = formDelDocId,
                                FormDocumentId = formDelDocId,
                                Type = "application/pdf",
                                Name = "FileNameNl",
                                Path = pathNl
                            }
                        }
                    },
                    new FormDocument()
                    {
                        Id = Guid.NewGuid(),
                        LanguageCode = "id",
                        Documents = new List<Document>(){
                            new Document() {
                                Id = Guid.NewGuid(),
                                Type = "application/pdf",
                                Name = "FileNameId"
                            }
                        }
                    }

                }
            };

            var listFormMock = new List<Form>()
            {
                existingForm,
                new Form()
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Version = 1
                }
            };

            var nacabelMock = new List<Nacabel>(){ new Nacabel()
                {
                    Id = 3
                } 
            };


            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            nacabelRepository.Find(Arg.Any<Expression<Func<Nacabel, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Nacabel, bool>>>();
                return nacabelMock.Where(exp.Compile()).AsQueryable();
            });

            fileStorage.ExistsAsync(Arg.Any<string>()).Returns(true);
            fileStorage.DeleteAsync(Arg.Any<string>()).Returns(true);
            fileStorage.SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<string>()).Returns("http://127.0.0.1:10001/newpath");

            unitOfWork.SaveChangesAsync().Returns(1);

            var updateForm = await bll.UpdateFormAsync(formId, updateFormDTO);

            Assert.AreEqual(formId, updateForm.Id);

            Assert.AreEqual(1, updateForm.Labels.Count());
            Assert.AreEqual("Label Form 1 Update", existingForm.Labels.ElementAt(0).Text);
            Assert.AreEqual("Label Form 1 Update", updateForm.Labels.ElementAt(0).Text);

            Assert.AreEqual(1, updateForm.Urls.Count());
            Assert.AreEqual("url_form_1_update", existingForm.Urls.ElementAt(0).Text);
            Assert.AreEqual("url_form_1_update", updateForm.Urls.ElementAt(0).Text);

            Assert.AreEqual(1, updateForm.Descriptions.Count());
            Assert.AreEqual("Description Form 1 Update", existingForm.Descriptions.ElementAt(0).Text);
            Assert.AreEqual("Description Form 1 Update", updateForm.Descriptions.ElementAt(0).Text);

            Assert.AreEqual(2, updateForm.Tags.Count());
            Assert.AreEqual(2, existingForm.Nacabels.ElementAt(0).Id);
            Assert.AreEqual(3, existingForm.Nacabels.ElementAt(1).Id);
            Assert.AreEqual(2, updateForm.Tags.ElementAt(0));
            Assert.AreEqual(3, updateForm.Tags.ElementAt(1));

            Assert.AreEqual(2, existingForm.FormCategoryId);
            Assert.AreEqual(2, updateForm.FormCategoryId);

            Assert.AreEqual(4, existingForm.FormDocuments.Count);
            Assert.AreEqual(4, updateForm.FormDocuments.Count());
            Assert.AreEqual("FileNameEn_update", existingForm.FormDocuments.FirstOrDefault(f => f.Id == formEdDocId)?.Documents.First().Name);
            Assert.AreEqual("http://127.0.0.1:10001/newpath", existingForm.FormDocuments.FirstOrDefault(f => f.Id == formEdDocId)?.Documents.First().Path);
            Assert.AreEqual(formKeepDocId, existingForm.FormDocuments.FirstOrDefault(f => f.Id == formKeepDocId)?.Documents.ElementAt(0).Id);
            Assert.AreEqual(0, existingForm.FormDocuments.FirstOrDefault(f => f.Id == formDelDocId)?.Documents.Count);
            Assert.IsTrue(existingForm.FormDocuments.Any(f => f.LanguageCode == "tw"));
            Assert.IsTrue(existingForm.FormDocuments.FirstOrDefault(f => f.LanguageCode == "tw")?.Documents.Any());
            Assert.AreEqual("http://127.0.0.1:10001/newpath", existingForm.FormDocuments.FirstOrDefault(f => f.LanguageCode == "tw")?.Documents.First().Path);
        }

        [TestMethod]
        public async Task UpdateFormAsyncTestWebform()
        {
            Guid formId = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E");
            var updateFormDTO = new DTO.Internal.Forms.UpdateFormDTO
            {
                Id = formId,
                IsActive = true,
                Version = 1,
                Tags = new[] { 2, 3 },
                FormCategoryId = 2,
                IsFormPdf = false,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> {
                    new TranslationDTO()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label Form 1 Update",
                    }
                },
                Urls = new List<DTO.Internal.Translation.TranslationDTO> {
                    new TranslationDTO()
                    {
                        Id = 2,
                        LanguageCode = "en",
                        Text = "url_form_1_update",
                    }
                },
                Descriptions = new List<DTO.Internal.Translation.TranslationDTO> {
                    new TranslationDTO()
                    {
                        Id = 3,
                        LanguageCode = "en",
                        Text = "Description Form 1 Update",
                    }
                }
            };
            var existingForm = new Form()
            {
                Id = formId,
                IsActive = true,
                Version = 1,
                FormCategoryId = 1,
                Type = FormType.Webform.ToString(),
                FormCategory = new FormCategory()
                {
                    Id = 1,
                    Code = "IAII"
                },
                Nacabels = new List<Nacabel>() {
                    new Nacabel()
                    {
                        Id = 1
                    },
                    new Nacabel()
                    {
                        Id = 2
                    }
                },
                Labels = new List<Translation> {
                    new Translation()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label Form 1",
                    }
                },
                Urls = new List<Translation> {
                    new Translation()
                    {
                        Id = 2,
                        LanguageCode = "en",
                        Text = "url_form_1",
                    }
                },
                Descriptions = new List<Translation> {
                    new Translation()
                    {
                        Id = 3,
                        LanguageCode = "en",
                        Text = "Description Form 1",
                    }
                }
            };

            var listFormMock = new List<Form>()
            {
                existingForm,
                new Form()
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Version = 1
                }
            };

            var nacabelMock = new List<Nacabel>(){ new Nacabel()
                {
                    Id = 3
                }
            };


            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            nacabelRepository.Find(Arg.Any<Expression<Func<Nacabel, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Nacabel, bool>>>();
                return nacabelMock.Where(exp.Compile()).AsQueryable();
            });

            unitOfWork.SaveChangesAsync().Returns(1);

            var updateForm = await bll.UpdateFormAsync(formId, updateFormDTO);

            Assert.AreEqual(formId, updateForm.Id);
            Assert.AreEqual(FormType.Webform.ToString(), updateForm.Type);

            Assert.AreEqual(1, updateForm.Labels.Count());
            Assert.AreEqual("Label Form 1 Update", existingForm.Labels.ElementAt(0).Text);
            Assert.AreEqual("Label Form 1 Update", updateForm.Labels.ElementAt(0).Text);

            Assert.AreEqual(1, updateForm.Urls.Count());
            Assert.AreEqual("url_form_1_update", existingForm.Urls.ElementAt(0).Text);
            Assert.AreEqual("url_form_1_update", updateForm.Urls.ElementAt(0).Text);

            Assert.AreEqual(1, updateForm.Descriptions.Count());
            Assert.AreEqual("Description Form 1 Update", existingForm.Descriptions.ElementAt(0).Text);
            Assert.AreEqual("Description Form 1 Update", updateForm.Descriptions.ElementAt(0).Text);

            Assert.AreEqual(2, updateForm.Tags.Count());
            Assert.AreEqual(2, existingForm.Nacabels.ElementAt(0).Id);
            Assert.AreEqual(3, existingForm.Nacabels.ElementAt(1).Id);
            Assert.AreEqual(2, updateForm.Tags.ElementAt(0));
            Assert.AreEqual(3, updateForm.Tags.ElementAt(1));

            Assert.AreEqual(2, existingForm.FormCategoryId);
            Assert.AreEqual(2, updateForm.FormCategoryId);
        }

        [TestMethod()]
        public async Task UpdateFormAsyncTestArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await bll.UpdateFormAsync(default, default);
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task UpdateFormAsyncTestKeyNotFoundException()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.UpdateFormAsync(Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E"), new UpdateFormDTO());
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task UpdateFormAsyncTestArgumentExceptionPdfEmpty()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                Guid formId = Guid.NewGuid();
                var updateFormDTO = new DTO.Internal.Forms.UpdateFormDTO
                {
                    Id = formId,
                    IsActive = true,
                    Version = 1,
                    Tags = new[] { 2, 3 },
                    FormCategoryId = 2,
                    IsFormPdf = true,
                    FormDocuments = new List<UpdateFormDocumentDTO>()
                    {
                        new UpdateFormDocumentDTO()
                        {
                            LanguageCode = "en",
                            Documents = new List<UpdateDocumentDTO>(){
                                new UpdateDocumentDTO() {
                                    Type = "application/pdf",
                                    Name = "FileNameEn_update",
                                    Base64 = ""
                                }
                            }
                        }
                    }
                };
                var existingForm = new Form()
                {
                    Id = formId,
                    IsActive = true,
                    Version = 1,
                    FormCategoryId = 1,
                    FormCategory = new FormCategory()
                    {
                        Id = 1,
                        Code = "IAII"
                    },
                    Nacabels = new List<Nacabel>() {
                        new Nacabel()
                        {
                            Id = 1
                        },
                        new Nacabel()
                        {
                            Id = 2
                        }
                    },
                    FormDocuments = new List<FormDocument>()
                };

                var listFormMock = new List<Form>()
            {
                existingForm,
                new Form()
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Version = 1
                }
            };

                var nacabelMock = new List<Nacabel>(){ new Nacabel()
                {
                    Id = 3
                }
            };


                formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
                {
                    var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                    return listFormMock.Where(exp.Compile()).AsQueryable();
                });

                nacabelRepository.Find(Arg.Any<Expression<Func<Nacabel, bool>>>()).Returns(callinfo =>
                {
                    var exp = callinfo.Arg<Expression<Func<Nacabel, bool>>>();
                    return nacabelMock.Where(exp.Compile()).AsQueryable();
                });


                var updateForm = await bll.UpdateFormAsync(formId, updateFormDTO);

            });
            await fileStorage.DidNotReceiveWithAnyArgs().ExistsAsync(Arg.Any<string>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task UpdateFormAsyncTestArgumentExceptionBase64FormatWrong()
        {
            await Assert.ThrowsExceptionAsync<FormatException>(async () =>
            {
                Guid formId = Guid.NewGuid();
                var updateFormDTO = new DTO.Internal.Forms.UpdateFormDTO
                {
                    Id = formId,
                    IsActive = true,
                    Version = 1,
                    Tags = new[] { 2, 3 },
                    FormCategoryId = 2,
                    IsFormPdf = true,
                    FormDocuments = new List<UpdateFormDocumentDTO>()
                    {
                        new UpdateFormDocumentDTO()
                        {
                            LanguageCode = "en",
                            Documents = new List<UpdateDocumentDTO>(){
                                new UpdateDocumentDTO() {
                                    Type = "application/pdf",
                                    Name = "FileNameEn_update",
                                    Base64 = "1234566778899"
                                }
                            }
                        }
                    }
                };
                var existingForm = new Form()
                {
                    Id = formId,
                    IsActive = true,
                    Version = 1,
                    FormCategoryId = 1,
                    FormCategory = new FormCategory()
                    {
                        Id = 1,
                        Code = "IAII"
                    },
                    Nacabels = new List<Nacabel>() {
                        new Nacabel()
                        {
                            Id = 1
                        },
                        new Nacabel()
                        {
                            Id = 2
                        }
                    },
                    FormDocuments = new List<FormDocument>()
                };

                var listFormMock = new List<Form>()
            {
                existingForm,
                new Form()
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Version = 1
                }
            };

                var nacabelMock = new List<Nacabel>(){ new Nacabel()
                {
                    Id = 3
                }
            };


                formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
                {
                    var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                    return listFormMock.Where(exp.Compile()).AsQueryable();
                });

                nacabelRepository.Find(Arg.Any<Expression<Func<Nacabel, bool>>>()).Returns(callinfo =>
                {
                    var exp = callinfo.Arg<Expression<Func<Nacabel, bool>>>();
                    return nacabelMock.Where(exp.Compile()).AsQueryable();
                });


                var updateForm = await bll.UpdateFormAsync(formId, updateFormDTO);

            });
            await fileStorage.DidNotReceiveWithAnyArgs().ExistsAsync(Arg.Any<string>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod]
        public void GetFormDetailTest()
        {
            Guid formId = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E");
            Guid docId1 = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6F");
            Guid docId2 = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F7F");
            
            var existingForm = new Form()
            {
                Id = formId,
                IsActive = true,
                Version = 1,
                FormCategoryId = 1,
                FormCategory = new FormCategory()
                {
                    Id = 1,
                    Code = "IAII"
                },
                Nacabels = new List<Nacabel>() {
                    new Nacabel()
                    {
                        Id = 1
                    },
                    new Nacabel()
                    {
                        Id = 2
                    }
                },
                Labels = new List<Translation> {
                    new Translation()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label Form 1",
                    }
                },
                Urls = new List<Translation> {
                    new Translation()
                    {
                        Id = 2,
                        LanguageCode = "en",
                        Text = "url_form_1",
                    }
                },
                Descriptions = new List<Translation> {
                    new Translation()
                    {
                        Id = 3,
                        LanguageCode = "en",
                        Text = "Description Form 1",
                    }
                },
                FormDocuments = new List<FormDocument>()
                {
                    new FormDocument()
                    {
                        Id = docId1,
                        LanguageCode = "nl",
                        Documents = new List<Document>(){
                            new Document() {
                                Id = docId1,
                                FormDocumentId = docId1,
                                Type = "application/pdf",
                                Name = "FileName1"
                            }
                        }
                    },
                    new FormDocument()
                    {
                        Id = docId2,
                        LanguageCode = "en",
                        Documents = new List<Document>(){
                            new Document() {
                                Id = docId2,
                                Type = "application/pdf",
                                Name = "FileName2"
                            }
                        }
                    }
                }
            };

            var listFormMock = new List<Form>()
            {
                existingForm
            };


            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });
            
            var result = bll.GetFormDetails(formId);

            Assert.AreEqual(existingForm.Id, result.Id);

            Assert.AreEqual(existingForm.Labels.Count(), result.Labels.Count());
            Assert.AreEqual(existingForm.Labels.ElementAt(0).Text, result.Labels.ElementAt(0).Text);

            Assert.AreEqual(1, result.Urls.Count());
            Assert.AreEqual(existingForm.Urls.ElementAt(0).Text, result.Urls.ElementAt(0).Text);

            Assert.AreEqual(1, result.Descriptions.Count());
            Assert.AreEqual(existingForm.Descriptions.ElementAt(0).Text, result.Descriptions.ElementAt(0).Text);

            Assert.AreEqual(2, result.Tags.Count());
            Assert.AreEqual(existingForm.Nacabels.ElementAt(0).Id, result.Tags.ElementAt(0));
            Assert.AreEqual(existingForm.Nacabels.ElementAt(1).Id, result.Tags.ElementAt(1));

            Assert.AreEqual(existingForm.FormCategoryId, result.FormCategoryId);

            Assert.AreEqual(2, result.FormDocuments.Count());
            Assert.AreEqual(existingForm.FormDocuments.ElementAt(0).Documents.ElementAt(0).Name, result.FormDocuments.ElementAt(0).Documents.ElementAt(0).Name);
            Assert.AreEqual(existingForm.FormDocuments.ElementAt(1).Documents.ElementAt(0).Name, result.FormDocuments.ElementAt(1).Documents.ElementAt(0).Name);
        }

        [TestMethod]
        public void GetFormDetailTestByUrl()
        {
            Guid formId = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E");
            Guid docId1 = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6F");
            Guid docId2 = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F7F");
            string shortUrl = "url_form_1";
            var existingForm = new Form()
            {
                Id = formId,
                IsActive = true,
                Version = 1,
                FormCategoryId = 1,
                FormCategory = new FormCategory()
                {
                    Id = 1,
                    Code = "IAII"
                },
                Nacabels = new List<Nacabel>() {
                    new Nacabel()
                    {
                        Id = 1
                    },
                    new Nacabel()
                    {
                        Id = 2
                    }
                },
                Labels = new List<Translation> {
                    new Translation()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label Form 1",
                    }
                },
                Urls = new List<Translation> {
                    new Translation()
                    {
                        Id = 2,
                        LanguageCode = "en",
                        Text = shortUrl,
                    }
                },
                Descriptions = new List<Translation> {
                    new Translation()
                    {
                        Id = 3,
                        LanguageCode = "en",
                        Text = "Description Form 1",
                    }
                },
                FormDocuments = new List<FormDocument>()
                {
                    new FormDocument()
                    {
                        Id = docId1,
                        LanguageCode = "nl",
                        Documents = new List<Document>(){
                            new Document() {
                                Id = docId1,
                                FormDocumentId = docId1,
                                Type = "application/pdf",
                                Name = "FileName1"
                            }
                        }
                    },
                    new FormDocument()
                    {
                        Id = docId2,
                        LanguageCode = "en",
                        Documents = new List<Document>(){
                            new Document() {
                                Id = docId2,
                                Type = "application/pdf",
                                Name = "FileName2"
                            }
                        }
                    }
                }
            };

            var listFormMock = new List<Form>()
            {
                existingForm
            };


            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var result = bll.GetFormDetailsByFormUrl(shortUrl);

            Assert.AreEqual(existingForm.Id, result.Id);

            Assert.AreEqual(existingForm.Labels.Count, result.Labels.Count());
            Assert.AreEqual(existingForm.Labels.ElementAt(0).Text, result.Labels.ElementAt(0).Text);

            Assert.AreEqual(1, result.Urls.Count());
            Assert.AreEqual(existingForm.Urls.ElementAt(0).Text, result.Urls.ElementAt(0).Text);

            Assert.AreEqual(1, result.Descriptions.Count());
            Assert.AreEqual(existingForm.Descriptions.ElementAt(0).Text, result.Descriptions.ElementAt(0).Text);

            Assert.AreEqual(2, result.Tags.Count());
            Assert.AreEqual(existingForm.Nacabels.ElementAt(0).Id, result.Tags.ElementAt(0));
            Assert.AreEqual(existingForm.Nacabels.ElementAt(1).Id, result.Tags.ElementAt(1));

            Assert.AreEqual(existingForm.FormCategoryId, result.FormCategoryId);

            Assert.AreEqual(2, result.FormDocuments.Count());
            Assert.AreEqual(existingForm.FormDocuments.ElementAt(0).Documents.ElementAt(0).Name, result.FormDocuments.ElementAt(0).Documents.ElementAt(0).Name);
            Assert.AreEqual(existingForm.FormDocuments.ElementAt(1).Documents.ElementAt(0).Name, result.FormDocuments.ElementAt(1).Documents.ElementAt(0).Name);
        }

        private List<Form> MockForms()
        {
            var forms = new List<Form>()
            {
                new Form
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d61"),
                    ExternalId = 1,
                    Status = FormStatus.Offline,
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d61"),
                            Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                            Type = "Section",
                            FieldType = "Text",
                            Order = 1
                        }
                    }
                },
                new Form
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d62"),
                    ExternalId = 1,
                    Status = FormStatus.Draft,
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d62"),
                            Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                            Type = "Section",
                            FieldType = "Text",
                            Order = 1
                        }
                    }
                },
                new Form
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d63"),
                    ExternalId = 1,
                    Status = FormStatus.Online,
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d63"),
                            Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                            Type = "Section",
                            FieldType = "Text",
                            Order = 1
                        }
                    }
                }
            };
            return forms;
        }
        
        [TestMethod]
        public async Task PublisFormTest_StatusDraft()
        {
            // arrange
            var param = new PublishFormDTO
            {
                ExternalId = 1,
                Status = FormStatus.Draft.ToString()
            };

            var forms = MockForms();

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            var result = await bll.PublisForm(new Guid("f74091a3-453a-4139-82a1-4cae27913d62"), param);

            formRepository.ReceivedWithAnyArgs(2).Update(Arg.Any<Form>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }
        
        [TestMethod]
        public async Task PublisFormTest_StatusOffline()
        {
            // arrange
            var param = new PublishFormDTO
            {
                ExternalId = 2,
                Status = FormStatus.Offline.ToString()
            };

            var newForms = new List<Form>()
            {
                new Form
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d62"),
                    ExternalId = 2,
                    Status = FormStatus.Draft,
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d62"),
                            Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                            Type = "Section",
                            FieldType = "Text",
                            Order = 1
                        }
                    }
                },
                new Form
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-8ea094c2d572"),
                    ExternalId = 2,
                    Status = FormStatus.Offline,
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            FormId = new Guid("f74091a3-453a-4139-82a1-8ea094c2d572"),
                            Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                            Type = "Section",
                            FieldType = "Text",
                            Order = 1
                        }
                    }
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return newForms.Where(exp.Compile()).AsQueryable();
            });
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            var result = await bll.PublisForm(new Guid("f74091a3-453a-4139-82a1-8ea094c2d572"), param);

            formRepository.ReceivedWithAnyArgs(1).Update(Arg.Any<Form>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod]
        public async Task PublisFormTest_StatusOffline_ErrorArgumentException()
        {
            // arrange
            var param = new PublishFormDTO
            {
                ExternalId = 1,
                Status = FormStatus.Offline.ToString()
            };

            var forms = MockForms();
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(forms.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.PublisForm(new Guid("f74091a3-453a-4139-82a1-4cae27913d63"), param);
            });
        }

        [TestMethod]
        public async Task PublisFormTest_EmptyFormNodes()
        {
            // arrange
            var param = new PublishFormDTO
            {
                ExternalId = 2,
                Status = FormStatus.Offline.ToString()
            };

            var newForms = new List<Form>()
            {
                new Form
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d62"),
                    ExternalId = 2,
                    Status = FormStatus.Draft,
                    FormNodes = new List<FormNodes>()
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(newForms.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.PublisForm(new Guid("f74091a3-453a-4139-82a1-4cae27913d62"), param);
            });
        }
        
        [TestMethod()]
        public async Task PublisFormTest_ExceptionOnSaveChange()
        {
            // arrange
            var param = new PublishFormDTO
            {
                ExternalId = 1,
                Status = FormStatus.Draft.ToString()
            };
            var forms = MockForms();
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.PublisForm(new Guid("f74091a3-453a-4139-82a1-4cae27913d62"), param);
            });
            await unitOfWork.ReceivedWithAnyArgs(1).SaveChangesAsync();
        }

        [TestMethod]
        public async Task SubmitFormAsync_WebForm_SuccessfullTest()
        {
            var encryptedValue = "e5t879y8huogivgctd759ed97f68t79yhuojjhvutyrut";

            aESEncryptService.EncryptString(Arg.Any<string>()).Returns(encryptedValue);

			unitOfWork.SaveChangesAsync().Returns(1);

            await bll.SubmitFormRequestAsync(this.GenerateSubmitFormRequestDto(FormSubmissionType.Webform));

            aESEncryptService.Received(1).EncryptString(Arg.Any<string>());
            await fileStorage.DidNotReceiveWithAnyArgs().SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<string>());

			await unitOfWork.Received(1).SaveChangesAsync();
		}

		[TestMethod()]
		public async Task SubmitFormAsync_PdfForm_SuccessfullTest()
		{
			var pathFile = "http://localhost.com/document/1en.pdf";

			fileStorage.SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(pathFile);

			unitOfWork.SaveChangesAsync().Returns(1);

			await bll.SubmitFormRequestAsync(this.GenerateSubmitFormRequestDto(FormSubmissionType.Pdf));

            await fileStorage.Received(1).SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<string>());
			aESEncryptService.DidNotReceiveWithAnyArgs().EncryptString(Arg.Any<string>());

			await unitOfWork.Received(1).SaveChangesAsync();
		}

		[TestMethod()]
		public async Task SubmitFormAsync_NullParameter_FailedTest()
		{
			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
			{
				await bll.SubmitFormRequestAsync(default);
			});

			await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
		}

		[TestMethod()]
		public async Task SubmitFormAsync_NotSaving_FailedTest()
		{
			unitOfWork.SaveChangesAsync().Returns(0);

			await Assert.ThrowsExceptionAsync<Exception>(async () =>
			{
				await bll.SubmitFormRequestAsync(this.GenerateSubmitFormRequestDto(FormSubmissionType.Webform));
			});

			await unitOfWork.Received(1).SaveChangesAsync();
		}

		[TestMethod()]
		public async Task SubmitFormAsync_PdfForm_InvalidBase64_FailedTest()
		{
            var dto = this.GenerateSubmitFormRequestDto(FormSubmissionType.Pdf);
            dto.Value = "test invalid base64";

			unitOfWork.SaveChangesAsync().Returns(1);

			await Assert.ThrowsExceptionAsync<FormatException>(async () =>
			{
				await bll.SubmitFormRequestAsync(dto);
			});

			await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
		}

		private CreateFormRequestDTO GenerateSubmitFormRequestDto(FormSubmissionType type)
        {
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");

			var submitDto = new CreateFormRequestDTO
			{
				FormId = formId,
				CompanyName = "Test Company",
				CompanyType = "LE",
				Email = "test@gmail.com",
				FancOrganisationId = "5C6BEB26-CFE2-49FF-A77F-36DCFACC1F69",
				FormSubmissionType = type,
				UserFullName = "John Doe",
				UserId = "13",
				Value = type == FormSubmissionType.Webform ? "{ \"name\": \"john doe\" }" : "dGVzdA==", 
				Version = 1
			};

            return submitDto;
		}

		[TestMethod]
		public async Task DownloadPdfForm_Targetlanguage_SuccessfullTest()
		{
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");
			var fileContent = new byte[0];
			var mimetype = "application/pdf";
			var englishDocument = "ENFileName2";
			var dutchDocument = "NLFileName2";
			var resultDocument = "ENFileName2.pdf";

			var forms = new List<Form>
			{
				new Form()
				{
					Id = formId,
					IsActive = true,
					Version = 1,
					FormCategoryId = 1,
					Type = FormType.Pdf.ToString(),
					FormCategory = new FormCategory()
					{
						Id = 1,
						Code = "IAII"
					},
					Nacabels = new List<Nacabel>() {
						new Nacabel()
						{
							Id = 1
						},
						new Nacabel()
						{
							Id = 2
						}
					},
					Labels = new List<Translation> {
						new Translation()
						{
							Id = 1,
							LanguageCode = "en",
							Text = "Label Form 1",
						}
					},
					Urls = new List<Translation> {
						new Translation()
						{
							Id = 2,
							LanguageCode = "en",
							Text = "url_form_1",
						}
					},
					Descriptions = new List<Translation> {
						new Translation()
						{
							Id = 3,
							LanguageCode = "en",
							Text = "Description Form 1",
						}
					},
					FormDocuments = new List<FormDocument>()
					{
						new FormDocument()
						{
							Id = Guid.Parse("611BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "nl",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("612BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = dutchDocument,
									Path = "localpath/doc.pdf"
								}
							}
						},
						new FormDocument()
						{
							Id = Guid.Parse("621BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "en",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("622BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = englishDocument,
									Path = "localpath/doc.pdf"
								}
							}
						}
					}
				}
			};

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(forms.AsQueryable());

			fileStorage.ExistsAsync(Arg.Any<string>()).Returns(true);
			fileStorage.GetAsync(Arg.Any<string>()).Returns(fileContent);

			var dto = await bll.DownloadAsync(formId, "en");

			await fileStorage.Received(1).ExistsAsync(Arg.Any<string>());
			await fileStorage.Received(1).GetAsync(Arg.Any<string>());

			Assert.AreEqual(dto.Content, fileContent);
			Assert.AreEqual(dto.MimeType, mimetype);
			Assert.AreEqual(dto.Name, resultDocument);
		}

        [TestMethod]
		public async Task DownloadPdfForm_UseTheOnlyLanguage_SuccessfullTest()
		{
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");
			var fileContent = new byte[0];
			var mimetype = "application/pdf";
			var dutchDocument = "NLFileName2";
			var resultDocument = "NLFileName2.pdf";

			var forms = new List<Form>
			{
				new Form()
				{
					Id = formId,
					IsActive = true,
					Version = 1,
					FormCategoryId = 1,
					Type = FormType.Pdf.ToString(),
					FormCategory = new FormCategory()
					{
						Id = 1,
						Code = "IAII"
					},
					Nacabels = new List<Nacabel>() {
						new Nacabel()
						{
							Id = 1
						},
						new Nacabel()
						{
							Id = 2
						}
					},
					Labels = new List<Translation> {
						new Translation()
						{
							Id = 1,
							LanguageCode = "en",
							Text = "Label Form 1",
						}
					},
					Urls = new List<Translation> {
						new Translation()
						{
							Id = 2,
							LanguageCode = "en",
							Text = "url_form_1",
						}
					},
					Descriptions = new List<Translation> {
						new Translation()
						{
							Id = 3,
							LanguageCode = "en",
							Text = "Description Form 1",
						}
					},
					FormDocuments = new List<FormDocument>()
					{
						new FormDocument()
						{
							Id = Guid.Parse("611BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "nl",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("612BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = dutchDocument,
									Path = "localpath/doc.pdf"
								}
							}
						}
					}
				}
			};

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(forms.AsQueryable());

			fileStorage.ExistsAsync(Arg.Any<string>()).Returns(true);
			fileStorage.GetAsync(Arg.Any<string>()).Returns(fileContent);

			var dto = await bll.DownloadAsync(formId, "en");

			await fileStorage.Received(1).ExistsAsync(Arg.Any<string>());
			await fileStorage.Received(1).GetAsync(Arg.Any<string>());

			Assert.AreEqual(dto.Content, fileContent);
			Assert.AreEqual(dto.MimeType, mimetype);
			Assert.AreEqual(dto.Name, resultDocument);
		}

        [TestMethod]
        public async Task DownloadPdfForm_MissingLanguageWhenDocumentHasMoreThanOneLanguage_FailedTest()
        {
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");
			var fileContent = new byte[0];
			var mimetype = "application/pdf";
			var englishDocument = "ENFileName2";
			var dutchDocument = "NLFileName2";
			var resultDocument = "ENFileName2.pdf";

			var forms = new List<Form>
			{
				new Form()
				{
					Id = formId,
					IsActive = true,
					Version = 1,
					FormCategoryId = 1,
					Type = FormType.Pdf.ToString(),
					FormCategory = new FormCategory()
					{
						Id = 1,
						Code = "IAII"
					},
					Nacabels = new List<Nacabel>() {
						new Nacabel()
						{
							Id = 1
						},
						new Nacabel()
						{
							Id = 2
						}
					},
					Labels = new List<Translation> {
						new Translation()
						{
							Id = 1,
							LanguageCode = "en",
							Text = "Label Form 1",
						}
					},
					Urls = new List<Translation> {
						new Translation()
						{
							Id = 2,
							LanguageCode = "en",
							Text = "url_form_1",
						}
					},
					Descriptions = new List<Translation> {
						new Translation()
						{
							Id = 3,
							LanguageCode = "en",
							Text = "Description Form 1",
						}
					},
					FormDocuments = new List<FormDocument>()
					{
						new FormDocument()
						{
							Id = Guid.Parse("611BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "nl",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("612BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = dutchDocument,
									Path = "localpath/doc.pdf"
								}
							}
						},
						new FormDocument()
						{
							Id = Guid.Parse("621BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "en",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("622BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = englishDocument,
									Path = "localpath/doc.pdf"
								}
							}
						}
					}
				}
			};

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(forms.AsQueryable());

			fileStorage.ExistsAsync(Arg.Any<string>()).Returns(true);
			fileStorage.GetAsync(Arg.Any<string>()).Returns(fileContent);

			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
			{
				await bll.DownloadAsync(formId, "fr");
			});

			await fileStorage.DidNotReceiveWithAnyArgs().ExistsAsync(Arg.Any<string>());
			await fileStorage.DidNotReceiveWithAnyArgs().GetAsync(Arg.Any<string>());
		}

		[TestMethod]
		public async Task DownloadPdfForm_DocumentPathNotExists_FailedTest()
		{
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");
			var fileContent = new byte[0];
			var mimetype = "application/pdf";
			var englishDocument = "ENFileName2";
			var dutchDocument = "NLFileName2";
			var resultDocument = "ENFileName2.pdf";

			var forms = new List<Form>
			{
				new Form()
				{
					Id = formId,
					IsActive = true,
					Version = 1,
					FormCategoryId = 1,
					Type = FormType.Pdf.ToString(),
					FormCategory = new FormCategory()
					{
						Id = 1,
						Code = "IAII"
					},
					Nacabels = new List<Nacabel>() {
						new Nacabel()
						{
							Id = 1
						},
						new Nacabel()
						{
							Id = 2
						}
					},
					Labels = new List<Translation> {
						new Translation()
						{
							Id = 1,
							LanguageCode = "en",
							Text = "Label Form 1",
						}
					},
					Urls = new List<Translation> {
						new Translation()
						{
							Id = 2,
							LanguageCode = "en",
							Text = "url_form_1",
						}
					},
					Descriptions = new List<Translation> {
						new Translation()
						{
							Id = 3,
							LanguageCode = "en",
							Text = "Description Form 1",
						}
					},
					FormDocuments = new List<FormDocument>()
					{
						new FormDocument()
						{
							Id = Guid.Parse("611BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "nl",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("612BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = dutchDocument,
									Path = "localpath/doc.pdf"
								}
							}
						},
						new FormDocument()
						{
							Id = Guid.Parse("621BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "en",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("622BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = englishDocument,
									Path = "localpath/doc.pdf"
								}
							}
						}
					}
				}
			};

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(forms.AsQueryable());

			fileStorage.ExistsAsync(Arg.Any<string>()).Returns(false);
			fileStorage.GetAsync(Arg.Any<string>()).Returns(fileContent);

			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
			{
				await bll.DownloadAsync(formId, "en");
			});

			await fileStorage.Received(1).ExistsAsync(Arg.Any<string>());
			await fileStorage.DidNotReceiveWithAnyArgs().GetAsync(Arg.Any<string>());
		}

		[TestMethod]
		public async Task DownloadPdfForm_DocumentPathEmpty_FailedTest()
		{
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");
			var fileContent = new byte[0];
			var mimetype = "application/pdf";
			var englishDocument = "ENFileName2";
			var dutchDocument = "NLFileName2";
			var resultDocument = "ENFileName2.pdf";

			var forms = new List<Form>
			{
				new Form()
				{
					Id = formId,
					IsActive = true,
					Version = 1,
					FormCategoryId = 1,
					Type = FormType.Pdf.ToString(),
					FormCategory = new FormCategory()
					{
						Id = 1,
						Code = "IAII"
					},
					Nacabels = new List<Nacabel>() {
						new Nacabel()
						{
							Id = 1
						},
						new Nacabel()
						{
							Id = 2
						}
					},
					Labels = new List<Translation> {
						new Translation()
						{
							Id = 1,
							LanguageCode = "en",
							Text = "Label Form 1",
						}
					},
					Urls = new List<Translation> {
						new Translation()
						{
							Id = 2,
							LanguageCode = "en",
							Text = "url_form_1",
						}
					},
					Descriptions = new List<Translation> {
						new Translation()
						{
							Id = 3,
							LanguageCode = "en",
							Text = "Description Form 1",
						}
					},
					FormDocuments = new List<FormDocument>()
					{
						new FormDocument()
						{
							Id = Guid.Parse("611BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "nl",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("612BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = dutchDocument,
                                    Path = string.Empty
								}
							}
						},
						new FormDocument()
						{
							Id = Guid.Parse("621BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "en",
							Documents = new List<Document>(){
								new Document() {
									Id = Guid.Parse("622BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
									Type = mimetype,
									Name = englishDocument,
									Path = string.Empty
								}
							}
						}
					}
				}
			};

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(forms.AsQueryable());

			fileStorage.ExistsAsync(Arg.Any<string>()).Returns(false);
			fileStorage.GetAsync(Arg.Any<string>()).Returns(fileContent);

			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
			{
				await bll.DownloadAsync(formId, "en");
			});

			await fileStorage.DidNotReceiveWithAnyArgs().ExistsAsync(Arg.Any<string>());
			await fileStorage.DidNotReceiveWithAnyArgs().GetAsync(Arg.Any<string>());
		}

		[TestMethod]
		public async Task DownloadPdfForm_DocumentNulled_FailedTest()
		{
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");
			var fileContent = new byte[0];
			var mimetype = "application/pdf";
			var englishDocument = "ENFileName2";
			var dutchDocument = "NLFileName2";
			var resultDocument = "ENFileName2.pdf";

			var forms = new List<Form>
			{
				new Form()
				{
					Id = formId,
					IsActive = true,
					Version = 1,
					FormCategoryId = 1,
					Type = FormType.Pdf.ToString(),
					FormCategory = new FormCategory()
					{
						Id = 1,
						Code = "IAII"
					},
					Nacabels = new List<Nacabel>() {
						new Nacabel()
						{
							Id = 1
						},
						new Nacabel()
						{
							Id = 2
						}
					},
					Labels = new List<Translation> {
						new Translation()
						{
							Id = 1,
							LanguageCode = "en",
							Text = "Label Form 1",
						}
					},
					Urls = new List<Translation> {
						new Translation()
						{
							Id = 2,
							LanguageCode = "en",
							Text = "url_form_1",
						}
					},
					Descriptions = new List<Translation> {
						new Translation()
						{
							Id = 3,
							LanguageCode = "en",
							Text = "Description Form 1",
						}
					},
					FormDocuments = new List<FormDocument>()
					{
						new FormDocument()
						{
							Id = Guid.Parse("611BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "nl",
							Documents = new List<Document>()
						},
						new FormDocument()
						{
							Id = Guid.Parse("621BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "en",
							Documents = new List<Document>()
						}
					}
				}
			};

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(forms.AsQueryable());

			fileStorage.ExistsAsync(Arg.Any<string>()).Returns(false);
			fileStorage.GetAsync(Arg.Any<string>()).Returns(fileContent);

			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
			{
				await bll.DownloadAsync(formId, "en");
			});

			await fileStorage.DidNotReceiveWithAnyArgs().ExistsAsync(Arg.Any<string>());
			await fileStorage.DidNotReceiveWithAnyArgs().GetAsync(Arg.Any<string>());
		}

		[TestMethod]
		public async Task DownloadPdfForm_NonPdfForm_FailedTest()
		{
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");
			var fileContent = new byte[0];
			var mimetype = "application/pdf";
			var englishDocument = "ENFileName2";
			var dutchDocument = "NLFileName2";
			var resultDocument = "ENFileName2.pdf";

			var forms = new List<Form>
			{
				new Form()
				{
					Id = formId,
					IsActive = true,
					Version = 1,
					FormCategoryId = 1,
                    Type = FormType.Webform.ToString(),
					FormCategory = new FormCategory()
					{
						Id = 1,
						Code = "IAII"
					},
					Nacabels = new List<Nacabel>() {
						new Nacabel()
						{
							Id = 1
						},
						new Nacabel()
						{
							Id = 2
						}
					},
					Labels = new List<Translation> {
						new Translation()
						{
							Id = 1,
							LanguageCode = "en",
							Text = "Label Form 1",
						}
					},
					Urls = new List<Translation> {
						new Translation()
						{
							Id = 2,
							LanguageCode = "en",
							Text = "url_form_1",
						}
					},
					Descriptions = new List<Translation> {
						new Translation()
						{
							Id = 3,
							LanguageCode = "en",
							Text = "Description Form 1",
						}
					},
					FormDocuments = new List<FormDocument>()
					{
						new FormDocument()
						{
							Id = Guid.Parse("611BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "nl",
							Documents = new List<Document>()
						},
						new FormDocument()
						{
							Id = Guid.Parse("621BEB26-CFE2-49FF-A07F-36DCFACC1F69"),
							LanguageCode = "en",
							Documents = new List<Document>()
						}
					}
				}
			};

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(forms.AsQueryable());

			fileStorage.ExistsAsync(Arg.Any<string>()).Returns(false);
			fileStorage.GetAsync(Arg.Any<string>()).Returns(fileContent);

			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
			{
				await bll.DownloadAsync(formId, "en");
			});

			await fileStorage.DidNotReceiveWithAnyArgs().ExistsAsync(Arg.Any<string>());
			await fileStorage.DidNotReceiveWithAnyArgs().GetAsync(Arg.Any<string>());
		}

		[TestMethod]
		public async Task DownloadPdfForm_FormNotFound_FailedTest()
		{
			Guid formId = Guid.Parse("666BEB26-CFE2-49FF-A07F-36DCFACC1F69");

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns((new List<Form>()).AsQueryable());

			fileStorage.ExistsAsync(Arg.Any<string>()).Returns(false);
			fileStorage.GetAsync(Arg.Any<string>()).Returns(new byte[0]);

			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
			{
				await bll.DownloadAsync(formId, "en");
			});

			await fileStorage.DidNotReceiveWithAnyArgs().ExistsAsync(Arg.Any<string>());
			await fileStorage.DidNotReceiveWithAnyArgs().GetAsync(Arg.Any<string>());
		}

        [TestMethod()]
        public async Task UnpublishFormTest()
        {
            var forms = MockForms();
            var param = new PublishFormDTO
            {
                ExternalId = 1,
                Status = FormStatus.Online.ToString()
            };
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d63");
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            var result = await bll.UnpublisForm(formId, param);
            formRepository.ReceivedWithAnyArgs(1).Update(Arg.Any<Form>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UnpublishFormTest_FormIdNotFound()
        {
            var forms = MockForms();
            var param = new PublishFormDTO
            {
                ExternalId = 1,
                Status = FormStatus.Online.ToString()
            };
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d64");
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            try
            {
                var result = await bll.UnpublisForm(formId, param);
            }
            catch(Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"Form with ID {formId} not found", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task UnpublishFormTest_FailedUpdate()
        {
            var forms = MockForms();
            var param = new PublishFormDTO
            {
                ExternalId = 1,
                Status = FormStatus.Online.ToString()
            };
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d63");
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });
            try
            {
                var result = await bll.UnpublisForm(formId, param);
            }
            catch (Exception ex)
            {
                unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(0);
                Assert.AreEqual($"no change has been commited, Unpublish form not affected", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task UnpublishFormTest_ExceptionAlreadyHasNewestVersion()
        {
            var forms = MockForms();
            var param = new PublishFormDTO
            {
                ExternalId = 1,
                Status = FormStatus.Offline.ToString()
            };
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d61");
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            try
            {
                var result = await bll.UnpublisForm(formId, param);
            }
            catch(Exception ex)
            {
                var onlineFormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d63");
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"This form has been updated, please click ${onlineFormId} to go to the most recent version.", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UnpublishFormTest_ExceptionFormOnlineNotAvailable()
        {
            var forms = new List<Form>()
            {
                new Form
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d61"),
                    ExternalId = 2,
                    Status = FormStatus.Offline,
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d61"),
                            Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                            Type = "Section",
                            FieldType = "Text",
                            Order = 1
                        }
                    }
                },
                new Form
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d62"),
                    ExternalId = 2,
                    Status = FormStatus.Draft,
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d62"),
                            Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                            Type = "Section",
                            FieldType = "Text",
                            Order = 1
                        }
                    }
                }
            };
            var param = new PublishFormDTO
            {
                ExternalId = 1,
                Status = FormStatus.Offline.ToString()
            };
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d61");
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            try
            {
                var result = await bll.UnpublisForm(formId, param);
            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"This form is not available anymore, please use our wizard to find the form you need", ex.Message);
                throw;
            }
        }
    }
}

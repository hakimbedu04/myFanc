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
using MyFanc.Core;
using MyFanc.DTO.Internal.Forms;
using MyFanc.DTO.Internal.Translation;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using Newtonsoft.Json;
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
    public class MyFanc_Form_Edit_Form_Content_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IGenericRepository<Form> formRepository;
        private IGenericRepository<Nacabel> nacabelRepository;
        private IGenericRepository<FormNodes> formNodesRepository;
        private IGenericRepository<FormNodeFields> formNodeFieldsRepository;
        private IGenericRepository<FormConditionals> formConditionalsRepository;
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
            this.formRepository = Substitute.For<IGenericRepository<Form>>();
            this.formNodesRepository = Substitute.For<IGenericRepository<FormNodes>>();
            this.formNodeFieldsRepository = Substitute.For<IGenericRepository<FormNodeFields>>();
            this.nacabelRepository = Substitute.For<IGenericRepository<Nacabel>>();
            this.formConditionalsRepository= Substitute.For<IGenericRepository<FormConditionals>>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.sharedDataCache = Substitute.For<ISharedDataCache>();
			this.identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();

			this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();

            this.unitOfWork.GetGenericRepository<Form>().Returns(this.formRepository);
            this.unitOfWork.GetGenericRepository<Nacabel>().Returns(this.nacabelRepository);
            this.unitOfWork.GetGenericRepository<FormNodes>().Returns(this.formNodesRepository);
            this.unitOfWork.GetGenericRepository<FormNodeFields>().Returns(this.formNodeFieldsRepository);
            this.unitOfWork.GetGenericRepository<FormConditionals>().Returns(this.formConditionalsRepository);

            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new WizardProfile());
                cfg.AddProfile(new FormProfile());
            });

            this.mapper = new Mapper(mapperConfig);

            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();
            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        [TestMethod]
        public async Task CreateFormNodeAsyncTest()
        {
            var formId = Guid.NewGuid();
            var newId = Guid.NewGuid();
            var createUpdateFormNodeDTO = new CreateUpdateFormNodeDto()
            {
                Id = null,
                IsActive = true,
                Labels = new List<TranslationDTO>()
                {
                    new TranslationDTO() { LanguageCode = "en", Text = "FormNode 1 en" },
                    new TranslationDTO() { LanguageCode = "fr", Text = "FormNode 1 fr" }

                },
                Version = 1,
                FormId = formId,
                ParentId = null,
                Type = Enums.FormNodeType.Section.ToString(),
                FieldType = "",
                Order = 1
            };

            var mockFormNodesInDb = new List<FormNodes>() { new FormNodes()
                {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    Order = 1,
                }
            };

            var mockFormInDb = new List<Form>()
            {
                new Form()
                {
                    Id = formId,
                    Status = FormStatus.Draft
                }
            };

			formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(mockFormInDb.AsQueryable());
            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).ReturnsForAnyArgs(mockFormNodesInDb.AsQueryable());
            formNodesRepository.When(f => f.Add(Arg.Any<FormNodes>())).Do(c => c.ArgAt<FormNodes>(0).Id = newId);
			unitOfWork.SaveChangesAsync().Returns(1);

            var result = await bll.CreateUpdateFormNodeAsync(createUpdateFormNodeDTO);
            formRepository.Received(2).Find(Arg.Any<Expression<Func<Form, bool>>>());
            formNodesRepository.Received(2).Find(Arg.Any<Expression<Func<FormNodes, bool>>>());
            formNodesRepository.Received(1).Add(Arg.Any<FormNodes>());
            await unitOfWork.Received(1).SaveChangesAsync();
            
            Assert.IsTrue(result.GetType().Equals(typeof(Guid)));
            Assert.AreEqual(newId, result);
        }

        [TestMethod]
        public async Task UpdateFormNodeAsyncTest()
        {
            var formId = Guid.NewGuid();
            var formNodeId = Guid.NewGuid();
            var createUpdateFormNodeDTO = new CreateUpdateFormNodeDto()
            {
                Id = formNodeId,
                IsActive = true,
                Labels = new List<TranslationDTO>()
                {
                    new TranslationDTO() { Id = 1, LanguageCode = "en", Text = "FormNode 1 en update" },
                    new TranslationDTO() { LanguageCode = "fr", Text = "FormNode 1 fr add new" }

                },
                Version = 1,
                FormId = formId,
                ParentId = null,
                Type = Enums.FormNodeType.FormField.ToString(),
                FieldType = Enums.FormNodeFieldType.Text.ToString(),
                Order = 1
            };

            var formNodeEntityDbMock = new FormNodes()
            {
                Id = formNodeId,
                IsActive = true,
                Labels = new List<Translation>()
                    {
                        new Translation() { Id = 1, LanguageCode = "en", Text = "FormNode 1 en" }

                    },
                Version = 1,
                FormId = formId,
                ParentId = null,
                Type = Enums.FormNodeType.FormField.ToString(),
                FieldType = Enums.FormNodeFieldType.Text.ToString(),
                Order = 1
            };

            var mockFormNodesInDb = new List<FormNodes>() { formNodeEntityDbMock };
            var mockFormInDb = new List<Form>()
            {
                new Form()
                {
                    Id = formId,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Draft
                }
            };

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(mockFormInDb.AsQueryable());
            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).ReturnsForAnyArgs(mockFormNodesInDb.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);

            var result = await bll.CreateUpdateFormNodeAsync(createUpdateFormNodeDTO);
            
            formNodesRepository.Received(1).Find(Arg.Any<Expression<Func<FormNodes, bool>>>());
            await unitOfWork.Received(1).SaveChangesAsync();
            Assert.AreEqual(formNodeId, result);
            Assert.AreEqual(formNodeEntityDbMock.Labels.Count(), createUpdateFormNodeDTO.Labels.Count());
            Assert.AreEqual(formNodeEntityDbMock.Labels.ElementAt(0).Text, createUpdateFormNodeDTO.Labels.ElementAt(0).Text);
            Assert.AreEqual(formNodeEntityDbMock.Labels.ElementAt(1).Text, createUpdateFormNodeDTO.Labels.ElementAt(1).Text);
        }

        [TestMethod()]
        public async Task CreateUpdateFormNodeAsyncTestArgumentExeption()
        {
            var listFormMock = new List<Form>()
            {
                new Form()
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d67"),
                    ExternalId = 1,
                    Type = FormType.Webform.ToString()
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });
            CreateUpdateFormNodeDto createUpdateFormNodeDTO = new CreateUpdateFormNodeDto
            {
                FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d67")
            };
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.CreateUpdateFormNodeAsync(createUpdateFormNodeDTO);
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUpdateFormNodeAsyncTest_ToPdfType_ArgumentException()
        {
            var listFormMock = new List<Form>()
            {
                new Form()
                {
                    Id = new Guid("f74091a3-453a-4139-82a1-4cae27913d67"),
                    ExternalId = 1,
                    Type = FormType.Pdf.ToString()
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });
            CreateUpdateFormNodeDto createUpdateFormNodeDTO = new CreateUpdateFormNodeDto
            {
                FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d67")
            };
            try
            {
                await bll.CreateUpdateFormNodeAsync(createUpdateFormNodeDTO);
            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"Form with id {createUpdateFormNodeDTO.FormId} is a PDF type and cannot have form content!", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        public async Task CreateUpdateFormNodeAsyncTestArgumentExeption2()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.CreateUpdateFormNodeAsync(new CreateUpdateFormNodeDto() { Id = Guid.NewGuid() });
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUpdateFormNodeAsync_ArgumentException()
        {
            var mockFormInDb = new List<Form>()
            {
                new Form()
                {
                    Id = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482"),
                    Status = FormStatus.Online,
                    Type = FormType.Block.ToString()
                }
            };

            var formNodeFeildEntityDbMock = new FormNodeFields()
            {
                Id = new Guid("7055787d-28df-4d9a-9a75-09c49c99a489"),
                Property = "Value",
                Type = FormNodeFieldEncodeType.Container.ToString(),
                Value = "7055787d-28df-4d9a-9a75-09c49c99a482"
            };

            var mockFormNodeFieldsInDb = new List<FormNodeFields>() { formNodeFeildEntityDbMock };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(mockFormInDb.AsQueryable());
            formNodeFieldsRepository.Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>()).ReturnsForAnyArgs(mockFormNodeFieldsInDb.AsQueryable());
            try
            {
                var param = new CreateUpdateFormNodeDto
                {
                    FormId = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482"),
                    Type = FormNodeType.FormBlock.ToString(),
                };
                await bll.CreateUpdateFormNodeAsync(param);
            }
            catch (Exception ex)
            {
                formRepository.Received(2).Find(Arg.Any<Expression<Func<Form, bool>>>());
                formNodeFieldsRepository.Received(0).Add(Arg.Any<FormNodeFields>());
                Assert.AreEqual($"Form block linked to form with status Online or Offline can't be modified.", ex.Message);
                throw;
            }
        }

        private static CreateUpdateFormContentDto CreateUpdateFormContentAsyncMock()
        {
            return new CreateUpdateFormContentDto
            {
                FormNodes = new List<CreateUpdateFormNodesDto>
                {
                    new CreateUpdateFormNodesDto
                    {
                        FormId = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482"),
                        Id = new Guid("51d0fc83-5606-47c5-9279-68f6358592b8"),
                        Type = FormNodeType.Section.ToString(),
                        Labels = new List<TranslationDTO>
                        {
                            new TranslationDTO
                            {
                                LanguageCode = "en",
                                Text = "section 1"
                            }
                        },
                        Order = 0,
                        FormNodes = new List<CreateUpdateFormNodesItemDto>
                        {
                            new CreateUpdateFormNodesItemDto
                            {
                                Id = new Guid("A75DCC31-4CC3-4983-82DA-D163857CCF8E"),
                                Labels = new List<TranslationDTO>
                                {
                                     new TranslationDTO
                                        {
                                            LanguageCode = "en",
                                            Text = "New Paragraph 1 section 1"
                                        }
                                },
                                Type = FormNodeType.FormField.ToString(),
                                FieldType = FormNodeFieldType.Paragraph.ToString(),
                                Order = 0,
                                ParentId = new Guid("51d0fc83-5606-47c5-9279-68f6358592b8"),
                                FormId = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482")
                            },
                            new CreateUpdateFormNodesItemDto
                            {
                                Id = new Guid("320C00A8-1960-4123-812E-008E45C4CB4D"),
                                Labels = new List<TranslationDTO>
                                {
                                     new TranslationDTO
                                        {
                                            LanguageCode = "en",
                                            Text = "New Paragraph 2 section 2"
                                        }
                                },
                                Type = FormNodeType.FormField.ToString(),
                                FieldType = FormNodeFieldType.Paragraph.ToString(),
                                Order = 1,
                                ParentId = new Guid("51d0fc83-5606-47c5-9279-68f6358592b8"),
                                FormId = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482")
                            }
                        }
                    },
                    new CreateUpdateFormNodesDto
                    {
                        FormId = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482"),
                        Id = new Guid("6f2325c7-9503-4d88-9601-d3397ad6de66"),
                        Type = FormNodeType.Section.ToString(),
                        Labels = new List<TranslationDTO>
                        {
                            new TranslationDTO
                            {
                                LanguageCode = "en",
                                Text = "section 2"
                            }
                        },
                        Order = 1,
                        FormNodes = new List<CreateUpdateFormNodesItemDto>
                        {
                            new CreateUpdateFormNodesItemDto
                            {
                                Id = new Guid("48175844-ABB2-4CA0-872B-95DB48F8B7CC"),
                                Labels = new List<TranslationDTO>
                                {
                                     new TranslationDTO
                                        {
                                            LanguageCode = "en",
                                            Text = "New Paragraph 1 section 2"
                                        }
                                },
                                Type = FormNodeType.FormField.ToString(),
                                FieldType = FormNodeFieldType.Paragraph.ToString(),
                                Order = 0,
                                ParentId = new Guid("6F2325C7-9503-4D88-9601-D3397AD6DE66"),
                                FormId = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482"),
                                FormNodeFields = new List<CreateUpdateFormNodeFieldDto>
                                {
                                    new CreateUpdateFormNodeFieldDto
                                    {
                                        Id = new Guid("25D88C19-886A-4551-BB1E-1CEB96E9BAA4"),
                                        Property = "Id",
                                        Value = "\"paragraph1section1\"",
                                        Type = FormNodeFieldType.Text.ToString()
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        [TestMethod()]
        public async Task CreateUpdateFormContentAsync_MoveOrder()
        {
            var formId = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482");
            var mockFormNoodeFieldsInDb = new List<FormNodeFields> { 
                new FormNodeFields {
                    Id = new Guid("25D88C19-886A-4551-BB1E-1CEB96E9BAA4"),
                    Property = "Id",
                    Value = "\"paragraph1section1\"",
                    Type = FormNodeFieldType.Text.ToString()
                }
            };
            var mockFormNodesInDb = new List<FormNodes>() 
            { 
                new FormNodes()
                {
                    Id = new Guid("51d0fc83-5606-47c5-9279-68f6358592b8"),
                    FormId = formId,
                    Order = 0,
                    Type = FormNodeType.Section.ToString()
                },
                new FormNodes()
                {
                    Id = new Guid("6F2325C7-9503-4D88-9601-D3397AD6DE66"),
                    FormId = formId,
                    Order = 0,
                    Type = FormNodeType.Section.ToString()
                },
                new FormNodes()
                {
                    Id = new Guid("A75DCC31-4CC3-4983-82DA-D163857CCF8E"),
                    FormId = formId,
                    Order = 0,
                    Type = FormNodeType.FormField.ToString(),
                    ParentId = new Guid("51D0FC83-5606-47C5-9279-68F6358592B8"),
                    Labels = new List<Translation>
                    {
                        new Translation
                        {
                            LanguageCode = "en",
                            Text = "New Paragraph 1 section 1"
                        }
                    }
                },
                new FormNodes()
                {
                    Id = new Guid("48175844-ABB2-4CA0-872B-95DB48F8B7CC"),
                    FormId = formId,
                    Order = 0,
                    Type = FormNodeType.FormField.ToString(),
                    ParentId = new Guid("6F2325C7-9503-4D88-9601-D3397AD6DE66"),
                    Labels = new List<Translation>
                    {
                        new Translation
                        {
                            LanguageCode = "en",
                            Text = "New Paragraph 1 section 2"
                        }
                    },
                    FormNodeFields = mockFormNoodeFieldsInDb
                },
                new FormNodes()
                {
                    Id = new Guid("320C00A8-1960-4123-812E-008E45C4CB4D"),
                    FormId = formId,
                    Order = 1,
                    Type = FormNodeType.FormField.ToString(),
                    ParentId = new Guid("6F2325C7-9503-4D88-9601-D3397AD6DE66"),
                    Labels = new List<Translation>
                    {
                        new Translation
                        {
                            LanguageCode = "en",
                            Text = "New Paragraph 2 section 2"
                        }
                    }
                }
                
            };
            var mockFormInDb = new List<Form>()
            {
                new Form()
                {
                    Id = formId,
                    Status = FormStatus.Draft,
                    Type = FormType.Webform.ToString(),
                    FormNodes = mockFormNodesInDb
                }
            };

            var paramContentMock = CreateUpdateFormContentAsyncMock();

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return mockFormInDb.Where(exp.Compile()).AsQueryable();
            });
            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).ReturnsForAnyArgs(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormNodes, bool>>>();
                return mockFormNodesInDb.Where(exp.Compile()).AsQueryable();
            });
            formNodeFieldsRepository.Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>()).ReturnsForAnyArgs(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormNodeFields, bool>>>();
                return mockFormNoodeFieldsInDb.Where(exp.Compile()).AsQueryable();
            });
            var result = await bll.CreateUpdateFormContentAsync(formId, paramContentMock);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(new Guid("51d0fc83-5606-47c5-9279-68f6358592b8"), result.ToArray()[0]);
            Assert.AreEqual(new Guid("6F2325C7-9503-4D88-9601-D3397AD6DE66"), result.ToArray()[1]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateUpdateFormNodeAsync_ArgumentException2()
        {
            var mockFormInDb = new List<Form>()
            {
                new Form()
                {
                    Id = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482"),
                    Status = FormStatus.Draft,
                    Type = FormType.Webform.ToString()
                }
            };

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(mockFormInDb.AsQueryable());
            try
            {
                var param = new CreateUpdateFormNodeDto
                {
                    FormId = new Guid("7055787d-28df-4d9a-9a75-09c49c99a482"),
                    Type = FormNodeType.FormBlock.ToString(),
                };
                await bll.CreateUpdateFormNodeAsync(param);
            }
            catch (Exception ex)
            {
                formRepository.Received(2).Find(Arg.Any<Expression<Func<Form, bool>>>());
                Assert.AreEqual($"Cannot add/update FormField/FormBlock directly to the Form", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        public async Task CreateUpdateFormNodeAsyncTestArgumentExeptionFormNotInStatusDraft()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                var formId = Guid.NewGuid();
                var mockFormInDb = new List<Form>()
                {
                        new Form()
                        {
                        Id = formId,
                        Status = FormStatus.Online
                    }
                };

                formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(mockFormInDb.AsQueryable());
                await bll.CreateUpdateFormNodeAsync(new CreateUpdateFormNodeDto() { Id = Guid.NewGuid(), FormId = formId });
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod]
        public async Task CreateFormNodeFieldAsyncTest()
        {
            var formNodeId = Guid.NewGuid();
            var newId = Guid.NewGuid();
            var createUpdateFormNodeFieldDTO = new CreateUpdateFormNodeFieldDto()
            {
                Id = null,
                FormNodeId = formNodeId,
                Property = "Text Label",
                Type = FormNodeFieldEncodeType.Text.ToString(),
                Value = "Text 1"
            };

            var mockFormNodeFieldsDb = new List<FormNodes>();

            formNodeFieldsRepository.When(f => f.Add(Arg.Any<FormNodeFields>())).Do(c => c.ArgAt<FormNodeFields>(0).Id = newId);
            unitOfWork.SaveChangesAsync().Returns(1);

            var result = await bll.CreateUpdateFormNodeFieldAsync(createUpdateFormNodeFieldDTO);
            formNodeFieldsRepository.Received(1).Add(Arg.Any<FormNodeFields>());
            await unitOfWork.Received(1).SaveChangesAsync();

            Assert.IsTrue(result.GetType().Equals(typeof(Guid)));
            Assert.AreEqual(newId, result);
        }

        [TestMethod]
        public async Task UpdateFormNodeFieldsAsyncTest()
        {
            var formNodeId = Guid.NewGuid();
            var formNodeFieldId = Guid.NewGuid();
            var createUpdateFormNodeFieldDTO = new CreateUpdateFormNodeFieldDto()
            {
                Id = formNodeFieldId,
                FormNodeId = formNodeId,
                Property = "Text Label",
                Type = FormNodeFieldEncodeType.Text.ToString(),
                Value = "Text 1 Update"
            };

            var formNodeFeildEntityDbMock = new FormNodeFields()
            {
                Id = formNodeFieldId,
                FormNodeId = formNodeId,
                Property = "Text Label",
                Type = FormNodeFieldEncodeType.Text.ToString(),
                Value = "Text 1"
            };

            var mockFormNodeFieldsInDb = new List<FormNodeFields>() { formNodeFeildEntityDbMock };

            formNodeFieldsRepository.Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>()).ReturnsForAnyArgs(mockFormNodeFieldsInDb.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);

            var result = await bll.CreateUpdateFormNodeFieldAsync(createUpdateFormNodeFieldDTO);

            formNodeFieldsRepository.Received(1).Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>());
            await unitOfWork.Received(1).SaveChangesAsync();
            Assert.AreEqual(formNodeFieldId, result);
            Assert.AreEqual(formNodeFeildEntityDbMock.Value, createUpdateFormNodeFieldDTO.Value);
            Assert.AreEqual(formNodeFeildEntityDbMock.FormNodeId, createUpdateFormNodeFieldDTO.FormNodeId);
        }

        [TestMethod()]
        public async Task CreateUpdateFormNodeFieldAsyncTestArgumentExeption()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.CreateUpdateFormNodeFieldAsync(default);
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task CreateUpdateFormNodeFieldAsyncTestArgumentExeption2()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.CreateUpdateFormNodeFieldAsync(new CreateUpdateFormNodeFieldDto() { Id = Guid.NewGuid() });
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task CreateUpdateFormNodeFieldAsyncTestArgumentExeptionIdNameAlreadyUsed()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                var formId = Guid.NewGuid();
                var formNodeId = Guid.NewGuid();
                var mockFormInDb = new List<Form>()
                {
                        new Form()
                        {
                        Id = formId,
                        Status = FormStatus.Draft,
                        FormNodes = new List<FormNodes>()
                        {
                            new FormNodes()
                            {
                                Id = formNodeId,
                                FormId = formId,
                                FormNodeFields = new List<FormNodeFields>()
                                {
                                    new FormNodeFields()
                                    {
                                        Id = Guid.NewGuid(),
                                        Property = "Hide by default",
                                        Value = "true"
                                    }
                                }
                            },
                            new FormNodes()
                            {
                                Id = Guid.NewGuid(),
                                FormId = formId,
                                FormNodeFields = new List<FormNodeFields>()
                                {
                                    new FormNodeFields()
                                    {
                                        Id = Guid.NewGuid(),
                                        Property = "ID",
                                        Value = "First_Name"
                                    }
                                }
                            }
                        }
                    }
                };
                var mockFormNodeInDB = new List<FormNodes>()
                {
                    new FormNodes()
                    {
                        Id = formNodeId,
                        FormId = formId,
                    }
                };
                formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).ReturnsForAnyArgs(mockFormNodeInDB.AsQueryable());
                formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs(mockFormInDb.AsQueryable());
                await bll.CreateUpdateFormNodeFieldAsync(new CreateUpdateFormNodeFieldDto() { FormNodeId = formNodeId, Property = "ID", Value = "First_Name" });
            });
            formNodesRepository.ReceivedWithAnyArgs(1).Find(Arg.Any<Expression<Func<FormNodes, bool>>>());
            formRepository.ReceivedWithAnyArgs(1).Find(Arg.Any<Expression<Func<Form, bool>>>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod]
        public void GetFormContentTest()
        {
            Guid formId = Guid.NewGuid();
            Guid formNodeTypeTextId = Guid.NewGuid();
            Guid formNodeTypeChexboxId = Guid.NewGuid();
            Guid formNodeFieldsCheckboxListId = Guid.NewGuid();
            Guid formNodeFieldsCheckboxConditionalId = Guid.NewGuid();
            Guid formNodeTypeListId = Guid.NewGuid();
            Guid formNodeTypeBlockId = Guid.NewGuid();
            
            Guid formBlockId = Guid.NewGuid();
            Guid formBlockNodeTextId = Guid.NewGuid();

            var formBlockEntityMock = new Form()
            {
                Id = formBlockId,
                FormNodes = new List<FormNodes>() {
                    new FormNodes()
                    {
                        Id = formBlockNodeTextId,
                        Labels = new List<Translation>(){ new Translation() { Id=1, LanguageCode = "en", Text = "Form Field Text Label en"} },
                        FormId = formBlockId,
                        Type = FormNodeType.FormField.ToString(),
                        FieldType = FormNodeFieldType.Text.ToString(),
                        Order = 1,
                        FormNodeFields = new List<FormNodeFields>()
                        {
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formBlockNodeTextId,
                                Property = "Hide by default",
                                Value = "false",
                                Type = FormNodeFieldEncodeType.Text.ToString()
                            },
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formBlockNodeTextId,
                                Property = "Text Label",
                                Value = "",
                                Type = FormNodeFieldEncodeType.Translation.ToString(),
                                Labels = new List<Translation>(){ new Translation()
                                    {
                                        Id = 12,
                                        LanguageCode = "en",
                                        Text = "This is text label en"
                                    },
                                    new Translation()
                                    {
                                        Id = 13,
                                        LanguageCode = "fr",
                                        Text = "This is text label fr"
                                    }
                                }
                            },
                        }
                    } 
                }
            };

            var formEntityMock = new Form()
            {
                Id = formId,
                IsActive = true,
                Version = 1,
                FormCategoryId = 1,
                Labels = new List<Translation> {
                    new Translation()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label Form 1",
                    }
                },
                FormNodes = new List<FormNodes>() { 
                    new FormNodes()
                    {
                        Id = formNodeTypeTextId,
                        Labels = new List<Translation>(){ new Translation() { Id=1, LanguageCode = "en", Text = "Form Field Text Label en"} },
                        FormId = formId,
                        Type = FormNodeType.FormField.ToString(),
                        FieldType = FormNodeFieldType.Text.ToString(),
                        Order = 1,
                        FormNodeFields = new List<FormNodeFields>()
                        {
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeTextId,
                                Property = "Hide by default",
                                Value = "false",
                                Type = FormNodeFieldEncodeType.Text.ToString()
                            },
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeTextId,
                                Property = "Text Label",
                                Value = "",
                                Type = FormNodeFieldEncodeType.Translation.ToString(),
                                Labels = new List<Translation>(){ new Translation()
                                    {
                                        Id = 2,
                                        LanguageCode = "en",
                                        Text = "This is text label en"
                                    },
                                    new Translation()
                                    {
                                        Id = 3,
                                        LanguageCode = "fr",
                                        Text = "This is text label fr"
                                    }
                                }
                            },
                        }
                    },
                    new FormNodes()
                    {
                        Id = formNodeTypeChexboxId,
                        Labels = new List<Translation>(){ new Translation() { Id=1, LanguageCode = "en", Text = "Form Field Text Label en"} },
                        FormId = formId,
                        Type = FormNodeType.FormField.ToString(),
                        FieldType = FormNodeFieldType.CheckBox.ToString(),
                        Order = 2,
                        FormNodeFields = new List<FormNodeFields>()
                        {
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeChexboxId,
                                Property = "Hide by default",
                                Value = "true",
                                Type = FormNodeFieldEncodeType.Text.ToString()
                            },
                            new FormNodeFields()
                            {
                                Id= formNodeFieldsCheckboxListId,
                                FormNodeId = formNodeTypeChexboxId,
                                Property = "List",
                                Value = "5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E",
                                Type = FormNodeFieldEncodeType.CustomList.ToString(),
                                FormValueFields = new List<FormValueFields>(){
                                    new FormValueFields()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxListId,
                                        Labels = new List<Translation>(){ new Translation() { Id = 4, LanguageCode = "en", Text = "Option 1 en" } },
                                        Value = "Option1"
                                    },
                                    new FormValueFields()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxListId,
                                        Labels = new List<Translation>(){ new Translation() { Id = 5, LanguageCode = "en", Text = "Option 2 en" } },
                                        Value = "Option2"
                                    },
                                    new FormValueFields()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxListId,
                                        Labels = new List<Translation>(){ new Translation() { Id = 6, LanguageCode = "en", Text = "Option 3 en" } },
                                        Value = "Option3"
                                    }
                                }
                            },
                            new FormNodeFields()
                            {
                                Id= formNodeFieldsCheckboxConditionalId,
                                FormNodeId = formNodeTypeChexboxId,
                                Property = "Conditional Logic",
                                Value = "",
                                Type = FormNodeFieldEncodeType.Conditional.ToString(),
                                FormConditionals = new List<FormConditionals>(){
                                    new FormConditionals()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxConditionalId,
                                        Condition = "Option1",
                                        State = "show",
                                        FormNodeId = formNodeTypeTextId
                                    },
                                    new FormConditionals()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxConditionalId,
                                        Condition = "Option2",
                                        State = "hide",
                                        FormNodeId = formNodeTypeTextId
                                    }
                                }
                            },
                        }
                    },
                    new FormNodes()
                    {
                        Id = formNodeTypeListId,
                        Labels = new List<Translation>(){ new Translation() { Id=1, LanguageCode = "en", Text = "Form Field List Label en"} },
                        FormId = formId,
                        Type = FormNodeType.FormField.ToString(),
                        FieldType = FormNodeFieldType.List.ToString(),
                        Order = 3,
                        FormNodeFields = new List<FormNodeFields>()
                        {
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeListId,
                                Property = "Hide by default",
                                Value = "false",
                                Type = FormNodeFieldEncodeType.Text.ToString()
                            },
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeListId,
                                Property = "Form Field Description",
                                Value = "",
                                Type = FormNodeFieldEncodeType.Translation.ToString(),
                                Labels = new List<Translation>(){ new Translation()
                                    {
                                        Id = 7,
                                        LanguageCode = "en",
                                        Text = "This is list description en"
                                    },
                                    new Translation()
                                    {
                                        Id = 8,
                                        LanguageCode = "fr",
                                        Text = "This is list description fr"
                                    }
                                }
                            },
                             new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeListId,
                                Property = "List",
                                Value = "5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E",
                                Type = FormNodeFieldEncodeType.PredefineList.ToString()
                            },
                        }
                    },
                    new FormNodes()
                    {
                        Id = formNodeTypeBlockId,
                        Labels = new List<Translation>(){ new Translation() { Id=1, LanguageCode = "en", Text = "Form Block Label en"} },
                        FormId = formId,
                        Type = FormNodeType.FormBlock.ToString(),
                        Order = 4,
                        FormNodeFields = new List<FormNodeFields>()
                        {
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeBlockId,
                                Property = "Value",
                                Value = formBlockEntityMock.Id.ToString(),
                                Type = FormNodeFieldEncodeType.Container.ToString()
                            }
                        }
                    },
                }
            };

            var listFormDbMock = new List<Form>()
            {
                formEntityMock,
                formBlockEntityMock
            };


            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormDbMock.Where(exp.Compile()).AsQueryable();
            });

            var result = bll.GetFormContent(formId);

            Assert.AreEqual(formEntityMock.Id, result.Id);

            Assert.AreEqual(formEntityMock.FormNodes.Count(), result.FormNodes.Count());
            Assert.AreEqual(formEntityMock.Labels.ElementAt(0).Text, result.Labels.ElementAt(0).Text);
            Assert.AreEqual(formEntityMock.FormNodes.ElementAt(0).FieldType, result.FormNodes.ElementAt(0).FieldType);
            Assert.AreEqual(formEntityMock.FormNodes.ElementAt(1).FieldType, result.FormNodes.ElementAt(1).FieldType);
            Assert.AreEqual(formEntityMock.FormNodes.ElementAt(2).FieldType, result.FormNodes.ElementAt(2).FieldType);

            Assert.AreEqual(FormNodeFieldEncodeType.PredefineList.ToString(), result.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Type);
            Assert.AreEqual(formEntityMock.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Value, result.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Value);

            Assert.AreEqual(FormNodeFieldEncodeType.CustomList.ToString(), result.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).Type);
            var listDtoCustList = JsonConvert.DeserializeObject<List<CustomListDTO>>(result.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).Value);
            if(listDtoCustList != null)
            {
                var entityDb = formEntityMock.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).FormValueFields;
                for (int i = 0; i < listDtoCustList.Count; i++)
                {
                    var entity = entityDb.ElementAt(i);
                    var resultDto = listDtoCustList.ElementAt(i);
                    Assert.AreEqual(entity.Id, resultDto.Id);
                    Assert.AreEqual(entity.Value, resultDto.Value);
                    Assert.AreEqual(entity.Labels.Count(), resultDto.Translations.Count());
                }
            }

            Assert.AreEqual(FormNodeFieldEncodeType.Conditional.ToString(), result.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).Type);
            var listDtoConditional = JsonConvert.DeserializeObject<List<FormConditionalDTO>>(result.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).Value);
            if (listDtoConditional != null)
            {
                var entityDb = formEntityMock.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).FormConditionals;
                for (int i = 0; i < listDtoConditional.Count; i++)
                {
                    var entity = entityDb.ElementAt(i);
                    var resultDto = listDtoConditional.ElementAt(i);
                    Assert.AreEqual(entity.Id, resultDto.Id);
                    Assert.AreEqual(entity.Condition, resultDto.Condition);
                    Assert.AreEqual(entity.State, resultDto.State);
                    Assert.AreEqual(entity.FormNodeId, resultDto.FormNodeId);
                }
            }

            Assert.AreEqual(FormNodeFieldEncodeType.Container.ToString(), result.FormNodes.ElementAt(3).FormNodeFields.ElementAt(0).Type);
            Assert.AreEqual(formBlockEntityMock.Id.ToString(), result.FormNodes.ElementAt(3).FormNodeFields.ElementAt(0).Value);
            Assert.AreEqual("Value", result.FormNodes.ElementAt(3).FormNodeFields.ElementAt(0).Property);
            Assert.AreEqual(result.FormNodes.ElementAt(3).FormNodes.Count(),formBlockEntityMock.FormNodes.Count());
        }

        [TestMethod()]
        public void GetFormContentTestArgumentExeption()
        {
            Assert.ThrowsException<ArgumentException>( () =>
            {
                bll.GetFormContent(default);
            });
            formRepository.DidNotReceiveWithAnyArgs().Find(Arg.Any<Expression<Func<Form, bool>>>());
        }

        [TestMethod()]
        public void GetFormContentTestKeyNotFoundException()
        {
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                bll.GetFormContent(Guid.NewGuid());
            });
        }

        [TestMethod]
        public async Task DuplicateFormBlockTest()
        {
            Guid duplicatedFormId = Guid.NewGuid();
            Guid formBlockId = Guid.NewGuid();
            Guid formNodeTypeTextId = Guid.NewGuid();
            Guid formNodeTypeChexboxId = Guid.NewGuid();
            Guid formNodeFieldsCheckboxListId = Guid.NewGuid();
            Guid formNodeFieldsCheckboxConditionalId = Guid.NewGuid();
            Guid formNodeTypeListId = Guid.NewGuid();
            Guid formNodeTypeBlockId = Guid.NewGuid();
            Guid formBlockNodeTextId = Guid.NewGuid();
            
            var formBlockEntityMock = new Form()
            {
                Id = formBlockId,
                ExternalId = 1,
                Status = FormStatus.Online,
                IsActive = true,
                Version = 1,
                Type = FormType.Block.ToString(),
                Labels = new List<Translation> {
                    new Translation()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label Form Block 1",
                    }
                },
                FormNodes = new List<FormNodes>() {
                    new FormNodes()
                    {
                        Id = formNodeTypeTextId,
                        Labels = new List<Translation>(){ new Translation() { Id=1, LanguageCode = "en", Text = "Form Field Text Label en"} },
                        FormId = formBlockId,
                        Type = FormNodeType.FormField.ToString(),
                        FieldType = FormNodeFieldType.Text.ToString(),
                        Order = 1,
                        FormNodeFields = new List<FormNodeFields>()
                        {
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeTextId,
                                Property = "Hide by default",
                                Value = "false",
                                Type = FormNodeFieldEncodeType.Text.ToString()
                            },
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeTextId,
                                Property = "Text Label",
                                Value = "",
                                Type = FormNodeFieldEncodeType.Translation.ToString(),
                                Labels = new List<Translation>(){ new Translation()
                                    {
                                        Id = 2,
                                        LanguageCode = "en",
                                        Text = "This is text label en"
                                    },
                                    new Translation()
                                    {
                                        Id = 3,
                                        LanguageCode = "fr",
                                        Text = "This is text label fr"
                                    }
                                }
                            },
                        }
                    },
                    new FormNodes()
                    {
                        Id = formNodeTypeChexboxId,
                        Labels = new List<Translation>(){ new Translation() { Id=1, LanguageCode = "en", Text = "Form Field Text Label en"} },
                        FormId = formBlockId,
                        Type = FormNodeType.FormField.ToString(),
                        FieldType = FormNodeFieldType.CheckBox.ToString(),
                        Order = 2,
                        FormNodeFields = new List<FormNodeFields>()
                        {
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeChexboxId,
                                Property = "Hide by default",
                                Value = "true",
                                Type = FormNodeFieldEncodeType.Text.ToString()
                            },
                            new FormNodeFields()
                            {
                                Id= formNodeFieldsCheckboxListId,
                                FormNodeId = formNodeTypeChexboxId,
                                Property = "List",
                                Value = "5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E",
                                Type = FormNodeFieldEncodeType.CustomList.ToString(),
                                FormValueFields = new List<FormValueFields>(){
                                    new FormValueFields()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxListId,
                                        Labels = new List<Translation>(){ new Translation() { Id = 4, LanguageCode = "en", Text = "Option 1 en" } },
                                        Value = "Option1"
                                    },
                                    new FormValueFields()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxListId,
                                        Labels = new List<Translation>(){ new Translation() { Id = 5, LanguageCode = "en", Text = "Option 2 en" } },
                                        Value = "Option2"
                                    },
                                    new FormValueFields()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxListId,
                                        Labels = new List<Translation>(){ new Translation() { Id = 6, LanguageCode = "en", Text = "Option 3 en" } },
                                        Value = "Option3"
                                    }
                                }
                            },
                            new FormNodeFields()
                            {
                                Id= formNodeFieldsCheckboxConditionalId,
                                FormNodeId = formNodeTypeChexboxId,
                                Property = "Conditional Logic",
                                Value = "",
                                Type = FormNodeFieldEncodeType.Conditional.ToString(),
                                FormConditionals = new List<FormConditionals>(){
                                    new FormConditionals()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxConditionalId,
                                        Condition = "Option1",
                                        State = "show",
                                        FormNodeId = formNodeTypeTextId
                                    },
                                    new FormConditionals()
                                    {
                                        Id = Guid.NewGuid(),
                                        FormNodeFieldId = formNodeFieldsCheckboxConditionalId,
                                        Condition = "Option2",
                                        State = "hide",
                                        FormNodeId = formNodeTypeTextId
                                    }
                                }
                            },
                        }
                    },
                    new FormNodes()
                    {
                        Id = formNodeTypeListId,
                        Labels = new List<Translation>(){ new Translation() { Id=1, LanguageCode = "en", Text = "Form Field List Label en"} },
                        FormId = formBlockId,
                        Type = FormNodeType.FormField.ToString(),
                        FieldType = FormNodeFieldType.List.ToString(),
                        Order = 3,
                        FormNodeFields = new List<FormNodeFields>()
                        {
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeListId,
                                Property = "Hide by default",
                                Value = "false",
                                Type = FormNodeFieldEncodeType.Text.ToString()
                            },
                            new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeListId,
                                Property = "Form Field Description",
                                Value = "",
                                Type = FormNodeFieldEncodeType.Translation.ToString(),
                                Labels = new List<Translation>(){ new Translation()
                                    {
                                        Id = 7,
                                        LanguageCode = "en",
                                        Text = "This is list description en"
                                    },
                                    new Translation()
                                    {
                                        Id = 8,
                                        LanguageCode = "fr",
                                        Text = "This is list description fr"
                                    }
                                }
                            },
                             new FormNodeFields()
                            {
                                Id= Guid.NewGuid(),
                                FormNodeId = formNodeTypeListId,
                                Property = "List",
                                Value = "5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E",
                                Type = FormNodeFieldEncodeType.PredefineList.ToString()
                            },
                        }
                    }
                }
            };
            
            var listFormBlockDbMock = new List<Form>()
            {
                formBlockEntityMock
            };
            Form formBlockDuplicateResult = null;
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormBlockDbMock.Where(exp.Compile()).AsQueryable();
            });
            formRepository.When(f => f.Add(Arg.Any<Form>())).Do(c => {
                c.ArgAt<Form>(0).Id = duplicatedFormId;
                formBlockDuplicateResult = c.ArgAt<Form>(0);
            });
            unitOfWork.SaveChangesAsync().Returns(1);
            var result = await bll.DuplicateFormBlock(formBlockId);
            formRepository.Received(2).Find(Arg.Any<Expression<Func<Form, bool>>>());
            formRepository.Received(1).Add(Arg.Any<Form>());
            await unitOfWork.Received(3).SaveChangesAsync();
            formConditionalsRepository.Received(2).Update(Arg.Any<FormConditionals>());
            Assert.IsTrue(result.GetType().Equals(typeof(Guid)));
            Assert.AreEqual(duplicatedFormId, result);
            Assert.AreEqual(formBlockEntityMock.FormNodes.Count(), formBlockDuplicateResult.FormNodes.Count());
            Assert.AreEqual(formBlockEntityMock.ExternalId + 1, formBlockDuplicateResult.ExternalId);
            Assert.AreEqual($"{formBlockEntityMock.Labels.ElementAt(0).Text}_{formBlockEntityMock.ExternalId+1}", formBlockDuplicateResult.Labels.ElementAt(0).Text);
            Assert.AreEqual(formBlockEntityMock.FormNodes.ElementAt(0).FieldType, formBlockDuplicateResult.FormNodes.ElementAt(0).FieldType);
            Assert.AreEqual(formBlockEntityMock.FormNodes.ElementAt(1).FieldType, formBlockDuplicateResult.FormNodes.ElementAt(1).FieldType);
            Assert.AreEqual(formBlockEntityMock.FormNodes.ElementAt(2).FieldType, formBlockDuplicateResult.FormNodes.ElementAt(2).FieldType);
            Assert.AreEqual(FormNodeFieldEncodeType.PredefineList.ToString(), formBlockDuplicateResult.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Type);
            Assert.AreEqual(formBlockEntityMock.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Value, formBlockDuplicateResult.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Value);
            Assert.AreEqual(FormNodeFieldEncodeType.CustomList.ToString(), formBlockDuplicateResult.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).Type);
            var dupCustList = formBlockDuplicateResult.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).FormValueFields;
            if (dupCustList != null)
            {
                var sourceCustList = formBlockEntityMock.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).FormValueFields;
                for (int i = 0; i < dupCustList.Count; i++)
                {
                    var src = sourceCustList.ElementAt(i);
                    var dup = dupCustList.ElementAt(i);
                    Assert.AreEqual(src.Value, dup.Value);
                    Assert.AreEqual(src.Labels.Count(), dup.Labels.Count());
                }
            }
            Assert.AreEqual(FormNodeFieldEncodeType.Conditional.ToString(), formBlockDuplicateResult.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).Type);
            var dupCondList = formBlockDuplicateResult.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).FormConditionals;
            if (dupCondList != null)
            {
                var srcCondList = formBlockEntityMock.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).FormConditionals;
                for (int i = 0; i < dupCondList.Count; i++)
                {
                    var src = srcCondList.ElementAt(i);
                    var dup = dupCondList.ElementAt(i);
                    Assert.AreEqual(src.Condition, dup.Condition);
                    Assert.AreEqual(src.State, dup.State);
                    Assert.AreNotEqual(src.FormNodeId, dup.FormNodeId);
                }
            }
        }
        [TestMethod()]
        public async Task DuplicateFormBlockTesArgumentExeption()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.DuplicateFormBlock(default);
            });
            formRepository.DidNotReceiveWithAnyArgs().Find(Arg.Any<Expression<Func<Form, bool>>>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }
        [TestMethod()]
        public async Task DuplicateFormBlockTesArgumentExeptio2()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.DuplicateFormBlock(Guid.NewGuid());
            });
            formRepository.ReceivedWithAnyArgs(1).Find(Arg.Any<Expression<Func<Form, bool>>>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod]
        public async Task CreateFormNodeFieldTypeCheckboxAsyncTest()
        {
            var formNodeId = Guid.NewGuid();
            var newId = Guid.NewGuid();
            var createUpdateFormNodeFieldDTO = new CreateUpdateFormNodeFieldDto()
            {
                Id = null,
                FormNodeId = formNodeId,
                Property = "List",
                Type = FormNodeFieldEncodeType.CustomList.ToString(),
                Value = "[{ 'translations': [ { 'id': null, 'languageCode': 'en', 'text': 'property value language EN 1' }, { 'id': null, 'languageCode': 'fr', 'text': 'property value language FR 1' }, { 'id': null, 'languageCode': 'nl', 'text': 'property value language NL 1' } ], 'value': 1, 'order': 1 }, { 'translations': [ { 'id': null, 'languageCode': 'en', 'text': 'property value language EN 2' }, { 'id': null, 'languageCode': 'fr', 'text': 'property value language FR 2' }, { 'id': null, 'languageCode': 'nl', 'text': 'property value language NL 2' } ], 'value': 2, 'order': 2 }]"
            };

            var mockFormNodeFieldsDb = new List<FormNodes>();

            formNodeFieldsRepository.When(f => f.Add(Arg.Any<FormNodeFields>())).Do(c => c.ArgAt<FormNodeFields>(0).Id = newId);
            unitOfWork.SaveChangesAsync().Returns(1);

            var result = await bll.CreateUpdateFormNodeFieldAsync(createUpdateFormNodeFieldDTO);
            formNodeFieldsRepository.Received(1).Add(Arg.Any<FormNodeFields>());
            await unitOfWork.Received(2).SaveChangesAsync();

            Assert.IsTrue(result.GetType().Equals(typeof(Guid)));
            Assert.AreEqual(newId, result);
        }

        [TestMethod]
        public async Task UpdateFormNodeFieldTypeCHeckboxAsyncTest()
        {
            var formNodeId = Guid.NewGuid();
            var formNodeFieldId = Guid.NewGuid();
            var trans1enid = 1;
            var trans1frid = 2;
            var trans1nlid = 3;
            var trans2enid = 4;
            var trans2frid = 5;
            var trans2nlid = 6;
            var createUpdateFormNodeFieldDTO = new CreateUpdateFormNodeFieldDto()
            {
                Id = formNodeFieldId,
                FormNodeId = formNodeId,
                Property = "List",
                Type = FormNodeFieldEncodeType.CustomList.ToString(),
                Value = "[{ 'translations': [ { 'id': " + trans1enid + ", 'languageCode': 'en', 'text': 'property value language EN 1' }, { 'id': " + trans1frid + ", 'languageCode': 'fr', 'text': 'property value language FR 1' }, { 'id': " + trans1nlid + ", 'languageCode': 'nl', 'text': 'property value language NL 1' } ], 'value': 1, 'order': 2 }, { 'translations': [ { 'id': " + trans2enid + ", 'languageCode': 'en', 'text': 'property value language EN 2' }, { 'id': " + trans2frid + ", 'languageCode': 'fr', 'text': 'property value language FR 2' }, { 'id': " + trans2nlid + ", 'languageCode': 'nl', 'text': 'property value language NL 2' } ], 'value': 2, 'order': 1 }]"
            };

            var formNodeFeildEntityDbMock = new FormNodeFields()
            {
                Id = formNodeFieldId,
                FormNodeId = formNodeId,
                Property = "List",
                Type = FormNodeFieldEncodeType.CustomList.ToString(),
                Value = "[{ 'translations': [ { 'id': " + trans1enid + ", 'languageCode': 'en', 'text': 'property value language EN 1' }, { 'id': " + trans1frid + ", 'languageCode': 'fr', 'text': 'property value language FR 1' }, { 'id': " + trans1nlid + ", 'languageCode': 'nl', 'text': 'property value language NL 1' } ], 'value': 1, 'order': 1 }, { 'translations': [ { 'id': " + trans2enid + ", 'languageCode': 'en', 'text': 'property value language EN 2' }, { 'id': " + trans2frid + ", 'languageCode': 'fr', 'text': 'property value language FR 2' }, { 'id': " + trans2nlid + ", 'languageCode': 'nl', 'text': 'property value language NL 2' } ], 'value': 2, 'order': 2 }]"
            };

            var mockFormNodeFieldsInDb = new List<FormNodeFields>() { formNodeFeildEntityDbMock };

            formNodeFieldsRepository.Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>()).ReturnsForAnyArgs(mockFormNodeFieldsInDb.AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);

            var result = await bll.CreateUpdateFormNodeFieldAsync(createUpdateFormNodeFieldDTO);

            formNodeFieldsRepository.Received(1).Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>());
            await unitOfWork.Received(1).SaveChangesAsync();
            Assert.AreEqual(formNodeFieldId, result);
            Assert.AreEqual(formNodeFeildEntityDbMock.FormNodeId, createUpdateFormNodeFieldDTO.FormNodeId);
            Assert.AreEqual(1, formNodeFeildEntityDbMock.FormValueFields?.FirstOrDefault(x => x.Value == "2")?.Order);
            Assert.AreEqual(2, formNodeFeildEntityDbMock.FormValueFields?.FirstOrDefault(x => x.Value == "1")?.Order);
        }
    }
}

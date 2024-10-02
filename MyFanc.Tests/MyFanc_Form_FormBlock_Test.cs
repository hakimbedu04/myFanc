using AutoMapper;
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
    public class MyFanc_Form_FormBlock_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IGenericRepository<Form> formRepository;
        private IGenericRepository<FormNodes> formNodesRepository;
        private IGenericRepository<FormNodeFields> formNodeFieldsRepository;
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
            this.unitOfWork = Substitute.For<IUnitOfWork>();
            this.formRepository = Substitute.For<IGenericRepository<Form>>();
            this.formNodesRepository = Substitute.For<IGenericRepository<FormNodes>>();
            this.formNodeFieldsRepository = Substitute.For<IGenericRepository<FormNodeFields>>();
            this.nacabelRepository = Substitute.For<IGenericRepository<Nacabel>>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.sharedDataCache = Substitute.For<ISharedDataCache>();
            this.identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();
            this.aESEncryptService = Substitute.For<IAESEncryptService>();

            this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();

            this.unitOfWork.GetGenericRepository<Form>().Returns(this.formRepository);
            this.unitOfWork.GetGenericRepository<FormNodes>().Returns(this.formNodesRepository);
            this.unitOfWork.GetGenericRepository<Nacabel>().Returns(this.nacabelRepository);
            this.unitOfWork.GetGenericRepository<FormNodeFields>().Returns(this.formNodeFieldsRepository);

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
        public async Task CreateFormBlockAsyncTest()
        {
            var createFormBLockDTO = new CreateFormBlockDTO()
            {
                IsActive = true,
                Labels = new List<TranslationDTO>()
                {
                    new TranslationDTO(){LanguageCode = "en", Text = "FormBlock 1 en"},
                    new TranslationDTO(){LanguageCode = "fr", Text = "FormBlock 1 fr"}

                }
            };

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs((new[] { new Form() }).AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);
            var createFormBlock = await bll.CreateFormBlockAsync(createFormBLockDTO);
            Assert.AreEqual(2, createFormBlock.ExternalId);
            Assert.AreEqual(true, createFormBlock.IsActive);
            Assert.AreEqual(Core.Enums.FormStatus.Online.ToString(), createFormBlock.Status);
            Assert.AreEqual(2, createFormBlock.Labels.Count());
            Assert.AreEqual("FormBlock 1 en", createFormBlock.Labels.ElementAt(0).Text);
            Assert.AreEqual("FormBlock 1 fr", createFormBlock.Labels.ElementAt(1).Text);
        }

        [TestMethod()]
        public async Task CreateFormBlockAsyncTestArgumentExeption()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.CreateFormBlockAsync(default);
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task CreateFormBlockAsyncTestExceptionOnSaveChange()
        {
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs((new[] { new Form() }).AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(0);
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.CreateFormBlockAsync(new CreateFormBlockDTO());
            });
            await unitOfWork.ReceivedWithAnyArgs(1).SaveChangesAsync();
        }

        [TestMethod]
        public async Task AddFormBlockToForm_SuccessfullTest()
        {
            var formId = Guid.NewGuid();
            var formBlockId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();

            var forms = new List<Form>
            {
                new Form
                {
                    Id = formId,
                    ExternalId = 1,
                    Status = FormStatus.Draft,
                    Type = FormType.Webform.ToString(),
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            FormId = formId,
                            Id = sectionId,
                            Type = "Section",
                            FieldType = "",
                            Order = 1
                        }
                    }
                },
                new Form
                {
                    Id = formBlockId,
                    ExternalId = 1,
                    Type = FormType.Block.ToString()
                }
            };

            var formNodes = new List<FormNodes>() {
                new FormNodes
                        {
                            FormId = formId,
                            Id = sectionId,
                            Type = "Section",
                            FieldType = "",
                            Order = 1
                        }
            };

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });

            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormNodes, bool>>>();
                return formNodes.Where(exp.Compile()).AsQueryable();
            });

            await bll.AddFormBlockToForm(new AddFormBlockToFormDTO
            {
                FormBlockId = formBlockId,
                SectionId = sectionId,
                FormId = formId
            });

            await unitOfWork.Received(2).SaveChangesAsync();
        }

        [TestMethod]
        public async Task AddFormBlockToForm_NullParameter_FailedTest()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.AddFormBlockToForm(default);
            });

            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod]
        public async Task AddFormBlockToForm_MissingFormBlock_FailedTest()
        {
            var formId = Guid.NewGuid();
            var formBlockId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns((new List<Form>()).AsQueryable());

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.AddFormBlockToForm(new AddFormBlockToFormDTO
                {
                    FormBlockId = formBlockId,
                    SectionId = sectionId,
                    FormId = formId
                });
            });

            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod]
        public async Task AddFormBlockToForm_Failed_FormStatusOnline_Test()
        {
            var formId = Guid.NewGuid();
            var formBlockId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns((new List<Form>() { new Form() { Id = formId, Status = FormStatus.Online } }).AsQueryable());
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.AddFormBlockToForm(new AddFormBlockToFormDTO
                {
                    FormBlockId = formBlockId,
                    SectionId = sectionId,
                    FormId = formId
                });
            });
            formRepository.ReceivedWithAnyArgs(2).Find(Arg.Any<Expression<Func<Form, bool>>>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetDetailFormResponseTest_ToPdfType_ArgumentException()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d67");
            var forms = new List<Form>()
            {
                new Form()
                {
                    Id = formId,
                    ExternalId = 1,
                    Type = FormType.Pdf.ToString()
                }
            };

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return forms.Where(exp.Compile()).AsQueryable();
            });
            AddFormBlockToFormDTO param = new AddFormBlockToFormDTO()
            {
                FormBlockId = new Guid("95B7C748-E5DC-4589-B94F-F60C17864BB7"),
                SectionId = new Guid("D6E5AFD8-DCCE-41E5-9DE6-80BC90788E9F"),
                FormId = formId
            };
            try
            {
                var result = await bll.AddFormBlockToForm(param);
            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"Form with id {formId} is a PDF type and cannot have form content!", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void GetListFormBlockTest()
        {
            var formId = new Guid("BBCE2E78-56E5-461B-8F09-4E55D5AFA648");
            var formNodeFieldMock = MockFormNodeFieldsForFormBLockTest();
            var listFormBLockMock = MockFormForFormBLockTest();
            var expectedResult = new List<ListFormBlockDTO>
            {
                new ListFormBlockDTO
                {
                    OriginalId = new Guid("bbce2e78-56e5-461b-8f09-4e55d5afa648"),
                    Id= 8,
                    Labels = new List<TranslationDTO>()
                    {
                        new TranslationDTO()
                        {
                            Id = 285,
                            Text = "FORM BLOCK CHILD",
                            LanguageCode = "en"
                        }
                    },
                    IsUsed= true,
                    IsEmptyForm= false
                }
            };

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormBLockMock.Where(exp.Compile()).AsQueryable();
            });
            formNodeFieldsRepository.Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormNodeFields, bool>>>();
                return formNodeFieldMock.Where(exp.Compile()).AsQueryable();
            });

            var formBlockList = bll.GetListFormBlock(null, null, Core.Enums.IsUsedParam.All);
            Assert.IsNotNull(formBlockList);
            for (int i = 0; i < formBlockList.Count(); i++)
            {
                var result = formBlockList.ElementAt(i);
                var source = listFormBLockMock.ElementAt(i);
                Assert.AreEqual(source.ExternalId, result.Id);
                Assert.AreEqual(source.Id, result.OriginalId);
                Assert.AreEqual(source.Labels.Count, result.Labels.Count());
            }
        }

        [TestMethod]
        public void GetListFormBlockTestFilterById()
        {
            var listFormBLockMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
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
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormBLockMock.Where(exp.Compile()).AsQueryable();
            });

            var formBlockList = bll.GetListFormBlock(1, null, Core.Enums.IsUsedParam.All);

            var result = formBlockList.ElementAt(0);
            var source = listFormBLockMock.ElementAt(0);
            Assert.AreEqual(source.ExternalId, result.Id);
            Assert.AreEqual(source.Id, result.OriginalId);
            Assert.AreEqual(source.Labels.Count, result.Labels.Count());

        }

        [TestMethod]
        public async Task GetListFormBlockTestFilterByLabel()
        {
            var listFormBLockMock = new List<Form>() {
                new Form() {
                    Id = Guid.NewGuid(),
                    ExternalId = 1,
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
                }
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormBLockMock.Where(exp.Compile()).AsQueryable();
            });

            var formBlockList = bll.GetListFormBlock(null, "Form Block 2", Core.Enums.IsUsedParam.All);

            var result = formBlockList.ElementAt(0);
            var source = listFormBLockMock.ElementAt(1);
            Assert.AreEqual(source.ExternalId, result.Id);
            Assert.AreEqual(source.Id, result.OriginalId);
            Assert.AreEqual(source.Labels.Count, result.Labels.Count());

        }


        [TestMethod]
        public async Task UpdateFormBlockAsyncTest()
        {
            Guid formBlockId = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E");
            var updateFormBlockDTO = new DTO.Internal.Forms.UpdateFormBlockDTO
            {
                Id = formBlockId,
                IsActive = true,
                Version = 2,
                Labels = new List<DTO.Internal.Translation.TranslationDTO> {
                    new TranslationDTO()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label 1 Update",
                    }
                }
            };
            var existingFormBlock = new Form()
            {
                Id = formBlockId,
                IsActive = true,
                Version = 1,
                Labels = new List<Translation> {
                    new Translation()
                    {
                        Id = 1,
                        LanguageCode = "en",
                        Text = "Label 1"
                    }
                }
            };
            var formBlocks = formRepository.Find(default).ReturnsForAnyArgs((new[] { existingFormBlock }).AsQueryable());
            unitOfWork.SaveChangesAsync().Returns(1);
            var updateFormBlock = await bll.UpdateFormBlockAsync(formBlockId, updateFormBlockDTO);

            Assert.AreEqual(formBlockId, updateFormBlock.Id);
            Assert.AreEqual(1, updateFormBlock.Labels.Count());
            Assert.AreEqual("Label 1 Update", updateFormBlock.Labels.ElementAt(0).Text);
            Assert.AreEqual(2, updateFormBlock.Version);
        }

        [TestMethod()]
        public async Task UpdateFormBlockAsyncTestArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await bll.UpdateFormBlockAsync(default, default);
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task UpdateFormBlockAsyncTestKeyNotFoundException()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.UpdateFormBlockAsync(Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E"), new UpdateFormBlockDTO());
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task DeleteFormBlockAsyncTest()
        {
            Form formBlock = new Form()
            {
                Id = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E"),
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                Labels = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1
            };

            formRepository.Find(default).ReturnsForAnyArgs((new[] { formBlock }).AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);

            await bll.DeleteFormBlockAsync(Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E"));

            formRepository.ReceivedWithAnyArgs(1).Delete(Arg.Any<Form>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod()]
        public async Task DeleteFormBlockAsyncTestKeyNotFoundException()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await bll.DeleteFormBlockAsync(Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E"));
            });
            formRepository.DidNotReceiveWithAnyArgs().Delete(Arg.Any<Form>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task DeleteFormBlockAsyncTestException()
        {
            Form formBlock = new Form()
            {
                Id = Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E"),
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                DeletedTime = null,
                DeleterUserId = null,
                Labels = new List<Translation>(),
                LatestUpdateTime = DateTime.Now,
                LatestUpdateUserId = 1
            };

            formRepository.Find(default).ReturnsForAnyArgs((new[] { formBlock }).AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(0);
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.DeleteFormBlockAsync(Guid.Parse("5E1BEB26-CFE2-49FF-A07F-36DCFACC1F6E"));
            });
            formRepository.ReceivedWithAnyArgs(1).Delete(Arg.Any<Form>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod()]
        public async Task DeleteFormBlockAsyncTestExceptionFormBlokUsedByForm()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                var formNodeFieldId = Guid.NewGuid();
                var formBlockId = Guid.NewGuid();
                FormNodeFields formNodeFields = new FormNodeFields()
                {
                    Id = formNodeFieldId,
                    Property = "Value",
                    Value = formBlockId.ToString(),
                    DeletedTime = null,
                    FormNodes = new FormNodes()
                    {
                        Form = new Form()
                        {
                            DeletedTime = null
                        }
                    }
                };
                List<FormNodeFields> mockFormNodeFields = new List<FormNodeFields>()
                {
                    formNodeFields
                };

                formNodeFieldsRepository.Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>()).Returns(callinfo =>
                {
                    var exp = callinfo.Arg<Expression<Func<FormNodeFields, bool>>>();
                    return mockFormNodeFields.Where(exp.Compile()).AsQueryable();
                });

                await bll.DeleteFormBlockAsync(formBlockId);
            });
            formRepository.DidNotReceiveWithAnyArgs().Delete(Arg.Any<Form>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        private List<Form> MockFormForFormBLockTest()
        {
            var forms = new List<Form>
            {
                new Form
                {
                    Id = new Guid("46F5BA2C-51F3-45F5-A6E2-DDB216DA9B72"),
                    ExternalId = 7,
                    Version = 1,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Offline,
                    FormNodes = new List<FormNodes>
                    {
                        new FormNodes
                        {
                            Id = new Guid("B358F43F-C204-4061-B2D1-245FB60DD56A"),
                            Type = FormNodeType.FormBlock.ToString()
                        }
                    }
                },
                new Form
                {
                    Id = new Guid("B65DB9E2-C987-4C54-B313-9C3FCF32D8CA"),
                    ExternalId = 7,
                    Version = 2,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Online
                },
                new Form
                {
                    Id = new Guid("BAA3D0EC-75EF-47F7-B151-AB3F9BC22E6C"),
                    ExternalId = 7,
                    Version = 3,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Draft
                },
                new Form
                {
                    Id = new Guid("BBCE2E78-56E5-461B-8F09-4E55D5AFA648"),
                    ExternalId = 8,
                    Version = 1,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Online
                },
                new Form
                {
                    Id = new Guid("4ED88896-4B3A-4400-B7A5-7039CF1E4B4A"),
                    ExternalId = 9,
                    Version = 1,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Online
                },
                new Form
                {
                    Id = new Guid("152AF15E-4C45-492E-B1C7-D847CB4BAAA3"),
                    ExternalId = 12,
                    Version = 0,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Online
                },
                new Form
                {
                    Id = new Guid("6875073A-BBA9-4EED-BF7C-67A664E7627F"),
                    ExternalId = 13,
                    Version = 0,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Online
                },
                new Form
                {
                    Id = new Guid("D9434C93-8305-4605-A5A7-97968009BB33"),
                    ExternalId = 16,
                    Version = 0,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Online
                },
                new Form
                {
                    Id = new Guid("3DED990B-4455-46A7-BE80-59B04578943C"),
                    ExternalId = 18,
                    Version = 0,
                    Type = FormType.Block.ToString(),
                    Status = FormStatus.Online
                }
            };
            return forms;
        }

        private List<FormNodeFields> MockFormNodeFieldsForFormBLockTest()
        {
            var formNodeFields = new List<FormNodeFields>
            {
                new FormNodeFields
                {
                    Id = new Guid("BBCE2E78-56E5-461B-8F09-4E55D5AFA648"),
                    FormNodeId = new Guid("5887EB78-7CB5-489E-B116-0B4A1787C64B"),
                    Property = "Value",
                    Type = "Container",
                    Value = "152af15e-4c45-492e-b1c7-d847cb4baaa3",
                    FormNodes = new FormNodes
                    {
                        Id = new Guid("5887EB78-7CB5-489E-B116-0B4A1787C64B"),
                        Type = FormNodeType.FormBlock.ToString(),
                        Form = new Form
                        {
                            Id = new Guid("147EAC58-FCB5-49B5-8614-4267630C4E80")
                        }
                    }
                },
                new FormNodeFields
                {
                    Id = new Guid("F7E17624-DD07-4FBF-8FAC-2515A4806CD9"),
                    FormNodeId = new Guid("641C0478-DC28-40E1-92C5-A996FD370052"),
                    Property = "Value",
                    Type = "Container",
                    Value = "152af15e-4c45-492e-b1c7-d847cb4baaa3",
                    FormNodes = new FormNodes
                    {
                        Id = new Guid("641C0478-DC28-40E1-92C5-A996FD370052"),
                        Type = FormNodeType.FormBlock.ToString(),
                        Form = new Form
                        {
                            Id = new Guid("EAE8812D-B6F3-4B63-B103-41EB211BDBEB")
                        }
                    }
                },
                new FormNodeFields
                {
                    Id = new Guid("354CA94A-1DF6-4B19-B5A8-6CCC7EA08DA1"),
                    FormNodeId = new Guid("1AFFC0EF-EC2F-4FE6-8BA6-3856F9758188"),
                    Property = "Value",
                    Type = "Container",
                    Value = "152af15e-4c45-492e-b1c7-d847cb4baaa3",
                    FormNodes = new FormNodes
                    {
                        Id = new Guid("1AFFC0EF-EC2F-4FE6-8BA6-3856F9758188"),
                        Type = FormNodeType.FormBlock.ToString(),
                        Form = new Form
                        {
                            Id = new Guid("FF9A8569-99AC-454C-AE27-8F94B58FE8CF")
                        }
                    }
                },
                new FormNodeFields
                {
                    Id = new Guid("B1E55CAD-D997-42E3-B06A-73162F35FFFD"),
                    FormNodeId = new Guid("8C4239A9-AC4A-4D56-9B78-97DF01983E51"),
                    Property = "Value",
                    Type = "Container",
                    Value = "69ba0e50-019f-4ce7-8853-c465f1d38bde",
                    FormNodes = new FormNodes
                    {
                        Id = new Guid("8C4239A9-AC4A-4D56-9B78-97DF01983E51"),
                        Type = FormNodeType.FormBlock.ToString(),
                        Form = new Form
                        {
                            Id = new Guid("69BA0E50-019F-4CE7-8853-C465F1D38BDE")
                        }
                    }
                },
                new FormNodeFields
                {
                    Id = new Guid("11043A46-C763-4116-8DAB-A4230AF40784"),
                    FormNodeId = new Guid("B358F43F-C204-4061-B2D1-245FB60DD56A"),
                    Property = "Value",
                    Type = "Container",
                    Value = "bbce2e78-56e5-461b-8f09-4e55d5afa648",
                    FormNodes = new FormNodes
                    {
                        Id = new Guid("B358F43F-C204-4061-B2D1-245FB60DD56A"),
                        Type = FormNodeType.FormBlock.ToString(),
                        Form = new Form
                        {
                            Id = new Guid("46F5BA2C-51F3-45F5-A6E2-DDB216DA9B72")
                        }
                    }
                }
            };
            return formNodeFields;
        }
    }
}

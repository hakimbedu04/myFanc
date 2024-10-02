using AutoMapper;
using Microsoft.Extensions.Logging;
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
    public class MyFanc_Form_Edit_FormContent_Test
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
            this.formConditionalsRepository = Substitute.For<IGenericRepository<FormConditionals>>();
            this.nacabelRepository = Substitute.For<IGenericRepository<Nacabel>>();
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
        public async Task DuplicateFormTest()
        {
            Guid duplicatedFormId = Guid.NewGuid();
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
                Status = FormStatus.Online,
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

            Form formDuplicateResult = null;

            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormDbMock.Where(exp.Compile()).AsQueryable();
            });

            formRepository.When(f => f.Add(Arg.Any<Form>())).Do(c => {
                c.ArgAt<Form>(0).Id = duplicatedFormId;
                formDuplicateResult = c.ArgAt<Form>(0);
            });
            unitOfWork.SaveChangesAsync().Returns(1);

            var result = await bll.DuplicateForm(formId);

            formRepository.Received(1).Find(Arg.Any<Expression<Func<Form, bool>>>());
            formRepository.Received(1).Add(Arg.Any<Form>());
            await unitOfWork.Received(3).SaveChangesAsync();
            formConditionalsRepository.Received(2).Update(Arg.Any<FormConditionals>());

            Assert.IsTrue(result.GetType().Equals(typeof(Guid)));
            Assert.AreEqual(duplicatedFormId, result);




            Assert.AreEqual(formEntityMock.FormNodes.Count(), formDuplicateResult.FormNodes.Count());
            Assert.AreEqual(formEntityMock.Labels.ElementAt(0).Text, formDuplicateResult.Labels.ElementAt(0).Text);
            Assert.AreEqual(formEntityMock.FormNodes.ElementAt(0).FieldType, formDuplicateResult.FormNodes.ElementAt(0).FieldType);
            Assert.AreEqual(formEntityMock.FormNodes.ElementAt(1).FieldType, formDuplicateResult.FormNodes.ElementAt(1).FieldType);
            Assert.AreEqual(formEntityMock.FormNodes.ElementAt(2).FieldType, formDuplicateResult.FormNodes.ElementAt(2).FieldType);

            Assert.AreEqual(FormNodeFieldEncodeType.PredefineList.ToString(), formDuplicateResult.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Type);
            Assert.AreEqual(formEntityMock.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Value, formDuplicateResult.FormNodes.ElementAt(2).FormNodeFields.ElementAt(2).Value);

            Assert.AreEqual(FormNodeFieldEncodeType.CustomList.ToString(), formDuplicateResult.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).Type);
            var dupCustList = formDuplicateResult.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).FormValueFields;
            if (dupCustList != null)
            {
                var sourceCustList = formEntityMock.FormNodes.ElementAt(1).FormNodeFields.ElementAt(1).FormValueFields;
                for (int i = 0; i < dupCustList.Count; i++)
                {
                    var src = sourceCustList.ElementAt(i);
                    var dup = dupCustList.ElementAt(i);
                    Assert.AreEqual(src.Value, dup.Value);
                    Assert.AreEqual(src.Labels.Count(), dup.Labels.Count());
                }
            }

            Assert.AreEqual(FormNodeFieldEncodeType.Conditional.ToString(), formDuplicateResult.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).Type);
            var dupCondList = formDuplicateResult.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).FormConditionals;
            if (dupCondList != null)
            {
                var srcCondList = formEntityMock.FormNodes.ElementAt(1).FormNodeFields.ElementAt(2).FormConditionals;
                for (int i = 0; i < dupCondList.Count; i++)
                {
                    var src = srcCondList.ElementAt(i);
                    var dup = dupCondList.ElementAt(i);
                    Assert.AreEqual(src.Condition, dup.Condition);
                    Assert.AreEqual(src.State, dup.State);
                    Assert.AreNotEqual(src.FormNodeId, dup.FormNodeId);
                }
            }

            Assert.AreEqual(FormNodeFieldEncodeType.Container.ToString(), formDuplicateResult.FormNodes.ElementAt(3).FormNodeFields.ElementAt(0).Type);
            Assert.AreEqual(formBlockEntityMock.Id.ToString(), formDuplicateResult.FormNodes.ElementAt(3).FormNodeFields.ElementAt(0).Value);
            Assert.AreEqual("Value", formDuplicateResult.FormNodes.ElementAt(3).FormNodeFields.ElementAt(0).Property);

        }

        [TestMethod()]
        public async Task DuplicateFormTesArgumentExeption()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.DuplicateForm(default);
            });
            formRepository.DidNotReceiveWithAnyArgs().Find(Arg.Any<Expression<Func<Form, bool>>>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public async Task DuplicateFormTesArgumentExeptio2()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await bll.DuplicateForm(Guid.NewGuid());
            });
            formRepository.ReceivedWithAnyArgs(1).Find(Arg.Any<Expression<Func<Form, bool>>>());
            await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync();
        }

        [TestMethod()]
        public void IsOfflineFormHaveOnlineVersionTestOnlineVErsionAvailable()
        {
            var externalId = 1;
            var existingOnlineForm = new Form()
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                IsActive = true,
                Version = 1,
                Status = FormStatus.Online,
                Type = FormType.Webform.ToString()
            };

            var listFormMock = new List<Form>()
            {
                existingOnlineForm
            };


            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var result = bll.IsOfflineFormHaveOnlineVersion(externalId);

            Assert.AreEqual(true, result.IsHaveOnlineVersionForm);
            Assert.AreEqual(existingOnlineForm.Id, result.OnlineFormId);
        }

        [TestMethod()]
        public void IsOfflineFormHaveOnlineVersionTestOnlineVersionNotAvailable()
        {
            var externalId = 1;
            var existingOnlineForm = new Form()
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                IsActive = true,
                Version = 1,
                Status = FormStatus.Offline,
                Type = FormType.Webform.ToString()
            };

            var listFormMock = new List<Form>()
            {
                existingOnlineForm
            };


            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var result = bll.IsOfflineFormHaveOnlineVersion(externalId);

            Assert.AreEqual(false, result.IsHaveOnlineVersionForm);
            Assert.AreEqual(Guid.Empty, result.OnlineFormId);
        }

        [TestMethod()]
        public void IsOfflineFormHaveOnlineVersionTestArgumentExeption()
        {
             Assert.ThrowsException<ArgumentException>(() =>
            {
                bll.IsOfflineFormHaveOnlineVersion(default);
            });
            formRepository.DidNotReceiveWithAnyArgs().Find(Arg.Any<Expression<Func<Form, bool>>>());
        }
    }
}

using AutoMapper;
using Microsoft.Extensions.Logging;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.Services.FancRadApi;
using MyFanc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.Api.Mapper;
using MyFanc.DTO.Internal.Forms;
using NSubstitute;
using System.Linq.Expressions;
using static MyFanc.Core.Enums;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_Form_FormNode_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IGenericRepository<Form> formBlockRepository;
        private IGenericRepository<FormNodes> formNodesRepository;
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
            this.formBlockRepository = Substitute.For<IGenericRepository<Form>>();
            this.formNodesRepository = Substitute.For<IGenericRepository<FormNodes>>();
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.sharedDataCache = Substitute.For<ISharedDataCache>();
            this.identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();

            this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();

            this.unitOfWork.GetGenericRepository<Form>().Returns(this.formBlockRepository);
            this.unitOfWork.GetGenericRepository<FormNodes>().Returns(this.formNodesRepository);

            MapperConfiguration mapperConfig = new(
            cfg =>
            {
                cfg.AddProfile(new WizardProfile());
                cfg.AddProfile(new FormProfile());
            });

            var implMapper = new Mapper(mapperConfig);
            this.mapper = Substitute.For<IMapper>();

            this.mapper.Map<List<NodesDTO>>(Arg.Any<List<FormNodes>>())
                .Returns(callinfo => implMapper.Map<List<NodesDTO>>(callinfo.Arg<List<FormNodes>>()));

            this.mapper.Map<PreviewDetailsDTO>(Arg.Any<Form>())
                .Returns(callinfo => implMapper.Map<PreviewDetailsDTO>(callinfo.Arg<Form>()));
            this.mapper.ConfigurationProvider.AssertConfigurationIsValid();
            this.bll = new Bll(unitOfWork, mapper, logger, fancRADApi, breadCrumbService, sharedDataCache, emailService, tokenConfiguration, aESEncryptService, nacabelHelper, fileStorage, identityProviderConfiguration);
        }

        private Form MockForm()
        {
            Guid formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d66");
            var form = new Form
            {
                Id = formId,
                ExternalId = 6,
                Labels = new List<Translation>()
                {
                    new Translation() { LanguageCode = "en", Text = "Label FORM 6 en" },
                    new Translation() { LanguageCode = "fr", Text = "Label FORM 6 fr" }

                },
                Version = 1,
                IsActive = true,
                Type = FormType.Block.ToString(),
                Status = FormStatus.Online,
                FormCategoryId = 1,
                Descriptions = new List<Translation>()
                {
                    new Translation() { LanguageCode = "en", Text = "description FORM 6 en" },
                    new Translation() { LanguageCode = "fr", Text = "description FORM 6 fr" }

                },
                Nacabels = new HashSet<Nacabel>
                {
                    new Nacabel
                    {
                        Id = 2,
                        NacabelCode = "01.01",
                        NacabelTranslation = new List<NacabelTranslation> {
                            new NacabelTranslation
                            {
                                Id = 1,
                                NacabelId = 1,
                                LanguageCode = "en",
                                Description = "Dentist"
                            },
                            new NacabelTranslation
                            {
                                Id = 2,
                                NacabelId = 2,
                                LanguageCode = "fr",
                                Description = "Dentiste"
                            },
                            new NacabelTranslation
                            {
                                Id = 3,
                                NacabelId = 2,
                                LanguageCode = "nl",
                                Description = "Tandarts"
                            }
                        }
                    },
                    new Nacabel
                    {
                        Id = 3,
                        NacabelCode = "01.02",
                        NacabelTranslation = new List<NacabelTranslation> {
                            new NacabelTranslation
                            {
                                Id = 4,
                                NacabelId = 3,
                                LanguageCode = "en",
                                Description = "Radiopharma"
                            },
                            new NacabelTranslation
                            {
                                Id = 5,
                                NacabelId = 3,
                                LanguageCode = "fr",
                                Description = "Radiopharma"
                            },
                            new NacabelTranslation
                            {
                                Id = 7,
                                NacabelId = 3,
                                LanguageCode = "nl",
                                Description = "RadioFarma"
                            }
                        }
                    }
                },
                FormDocuments = new List<FormDocument>
                {
                    new FormDocument
                    {
                        Id = new Guid("e55c037c-13a9-452e-9928-5b631d21aa8f"),
                        Documents = new List<Document> {
                            new Document
                            {
                                Id = new Guid("7e4edc49-af10-4034-b195-61892b4f1d07"),
                                Path = "www.blobstorage.com/form6_en_pdf",
                                Type = "application/pdf"
                            }
                        }

                    },
                    new FormDocument
                    {
                        Id = new Guid("d9568e32-dc21-4eb3-96df-96d604d13a0a"),
                        Documents = new List<Document> {
                            new Document
                            {
                                Id = new Guid("ace04d5c-9fbc-4cbf-a0e6-4e58506c3abf"),
                                Path = "www.blobstorage.com/form6_fr_pdf",
                                Type = "application/pdf"
                            }
                        }

                    }
                },
                Urls = new List<Translation>()
                {
                    new Translation() { LanguageCode = "en", Text = "label_form_6_url_en" },
                    new Translation() { LanguageCode = "fr", Text = "label_form_6_url_fr" }

                },
                FormNodes = new List<FormNodes>(){
                    new FormNodes
                    {
                        Id = new Guid("78262acc-769e-4da0-a336-507dafa5f254"),
                        ParentId = Guid.Empty,
                        Order = 1,
                        Type = "FormField",
                        Labels = new List<Translation>()
                        {
                            new Translation() { LanguageCode = "en", Text = "THIS IS FORM NODE" }
                        },
                        FormNodeFields = new List<FormNodeFields>{
                            new FormNodeFields
                            {
                                Property = "List",
                                Type = "CustomList",
                                Value = "0e91d978-55e2-48ff-9dc5-eb3d2b75689b"
                            },
                            new FormNodeFields
                            {
                                Property = "List",
                                Type = "PredefineList",
                                Value = "Organisation.MainAddress.CountryCode"
                            },
                            new FormNodeFields
                            {
                                Property = "List",
                                Type = "PredefineList",
                                Value = "Organisation.Name"
                            },
                            new FormNodeFields
                            {
                                Property = "List",
                                Type = "PredefineList",
                                Value = "ConnectedUser.LanguageCode"
                            },
                            new FormNodeFields
                            {
                                Property = "List",
                                Type = "PredefineList",
                                Value = "ConnectedUser.StructuredAddress.StreetName"
                            },
                            new FormNodeFields
                            {
                                Property = "List",
                                Type = "PredefineList",
                                Value = "ConnectedUser.LastName"
                            },
                            new FormNodeFields
                            {
                                Property = "List",
                                Type = "PredefineList",
                                Value = "ConnectedUser.FirstName"
                            },
                            new FormNodeFields
                            {
                                Property = "Conditional Logic",
                                Type = "Conditional",
                                Value = "[{ 'id': null, 'condition': 'condition', 'state': 'show', 'formNodeId': '78262ACC-769E-4DA0-A336-507DAFA5F254' }, { 'id': null, 'condition': 'condition', 'state': 'hide', 'formNodeId': '78262ACC-769E-4DA0-A336-507DAFA5F254' }]",
                                FormConditionals = new List<FormConditionals>
                                {
                                    new FormConditionals
                                    {
                                        Id = new Guid("4b7e3055-09b0-4b4d-bb73-62ce91b753b7"),
                                        Condition = "condition",
                                        State = "show",
                                        FormNodeId = new Guid("05f59b21-62b4-4b30-a83f-dafc34e1ca1d"),
                                    },
                                    new FormConditionals
                                    {
                                        Id = new Guid("446425c8-59f8-4eff-8c93-e44f143a57a4"),
                                        Condition = "condition",
                                        State = "hide",
                                        FormNodeId = new Guid("8c4239a9-ac4a-4d56-9b78-97df01983e51"),
                                    }
                                }
                            },
                            new FormNodeFields
                            {
                                Property = "List",
                                Type = "PredefineList",
                                Value = "Organisation.FancOrganisationId"
                            },
                            new FormNodeFields
                            {
                                Property = "Conditional Logic",
                                Type = "Conditional",
                                Value = "[{ 'id': null, 'condition': 'condition', 'state': 'show', 'formNodeId': '78262ACC-769E-4DA0-A336-507DAFA5F254' }, { 'id': null, 'condition': 'condition', 'state': 'hide', 'formNodeId': '78262ACC-769E-4DA0-A336-507DAFA5F254' }]",
                                FormConditionals = new List<FormConditionals>
                                {
                                    new FormConditionals
                                    {
                                        Id = new Guid("234e9e84-821a-4c5f-bba7-cf4d201d995c"),
                                        Condition = "condition",
                                        State = "hide",
                                        FormNodeId = new Guid("78262acc-769e-4da0-a336-507dafa5f254"),
                                    },
                                    new FormConditionals
                                    {
                                        Id = new Guid("7f87dd04-f886-4e09-a9a9-fa218f0f4ac1"),
                                        Condition = "condition",
                                        State = "show",
                                        FormNodeId = new Guid("78262acc-769e-4da0-a336-507dafa5f254"),
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return form;
        }

        [TestMethod()]
        public async Task DeleteSectionFromFormNodeTest()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d66");
            var formNodeId = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b");
            var form = MockForm();
            var formNodes = new List<FormNodes>
            {
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 1
                },
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 2
                }
            };

            form.FormNodes = formNodes;

            // Act
            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).Returns(form.FormNodes.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            await bll.DeleteFormNodeSectionAsync(formId, formNodeId);

            //// Assert
            formNodesRepository.ReceivedWithAnyArgs(1).Delete(Arg.Any<FormNodes>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod()]
        public async Task DeleteSectionFromFormNodeTest_FailedDelete()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d66");
            var formNodeId = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b");
            var form = MockForm();
            var formNodes = new List<FormNodes>
            {
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 1
                },
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 2
                }
            };

            form.FormNodes = formNodes;

            // Act
            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).Returns(form.FormNodes.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(0);

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.DeleteFormNodeSectionAsync(formId, formNodeId);
            });
            formNodesRepository.ReceivedWithAnyArgs(1).Delete(Arg.Any<FormNodes>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task DeleteSectionFromFormNodeTest_OnlyOneSection()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d66");
            var formNodeId = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b");
            var form = MockForm();
            var formNodes = new List<FormNodes>
            {
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 1
                }
            };

            form.FormNodes = formNodes;

            // Act
            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).Returns(form.FormNodes.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            try
            {
                await bll.DeleteFormNodeSectionAsync(formId, formNodeId);
            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"You cannot delete a section when it is the only section of a form.", ex.Message);
                throw;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteFormNodeSectionAsync_ReturnsKeyNotFound()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d65");

            try
            {
                var formNodeId = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572");
                var form = MockForm();

                var formNodes = new List<FormNodes>()
                {
                    new FormNodes()
                    {
                        FormId = formId,
                        Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                        Type = "Section",
                        FieldType = "Text",
                        Order = 1
                    },
                    new FormNodes()
                    {
                        FormId = formId,
                        Id = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b"),
                        Type = "Section",
                        FieldType = "Text",
                        Order = 2
                    }
                };
                form.FormNodes = formNodes;

                // Act
                formBlockRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs((new[] { form }).AsQueryable());
                await bll.DeleteFormNodeSectionAsync(formId, formNodeId);

            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"Delete form field tried on unexisting form id {formId}", ex.Message);
                throw;
            }
        }
        
        [TestMethod()]
        public async Task DeleteFormNodeAsyncTest()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d66");
            var formNodeId = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b");
            var form = MockForm();
            var formNodes = new List<FormNodes>
            {
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 1
                },
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 2
                }
            };

            form.FormNodes = formNodes;

            // Act
            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).Returns(form.FormNodes.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(1);
            await bll.DeleteFormNodeAsync(formNodeId);

            //// Assert
            formNodesRepository.ReceivedWithAnyArgs(1).Delete(Arg.Any<FormNodes>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod()]
        public async Task DeleteFormNodeAsyncTest_FailedDelete()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d66");
            var formNodeId = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b");
            var form = MockForm();
            var formNodes = new List<FormNodes>
            {
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 1
                },
                new FormNodes
                {
                    FormId = formId,
                    Id = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b"),
                    Type = "Section",
                    FieldType = "Text",
                    Order = 2
                }
            };

            form.FormNodes = formNodes;

            // Act
            formNodesRepository.Find(Arg.Any<Expression<Func<FormNodes, bool>>>()).Returns(form.FormNodes.AsQueryable());
            unitOfWork.SaveChangesAsync().ReturnsForAnyArgs(0);

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await bll.DeleteFormNodeAsync(formNodeId);
            });
            formNodesRepository.ReceivedWithAnyArgs(1).Delete(Arg.Any<FormNodes>());
            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteFormNodeAsyncAsync_ReturnsKeyNotFound()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d65");
            var formNodeId = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572");
            try
            {
                var form = MockForm();

                var formNodes = new List<FormNodes>()
                {
                    new FormNodes()
                    {
                        FormId = formId,
                        Id = new Guid("8cb426f0-da66-4263-956b-8ea094c2d572"),
                        Type = "Section",
                        FieldType = "Text",
                        Order = 1
                    },
                    new FormNodes()
                    {
                        FormId = formId,
                        Id = new Guid("5887eb78-7cb5-489e-b116-0b4a1787c64b"),
                        Type = "Section",
                        FieldType = "Text",
                        Order = 2
                    }
                };
                form.FormNodes = formNodes;

                // Act
                formBlockRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs((new[] { form }).AsQueryable());
                await bll.DeleteFormNodeAsync(formNodeId);

            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"Delete form field tried on unexisting form field id {formNodeId}", ex.Message);
                throw;
            }
        }
    }
}

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
    public class MyFanc_Form_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IGenericRepository<Form> formRepository;
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
            this.nacabelHelper = Substitute.For<INacabelHelper>();
            this.fileStorage = Substitute.For<IFileStorage>();
            this.sharedDataCache = Substitute.For<ISharedDataCache>();
            this.identityProviderConfiguration = Substitute.For<IIdentityProviderConfiguration>();

            this.logger = Substitute.For<ILogger<Bll>>();
            this.fancRADApi = Substitute.For<IFancRADApi>();

            this.unitOfWork.GetGenericRepository<Form>().Returns(this.formRepository);

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

        [TestMethod()]
        public async Task GetFormObjectList()
        {
            // Arrange
            var expectedProperties = new List<string>
            {
                "ConnectedUser.Id",
                "ConnectedUser.NrNumber",
                "ConnectedUser.FirstName",
                "ConnectedUser.LastName",
                "ConnectedUser.LanguageCode",
                "ConnectedUser.InterfaceLanguageCode",
                "ConnectedUser.GenderCode",
                "ConnectedUser.BirthDate",
                "ConnectedUser.BirthPlace",
                "ConnectedUser.StructuredAddress",
                "ConnectedUser.UnstructuredAddress",
                "ConnectedUser.Email",
                "ConnectedUser.Phone1",
                "ConnectedUser.Phone2",
                "ConnectedUser.LastAuthenticSourceRefreshDate",
                "ConnectedUser.ManualUpdateAllowed",
                "ConnectedUser.ValidationStatus",
                "ConnectedUser.NationalityCode",
                "ConnectedUser.ForeignIdentityOrPassportNumber",
                "Organisation.FancOrganisationId",
                "Organisation.FancNumber",
                "Organisation.EnterpriseCBENumber",
                "Organisation.Name",
                "Organisation.DisplayName",
                "Organisation.Email",
                "Organisation.Phone",
                "Organisation.MainAddress",
                "Organisation.InvoiceAddress",
                "Organisation.Activated",
                "Organisation.Nacebel2008Codes",
                "Organisation.Establishments",
                "Organisation.Sectors",
                "Organisation.MainAddressIsInvoiceAddress",
				"Organisation.DefaultLanguageCode",
				"ConnectedUser.StructuredAddress.CountryCode",
                "ConnectedUser.StructuredAddress.StreetName",
                "ConnectedUser.StructuredAddress.HouseNumber",
                "ConnectedUser.StructuredAddress.PostalCode",
                "ConnectedUser.StructuredAddress.CityName",
                "ConnectedUser.UnstructuredAddress.CountryCode",
                "ConnectedUser.UnstructuredAddress.Address",
                "Organisation.MainAddress.CountryCode",
                "Organisation.MainAddress.StreetName",
                "Organisation.MainAddress.HouseNumber",
                "Organisation.MainAddress.PostalCode",
                "Organisation.MainAddress.CityName",
                "Organisation.InvoiceAddress.CountryCode",
                "Organisation.InvoiceAddress.StreetName",
                "Organisation.InvoiceAddress.HouseNumber",
                "Organisation.InvoiceAddress.PostalCode",
                "Organisation.InvoiceAddress.CityName",
                "Organisation.Sectors.Id",
                "Organisation.Sectors.Code",
                "Organisation.Sectors.Label"
            };

            // Act
            var result = bll.GetFormObjectList();

            // Assert
            Assert.AreEqual(expectedProperties.Count, result.Count);
            foreach (var expectedProperty in expectedProperties)
            {
                Assert.IsTrue(result.Contains(expectedProperty));
            }
        }

        [TestMethod()]
        public void GetFormObjectList_DoesNotReturnUnexpectedProperties()
        {
            // Arrange
            var unexpectedProperties = new List<string>
            {
                "UnexpectedProperty1",
                "UnexpectedProperty2",
                "UnexpectedProperty3",
            };

            // Act
            var result = bll.GetFormObjectList();

            // Assert
            foreach (var unexpectedProperty in unexpectedProperties)
            {
                Assert.IsFalse(result.Contains(unexpectedProperty));
            }
        }

        [TestMethod()]
        public void PreviewAsync_ReturnsPreviewFormDTO()
        {
            // Arrange
            var form = MockFormForPreviewForm();
            var previewForm = MockResultPreviewForm();
           
            // Act
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).ReturnsForAnyArgs((new[] { form }).AsQueryable());

            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d66");
            PreviewFormDTO result = bll.PreviewAsync(formId);
            

            Assert.IsNotNull(result);
            Assert.AreEqual(previewForm.FormId, result.FormId);
            // Details
            Assert.AreEqual(previewForm.Details.CaregoryId, result.Details.CaregoryId);
            Assert.AreEqual(previewForm.Details.ExternalId, result.Details.ExternalId);
            Assert.AreEqual(previewForm.Details.Type, result.Details.Type);
            Assert.AreEqual(previewForm.Details.Version, result.Details.Version);
            Assert.AreEqual(previewForm.Details.Description.Count(), result.Details.Description.Count());

            Assert.AreEqual(previewForm.Details.Description.ToArray()[0].LanguageCode, result.Details.Description.ToArray()[0].LanguageCode);
            Assert.AreEqual(previewForm.Details.Description.ToArray()[0].Text, result.Details.Description.ToArray()[0].Text);
            Assert.AreEqual(previewForm.Details.Description.ToArray()[1].LanguageCode, result.Details.Description.ToArray()[1].LanguageCode);
            Assert.AreEqual(previewForm.Details.Description.ToArray()[1].Text, result.Details.Description.ToArray()[1].Text);

            Assert.AreEqual(previewForm.Details.CaregoryId, result.Details.CaregoryId);

            Assert.AreEqual(previewForm.Details.Labels.Count(), result.Details.Labels.Count());
            Assert.AreEqual(previewForm.Details.Labels.ToArray()[0].LanguageCode, result.Details.Labels.ToArray()[0].LanguageCode);
            Assert.AreEqual(previewForm.Details.Labels.ToArray()[0].Text, result.Details.Labels.ToArray()[0].Text);
            Assert.AreEqual(previewForm.Details.Labels.ToArray()[1].LanguageCode, result.Details.Labels.ToArray()[1].LanguageCode);
            Assert.AreEqual(previewForm.Details.Labels.ToArray()[1].Text, result.Details.Labels.ToArray()[1].Text);

            Assert.AreEqual(previewForm.Details.Documents.Count(), result.Details.Documents.Count());
            Assert.AreEqual(previewForm.Details.Documents.ToArray()[0].Url, result.Details.Documents.ToArray()[0].Url);
            Assert.AreEqual(previewForm.Details.Documents.ToArray()[0].Type, result.Details.Documents.ToArray()[0].Type);
            Assert.AreEqual(previewForm.Details.Documents.ToArray()[1].Url, result.Details.Documents.ToArray()[1].Url);
            Assert.AreEqual(previewForm.Details.Documents.ToArray()[1].Type, result.Details.Documents.ToArray()[1].Type);

            Assert.AreEqual(previewForm.Details.Urls.Count(), result.Details.Urls.Count());
            Assert.AreEqual(previewForm.Details.Urls.ToArray()[0].Url, result.Details.Urls.ToArray()[0].Url);
            Assert.AreEqual(previewForm.Details.Urls.ToArray()[0].LanguageCode, result.Details.Urls.ToArray()[0].LanguageCode);
            Assert.AreEqual(previewForm.Details.Urls.ToArray()[1].Url, result.Details.Urls.ToArray()[1].Url);
            Assert.AreEqual(previewForm.Details.Urls.ToArray()[1].LanguageCode, result.Details.Urls.ToArray()[1].LanguageCode);

            // Nodes
            Assert.AreEqual(previewForm.Nodes.Count(), result.Nodes.Count());
            Assert.AreEqual(previewForm.Nodes.ToArray()[0].Id, result.Nodes.ToArray()[0].Id);
            Assert.AreEqual(previewForm.Nodes.ToArray()[0].ParentId, result.Nodes.ToArray()[0].ParentId);
            Assert.AreEqual(previewForm.Nodes.ToArray()[0].Labels.Count(), result.Nodes.ToArray()[0].Labels.Count());
            Assert.AreEqual(previewForm.Nodes.ToArray()[0].Labels.ToArray()[0].LanguageCode, result.Nodes.ToArray()[0].Labels.ToArray()[0].LanguageCode);
            Assert.AreEqual(previewForm.Nodes.ToArray()[0].Labels.ToArray()[0].Text, result.Nodes.ToArray()[0].Labels.ToArray()[0].Text);
            Assert.AreEqual(previewForm.Nodes.ToArray()[0].Properties.Count(), result.Nodes.ToArray()[0].Properties.Count());
            for (var i = 0; i < previewForm.Nodes.ToArray()[0].Properties.Count(); i++)
            {
                Assert.AreEqual(previewForm.Nodes.ToArray()[0].Properties.ToArray()[i].Name, result.Nodes.ToArray()[0].Properties.ToArray()[i].Name);
                Assert.AreEqual(previewForm.Nodes.ToArray()[0].Properties.ToArray()[i].Value, result.Nodes.ToArray()[0].Properties.ToArray()[i].Value);
                Assert.AreEqual(previewForm.Nodes.ToArray()[0].Properties.ToArray()[i].Type, result.Nodes.ToArray()[0].Properties.ToArray()[i].Type);
            }
            Assert.AreEqual(previewForm.Nodes.ToArray()[0].Conditionals.Count(), result.Nodes.ToArray()[0].Conditionals.Count());

            for (var i = 0; i < previewForm.Nodes.ToArray()[0].Conditionals.Count(); i++)
            {
                Assert.AreEqual(previewForm.Nodes.ToArray()[0].Conditionals.ToArray()[i].Id, result.Nodes.ToArray()[0].Conditionals.ToArray()[i].Id);
                Assert.AreEqual(previewForm.Nodes.ToArray()[0].Conditionals.ToArray()[i].Condition, result.Nodes.ToArray()[0].Conditionals.ToArray()[i].Condition);
                Assert.AreEqual(previewForm.Nodes.ToArray()[0].Conditionals.ToArray()[i].State, result.Nodes.ToArray()[0].Conditionals.ToArray()[i].State);
                Assert.AreEqual(previewForm.Nodes.ToArray()[0].Conditionals.ToArray()[i].FormNodeId, result.Nodes.ToArray()[0].Conditionals.ToArray()[i].FormNodeId);
            }
        }

        private Form MockFormForPreviewForm()
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
    
        private PreviewFormDTO MockResultPreviewForm()
        {
            var previewFormDTO = new PreviewFormDTO
            {
                FormId = new Guid("f74091a3-453a-4139-82a1-4cae27913d66"),
                Nodes = new List<NodesDTO>
                {
                    new NodesDTO
                    {
                        Id = new Guid("78262acc-769e-4da0-a336-507dafa5f254"),
                        ParentId = Guid.Empty,
                        Order = 1,
                        Labels = new List<TranslationDTO>
                        {
                            new TranslationDTO { LanguageCode = "en", Text = "THIS IS FORM NODE" }
                        },
                        Type = "FormField",
                        Properties = new List<PropertiesDTO>
                        {
                            new PropertiesDTO
                            {
                                Name = "List",
                                Value =  "0e91d978-55e2-48ff-9dc5-eb3d2b75689b",
                                Type = "CustomList"
                            },
                            new PropertiesDTO
                            {
                                Name = "List",
                                Value =  "Organisation.MainAddress.CountryCode",
                                Type = "PredefineList"
                            },
                            new PropertiesDTO
                            {
                                Name = "List",
                                Value =  "Organisation.Name",
                                Type = "PredefineList"
                            },
                            new PropertiesDTO
                            {
                                Name = "List",
                                Value =  "ConnectedUser.LanguageCode",
                                Type = "PredefineList"
                            },
                            new PropertiesDTO
                            {
                                Name = "List",
                                Value =  "ConnectedUser.StructuredAddress.StreetName",
                                Type = "PredefineList"
                            },
                            new PropertiesDTO
                            {
                                Name = "List",
                                Value =  "ConnectedUser.LastName",
                                Type = "PredefineList"
                            },
                            new PropertiesDTO
                            {
                                Name = "List",
                                Value =  "ConnectedUser.FirstName",
                                Type = "PredefineList"
                            },
                            new PropertiesDTO
                            {
                                Name = "Conditional Logic",
                                Value =  "[{ 'id': null, 'condition': 'condition', 'state': 'show', 'formNodeId': '78262ACC-769E-4DA0-A336-507DAFA5F254' }, { 'id': null, 'condition': 'condition', 'state': 'hide', 'formNodeId': '78262ACC-769E-4DA0-A336-507DAFA5F254' }]",
                                Type = "Conditional"
                            },
                            new PropertiesDTO
                            {
                                Name = "List",
                                Value = "Organisation.FancOrganisationId",
                                Type = "PredefineList"
                            },
                            new PropertiesDTO
                            {
                                Name = "Conditional Logic",
                                Value =  "[{ 'id': null, 'condition': 'condition', 'state': 'show', 'formNodeId': '78262ACC-769E-4DA0-A336-507DAFA5F254' }, { 'id': null, 'condition': 'condition', 'state': 'hide', 'formNodeId': '78262ACC-769E-4DA0-A336-507DAFA5F254' }]",
                                Type = "Conditional"
                            }
                        }
                        ,Conditionals = new List<FormConditionalDTO>
                        {
                            new FormConditionalDTO
                            {
                                Id= new Guid("4b7e3055-09b0-4b4d-bb73-62ce91b753b7"),
                                Condition = "condition",
                                State= "show",
                                FormNodeId = new Guid("05f59b21-62b4-4b30-a83f-dafc34e1ca1d")
                            },
                            new FormConditionalDTO
                            {
                                Id= new Guid("446425c8-59f8-4eff-8c93-e44f143a57a4"),
                                Condition = "condition",
                                State= "hide",
                                FormNodeId = new Guid("8c4239a9-ac4a-4d56-9b78-97df01983e51")
                            },
                            new FormConditionalDTO
                            {
                                Id= new Guid("234e9e84-821a-4c5f-bba7-cf4d201d995c"),
                                Condition = "condition",
                                State= "hide",
                                FormNodeId = new Guid("78262acc-769e-4da0-a336-507dafa5f254")
                            },
                            new FormConditionalDTO
                            {
                                Id= new Guid("7f87dd04-f886-4e09-a9a9-fa218f0f4ac1"),
                                Condition = "condition",
                                State= "show",
                                FormNodeId = new Guid("78262acc-769e-4da0-a336-507dafa5f254")
                            }
                        }
                    }
                },
                Details = new PreviewDetailsDTO
                {
                    Version = "1",
                    CaregoryId = "1",
                    Labels = new List<TranslationDTO>()
                    {
                        new TranslationDTO() { LanguageCode = "en", Text = "Label FORM 6 en" },
                        new TranslationDTO() { LanguageCode = "fr", Text = "Label FORM 6 fr" }
                    },
                    Description = new List<TranslationDTO>()
                    {
                        new TranslationDTO() { LanguageCode = "en", Text = "description FORM 6 en" },
                        new TranslationDTO() { LanguageCode = "fr", Text = "description FORM 6 fr" }

                    },
                    Nacabels = new List<PreviewDetailsNacabelDTO>
                    {
                        new PreviewDetailsNacabelDTO
                        {
                            Code = "01.01",
                            Translations = new List<TranslationDTO>()
                            {
                                new TranslationDTO
                                {
                                    Id = 1,
                                    LanguageCode = "en",
                                    Text = "Dentist"
                                },
                                new TranslationDTO
                                {
                                    Id = 2,
                                    LanguageCode = "fr",
                                    Text = "Dentiste"
                                },
                                new TranslationDTO
                                {
                                    Id = 3,
                                    LanguageCode = "nl",
                                    Text = "Tandarts"
                                }
                            }
                        },
                        new PreviewDetailsNacabelDTO
                        {
                            Code = "01.02",
                            Translations = new List<TranslationDTO>()
                            {
                                new TranslationDTO
                                {
                                    Id = 4,
                                    LanguageCode = "en",
                                    Text = "Radiopharma"
                                },
                                new TranslationDTO
                                {
                                    Id = 5,
                                    LanguageCode = "fr",
                                    Text = "Radiopharma"
                                },
                                new TranslationDTO
                                {
                                    Id = 7,
                                    LanguageCode = "nl",
                                    Text = "RadioFarma"
                                }
                            }
                        }
                    },
                    Type = FormType.Block.ToString(),
                    ExternalId = 6,
                    Documents = new List<PreviewDetailsDocumentDTO> {
                        new PreviewDetailsDocumentDTO
                    {
                        Id = new Guid("e55c037c-13a9-452e-9928-5b631d21aa8f"),
                        Url = "www.blobstorage.com/form6_en_pdf",
                        Type = "application/pdf"
                    },
                    new PreviewDetailsDocumentDTO
                    {
                         Id = new Guid("ace04d5c-9fbc-4cbf-a0e6-4e58506c3abf"),
                        Url = "www.blobstorage.com/form6_fr_pdf",
                        Type = "application/pdf"
                    }},
                    Urls = new List<FormUrlDTO> { 
                        new FormUrlDTO
                        {
                            LanguageCode = "en",
                            Url = "label_form_6_url_en"

                        },
                        new FormUrlDTO
                        {
                            LanguageCode = "fr",
                            Url = "label_form_6_url_fr"
                        }
                    }
                }
            };

            return previewFormDTO;
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task PreviewAsync_ReturnsPreviewFormDTO_KeyNotFound()
        {
            var formId = new Guid("f74091a3-453a-4139-82a1-4cae27913d67");
            try
            {
                var result = bll.PreviewAsync(formId);
            }
            catch (Exception ex)
            {
                await unitOfWork.Received(0).SaveChangesAsync();
                Assert.AreEqual($"The form is not found.", ex.Message);
                throw;
            }
        }
    }
}
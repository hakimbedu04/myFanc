using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFanc.Api.Mapper;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.DTO.Internal;
using MyFanc.Services;
using MyFanc.Services.FancRadApi;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static MyFanc.Core.Enums;

namespace MyFanc.Test
{
    [TestClass()]
    public class MyFanc_Form_FormResponse_Test
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IBll bll;

        private IUnitOfWork unitOfWork;
        private IGenericRepository<Form> formRepository;
        private IGenericRepository<Nacabel> nacabelRepository;
        private IGenericRepository<FormSubmission> formSubmissionRepository;
        private IGenericRepository<FormNodeFields> formNodeFieldsRepository;
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
            this.formSubmissionRepository = Substitute.For<IGenericRepository<FormSubmission>>();
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
            this.unitOfWork.GetGenericRepository<FormSubmission>().Returns(this.formSubmissionRepository);
            this.unitOfWork.GetGenericRepository<FormNodeFields>().Returns(this.formNodeFieldsRepository);
            this.unitOfWork.GetGenericRepository<Nacabel>().Returns(this.nacabelRepository);

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
        public void GetListFormResponsesTest()
        {
            var formId = Guid.NewGuid();
            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Daniel",
                    Email = "daf@mail.com",
                    CompanyName = "DAF Inc",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Agoes",
                    Email = "ahn@mail.com",
                    CompanyName = "AHN Inc",
                    SubmissionDate = DateTime.Now,
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var responseList = bll.GetListFormResponses(formId, null, null, null);
            for (int i = 0; i < responseList.Count(); i++)
            {
                var result = responseList.ElementAt(i);
                var source = listFormSubmissionMock.ElementAt(i);
                Assert.AreEqual(source.Id, result.Id);
                Assert.AreEqual(source.UserName, result.UserName);
                Assert.AreEqual(source.CompanyName, result.CompanyName);
                Assert.AreEqual(source.CompanyType, result.CompanyType);
                Assert.AreEqual(source.Email, result.Email);
                Assert.AreEqual(source.SubmissionDate, result.SubmissionDate);
            }
        }

        [TestMethod]
        public void GetListFormResponsesTestFilterByUsername()
        {
            var formId = Guid.NewGuid();
            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Daniel",
                    Email = "daf@mail.com",
                    CompanyName = "DAF Inc",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Agoes",
                    Email = "ahn@mail.com",
                    CompanyName = "AHN Inc",
                    SubmissionDate = DateTime.Now,
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var responseList = bll.GetListFormResponses(formId, "john", null, null);
            Assert.AreEqual(responseList.Count(), 1);
            Assert.IsTrue(responseList.ElementAt(0).UserName.ToLower().Contains("john"));
        }

        [TestMethod]
        public void GetListFormResponsesTestFilterByCompanyName()
        {
            var formId = Guid.NewGuid();
            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Daniel",
                    Email = "daf@mail.com",
                    CompanyName = "DAF Inc",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Agoes",
                    Email = "ahn@mail.com",
                    CompanyName = "AHN Inc",
                    SubmissionDate = DateTime.Now,
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var responseList = bll.GetListFormResponses(formId, null, "inc", null);
            Assert.AreEqual(responseList.Count(), 2);
            Assert.IsTrue(responseList.ElementAt(0).CompanyName.ToLower().Contains("inc"));
            Assert.IsTrue(responseList.ElementAt(1).CompanyName.ToLower().Contains("inc"));
        }

        [TestMethod]
        public void GetListFormResponsesTestFilterByEmail()
        {
            var formId = Guid.NewGuid();
            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Daniel",
                    Email = "daf@mail.com",
                    CompanyName = "DAF Inc",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Agoes",
                    Email = "ahn@mail.com",
                    CompanyName = "AHN Inc",
                    SubmissionDate = DateTime.Now,
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var responseList = bll.GetListFormResponses(formId, null, null, "daf@mail.com");
            Assert.AreEqual(responseList.Count(), 1);
            Assert.IsTrue(responseList.ElementAt(0).Email.ToLower().Contains("daf@mail.com"));
        }

        [TestMethod]
        public void GetListFormResponsesTestFilterByUsernameCompanyName()
        {
            var formId = Guid.NewGuid();
            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Daniel",
                    Email = "daf@mail.com",
                    CompanyName = "DAF Inc",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Agoes",
                    Email = "ahn@mail.com",
                    CompanyName = "AHN Inc",
                    SubmissionDate = DateTime.Now,
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var responseList = bll.GetListFormResponses(formId, "agoes", "inc", null);
            Assert.AreEqual(responseList.Count(), 1);
            Assert.IsTrue(responseList.ElementAt(0).UserName.ToLower().Contains("agoes"));
            Assert.IsTrue(responseList.ElementAt(0).CompanyName.ToLower().Contains("inc"));
        }


        [TestMethod]
        public void GetListFormResponsesTestFilterByUsernameEmail()
        {
            var formId = Guid.NewGuid();
            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Daniel",
                    Email = "daf@mail.com",
                    CompanyName = "DAF Inc",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Agoes",
                    Email = "ahn@mail.com",
                    CompanyName = "AHN Inc",
                    SubmissionDate = DateTime.Now,
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var responseList = bll.GetListFormResponses(formId, "john", null, "@mail.com");
            Assert.AreEqual(responseList.Count(), 1);
            Assert.IsTrue(responseList.ElementAt(0).UserName.ToLower().Contains("john"));
            Assert.IsTrue(responseList.ElementAt(0).Email.ToLower().Contains("@mail.com"));
        }

        [TestMethod]
        public void GetListFormResponsesTestFilterByCompanyNameEmail()
        {
            var formId = Guid.NewGuid();
            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Daniel",
                    Email = "daf@mail.com",
                    CompanyName = "DAF Inc",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Agoes",
                    Email = "ahn@mail.com",
                    CompanyName = "AHN Inc",
                    SubmissionDate = DateTime.Now,
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var responseList = bll.GetListFormResponses(formId, null, "inc", "@mail.com");
            Assert.AreEqual(responseList.Count(), 2);
            Assert.IsTrue(responseList.ElementAt(0).CompanyName.ToLower().Contains("inc"));
            Assert.IsTrue(responseList.ElementAt(0).Email.ToLower().Contains("@mail.com"));
            Assert.IsTrue(responseList.ElementAt(1).CompanyName.ToLower().Contains("inc"));
            Assert.IsTrue(responseList.ElementAt(1).Email.ToLower().Contains("@mail.com"));
        }

        [TestMethod]
        public void GetListFormResponsesTestFilterByAllParam()
        {
            var formId = Guid.NewGuid();
            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Daniel",
                    Email = "daf@mail.com",
                    CompanyName = "DAF Inc",
                    SubmissionDate = DateTime.Now,
                },
                new FormSubmission() {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    UserName = "Agoes",
                    Email = "ahn@mail.com",
                    CompanyName = "AHN Inc",
                    SubmissionDate = DateTime.Now,
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var responseList = bll.GetListFormResponses(formId, "daniel", "inc", "@mail.com");
            Assert.AreEqual(responseList.Count(), 1);
            Assert.IsTrue(responseList.ElementAt(0).UserName.ToLower().Contains("daniel"));
            Assert.IsTrue(responseList.ElementAt(0).CompanyName.ToLower().Contains("inc"));
            Assert.IsTrue(responseList.ElementAt(0).Email.ToLower().Contains("@mail.com"));
        }

        [TestMethod()]
        public void GetDetailFormResponseTest()
        {
            var formSubmissionId = Guid.Parse("57f3f6a4-8183-4136-b45e-5b11aeaaf4f8");
            var formId = Guid.Parse("2c5acecb-5da4-44ca-b112-b0d6429ba639");
            var formBlockId = Guid.Parse("e15448c7-b4bc-461b-868c-55292d96acb6");
            var nodeFormBlockId = Guid.Parse("27ef37a5-9b2c-4c76-bae5-9407ad306545");
            var nodeSectionId = Guid.Parse("011d8060-d2f7-4eed-9910-bc9f6c51e10e");
            var checkBoxNodeId = Guid.Parse("4b77a55d-1132-40e6-8541-49d75f401f13");
            var checkboxNodeFieldCustomListId = Guid.Parse("e7bce154-8c81-4428-b341-f1095cbf8139");
            var listNodeId = Guid.Parse("cacb10b4-ab6a-4d3e-ab3a-0bef8f3685c3");
            var listNodeFieldPredifinedId = Guid.Parse("33c77ddf-074c-482f-b6a5-fd1fcf21f845");
            var userNameNodeId = Guid.Parse("9ea995dd-ff6d-43e1-8f80-5a8ebb0521e7");
            var emailNodeId = Guid.Parse("806adc26-d739-4075-be9a-6a2dc2ed40ad");
            var phoneNumberNodeId = Guid.Parse("90cfc961-f834-4dad-967e-8499919c13e1");


            var listFormSubmissionMock = new List<FormSubmission>() {
                new FormSubmission() {
                    Id = formSubmissionId,
                    FormId = formId,
                    UserName = "John Doe",
                    Email = "johdoe@mail.com",
                    CompanyName = "Company Test",
                    SubmissionDate = DateTime.Now,
                    Value = "0L/d6Hf+auJYQDEFkCSo79zlY/z7BhZcumrtZSafwG/t/E2UclNNbgr9eKNvr7QcuOIqQbhJrmLQeVgkrBDDvA4Ti9P4qBvfH7tOuGtjalk3QIGHKZfUGO6qy6wsl+hlyG3XLoy+UbB5+J00/YSc4rDR6AP3/tYv5JRiHb22dolrSeiakUhFNkNFhU0ddhErLjoaeccPOr7KQfzJsGJrAEP9hAEqLEM/RT0zoDDu974fPDSXaASy1+oTllEYsm6UDK9A0Rd3QT7OEO0Q7PKmFPRZcpeYSTuQC7JEcbTCenEuJ/m0uxXv6VYmbxg+OoKS/0FwaWTYinjssFR25Yq4eefNCImA+bNmyYA5o00ISlM1gXxRr2Acg7U5aYvgutzKJrWzmW4HHXDp8Lo4X21z1sD1JPI3ESOcwYNYgQVbzSM2rmAcJKI0DBrV0UmuHFJeny9rr+3iPQvkrFWlAMRDLZkuiGfOnsEx173SmWyMOoCcOgF52pOkrjlQUAE/owXMmLUIxu1mim6Ah/5EsZulsTXOkt2PbswqysJGjOiCKo6bsTNRRKp7wy8EBsUtS3seue7NbjJqAdAE0w2rMuG2x4jABfKsUXtokq/mexq8gv2gwGQr1VvuvqEVehciW4tMmCuLMTW6rqEoXumuMft2EpxGJTH/uY9ESIkXjF0L7BIjHPyO2tFd/1A9CpC4gzpE1ghHZgtRgspJyYPNZdYjnS/a3ZHaWaY94U4CSh6DlapeA3nBkcrzhaqRxrEgmpFqfGoYIfGVQuMY5dnbbRCFbi5PmYR3NKfQH4AsyV4ZNdq/eVQl2fdY77S66rbEIjNrYjPtGT8HpD5AqKaQU/inLATMjhSfdQtpAint8al/2N84xprEB19kTI1axpwFPJjDP1vlAl7kr0udxaLzfsDyz/eACfKZaEX696G7mRHS9a+VYdxZ+L873Ntc3LGSh9ON2It5ptY8118uTRsXtkHkTv4FjpZ+WNHeJiGq6DoNvvLyqw6jMBVXeZISQPR4NgEVIsKw5JsW2xFWo/6mTvNMEBje8mUCasalDKagtL87yrXvNEtkd5KWaE+D4CVVSC1FI3g0u79l2JQxrEHZsO1x1giCaLzDp4fE5Ir9ND3ec7v+v6LpHr5gJY0PoaIe491cfPO8k/dfazVtll4LSNNZ5JFjtujYnvAlHG1KlIFy1PBmaLSK1AJbpLlu2Lgag/mYRVUrFzA3YJ/TAHJnivlPk3qIKc4e6IuseGfPzm8dBrDthJRy7uW/T6h01FB8tKB/jtgcd6ts4OG0jNe64YxYYgDA/OP2owXK9isX35+qoeliqgTwkDelPI1tIgp1ZCoy"
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return listFormSubmissionMock.Where(exp.Compile()).AsQueryable();
            });

            var formBlockMock = new Form()
            {
                Id = formBlockId,
                FormNodes = new List<FormNodes>()
                {
                    new FormNodes()
                        {
                            Id= phoneNumberNodeId,
                            FormId = formBlockId,
                            Labels = new List<Translation>() {
                                new Translation() {
                                  Id = 117,
                                  LanguageCode = "en",
                                  Text = "Phone Number"
                                },
                                new Translation() {
                                  Id = 118,
                                  LanguageCode = "fr",
                                  Text = "Phone Number"
                                }
                            },
                            Type = "FormField",
                            FieldType = "Text",
                            Order = 1,
                            Version = 1,
                            FormNodeFields = new List<FormNodeFields>()
                            {
                                new FormNodeFields() {
                                  Id = Guid.NewGuid(),
                                  Property = "Hide by default",
                                  Type = "Text",
                                  Value = "false"
                                }
                            }
                        }
                }
            };
            var formMock = new Form()
            {
                Id = formId,
                Labels = new List<Translation>()
                    {
                        new Translation()
                        {
                             Id = 1,
                             LanguageCode = "en",
                             Text = "FORM FOR TEST RESPONSE DETAIL EN"
                        },
                        new Translation()
                        {
                             Id = 2,
                             LanguageCode = "fr",
                             Text = "FORM FOR TEST RESPONSE DETAIL FR"
                        }
                    },
                FormNodes = new List<FormNodes>()
                    {
                        new FormNodes()
                        {
                            Id = nodeSectionId,
                            FormId = formId,
                            Labels = new List<Translation>() {
                                new Translation() {
                                  Id = 3,
                                  LanguageCode = "en",
                                  Text = "User Information Section En"
                                },
                                new Translation() {
                                  Id = 4,
                                  LanguageCode = "fr",
                                  Text = "User Information Section Fr"
                                }
                            },
                            Type = "Section",
                            FieldType = "",
                            Order = 1,
                            Version = 1,
                        },
                        new FormNodes()
                        {
                            Id= userNameNodeId,
                            FormId = formId,
                            ParentId = nodeSectionId,
                            Labels = new List<Translation>() {
                                new Translation() {
                                  Id = 5,
                                  LanguageCode = "en",
                                  Text = "User Name"
                                },
                                new Translation() {
                                  Id = 6,
                                  LanguageCode = "fr",
                                  Text = "User Name"
                                }
                            },
                            Type = "FormField",
                            FieldType = "Text",
                            Order = 1,
                            Version = 1,
                            FormNodeFields = new List<FormNodeFields>()
                            {
                                new FormNodeFields() {
                                  Id = Guid.NewGuid(),
                                  Property = "Hide by default",
                                  Type = "Text",
                                  Value = "false"
                                }
                            }
                        },
                        new FormNodes()
                        {
                            Id= emailNodeId,
                            FormId = formId,
                            ParentId = nodeSectionId,
                            Labels = new List<Translation>() {
                                new Translation() {
                                  Id = 7,
                                  LanguageCode = "en",
                                  Text = "Email"
                                },
                                new Translation() {
                                  Id = 8,
                                  LanguageCode = "fr",
                                  Text = "Email"
                                }
                            },
                            Type = "FormField",
                            FieldType = "Text",
                            Order = 2,
                            Version = 1,
                            FormNodeFields = new List<FormNodeFields>()
                            {
                                new FormNodeFields() {
                                  Id = Guid.NewGuid(),
                                  Property = "Hide by default",
                                  Type = "Text",
                                  Value = "false"
                                }
                            }
                        },
                        new FormNodes()
                        {
                            Id= checkBoxNodeId,
                            FormId = formId,
                            ParentId = nodeSectionId,
                            Labels = new List<Translation>() {
                                new Translation() {
                                  Id = 17,
                                  LanguageCode = "en",
                                  Text = "Gender"
                                },
                                new Translation() {
                                  Id = 18,
                                  LanguageCode = "fr",
                                  Text = "Gender"
                                }
                            },
                            Type = "FormField",
                            FieldType = "CheckBox",
                            Order = 3,
                            Version = 1,
                            FormNodeFields = new List<FormNodeFields>()
                            {
                                new FormNodeFields() {
                                  Id = checkboxNodeFieldCustomListId,
                                  FormNodeId = checkBoxNodeId,
                                  Property = "List",
                                  Type = "CustomList",
                                  Value = "77b54ae6-11c0-4dea-bba8-751961b96a88",
                                  FormValueFields = new List<FormValueFields>()
                                  {
                                      new FormValueFields()
                                      {
                                          Id = Guid.NewGuid(),
                                          FormNodeFieldId = checkboxNodeFieldCustomListId,
                                          Labels = new List<Translation>() {
                                                new Translation() {
                                                  Id = 9,
                                                  LanguageCode = "en",
                                                  Text = "Male"
                                                },
                                                new Translation() {
                                                  Id = 10,
                                                  LanguageCode = "fr",
                                                  Text = "Male"
                                                }
                                           },
                                          Value = "M"
                                      },
                                      new FormValueFields()
                                      {
                                          Id = Guid.NewGuid(),
                                          FormNodeFieldId = checkboxNodeFieldCustomListId,
                                          Labels = new List<Translation>() {
                                                new Translation() {
                                                  Id = 11,
                                                  LanguageCode = "en",
                                                  Text = "Female"
                                                },
                                                new Translation() {
                                                  Id = 12,
                                                  LanguageCode = "fr",
                                                  Text = "Female"
                                                }
                                           },
                                          Value = "F"
                                      }
                                  }
                                }
                            }
                        },
                        new FormNodes()
                        {
                            Id= listNodeId,
                            FormId = formId,
                            ParentId = null,
                            Labels = new List<Translation>() {
                                new Translation() {
                                  Id = 13,
                                  LanguageCode = "en",
                                  Text = "Gender Predifened List"
                                },
                                new Translation() {
                                  Id = 14,
                                  LanguageCode = "fr",
                                  Text = "Gender Predifened List"
                                }
                            },
                            Type = "FormField",
                            FieldType = "List",
                            Order = 2,
                            Version = 1,
                            FormNodeFields = new List<FormNodeFields>()
                            {
                                new FormNodeFields() {
                                  Id = listNodeFieldPredifinedId,
                                  FormNodeId = listNodeId,
                                  Property = "List",
                                  Type = "PredefineList",
                                  Value = "77b54ae6-11c0-4dea-bba8-751961b96a88"
                                }
                            }
                        },
                        new FormNodes()
                        {
                            Id = nodeFormBlockId,
                            FormId = formId,
                            Labels = new List<Translation>() {
                                new Translation() {
                                  Id = 15,
                                  LanguageCode = "en",
                                  Text = "Contact Information Form Block En"
                                },
                                new Translation() {
                                  Id = 16,
                                  LanguageCode = "fr",
                                  Text = "Contact Information Form Block Fr"
                                }
                            },
                            Type = "FormBlock",
                            FieldType = "",
                            Order = 3,
                            Version = 1,
                            FormNodeFields = new List<FormNodeFields>()
                            {
                                new FormNodeFields()
                                {
                                    Id = Guid.NewGuid(),
                                    FormNodeId = nodeFormBlockId,
                                    Property = "Value",
                                    Type = "Container",
                                    Value = formBlockId.ToString()

                                }
                            }
                        },
                    }

            };
            var listFormMock = new List<Form>()
            {
                formBlockMock,
                formMock
            };
            formRepository.Find(Arg.Any<Expression<Func<Form, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<Form, bool>>>();
                return listFormMock.Where(exp.Compile()).AsQueryable();
            });

            var jsonResponseSubmitValue = "{'FormInformation': {'formId': '2c5acecb-5da4-44ca-b112-b0d6429ba639', 'version': '1', 'submissionDate': '2024-02-19', 'labels': [{'id': 1, 'languageCode': 'en', 'text': 'FORM FOR TEST RESPONSE DETAIL EN' }, { 'id': 2, 'languageCode': 'fr', 'text': 'FORM FOR TEST RESPONSE DETAIL FR' }]}, 'UserInformation': {'userId': '29', 'userName':'Maxim', 'email':'mdl@voxteneo.com', 'fancOrganisationId': '3fa85f64-5717-4562-b3fc-2c963f66afa6', 'companyName':'VOXTENEO'}, 'Fields': [{'nodeId': '9ea995dd-ff6d-43e1-8f80-5a8ebb0521e7', 'value':'Maxim', 'externalId':'externalId1', 'id':'field_id1', 'labels': [{'id': 5, 'languageCode': 'en', 'text': 'User Name' }, { 'id': 6, 'languageCode': 'fr', 'text': 'User Name'}]}, {'nodeId': '806adc26-d739-4075-be9a-6a2dc2ed40ad', 'value':'mdl@voxteneo', 'externalId':'externalId2', 'id':'field_id2', 'labels': [{'id': 7, 'languageCode': 'en', 'text': 'Email' }, { 'id': 8, 'languageCode': 'fr', 'text': 'Email'}]}, {'nodeId': '4b77a55d-1132-40e6-8541-49d75f401f13', 'value':'M,F', 'externalId':'externalId3', 'id':'field_id3', 'labels': [{'id': 17, 'languageCode': 'en', 'text': 'Gender' }, { 'id': 18, 'languageCode': 'fr', 'text': 'Gender'}]}, {'nodeId': 'cacb10b4-ab6a-4d3e-ab3a-0bef8f3685c3', 'value':'M', 'externalId':'externalId4', 'id':'field_id4', 'labels': [{'id': 13, 'languageCode': 'en', 'text': 'Gender Predifined List' }, { 'id': 14, 'languageCode': 'fr', 'text': 'Gender Predifined List'}]}, {'nodeId': '90cfc961-f834-4dad-967e-8499919c13e1', 'value':'+6281789654296', 'externalId':'externalId5', 'id':'field_id5', 'labels': [{'id': 117, 'languageCode': 'en', 'text': 'Phone Number' }, { 'id': 118, 'languageCode': 'fr', 'text': 'Phone Number'}]}]}";
            aESEncryptService.DecryptString(Arg.Any<string>()).Returns(jsonResponseSubmitValue);

            var listformNodeFieldsMock = new List<FormNodeFields>()
            {
                new FormNodeFields() {
                    Id = checkboxNodeFieldCustomListId,
                    FormNodeId = checkBoxNodeId,
                    Property = "List",
                    Type = "CustomList",
                    Value = "77b54ae6-11c0-4dea-bba8-751961b96a88",
                    FormValueFields = new List<FormValueFields>()
                    {
                        new FormValueFields()
                        {
                            Id = Guid.NewGuid(),
                            FormNodeFieldId = checkboxNodeFieldCustomListId,
                            Labels = new List<Translation>() {
                                new Translation() {
                                    Id = 9,
                                    LanguageCode = "en",
                                    Text = "Male"
                                },
                                new Translation() {
                                    Id = 10,
                                    LanguageCode = "fr",
                                    Text = "Male"
                                }
                            },
                            Value = "M"
                        },
                        new FormValueFields()
                        {
                            Id = Guid.NewGuid(),
                            FormNodeFieldId = checkboxNodeFieldCustomListId,
                            Labels = new List<Translation>() {
                                new Translation() {
                                    Id = 11,
                                    LanguageCode = "en",
                                    Text = "Female"
                                },
                                new Translation() {
                                    Id = 12,
                                    LanguageCode = "fr",
                                    Text = "Female"
                                }
                            },
                            Value = "F"
                        }
                    }
                }
            };
            formNodeFieldsRepository.Find(Arg.Any<Expression<Func<FormNodeFields, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormNodeFields, bool>>>();
                return listformNodeFieldsMock.Where(exp.Compile()).AsQueryable();
            });


            var responseDetails = bll.GetDetailFormResponse(formSubmissionId);
            Assert.AreEqual(3, responseDetails.Count);
            
            Assert.AreEqual(FormNodeType.Section.ToString(), responseDetails.ElementAt(0).Type);
            Assert.AreEqual(3, responseDetails.ElementAt(0).Children.Count);
            
            Assert.AreEqual(FormNodeType.FormField.ToString(), responseDetails.ElementAt(0).Children.ElementAt(0).Type);
            Assert.AreEqual(FormNodeFieldType.Text.ToString(), responseDetails.ElementAt(0).Children.ElementAt(0).FieldType);
            Assert.AreEqual("User Name", responseDetails.ElementAt(0).Children.ElementAt(0).Labels.ElementAt(0).Text);
            Assert.AreEqual("Maxim", responseDetails.ElementAt(0).Children.ElementAt(0).Value);

            Assert.AreEqual(FormNodeType.FormField.ToString(), responseDetails.ElementAt(0).Children.ElementAt(1).Type);
            Assert.AreEqual(FormNodeFieldType.Text.ToString(), responseDetails.ElementAt(0).Children.ElementAt(1).FieldType);
            Assert.AreEqual("Email", responseDetails.ElementAt(0).Children.ElementAt(1).Labels.ElementAt(0).Text);
            Assert.AreEqual("mdl@voxteneo", responseDetails.ElementAt(0).Children.ElementAt(1).Value);

            Assert.AreEqual(FormNodeType.FormField.ToString(), responseDetails.ElementAt(0).Children.ElementAt(2).Type);
            Assert.AreEqual(FormNodeFieldType.CheckBox.ToString(), responseDetails.ElementAt(0).Children.ElementAt(2).FieldType);
            Assert.AreEqual("Gender", responseDetails.ElementAt(0).Children.ElementAt(2).Labels.ElementAt(0).Text);
            
            var deseriliazeCheckbox = JsonConvert.DeserializeObject<List<CheckBoxResponseViewDto>>(responseDetails.ElementAt(0).Children.ElementAt(2).Value);
            Assert.AreEqual(2, deseriliazeCheckbox?.Count);
            Assert.AreEqual("Male", deseriliazeCheckbox?.ElementAt(0)?.Labels?.ElementAt(0).Text);
            Assert.AreEqual("M", deseriliazeCheckbox?.ElementAt(0)?.Value);
            Assert.AreEqual("Female", deseriliazeCheckbox?.ElementAt(1)?.Labels?.ElementAt(0).Text);
            Assert.AreEqual("F", deseriliazeCheckbox?.ElementAt(1)?.Value);

            Assert.AreEqual(FormNodeType.FormField.ToString(), responseDetails.ElementAt(1).Type);
            Assert.AreEqual(FormNodeFieldType.List.ToString(), responseDetails.ElementAt(1).FieldType);
            Assert.AreEqual("Gender Predifened List", responseDetails.ElementAt(1).Labels.ElementAt(0).Text);
            
            deseriliazeCheckbox = JsonConvert.DeserializeObject<List<CheckBoxResponseViewDto>>(responseDetails.ElementAt(1).Value);
            Assert.AreEqual(1, deseriliazeCheckbox?.Count);
            Assert.AreEqual("Male", deseriliazeCheckbox?.ElementAt(0)?.Labels?.ElementAt(0).Text);
            Assert.AreEqual("M", deseriliazeCheckbox?.ElementAt(0)?.Value);

            Assert.AreEqual(FormNodeType.FormBlock.ToString(), responseDetails.ElementAt(2).Type);
            Assert.AreEqual("", responseDetails.ElementAt(2).FieldType);
            Assert.AreEqual(1, responseDetails.ElementAt(2).Children.Count);
            Assert.AreEqual("Phone Number", responseDetails.ElementAt(2).Children.ElementAt(0).Labels.ElementAt(0).Text);
            Assert.AreEqual("+6281789654296", responseDetails.ElementAt(2).Children.ElementAt(0).Value);
        }

        [TestMethod()]
        public void GetDetailFormResponseTestKeyNotFoundException()
        {
            Assert.ThrowsException<KeyNotFoundException>( () =>
            {
                bll.GetDetailFormResponse(default);
            });
            formSubmissionRepository.ReceivedWithAnyArgs(1).Find(Arg.Any<Expression<Func<FormSubmission, bool>>>());
            aESEncryptService.DidNotReceiveWithAnyArgs().DecryptString(Arg.Any<string>());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDetailFormResponseTest_ToPdfType_ArgumentException()
        {
            var id = new Guid("f74091a3-453a-4139-82a1-4cae27913d67");
            var formSubmissionList = new List<FormSubmission>()
            {
                new FormSubmission()
                {
                    Id = id,
                    FormSubmissionType = FormSubmissionType.Pdf
                },
                new FormSubmission()
                {
                    Id = new Guid("2c5acecb-5da4-44ca-b112-b0d6429ba639"),
                    FormSubmissionType = FormSubmissionType.Webform
                }
            };
            formSubmissionRepository.Find(Arg.Any<Expression<Func<FormSubmission, bool>>>()).Returns(callinfo =>
            {
                var exp = callinfo.Arg<Expression<Func<FormSubmission, bool>>>();
                return formSubmissionList.Where(exp.Compile()).AsQueryable();
            });
            try
            {
                var result = bll.GetDetailFormResponse(id);
            }
            catch (Exception ex)
            {
                formSubmissionRepository.ReceivedWithAnyArgs(1).Find(Arg.Any<Expression<Func<FormSubmission, bool>>>());
                Assert.AreEqual($"Form submission with id {id} is a PDF type", ex.Message);
                throw;
            }
        }
    }
}

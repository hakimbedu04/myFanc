using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.Services.FancRadApi;

namespace MyFanc.BLL
{
    public partial class Bll : IBll
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Roles> _rolesRepository;
        private readonly IGenericRepository<Wizard> _wizardRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Form> _formRepository;
        private readonly IGenericRepository<FormNodes> _formNodesRepository;
        private readonly IGenericRepository<FormNodeFields> _formNodeFieldsRepository;
        private readonly IGenericRepository<FormValueFields> _formValueFieldsRepository;
        private readonly IGenericRepository<FormConditionals> _formConditionalsRepository;
        private readonly IGenericRepository<FormDataSource> _formDataSourceRepository;
        private readonly IGenericRepository<FormCategory> _formCategoryRepository;
        private readonly IGenericRepository<Nacabel> _nacabelRepository;
        private readonly IGenericRepository<FormSubmission> _formSubmissionRepository;
        private readonly IGenericRepository<Document> _documentRepository;
        private readonly IGenericRepository<NacabelsEntityMap> _nacabelsEntityMap;
        private readonly IGenericRepository<NacabelTranslation> _nacabelTranslation;
        private readonly IGenericRepository<Translation> _translation;
        private readonly IGenericRepository<PersonaCategories> _personaCategoriesRepository;
        private readonly IGenericRepository<UserPersonas> _userPersonasRepository;
        private readonly IGenericRepository<CompanyPersonas> _companyPersonasRepository;
        private readonly IGenericRepository<UserPersonaCategories> _userPersonaCategoriesRepository;
        private readonly IGenericRepository<CompanyPersonaCategories> _companyPersonaCategoriesRepository;


        private readonly IMapper _mapper;
        private readonly ILogger<Bll> _logger;
        private readonly IFancRADApi _fancRADApi;
        private readonly ISharedDataCache _sharedDataCache;
        private readonly IBreadCrumbService _breadCrumbService;
        private readonly IEmailService _emailService;
        private readonly ITokenConfiguration _tokenConfiguration;
        private readonly IIdentityProviderConfiguration _identityProviderConfiguration;
        private readonly IAESEncryptService _aESEncryptService;
        private readonly INacabelHelper _nacabelHelper;
        private readonly IFileStorage _fileStorage;
        private readonly IGenericRepository<NacabelsEntityMap> _nacabelsEntityMapRepository;

        public Bll(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Bll> logger,
            IFancRADApi fancRADApi, IBreadCrumbService breadCrumbService, 
            ISharedDataCache sharedDataCache, IEmailService emailService, 
            ITokenConfiguration tokenConfiguration, IAESEncryptService aESEncryptService, 
            INacabelHelper nacabelHelper, IFileStorage fileStorage, IIdentityProviderConfiguration identityProviderConfiguration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fancRADApi = fancRADApi;
            _userRepository = unitOfWork.GetGenericRepository<User>();
            _rolesRepository = unitOfWork.GetGenericRepository<Roles>();
            _wizardRepository = unitOfWork.GetGenericRepository<Wizard>();
            _questionRepository = unitOfWork.GetGenericRepository<Question>();
            _formRepository = unitOfWork.GetGenericRepository<Form>();
            _formNodesRepository = unitOfWork.GetGenericRepository<FormNodes>();
            _formNodeFieldsRepository = unitOfWork.GetGenericRepository<FormNodeFields>();
            _formValueFieldsRepository = unitOfWork.GetGenericRepository<FormValueFields>();
            _formConditionalsRepository = unitOfWork.GetGenericRepository<FormConditionals>();
            _formDataSourceRepository = unitOfWork.GetGenericRepository<FormDataSource>();
            _documentRepository = unitOfWork.GetGenericRepository<Document>();
            _sharedDataCache = sharedDataCache;
            _breadCrumbService = breadCrumbService;
            _emailService = emailService;
            _tokenConfiguration = tokenConfiguration;
            _aESEncryptService = aESEncryptService;
            _nacabelHelper = nacabelHelper;
            _fileStorage = fileStorage;

            _formCategoryRepository = unitOfWork.GetGenericRepository<FormCategory>();
            _nacabelRepository = unitOfWork.GetGenericRepository<Nacabel>();
            _formSubmissionRepository = unitOfWork.GetGenericRepository<FormSubmission>();
            _nacabelsEntityMap = unitOfWork.GetGenericRepository<NacabelsEntityMap>();
            _nacabelTranslation = unitOfWork.GetGenericRepository<NacabelTranslation>();
            _nacabelsEntityMapRepository = unitOfWork.GetGenericRepository<NacabelsEntityMap>();
            _identityProviderConfiguration = identityProviderConfiguration;
            _translation = unitOfWork.GetGenericRepository<Translation>();
            _personaCategoriesRepository = unitOfWork.GetGenericRepository<PersonaCategories>();
            _userPersonasRepository = unitOfWork.GetGenericRepository<UserPersonas>();
            _companyPersonasRepository = unitOfWork.GetGenericRepository<CompanyPersonas>();
            _userPersonaCategoriesRepository = unitOfWork.GetGenericRepository<UserPersonaCategories>();
            _companyPersonaCategoriesRepository = unitOfWork.GetGenericRepository<CompanyPersonaCategories>();
        }
    }
}
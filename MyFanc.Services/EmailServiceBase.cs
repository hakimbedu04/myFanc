using AutoMapper;
using Microsoft.Extensions.Logging;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.DAL;
using MyFanc.Services.FancRadApi;
using Newtonsoft.Json;
using static MyFanc.Core.Enums;

namespace MyFanc.Services
{
    public class EmailServiceBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<HistoryMail> _historyMailRepository;
        private readonly ILogger<EmailServiceBase> _logger;
        public EmailServiceBase(IUnitOfWork unitOfWork, ILogger<EmailServiceBase> logger) {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _historyMailRepository = unitOfWork.GetGenericRepository<HistoryMail>();
        }

        public async Task LogInformation(string emailTo, string Attributes, EmailType type, SendEmailStatus status)
        {
            HistoryMail historyMail = new HistoryMail()
            {
                MailTo = emailTo,
                Attributes = Attributes,
                Type = type,
                Status = status
            };
            _historyMailRepository.Add(historyMail);
            await _unitOfWork.SaveChangesAsync();
            string jsonForLog = JsonConvert.SerializeObject(historyMail);
            _logger.LogInformation("save new history mail: {Object}", jsonForLog);
        }
    }
}

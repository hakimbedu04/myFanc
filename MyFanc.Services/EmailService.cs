using Microsoft.Extensions.Logging;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static MyFanc.Core.Enums;

namespace MyFanc.Services
{
    public class EmailService : EmailServiceBase, IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        public EmailService(IUnitOfWork unitOfWork, ILogger<EmailService> logger): base(unitOfWork, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task SendInvitationAsync(string toEmail, string token, string fancOrganisationId, string roles)
        {
            Dictionary<string, string> atrributes = new Dictionary<string, string>();
            atrributes.Add("Token", token);
            atrributes.Add("FancOrganisationId", fancOrganisationId);
            atrributes.Add("Roles", roles);

            string atrributesSerialize = JsonConvert.SerializeObject(atrributes);

            //do call rad api here when rad api for send invitation email completed
            //check if return sucess then log the status SendEmailStatus.Success
            var sendStatus = SendEmailStatus.Success;

            await LogInformation(toEmail, atrributesSerialize, EmailType.Invitation, sendStatus);
            _logger.LogInformation("send email invitation to {0} with status {1} object attributes: {2}", toEmail, sendStatus, atrributesSerialize);
        }

        public async Task SendInvitationAcceptOrRefuseNotificationAsync(string toEmail, string userId, string fancOrganisationId, IEnumerable<string> roles, bool isAccepted)
        {
            Dictionary<string, string> atrributes = new Dictionary<string, string>();
            atrributes.Add("UserId", userId);
            atrributes.Add("FancOrganisationId", fancOrganisationId);
            atrributes.Add("Roles", JsonConvert.SerializeObject(roles));
            atrributes.Add("IsAccepted", isAccepted.ToString());

            string atrributesSerialize = JsonConvert.SerializeObject(atrributes);

            //do call rad api here when rad api for send notification email completed
            //check if return sucess then log the status SendEmailStatus.Success
            var sendStatus = SendEmailStatus.Success;

            await LogInformation(toEmail, atrributesSerialize, isAccepted ? EmailType.AcceptInvitation : EmailType.RefuseInvitation, sendStatus);
            _logger.LogInformation("send email notification to {0} with status {1} object attributes: {2}", toEmail, sendStatus, atrributesSerialize);
        }

        public async Task SendDeleteUserFromOeNotificationAsync(string emailTo, string fancOrganisationId, IEnumerable<string> roles, string requestingUserId)
        {
            Dictionary<string, string> atrributes = new Dictionary<string, string>();
            atrributes.Add("FancOrganisationId", fancOrganisationId);
            atrributes.Add("Roles", JsonConvert.SerializeObject(roles));
            atrributes.Add("RequestingUserId", requestingUserId);

            string atrributesSerialize = JsonConvert.SerializeObject(atrributes);

            //do call rad api here when rad api for send notification to deleted user available
            //check if return sucess then log the status SendEmailStatus.Success
            var sendStatus = SendEmailStatus.Success;

            await LogInformation(emailTo, atrributesSerialize, EmailType.DeleteUserFromOe, sendStatus);
            _logger.LogInformation("send user delete email notification to {0} with status {1} object attributes: {2}", emailTo, sendStatus, atrributesSerialize);
        }
    }
}

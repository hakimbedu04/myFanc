namespace MyFanc.Contracts.Services
{
    public interface IEmailService
    {
        Task SendInvitationAsync(string toEmail, string token, string fancOrganisationId, string roles);
        Task SendDeleteUserFromOeNotificationAsync(string emailTo, string fancOrganisationId, IEnumerable<string> roles, string requestingUserId);
        Task SendInvitationAcceptOrRefuseNotificationAsync(string toEmail, string userId, string fancOrganisationId, IEnumerable<string> roles, bool isAccepted);
    }
}

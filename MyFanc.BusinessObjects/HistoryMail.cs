using static MyFanc.Core.Enums;

namespace MyFanc.BusinessObjects
{
    public class HistoryMail : AuditedEntity
    {
        public int Id { get; set; }
        public string MailTo { get; set; } = string.Empty;
        public string Attributes { get; set; } = string.Empty;
        public EmailType Type { get; set; }
        public SendEmailStatus Status { get; set; }
    }
}

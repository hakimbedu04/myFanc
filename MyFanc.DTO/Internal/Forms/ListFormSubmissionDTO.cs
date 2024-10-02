namespace MyFanc.DTO.Internal.Forms
{
    public class ListFormSubmissionDTO
    {
        public Guid Id { get; set; }
        public string  UserName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyType { get; set;}
        public string Email { get; set; }
        public DateTime SubmissionDate { get; set; }

    }
}

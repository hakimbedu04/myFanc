namespace MyFanc.DTO.Internal.Forms
{
    public class PagingResultDTO<T>
    {
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<T> Records { get; set; } = new List<T>();
    }
}

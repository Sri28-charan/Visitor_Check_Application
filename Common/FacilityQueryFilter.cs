namespace Visitor_Check_Application.Common
{
    public class FacilityQueryFilter
    {
        public string? Division { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public string? SortBy { get; set; }

        public string? Search { get; set; }
    }
}

namespace Visitor_Check_Application.Common
{
    public class PatientQueryFilter
    {
		//required domain filter
		public string? FacilityCode { get; set; }

		//pagination
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;

		//sorting
		public string? SortBy { get; set; }

		//search text
		public string? Search { get; set; }
	}
}

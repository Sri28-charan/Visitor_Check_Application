namespace Visitor_Check_Application.DTO
{
    public class PatientTable
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string FloorNumber { get; set; } = string.Empty;
        public string Room { get; set; }
        public int? Bed { get; set; }

        public string Isolation { get; set; } = string.Empty;

        public bool VisitorsAllowed { get; set; }
        public int VisitorsInRoom { get; set; }
		public DateTime? VisitorStartTime { get; set; }

        public bool IsFacilityAllowed { get; set; }
        public int MaxLimit { get; set; }

	}
}

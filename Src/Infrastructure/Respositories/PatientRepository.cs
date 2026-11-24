using Microsoft.EntityFrameworkCore;
using Visitor_Check_Application.Common;
using Visitor_Check_Application.DTO;
using Visitor_Check_Application.Interfaces;
using Visitor_Check_Application.Models;

namespace Visitor_Check_Application.Respositiries
{
    public class PatientRepository:IPatientInterface
    {
        private readonly VisitorCheckDbContext _context;
        private readonly ILogger<PatientRepository> _logger;
        public PatientRepository(VisitorCheckDbContext context,ILogger<PatientRepository> logger)
        {
            _logger = logger;
            _context = context;
        }


		public async Task<PagedResponse<PatientTable>> GetPatientsAsync(
		PatientQueryFilter filter,
		CancellationToken cancellationToken = default)
		{
			var pageNumber = Math.Max(1, filter.PageNumber);
			var pageSize = Math.Clamp(filter.PageSize, 1, 50);

			// Base query
			IQueryable<Patient> baseQuery = _context.Patients
				.AsNoTracking()
				.Where(p => p.FacilityCode == filter.FacilityCode);

			// Search
			baseQuery = baseQuery.ApplySearch(filter.Search);

			// Sort
			baseQuery = baseQuery.ApplySort(
				string.IsNullOrWhiteSpace(filter.SortBy)
					? "PatientName"
					: filter.SortBy);

			// Total count (before pagination)
			var totalRecords = await baseQuery.CountAsync(cancellationToken);

			// JOIN with Hospital + Projection
			var patients = await baseQuery
				.Join(
					_context.Hospitals,
					p => p.FacilityCode,
					h => h.FacilityCode,
					(p, h) => new PatientTable
					{
						PatientId = p.PatientId,
						PatientName = p.PatientName,
						FloorNumber = p.FloorNumber,
						Room = p.Room,
						Bed = p.Bed ?? 0,
						Isolation = p.Isolation,
						VisitorsAllowed = p.VisitorsAllowed,
						VisitorsInRoom = p.VisitorsInRoom ?? 0,
						VisitorStartTime = p.VisitorStartTime,
						IsFacilityAllowed = h.VisitorsAllowed?? false,
						MaxLimit=h.MaxLimit ?? 5
					}
				)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(cancellationToken);

			// ✅ Return paged response
			return new PagedResponse<PatientTable>
			{
				Data = patients,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalRecords = totalRecords,
				TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
			};
		}


	public async Task<List<PatientTable>> GetPatientsByFacilityAsync(string facilitycode)
        {
            _logger.LogInformation("Repository:Querying patients for FacilityCode={FacilityCode}", facilitycode);
            var result= await _context.Patients
                .Where(p => p.FacilityCode == facilitycode)
                .Join(
				   _context.Hospitals,
				   p=>p.FacilityCode,
				   h=>h.FacilityCode,
				   (p,h) => new PatientTable
				   {

					   PatientId = p.PatientId,
					   PatientName = p.PatientName ?? string.Empty,
					   FloorNumber = p.FloorNumber,
					   Room = p.Room ?? string.Empty,
					   Bed = p.Bed,
					   Isolation = p.Isolation ?? string.Empty,
					   VisitorsAllowed = p.VisitorsAllowed,
					   VisitorsInRoom = p.VisitorsInRoom ?? 0,

					   IsFacilityAllowed = h.VisitorsAllowed??false,
					   MaxLimit=h.MaxLimit ?? 5

				   }
				
                ).ToListAsync();
            _logger.LogInformation("Repository:Returned {Count} patients for FacilityCode={FacilityCode}", result.Count, facilitycode);
            return result;
        }

        public async Task<Patient?> GetPatientByIdAsync(string patientId)
        {
            _logger.LogInformation("Repository:Fetching patient by PatientId==patientId",patientId);
			return await _context.Patients
										  .FirstOrDefaultAsync(p => p.PatientId == patientId);
				                          
        }

        public async Task SaveAsync()
        {
            _logger.LogInformation("Repository:saving changes to databse");
            await _context.SaveChangesAsync();
        }

    }
}

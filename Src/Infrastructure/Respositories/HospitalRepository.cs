

using Microsoft.EntityFrameworkCore;
using Visitor_Check_Application.Common;
using Visitor_Check_Application.DTO_S;
using Visitor_Check_Application.Interfaces;
using Visitor_Check_Application.Models;
using Visitor_Check_Application.Src.Application.DTO;

namespace Visitor_Check_Application.Respositiries
{
    public class HospitalRepository : IHospitalInterface

    {
        private readonly VisitorCheckDbContext _context;
        private readonly ILogger<HospitalRepository> _logger;

       
        public HospitalRepository(VisitorCheckDbContext context,ILogger<HospitalRepository>logger)
        {
            _context = context;
            _logger = logger;
        }

       //getting hosp by division
        public async Task<List<HospitalDropDown>> GetHospitalsByDivisionAsync(string divisionId)
        {
            return await _context.Hospitals
                .Where(h => h.DivisionId == divisionId)
                .OrderBy(h => h.HospitalName)
                .Select(h => new HospitalDropDown
                {
                    Id = h.FacilityCode,
                    Name = h.HospitalName
                }
                )
                .ToListAsync();

        }

        public async Task<PagedResponse<FacilityTable>> GetFacilitiesAsync(FacilityQueryFilter filter,CancellationToken cancellationToken=default)
        {
            var pageNumber = Math.Max(1, filter.PageNumber);
            var pageSize = Math.Clamp(filter.PageSize, 1, 50);

            IQueryable<Hospital> query = _context.Hospitals.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Division))
            {
                query = query.Where(h => h.DivisionId == filter.Division);
            }

            //search
            query = query.ApplyFacilitySearch(filter.Search);

            //sort
            query = query.ApplySort(string.IsNullOrWhiteSpace(filter.SortBy)
                ? "HospitalName"
                : filter.SortBy
                );

            var totalRecords = await query.CountAsync(cancellationToken);

            //pagination
            var data =await query
                .ApplyPagination(pageNumber, pageSize)
                .Select(h => new FacilityTable 
                 { 
                    FacilityCode = h.FacilityCode,
                    HospitalName = h.HospitalName,
                    MaxLimit = h.MaxLimit,
                    VisitorsAllowed = h.VisitorsAllowed,
                 })
                 .ToListAsync(cancellationToken);

            return new PagedResponse<FacilityTable>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }

		public async Task<Hospital?> GetByCodeAsync(string facilityCode)
		{
			return await _context.Hospitals
				.FirstOrDefaultAsync(h => h.FacilityCode == facilityCode);
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}


	}
}

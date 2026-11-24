using Microsoft.EntityFrameworkCore;
using Visitor_Check_Application.Src.Application.DTO;

using Visitor_Check_Application.Src.Application.Interfaces;
using Visitor_Check_Application.Models;
namespace Visitor_Check_Application.Infrastructure.Respositories
{
    public class DivisionRepository:IDivisionInterface
    {
        private readonly VisitorCheckDbContext _context;

        public DivisionRepository(VisitorCheckDbContext context)
        {
            _context = context;
        }

		public async Task<List<DivisionDropDown>> GetAllDivisionsAsync()
		{
            return await _context.Divisions
                .OrderBy(d => d.DivisionName)
                .Select(d => new DivisionDropDown
                {
                    DivisionId = d.DivisionId,
                    DivisionName = d.DivisionName
                })
                .ToListAsync();
        }
    }
}

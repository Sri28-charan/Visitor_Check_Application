using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitor_Check_Application.Models;
using Visitor_Check_Application.Service_Layer;

namespace Visitor_Check_Application.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DivisionController : ControllerBase
	{
		private readonly DivisionService _divisionService;
		

		// Constructor injection
		public DivisionController(DivisionService divisionService)
		{
			_divisionService = divisionService;
		}

		[HttpGet]
		public async Task<IActionResult> GetDivisions()
		{
			var divisions = await _divisionService.GetAllDivisionsAsync();
			return Ok(divisions);
		}
	}
}



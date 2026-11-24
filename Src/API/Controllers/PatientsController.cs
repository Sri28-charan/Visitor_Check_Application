using Microsoft.AspNetCore.Mvc;
using Visitor_Check_Application.Common;
using Visitor_Check_Application.DTO;
using Visitor_Check_Application.Services;
using Visitor_Check_Application.Src.Application.DTO;

namespace Visitor_Check_Application.Controllers
{
	[ApiController]
	[Route("api/patients")]
	public class PatientsController : ControllerBase
	{
		private readonly PatientService _patientService;
		private readonly ILogger<PatientsController> _logger;

		public PatientsController(PatientService patientService, ILogger<PatientsController> logger)
		{
			_patientService = patientService;
			_logger = logger;
		}

		// GET patients by Facility Code



		// UPDATE visitors allowed + count

		[HttpPut("{patientId}/visitors")]
		public async Task<IActionResult> UpdateVisitors(
			string patientId,
			[FromBody] UpdateVisitors dto)
		{
			_logger.LogInformation("PUT /api/patients/{PatientId}/visitors payload:Allowed={Allowed},Count={Count}", patientId, dto?.VisitorsAllowed, dto?.VisitorsInRoom);

			if (dto == null)
			{
				_logger.LogWarning("UpdateVisitors called with null body");

				return Ok(new ApiResponse<string>
				{
					Success = false,
					Message = "Request body is required",
					Data = null
				});

			}

			await _patientService.UpdateVisitorsAsync(patientId, dto);

			_logger.LogInformation("Succefully updated visitors for patientId={PatientId}", patientId);
			return NoContent();
		}


		[HttpGet]
		public async Task<IActionResult> GetPatients(
	[FromQuery] PatientQueryFilter filter,
	CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(filter.FacilityCode))
			{
				return Ok(new ApiResponse<PagedResponse<PatientTable>>
				{
					Success = false,
					Message = "FacilityCode is required",
					Data = new PagedResponse<PatientTable>()
				});
			}

			var result = await _patientService.GetPatientsAsync(filter, cancellationToken);

			if (result.TotalRecords == 0)
			{
				return Ok(new ApiResponse<PagedResponse<PatientTable>>
				{
					Success = false,
					Message = "No patients found",
					Data = result
				});
			}

			return Ok(new ApiResponse<PagedResponse<PatientTable>>
			{
				Success = true,
				Message = "Patients fetched successfully",
				Data = result
			});
		}
	}

	}
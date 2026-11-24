using Microsoft.AspNetCore.Mvc;
using Visitor_Check_Application.Common;
using Visitor_Check_Application.Service_Layer;
using Visitor_Check_Application.Src.Application.DTO;


namespace Visitor_Check_Application.Controllers;
[ApiController]
[Route("api/hospitals")]
public class HospitalController:ControllerBase
{
    private readonly HospitalService _hospitalService;
    private readonly ILogger<HospitalController> _logger;

    public HospitalController(
        HospitalService hospitalService,
        ILogger<HospitalController> logger)
    {
        _hospitalService = hospitalService;
        _logger = logger;
    }
    

    [HttpGet]
    public async Task<IActionResult> GetHospitalsByDivision(
        [FromQuery] string divisionId)
    {
        // API Entry log
        _logger.LogInformation("GET /api/hospitals called with DivisionId={DivisionId}", divisionId);

        var hospitals = await _hospitalService.GetHospitalsByDivisionAsync(divisionId);

        //successful response log
        _logger.LogInformation("Returning {Count} hospitals for DivisionId={DivisionId}", hospitals.Count, divisionId);

		return Ok(hospitals);
	}

	[HttpGet("facilities")]
	public async Task<IActionResult> GetFacilities([FromQuery] FacilityQueryFilter filter,CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(filter.Division))
		{
			return Ok(new ApiResponse<PagedResponse<FacilityTable>>
			{
				Success = false,
				Message = "DivisionId is required",
				Data = new PagedResponse<FacilityTable>()
			});
		}

		var result = await _hospitalService.GetFacilitiesAsync(filter,cancellationToken);

		if (result.TotalRecords == 0)
		{
			return Ok(new ApiResponse<PagedResponse<FacilityTable>>
			{
				Success = false,
				Message = "No facilities found",
				Data = result
			});
		}

		return Ok(new ApiResponse<PagedResponse<FacilityTable>>
		{
			Success = true,
			Message = "Facilities fetched successfully",
			Data = result
		});
	}

	[HttpPut("{facilityCode}/settings")]
	public async Task<IActionResult> UpdateFacility(
	string facilityCode,
	[FromBody] FacilityUpdate dto)
	{
		if (dto == null)
		{
			return Ok(new ApiResponse<string>
			{
				Success = false,
				Message = "Request body is required",
				Data = null
			});
		}

		try
		{
			await _hospitalService.UpdateFacilityAsync(facilityCode, dto);

			return Ok(new ApiResponse<string>
			{
				Success = true,
				Message = "Facility updated successfully",
				Data = "OK"
			});
		}
		catch (KeyNotFoundException ex)
		{
			return Ok(new ApiResponse<string>
			{
				Success = false,
				Message = ex.Message,
				Data = null
			});
		}
		catch (ArgumentException ex)
		{
			return Ok(new ApiResponse<string>
			{
				Success = false,
				Message = ex.Message,
				Data = null
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating facility {FacilityCode}", facilityCode);

			return Ok(new ApiResponse<string>
			{
				Success = false,
				Message = "Unexpected error occurred",
				Data = null
			});
		}
	}
}

using Visitor_Check_Application.Common;
using Visitor_Check_Application.DTO_S;
using Visitor_Check_Application.Interfaces;
using Visitor_Check_Application.Src.Application.DTO;
using static System.Net.WebRequestMethods;

namespace Visitor_Check_Application.Service_Layer
{
	public class HospitalService
	{
		private readonly IHospitalInterface _hospitalInterface;
		private readonly ILogger<HospitalService> _logger;
		private readonly IPatientInterface _patientInterface;

		public HospitalService(
			IHospitalInterface hospitalInterface,
			ILogger<HospitalService> logger,IPatientInterface patientInterface)
		{
			_hospitalInterface = hospitalInterface;
			_logger = logger;
			_patientInterface = patientInterface;
		}

		public async Task<List<HospitalDropDown>> GetHospitalsByDivisionAsync(string divisionId)
		{
			// Basic validation
			if (string.IsNullOrWhiteSpace(divisionId))
			{
				_logger.LogWarning("DivisionId not provided");
				throw new ArgumentException("DivisionId is required");
			}

			// Business rule: divisionId must be numeric and < 1000
			if (!int.TryParse(divisionId, out int divisionIdValue))
			{
				_logger.LogWarning(
					"Invalid DivisionId format: {DivisionId}",
					divisionId
				);
				throw new ArgumentException("DivisionId must be a numeric value");
			}

			if (divisionIdValue >= 1000)
			{
				_logger.LogWarning(
					"DivisionId out of allowed range: {DivisionId}",
					divisionIdValue
				);
				throw new ArgumentException("DivisionId must be less than 1000");
			}

			// Business processing log
			_logger.LogInformation(
				"Fetching hospitals for DivisionId={DivisionId}",
				divisionIdValue
			);

			var hospitals =
				await _hospitalInterface.GetHospitalsByDivisionAsync(divisionId);

			// Business outcome log
			if (!hospitals.Any())
			{
				_logger.LogWarning(
					"No hospitals found for DivisionId={DivisionId}",
					divisionIdValue
				);
			}

			return hospitals;
		}


			public async Task<PagedResponse<FacilityTable>> GetFacilitiesAsync(
		FacilityQueryFilter filter,
		CancellationToken cancellationToken = default)
		{
			
			if (string.IsNullOrWhiteSpace(filter.Division))
			{
				_logger.LogWarning("Division is required");
				throw new ArgumentException("Division is required");
			}

			
			var result = await _hospitalInterface
				.GetFacilitiesAsync(filter, cancellationToken);

			if (result.TotalRecords == 0)
			{
				_logger.LogWarning("No facilities found");
			}

			return result;
		}


		public async Task UpdateFacilityAsync(string facilityCode, FacilityUpdate dto)
		{
			if (string.IsNullOrWhiteSpace(facilityCode))
				throw new ArgumentException("FacilityCode is required");

			var facility = await _hospitalInterface.GetByCodeAsync(facilityCode);

			if (facility == null)
				throw new KeyNotFoundException("Facility not found");

			facility.MaxLimit = dto.MaxLimit;
			facility.VisitorsAllowed = dto.VisitorsAllowed;

			await _hospitalInterface.SaveAsync();

			if (!(dto.VisitorsAllowed ?? false))
			{
				var patients = await _patientInterface.GetPatientsByFacilityAsync(facilityCode);

				foreach (var p in patients)
				{
					var entity = await _patientInterface.GetPatientByIdAsync(p.PatientId);

					if (entity != null)
					{
						entity.VisitorsAllowed = false;
						entity.VisitorsInRoom = 0;
						entity.VisitorStartTime = null;
					}
				}
				await _patientInterface.SaveAsync();
				
			}
		}

	
	}
}
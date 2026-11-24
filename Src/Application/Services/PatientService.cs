using Visitor_Check_Application.DTO;
using Visitor_Check_Application.Interfaces;
using Visitor_Check_Application.Common;
using Visitor_Check_Application.Src.Application.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Visitor_Check_Application.Services
{
	public class PatientService
	{
		private readonly IPatientInterface _patientRepository;
		private readonly ILogger<PatientService> _logger;
		private readonly IHospitalInterface _hospitalInterface;


		public PatientService(IPatientInterface patientRepository,ILogger<PatientService> logger,IHospitalInterface hospitalInterface
			)
		{
			_patientRepository = patientRepository;
			_logger = logger;
			_hospitalInterface= hospitalInterface;

		}

		public async Task<PagedResponse<PatientTable>> GetPatientsAsync(
			PatientQueryFilter filter,
			CancellationToken cancellationToken = default)
		{
			return await _patientRepository.GetPatientsAsync(filter, cancellationToken);
		}


		//  READ data (DTO)
		public async Task<List<PatientTable>> GetPatientsByFacilityAsync(string facilityCode)
		{
			_logger.LogInformation("Service:Fetching patients for FacilityCode={FacilityCode}", facilityCode);

			if (string.IsNullOrWhiteSpace(facilityCode))
			{
				_logger.LogWarning("Service:FacilityCode is null or empty");
				throw new ArgumentException("Facility code is required");
			}

			var patients= await _patientRepository.GetPatientsByFacilityAsync(facilityCode);

			_logger.LogInformation("Service:Found {Count} patients for FacilityCode={FacilityCode}", patients.Count, facilityCode);

			return patients;
		}

		//  UPDATE data (ENTITY)
		public async Task UpdateVisitorsAsync(string patientId, UpdateVisitors dto)
		{
			_logger.LogInformation("Service:Updating visitors for PatientId={PatientId}", patientId);

			if (string.IsNullOrWhiteSpace(patientId))
			{

				_logger.LogWarning("Service: PatientId is empty");
				throw new ArgumentException("PatientId is required");
			}

			if (dto.VisitorsInRoom < 0 || dto.VisitorsInRoom > 5)
			{
				_logger.LogWarning("Service:Invalid VisitorsInRoom={Count} for PatientId={PatienID}",dto.VisitorsInRoom,patientId);
				throw new ArgumentException("Visitors must be between 0 and 5");
			}

			
			var patient = await _patientRepository.GetPatientByIdAsync(patientId);

			if (patient == null)
			{
				_logger.LogWarning("Service:Patient not found:{PatientId}", patientId);
				throw new KeyNotFoundException("Patient not found");
			}

			var facility = await _hospitalInterface.GetByCodeAsync(patient.FacilityCode);

			if (facility == null)
			{
				_logger.LogError("Service:Facility not found for facilityCode={FacilityCode}",patient.FacilityCode);
				throw new Exception("Facility not found");
			}

			if (!(facility.VisitorsAllowed ?? false))
			{
				_logger.LogWarning("Service:visitors not allowed for facility {FacilityCode}", patient.FacilityCode);
				throw new Exception("Visitors are disabled for this hospital");
			}

			var oldVisitors = patient.VisitorsInRoom ?? 0;
			var newVisitors = dto.VisitorsInRoom;

			// If patient visitors are disabled, force everything to zero
			if (!dto.VisitorsAllowed)
			{
				patient.VisitorsAllowed = false;
				patient.VisitorsInRoom = 0;
				patient.VisitorStartTime = null;
			}
			else
			{
				// Start timer only when visitors go from 0 to greater than 0
				if (oldVisitors == 0 && newVisitors > 0)
				{
					patient.VisitorStartTime = DateTime.Now;
				}

				// Remove timer when visitor count becomes 0
				if (newVisitors == 0)
				{
					patient.VisitorStartTime = null;
				}

				patient.VisitorsAllowed = newVisitors>0;
				patient.VisitorsInRoom = newVisitors;
			}
			await _patientRepository.SaveAsync();

			_logger.LogInformation("Service:Visitors updated for PatientId={PatientId}", patientId);
		}
		
	}
}
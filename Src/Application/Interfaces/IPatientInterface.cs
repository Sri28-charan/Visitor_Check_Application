using Visitor_Check_Application.Common;
using Visitor_Check_Application.DTO;
using Visitor_Check_Application.Models;

namespace Visitor_Check_Application.Interfaces
{
    public interface IPatientInterface
    {
        Task<List<PatientTable>> GetPatientsByFacilityAsync(string facilityCode);
        Task<Patient?> GetPatientByIdAsync(string patientId);

        Task<PagedResponse<PatientTable>> GetPatientsAsync(
            PatientQueryFilter filter,
            CancellationToken cancelllationToken = default);
		Task SaveAsync();
    }
}

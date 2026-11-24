using Visitor_Check_Application.Common;
using Visitor_Check_Application.DTO_S;
using Visitor_Check_Application.Models;
using Visitor_Check_Application.Src.Application.DTO;


namespace Visitor_Check_Application.Interfaces;

public interface IHospitalInterface
{
	Task<List<HospitalDropDown>> GetHospitalsByDivisionAsync(string divisionId);


	Task<PagedResponse<FacilityTable>> GetFacilitiesAsync(FacilityQueryFilter filter, CancellationToken cancellationToken=default);

	Task<Hospital?> GetByCodeAsync(string facilityCode);

	Task SaveAsync();

}





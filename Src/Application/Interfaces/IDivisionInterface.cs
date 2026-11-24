using Visitor_Check_Application.DTO;
using Visitor_Check_Application.Src.Application.DTO;
namespace Visitor_Check_Application.Src.Application.Interfaces
{
    public interface IDivisionInterface
    {
        public  Task<List<DivisionDropDown>> GetAllDivisionsAsync();
    }
}

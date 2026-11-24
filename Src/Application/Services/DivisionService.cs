using Visitor_Check_Application.Models;
using Microsoft.EntityFrameworkCore;
using Visitor_Check_Application.Src.Application.Interfaces;
using Visitor_Check_Application.Src.Application.DTO;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace Visitor_Check_Application.Service_Layer
{
    public class DivisionService
    {
        private readonly IDivisionInterface _divisionRepository;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY = "DIVISIONS";
        public DivisionService(IDivisionInterface divisionRepository,IMemoryCache cache)
        {
            _divisionRepository=divisionRepository;
            _cache = cache;
        }

        public async Task<List<DivisionDropDown>> GetAllDivisionsAsync()
        {
            if (_cache.TryGetValue(CACHE_KEY, out List<DivisionDropDown> cachedData))
            {
				
				return cachedData;
            }
		
			var divisions= await _divisionRepository.GetAllDivisionsAsync();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            _cache.Set(CACHE_KEY, divisions, cacheOptions);
            return divisions;
        }
    }
}


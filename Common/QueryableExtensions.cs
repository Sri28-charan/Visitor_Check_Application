
using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Visitor_Check_Application.Models;

namespace Visitor_Check_Application.Common;

/// <summary>
/// IQueryable extension methods for pagination, sorting, and searching.
/// These extensions are composed into a single SQL query by EF Core.
/// </summary>
public static partial class QueryableExtensions
{

	// PAGINATION

	public static IQueryable<T> ApplyPagination<T>(
		this IQueryable<T> query,
		int pageNumber,
		int pageSize)
	{
		return query
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize);
	}

	// DYNAMIC SORTING

	public static IQueryable<T> ApplySort<T>(
		this IQueryable<T> query,
		string? sortBy) where T : class
	{
		if (string.IsNullOrWhiteSpace(sortBy))
			return query;

		var allowedProperties = typeof(T)
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Select(p => p.Name) //default name sorting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var expressions = new List<string>();

		foreach (var part in sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries))
		{
			var tokens = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length == 0 || !allowedProperties.Contains(tokens[0]))
				continue;

			var direction =
				tokens.Length > 1 &&
				tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
					? "descending"
					: "ascending";

			expressions.Add($"{tokens[0]} {direction}");
		}

		return expressions.Count > 0
			? query.OrderBy(string.Join(", ", expressions))
			: query;
	}


	// SEARCH (Patients-specific)

	public static IQueryable<Patient> ApplySearch(
		this IQueryable<Patient> query,
		string? search)
	{
		if (string.IsNullOrWhiteSpace(search) || search.Length<3)
			return query;

		return query.Where(p =>
			EF.Functions.Like(p.PatientName, $"%{search}%") ||
			EF.Functions.Like(p.Room, $"%{search}%") ||
			EF.Functions.Like(p.FloorNumber, $"%{search}%"));
	}


	public static IQueryable<Hospital> ApplyFacilitySearch(this IQueryable<Hospital> query, string? search)
	{
		if (string.IsNullOrWhiteSpace(search) || search.Length < 2)
			return query;

		return query.Where(h =>
		EF.Functions.Like(h.FacilityCode, $"%{search}%") ||
		EF.Functions.Like(h.HospitalName!, $"%{search}%"));
		
	}
}
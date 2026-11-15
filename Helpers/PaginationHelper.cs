using Microsoft.EntityFrameworkCore;
using TourismHajj.ViewModels;

namespace TourismHajj.Helpers;

public static class PaginationHelper
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            PageIndex = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages == 0 ? 1 : totalPages,
            Search = search
        };
    }
}
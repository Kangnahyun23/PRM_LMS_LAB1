using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.BusinessModels.Query;

namespace PRN232.LMS.Repositories.Query;

internal static class QueryExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int size)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;
        return query.Skip((page - 1) * size).Take(size);
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort)) return query;

        var fields = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (fields.Length == 0) return query;

        IOrderedQueryable<T>? ordered = null;
        foreach (var f in fields)
        {
            var desc = f.StartsWith("-");
            var name = desc ? f[1..] : f;
            if (string.IsNullOrWhiteSpace(name)) continue;

            ordered = ApplyOrderBy(ordered ?? query, name, desc, thenBy: ordered != null);
        }

        return ordered ?? query;
    }

    private static IOrderedQueryable<T> ApplyOrderBy<T>(IQueryable<T> query, string property, bool desc, bool thenBy)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = BuildPropertyAccess(parameter, property);
        var lambda = Expression.Lambda(body, parameter);

        var method = thenBy
            ? (desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy))
            : (desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy));

        var call = Expression.Call(
            typeof(Queryable),
            method,
            new[] { typeof(T), body.Type },
            query.Expression,
            Expression.Quote(lambda));

        return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(call);
    }

    private static Expression BuildPropertyAccess(Expression parameter, string propertyPath)
    {
        Expression body = parameter;
        foreach (var member in propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            body = Expression.PropertyOrField(body, member);
        }
        return body;
    }

    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int size,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;

        var totalItems = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);

        var items = await query
            .ApplyPaging(page, size)
            .ToListAsync(ct);

        return new PagedResult<T>
        {
            Items = items,
            Pagination = new PaginationMeta
            {
                Page = page,
                PageSize = size,
                TotalItems = totalItems,
                TotalPages = totalPages,
            },
        };
    }
}


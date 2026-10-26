using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Persistence.Specifications;

namespace Persistence.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, BaseSpecification<T> spec)
            where T : class
        {
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
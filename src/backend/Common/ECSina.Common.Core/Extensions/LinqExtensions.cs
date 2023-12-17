using System.Linq.Expressions;

namespace ECSina.Common.Core.Extensions;

public static class LinqExtensions
{
    /// <summary>
    /// A fluentAPI version of foreach statement
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
            action(item);
    }

    public static IQueryable<TEntity> WithPaging<TEntity, TOrderKey>(
        this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, TOrderKey>> orderKeySelector,
        int offset,
        int size,
        bool descendingOrder = false
    )
    {
        return descendingOrder
            ? queryable.OrderByDescending(orderKeySelector).Skip(offset).Take(size)
            : queryable.OrderBy(orderKeySelector).Skip(offset).Take(size);
    }
}

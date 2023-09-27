using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Models;


public static class DynamicSorter
{
    public static IQueryable<T> Sort<T>(this IQueryable<T> query, OrderByModel orderBy)
    {
        // Get the type of the object being sorted
        Type entityType = typeof(T);

        // Get the property to sort by based on the OrderByModel
        PropertyInfo property = GetSortingProperty(entityType, orderBy);

        // Create a parameter expression for the lambda
        ParameterExpression parameter = Expression.Parameter(entityType, "x");

        // Create a property access expression for the selected property
        MemberExpression propertyAccess = Expression.MakeMemberAccess(parameter, property);

        // Create the lambda expression for sorting
        LambdaExpression orderByExpression = Expression.Lambda(propertyAccess, parameter);

        // Determine whether to use OrderBy or OrderByDescending based on the OrderByModel
        string methodName = orderBy.ToString().EndsWith("Desc") ? "OrderByDescending" : "OrderBy";

        // Get the OrderBy method based on the property type
        var orderByMethod = typeof(Queryable).GetMethods()
            .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
            .Single(m => m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType, property.PropertyType);

        // Apply the sorting to the query and return the result
        return (IQueryable<T>)orderByMethod.Invoke(null, new object[] { query, orderByExpression });
    }

    private static PropertyInfo GetSortingProperty(Type entityType, OrderByModel orderBy)
    {
        // Map OrderByModel values to property names
        Dictionary<OrderByModel, string> propertyMap = new Dictionary<OrderByModel, string>
        {
            { OrderByModel.DownloadsDesc, "Downloads" },
            { OrderByModel.Price, "Price" },
            { OrderByModel.PriceDesc, "Price" },
            { OrderByModel.StarsDesc, "StarCount" },
            { OrderByModel.DateDesc, "DateOfCreated" },
            { OrderByModel.Date, "DateOfCreated" },
        };

        // Get the property name based on the OrderByModel
        string propertyName = propertyMap[orderBy];

        // Get the corresponding PropertyInfo
        PropertyInfo property = entityType.GetProperty(propertyName);

        if (property == null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found in type '{entityType.FullName}'.");
        }

        return property;
    }
}

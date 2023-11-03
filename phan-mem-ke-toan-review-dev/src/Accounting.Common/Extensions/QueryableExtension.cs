using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Common.Extensions
{
    public static class QueryableExtension
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string propertyName,
                                                    object value, FilterOperator filterOperator = FilterOperator.Equal)
        {
            var parameterExp = Expression.Parameter(typeof(T), "p");
            var propertyExp = Expression.Property(parameterExp, propertyName);
            var propertyInfo = typeof(T).GetProperties()
                                    .Where(c => c.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                                    .FirstOrDefault();
            var propertyType = propertyInfo.PropertyType;
            //var propertyType = typeof(T).GetProperty(propertyName).PropertyType;
            var _ILike = typeof(NpgsqlDbFunctionsExtensions).GetMethod("ILike",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        new[] {
                                            typeof(Microsoft.EntityFrameworkCore.DbFunctions),
                                            typeof(string),
                                            typeof(string)
                        },
                        null
                        );

            Expression expr = null;

            switch (filterOperator)
            {
                case FilterOperator.Contains:
                    MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    expr = Expression.Call(propertyExp, method, Expression.Constant(value, typeof(string)));
                    break;
                case FilterOperator.Equal:
                    expr = Expression.Equal(Expression.Property(parameterExp, propertyName), Expression.Constant(value, propertyType));
                    break;
                case FilterOperator.GreaterThan:
                    expr = Expression.GreaterThan(Expression.Property(parameterExp, propertyName), Expression.Constant(value, propertyType));
                    break;
                case FilterOperator.GreaterThanOrEqual:
                    expr = Expression.GreaterThanOrEqual(Expression.Property(parameterExp, propertyName), Expression.Constant(value, propertyType));
                    break;
                case FilterOperator.LessThan:
                    expr = Expression.LessThan(Expression.Property(parameterExp, propertyName), Expression.Constant(value, propertyType));
                    break;
                case FilterOperator.LessThanOrEqual:
                    expr = Expression.LessThanOrEqual(Expression.Property(parameterExp, propertyName), Expression.Constant(value, propertyType));
                    break;
                case FilterOperator.ILike:                    
                    expr = Expression.Call(_ILike, Expression.Constant(null, typeof(DbFunctions)), Expression.Property(parameterExp, propertyName), Expression.Constant("%" + value.ToString() + "%", typeof(string)));
                    break;
                default:
                    throw new Exception("Not Support Filter Operation");
            }

            var lambda = Expression.Lambda<Func<T, bool>>(expr, parameterExp);

            return source.Where(lambda);
        }
    }
    public enum FilterOperator
    {
        Contains,
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        ILike
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static QueryExpressionBuilder.Attributes.Numbers;
using static QueryExpressionBuilder.Attributes.String;

namespace QueryExpressionBuilder
{
    /// <summary>
    /// Class converter from Query model with attributes to predicate func
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        /// Extension method
        /// </summary>
        /// <typeparam name="TDB">Type in DB</typeparam>
        /// <typeparam name="TQE">Type of Query object</typeparam>
        /// <param name="queryParams">Dictionary</param>
        /// <returns>Predicate func</returns>
        public static Expression<Func<TDB, bool>>? ToPredicate<TDB, TQE>(this Dictionary<string, string> queryParams)
        {
            return GetPredicateFromDictionary<TDB, TQE>(queryParams);
        }

        /// <summary>
        /// Make predicate from dictionary
        /// </summary>
        /// <typeparam name="TDB">Type in DB</typeparam>
        /// <typeparam name="TQE">Type of Query object</typeparam>
        /// <param name="queryParams">Dictionary</param>
        /// <returns>Predicate func</returns>
        public static Expression<Func<TDB, bool>>? GetPredicateFromDictionary<TDB, TQE>(Dictionary<string, string> queryParams)
        {
            var query = GetQueryObject<TQE>(queryParams);
            return GetPredicate<TDB, TQE>(query);
        }

        /// <summary>
        /// Make Query instance
        /// </summary>
        /// <typeparam name="T">Type of Query</typeparam>
        /// <param name="queryParams">Dictionary</param>
        /// <returns>Instance</returns>
        public static T GetQueryObject<T>(Dictionary<string, string> queryParams)
        {
            Type type = typeof(T);
            T queryObject = Activator.CreateInstance<T>();

            foreach (var property in type.GetProperties())
            {
                if (queryParams.ContainsKey(property.Name))
                {
                    object value;
                    Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (propertyType == typeof(string) && string.IsNullOrEmpty(queryParams[property.Name]))
                    {
                        value = null;
                    }
                    else
                    {
                        value = Convert.ChangeType(queryParams[property.Name], propertyType);
                    }

                    property.SetValue(queryObject, value);
                }
            }

            return queryObject;
        }

        /// <summary>
        /// Make new predicate func
        /// </summary>
        /// <typeparam name="TDB">Type in DB</typeparam>
        /// <typeparam name="TQE">Query type</typeparam>
        /// <returns>Predicate func</returns>
        public static Expression<Func<TDB, bool>>? GetPredicate<TDB, TQE>(TQE query)
        {
            var userParameter = Expression.Parameter(typeof(TDB), typeof(TDB).Name.ToLower());
            List<Expression> conditions = new List<Expression>();

            //Только те свойства которые помечены атрибутами
            var T_pr_props = typeof(TQE).GetProperties().Where(prop => prop.GetCustomAttribute<LessOrEqualAttribute>() != null ||
                        prop.GetCustomAttribute<GreaterOrEqualAttribute>() != null || prop.GetCustomAttribute<StartWithAttribute>() != null || 
                        prop.GetCustomAttribute<ContainsAttribute>() != null || prop.GetCustomAttribute<EqualsAttribute>() != null).ToArray();

            //Проходимся по всем параметрам класса БД
            foreach (var p in typeof(TDB).GetProperties())
            {
                var yslovia = T_pr_props.Where(x => x.Name.Contains(p.Name));
                foreach (var y in yslovia)
                {
                    if (y.GetCustomAttribute<StartWithAttribute>() != null)
                    {
                        var propertyY = Expression.Property(userParameter, p.Name);
                        var propertyP = Expression.Property(userParameter, p.Name);
                        var containsCall = Expression.Call(propertyP, "StartsWith", null, Expression.Constant(y.GetValue(query), typeof(string)));
                        var obj = y.GetValue(query);
                        if (obj != null)
                        {
                            conditions.Add(containsCall);
                        }
                    }
                    else if(y.GetCustomAttribute<ContainsAttribute>() != null)
                    {
                        var propertyY = Expression.Property(userParameter, p.Name);
                        var propertyP = Expression.Property(userParameter, p.Name);
                        var containsCall = Expression.Call(propertyP, "Contains", null, Expression.Constant(y.GetValue(query), typeof(string)));
                        var obj = y.GetValue(query);
                        if (obj != null)
                        {
                            conditions.Add(containsCall);
                        }
                    }
                    else if (y.GetCustomAttribute<GreaterOrEqualAttribute>() != null)
                    {
                        var propertyName = y.GetCustomAttribute<GreaterOrEqualAttribute>().PropertyName;
                        var propertyY = Expression.Property(userParameter, propertyName);
                        var obj = y.GetValue(query);
                        if (obj != null)
                        {
                            var value = Convert.ChangeType(obj, p.PropertyType);
                            var constantValue = Expression.Constant(value, p.PropertyType);
                            var condition = Expression.GreaterThanOrEqual(propertyY, constantValue);
                            conditions.Add(condition);
                        }
                    }
                    else if (y.GetCustomAttribute<LessOrEqualAttribute>() != null)
                    {
                        var propertyName = y.GetCustomAttribute<LessOrEqualAttribute>().PropertyName;
                        var propertyY = Expression.Property(userParameter, propertyName);
                        var obj = y.GetValue(query);
                        if (obj != null)
                        {
                            var value = Convert.ChangeType(obj, p.PropertyType);
                            var constantValue = Expression.Constant(value, p.PropertyType);
                            var condition = Expression.LessThanOrEqual(propertyY, constantValue);
                            conditions.Add(condition);
                        }
                    }
                    else if(y.GetCustomAttribute<EqualsAttribute>() != null)
                    {
                        var propertyName = y.GetCustomAttribute<EqualsAttribute>().PropertyName;
                        var propertyY = Expression.Property(userParameter, propertyName);
                        var obj = y.GetValue(query);
                        if (obj != null)
                        {
                            var value = Convert.ChangeType(obj, p.PropertyType);
                            var constantValue = Expression.Constant(value, p.PropertyType);
                            var condition = Expression.Equal(propertyY, constantValue);
                            conditions.Add(condition);
                        }
                    }
                }
            }

            var body = conditions.Aggregate(Expression.AndAlso);
            var predicate = Expression.Lambda<Func<TDB, bool>>(body, userParameter);
            return predicate;
        }
    }
}

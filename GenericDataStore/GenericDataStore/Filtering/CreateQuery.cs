using System.ComponentModel;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using GenericDataStore.Models;
using System.Reflection.Metadata;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;

namespace GenericDataStore.Filtering
{
    public static class CreateQuery<T>
    {
        private static Expression BuildFilterExpression(Filter filter, ParameterExpression parameter)
        {
            //if (filter.Filters != null && filter.Filters.Any())
            //{
            //    if (filter.Logic?.ToLower() == "and")
            //    {
            //        var andFilters = filter.Filters.Select(f => BuildFilterExpression(f, parameter));
            //        return andFilters.Aggregate(Expression.AndAlso);
            //    }
            //    else if (filter.Logic?.ToLower() == "or")
            //    {
            //        var orFilters = filter.Filters.Select(f => BuildFilterExpression(f, parameter));
            //        return orFilters.Aggregate(Expression.OrElse);
            //    }
            //}

            if (filter.Value == null || string.IsNullOrWhiteSpace(filter.Value.ToString()))
            {
                return null;
            }

            var property = Expression.Property(parameter, filter.Field);
            var propertyType = ((PropertyInfo)property.Member).PropertyType;
            var converter = TypeDescriptor.GetConverter(propertyType);

            if (!converter.CanConvertFrom(typeof(string)))
            {
                throw new NotSupportedException();
            }

            var propertyValue = converter.ConvertFromInvariantString(filter.Value.ToString());
            var valueex = Expression.Constant(propertyValue);
            var constant = Expression.Convert(valueex, propertyType);

            if (propertyType == typeof(bool))
            {
                return Expression.Equal(property, constant);
            }

            switch (filter.Operator.ToLower())
            {
                case "equals":
                    return Expression.Equal(property, constant);
                case "notequals":
                    return Expression.NotEqual(property, constant);
                case "lt":
                    return Expression.LessThan(property, constant);
                case "lte":
                    return Expression.LessThanOrEqual(property, constant);
                case "gt":
                    return Expression.GreaterThan(property, constant);
                case "gte":
                    return Expression.GreaterThanOrEqual(property, constant);
                case "contains":
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    return Expression.Call(property, containsMethod, constant);
                case "startswith":
                    var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });
                    var constantLower = Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                    return Expression.Call(property, startsWithMethod, constant, Expression.Constant(StringComparison.OrdinalIgnoreCase));

                default:
                    throw new ArgumentException($"Unsupported operator: {filter.Operator}");
            }
        }

        private static Expression<Func<T, bool>>? GetAndFilterExpression(List<Filter> filters)
        {
            if (filters == null || !filters.Any())
            {
                return null;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? andExpression = null;

            foreach (var filter in filters)
            {
                var filterExpression = BuildFilterExpression(filter, parameter);
                if (filterExpression != null)
                {
                    if (andExpression == null)
                    {
                        andExpression = filterExpression;
                    }
                    else
                    {
                        andExpression = Expression.AndAlso(andExpression, filterExpression);
                    }
                }
            }

            if (andExpression == null)
            {
                andExpression = Expression.Constant(false);
            }

            return Expression.Lambda<Func<T, bool>>(andExpression, parameter);
        }

        private static Expression<Func<T, bool>>? GetOrFilterExpression(List<Filter> filters)
        {
            if (filters == null || !filters.Any())
            {
                return null;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? orExpression = null;

            foreach (var filter in filters)
            {
                var filterExpression = BuildFilterExpression(filter, parameter);
                if (filterExpression != null)
                {
                    if (orExpression == null)
                    {
                        orExpression = filterExpression;
                    }
                    else
                    {
                        orExpression = Expression.OrElse(orExpression, filterExpression);
                    }
                }
            }

            if (orExpression == null)
            {
                orExpression = Expression.Constant(false);
            }

            return Expression.Lambda<Func<T, bool>>(orExpression, parameter);
        }

        public static IQueryable<T> ApplyFilter(IQueryable<T> query, RootFilter filter)
        {
            if (filter == null || filter.Filters == null || !filter.Filters.Any())
            {
                return query;
            }

            Expression<Func<T, bool>>? compositeFilterExpression = null;

            if (filter.Logic?.ToLower() == "and")
            {
                compositeFilterExpression = GetAndFilterExpression(filter.Filters);
            }
            else if (filter.Logic?.ToLower() == "or")
            {
                compositeFilterExpression = GetOrFilterExpression(filter.Filters);
            }

            return compositeFilterExpression != null
                ? query.Where(compositeFilterExpression)
                : query;
        }

        public static IQueryable<T> SortData(IQueryable<T> data, IEnumerable<SortingParams> sortingParams)
        {
            IOrderedQueryable<T>? sortedData = null;
            foreach (var sortingParam in sortingParams.Where(x => !string.IsNullOrEmpty(x.Field)))
            {
                var col = typeof(T).GetProperty(sortingParam.Field, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (col != null)
                {
                    sortedData = sortedData == null ? sortingParam.Order == -1 ? data.OrderBy(x => col.GetValue(x, null))
                                                                                               : data.OrderByDescending(x => col.GetValue(x, null))
                                                    : sortingParam.Order == 1 ? sortedData.ThenBy(x => col.GetValue(x, null))
                                                                                        : sortedData.ThenByDescending(x => col.GetValue(x, null));
                }
            }

            return sortedData ?? data;
        }

        public static IQueryable<DataObject> ValueFilter(IQueryable<DataObject> query, RootFilter filters)
        {
            if (filters != null)
            {
                foreach (var item in filters.ValueFilters)
                {
                    switch (item.Operator.ToLower())
                    {
                        //case "child":
                        //    query = query.Where(x => x.ParentDataObjectId.ToString() == item.Value.ToString());
                        //    break;
                        //case "user":
                        //    query = query.Where(x => x.AppUserId.ToString() == item.Value.ToString());
                        //    break;
                        //case "objid":
                        //    query = query.Where(x => x.DataObjectId.ToString() == item.Value.ToString());
                        //    break;
                        case "equals":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString == item.Value.ToString());
                            break;
                        case "notequals":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != item.Value.ToString());

                            break;

                        case "lt":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? double.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) < double.Parse(item.Value.ToString()) : false);

                            break;

                        case "lte":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? double.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) <= double.Parse(item.Value.ToString()) : false);

                            break;

                        case "gt":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? double.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) > double.Parse(item.Value.ToString()) : false);

                            break;

                        case "gte":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? double.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) >= double.Parse(item.Value.ToString()) : false);


                            break;

                        case "contains":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            if (item.Value.ToString().ToLower() == "false" || item.Value.ToString().ToLower() == "true")
                            {
                                query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString.ToLower() == item.Value.ToString().ToLower() : false);

                            }
                            else
                            {
                                query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString.Contains(item.Value.ToString()) : false);

                            }
                            break;
                        case "notcontains":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? !x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString.Contains(item.Value.ToString()) : false);
                            break;

                        case "startswith":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString.StartsWith(item.Value.ToString()) : false);
                            break;
                        case "endswith":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString.EndsWith(item.Value.ToString()) : false);
                            break;

                        case "dateis":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            DateTime val1 = DateTime.Parse(item.Value.ToString().Split(new char[] { ' ' })[0]).AddDays(1);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? DateTime.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) == val1 : false);
                            break;
                        case "dateisnot":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            DateTime val2 = DateTime.Parse(item.Value.ToString().Split(new char[] { ' ' })[0]).AddDays(1);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? DateTime.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) != val2 : false);
                            break;
                        case "datebefore":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            DateTime val3 = DateTime.Parse(item.Value.ToString().Split(new char[] { ' ' })[0]).AddDays(1);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? DateTime.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) < val3 : false);
                            break;
                        case "dateafter":
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field) != null);
                            DateTime val4 = DateTime.Parse(item.Value.ToString().Split(new char[] { ' ' })[0]).AddDays(1);
                            query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? DateTime.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) > val4 : false);
                            break;

                        //default:
                            //throw new ArgumentException($"Unsupported operator: {item.Operator}");
                    }

                }
            }
            return query;

        }

        public static IQueryable<DataObject> ValueSort(IQueryable<DataObject> query, RootFilter filters)
        {
            if (filters != null)
            {
                foreach (var item in filters.ValueSortingParams)
                {
                    if(item.Order == 1)
                    {
                        if(item.Type == "numeric" || item.Type == "calculatednumeric")
                        {
                            query = query.OrderBy(x => Convert.ToDouble(x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != "∞" ? x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString : int.MaxValue, CultureInfo.InvariantCulture));

                        }
                        else if (item.Type == "date")
                        {
                            query = query.OrderBy(x => x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != null ? DateTime.ParseExact(x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString, new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss", }, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.MaxValue);
                        }
                        else
                        {
                            query = query.OrderBy(x => String.IsNullOrEmpty(x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString)).ThenBy(x => x.Value.FirstOrDefault(y => y.Name == item.Field) != null ? x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString: "");

                        }
                    }
                    else
                    {
                        if (item.Type == "numeric" || item.Type == "calculatednumeric")
                        {
                            query = query.OrderByDescending(x => Convert.ToDouble((x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != "∞" ? x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != null ? x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString : int.MinValue: int.MaxValue), CultureInfo.InvariantCulture));

                        }
                        else if (item.Type == "date")
                        {
                            query = query.OrderByDescending(x => x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != "" && x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString != null ? DateTime.ParseExact(x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString,new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss", }, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.MinValue);
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.Value.FirstOrDefault(y => y.Name == item.Field) != null ? x.Value.FirstOrDefault(y => y.Name == item.Field).ValueString : "");

                        }
                    }

                }
            }
            return query;

        }

        public static List<IQueryable<Value>> PreValueFilter(IQueryable<Value> query, RootFilter filters)
        {
            if (filters != null && filters?.ValueFilters?.Count > 0)
            {
                List<IQueryable<Value>> values = new List<IQueryable<Value>>();
                foreach (var item in filters.ValueFilters)
                {
                    switch (item.Operator.ToLower())
                    {
                        case "child":
                            break;
                        case "user":
                            break;
                        case "objid":
                            break;
                        case "equals":
                            values.Add(query.Where(x => x.Name == item.Field && x.ValueString == item.Value.ToString()));
                            break;
                        case "notequals":
                            values.Add(query.Where(x => x.Name == item.Field && x.ValueString != item.Value.ToString()));
                            break;
                        //case "lt":
                        //    values.Add(query.Where(x => x.Name == item.Field && x.ValueString  < item.Value.ToString()));
                        //    break;

                        //case "lte":
                        //    values.Add(query.Where(x => x.Name == item.Field && double.Parse(x.ValueString ?? int.MaxValue.ToString()) <= double.Parse(item.Value.ToString())));

                        //    break;

                        //case "gt":
                        //    values.Add(query.Where(x => x.Name == item.Field && double.Parse(x.ValueString ?? int.MaxValue.ToString()) > double.Parse(item.Value.ToString())));

                        //    break;

                        //case "gte":
                        //    values.Add(query.Where(x => x.Name == item.Field && double.Parse(x.ValueString ?? int.MaxValue.ToString()) >= double.Parse(item.Value.ToString())));
                        //    break;

                        case "contains":
                            if (item.Value.ToString().ToLower() == "false" || item.Value.ToString().ToLower() == "true")
                            {
                                values.Add(query.Where(x => x.Name == item.Field && (x.ValueString != null ? x.ValueString.ToLower() : "") == item.Value.ToString().ToLower()));
                            }
                            else
                            {
                                values.Add(query.Where(x => x.Name == item.Field && (x.ValueString != null ? x.ValueString.Contains(item.Value.ToString()) : false )));

                            }
                            break;
                        case "notcontains":
                            values.Add(query.Where(x => x.Name == item.Field && (x.ValueString != null ? !x.ValueString.Contains(item.Value.ToString()) : false)));
                            break;

                        case "startswith":
                            values.Add(query.Where(x => x.Name == item.Field && (x.ValueString != null ? x.ValueString.StartsWith(item.Value.ToString()) : false)));
                            break;
                        case "endswith":
                            values.Add(query.Where(x => x.Name == item.Field && (x.ValueString != null ? x.ValueString.EndsWith(item.Value.ToString()) : false)));
                            break;

                        //case "dateis":
                        //    DateTime val1 = DateTime.Parse(item.Value.ToString().Split(new char[] { ' ' })[0]).AddDays(1);
                        //    query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? DateTime.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) == val1 : false);
                        //    break;
                        //case "dateisnot":
                        //    DateTime val2 = DateTime.Parse(item.Value.ToString().Split(new char[] { ' ' })[0]).AddDays(1);
                        //    query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? DateTime.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) != val2 : false);
                        //    break;
                        //case "datebefore":
                        //    DateTime val3 = DateTime.Parse(item.Value.ToString().Split(new char[] { ' ' })[0]).AddDays(1);
                        //    query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? DateTime.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) < val3 : false);
                        //    break;
                        //case "dateafter":
                        //    DateTime val4 = DateTime.Parse(item.Value.ToString().Split(new char[] { ' ' })[0]).AddDays(1);
                        //    query = query.Where(x => x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString != null ? DateTime.Parse(x.Value.FirstOrDefault(x => x.Name == item.Field).ValueString) > val4 : false);
                        //    break;

                    }

                }

                return values;
            }

            return new List<IQueryable<Value>> { query };

        }

        public static IQueryable<Value> PreValueSort(IQueryable<Value> query, RootFilter filters)
        {
            if (filters != null)
            {
                foreach (var item in filters.ValueSortingParams)
                {
                    if (item.Order == 1)
                    {
                        if (item.Type == "numeric")
                        {
                            query = query.Where(x => x.Name == item.Field);
                            query = query.OrderBy(x => Convert.ToDouble(x.ValueString != "" && x.ValueString != null ? x.ValueString : int.MaxValue, CultureInfo.InvariantCulture));

                        }
                        else if (item.Type == "date")
                        {
                            query = query.Where(x => x.Name == item.Field);
                            query = query.OrderBy(x => x.ValueString != "" && x.ValueString != null ? DateTime.Parse(x.ValueString) : DateTime.MaxValue);
                        }
                        else
                        {
                            query = query.Where(x => x.Name == item.Field);
                            query = query.OrderBy(x => x != null ? x.ValueString : "");

                        }
                    }
                    else
                    {
                        if (item.Type == "numeric")
                        {
                            query = query.Where(x => x.Name == item.Field);
                            query = query.OrderByDescending(x => Convert.ToDouble(x.ValueString != "" && x.ValueString != null ? x.ValueString : int.MinValue, CultureInfo.InvariantCulture));

                        }
                        else if (item.Type == "date")
                        {
                            query = query.Where(x => x.Name == item.Field);
                            query = query.OrderByDescending(x => x.ValueString != "" && x.ValueString != null ? DateTime.Parse(x.ValueString) : DateTime.MinValue);
                        }
                        else
                        {
                            query = query.Where(x => x.Name == item.Field);
                            query = query.OrderByDescending(x => x != null ? x.ValueString : "");

                        }
                    }

                }
            }
            return query;

        }

        public static IQueryable<DataObject> ObjectFilter(IQueryable<DataObject> query, RootFilter filters)
        {
            if (filters != null)
            {
                foreach (var item in filters.ValueFilters)
                {
                    switch (item.Operator.ToLower())
                    {
                        //case "child":

                            //query = query.Where(x => x.ParentDataObjectId != null ? x.ParentDataObjectId.ToString() == item.Value.ToString() : false);
                            //break;
                            //case "user":
                            //    query = query.Where(x => x.AppUserId.ToString() == item.Value.ToString());
                            //    break;
                            //case "objid":
                            //    query = query.Where(x => x.DataObjectId.ToString() == item.Value.ToString());
                            //    break;
                    }

                }
            }
            return query;

        }



    }
}

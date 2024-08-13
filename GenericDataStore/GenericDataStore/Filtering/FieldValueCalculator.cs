using GenericDataStore.DatabaseConnector;
using GenericDataStore.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GenericDataStore.Filtering
{
    public static class FieldValueCalculator
    {
        public static string Calculate(string op, List<string> allvalue, string resultstring, object func)
        {
            switch (op.ToLower())
            {
                case "sum":
                    double sum = 0;
                    foreach (var item2 in allvalue)
                    {
                        if (item2 != null)
                        {
                            if (item2.Contains("."))
                            {
                                sum += double.Parse(item2, CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                sum += double.Parse(item2);
                            }
                        }
                    }
                    return resultstring.Replace(func.ToString(), sum.ToString());
                    break;
                case "avg":
                    double avg = 0;
                    foreach (var item2 in allvalue)
                    {
                        if (item2 != null)
                        {
                            if (item2.Contains("."))
                            {
                                avg += double.Parse(item2, CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                avg += double.Parse(item2);
                            }
                        }
                    }
                    if(allvalue.Count() == 0)
                    {
                        return resultstring.Replace(func.ToString(), "0");
                    }
                    avg = avg / allvalue.Count();
                    return resultstring.Replace(func.ToString(), avg.ToString());
                    break;
                case "min":
                    double min = double.MaxValue;
                    foreach (var item2 in allvalue)
                    {
                        if (item2 != null)
                        {
                            if (item2.Contains("."))
                            {
                                min = Math.Min(min, double.Parse(item2, CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                min = Math.Min(min, double.Parse(item2));
                            }
                        }
                    }
                    if(min == double.MaxValue)
                    {
                        return resultstring.Replace(func.ToString(), "");
                    }
                    return resultstring.Replace(func.ToString(), min.ToString());
                    break;
                case "max":
                    double max = double.MinValue;
                    foreach (var item2 in allvalue)
                    {
                        if (item2 != null)
                        {
                            if (item2.Contains("."))
                            {
                                max = Math.Max(max, double.Parse(item2, CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                max = Math.Max(max, double.Parse(item2));
                            }
                        }
                    }
                    if (max == double.MinValue)
                    {
                        return resultstring.Replace(func.ToString(), "");
                    }
                    return resultstring.Replace(func.ToString(), max.ToString());
                    break;
                case "count":
                    return resultstring.Replace(func.ToString(), allvalue.Count().ToString());
                    break;
                case "countdistinct":
                    return resultstring.Replace(func.ToString(), allvalue.Distinct().Count().ToString());
                    break;
                case "countnull":
                    return resultstring.Replace(func.ToString(), allvalue.Count(x => x == null || x == "").ToString());
                    break;
                case "countnotnull":
                    return resultstring.Replace(func.ToString(), allvalue.Count(x => x != null && x != "").ToString());
                    break;
                case "first":
                    return resultstring.Replace(func.ToString(), allvalue.FirstOrDefault() ?? "");
                    break;
                case "last":
                    return resultstring.Replace(func.ToString(), allvalue.LastOrDefault() ?? "");
                    break;
                case "concat":
                    return resultstring.Replace(func.ToString(), string.Join(",", allvalue));
                    break;
                case "minlenght":
                    return resultstring.Replace(func.ToString(), allvalue.Min(x => x.Length).ToString());

                    break;
                case "maxlenght":
                    return resultstring.Replace(func.ToString(), allvalue.Max(x => x.Length).ToString());
                    break;
                case "sumlenght":
                    return resultstring.Replace(func.ToString(), allvalue.Sum(x => x.Length).ToString());
                    break;
                case "avglength":
                    return resultstring.Replace(func.ToString(), allvalue.Average(x => x.Length).ToString());
                    break;
                case "all":
                    return resultstring.Replace(func.ToString(), allvalue.All(x => x?.ToLower() == "true").ToString());
                    break;
                case "any":
                    return resultstring.Replace(func.ToString(), allvalue.Any(x => x?.ToLower() == "true").ToString());
                    break;
                case "allnot":
                    return resultstring.Replace(func.ToString(), allvalue.All(x => x?.ToLower() == "false").ToString());
                    break;
                case "anynot":
                    return resultstring.Replace(func.ToString(), allvalue.Any(x => x?.ToLower() == "false").ToString());
                    break;
                case "moretrue":
                    return resultstring.Replace(func.ToString(), (allvalue.Count(x => x?.ToLower() == "true") > allvalue.Count(x => x?.ToLower() == "false")).ToString());
                    break;
                case "morefalse":
                    return resultstring.Replace(func.ToString(), (allvalue.Count(x => x?.ToLower() == "true") < allvalue.Count(x => x?.ToLower() == "false")).ToString());
                    break;

                default:
                    break;
            }
            return resultstring;
        }
        public static string CalculateValue(string op, List<string> allvalue, string resultstring, object func)
        {
            switch (op.ToLower())
            {
                case "sum":
                    double sum = 0;
                    foreach (var item2 in allvalue)
                    {
                        if (item2 != null)
                        {
                            if (item2.Contains("."))
                            {
                                sum += double.Parse(item2, CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                sum += double.Parse(item2);
                            }
                        }
                    }
                    return resultstring.Replace(func.ToString(), sum.ToString());
                    break;
                case "avg":
                    double avg = 0;
                    foreach (var item2 in allvalue)
                    {
                        if (item2 != null)
                        {
                            if (item2.Contains("."))
                            {
                                avg += double.Parse(item2, CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                avg += double.Parse(item2);
                            }
                        }
                    }
                    avg = avg / allvalue.Count();
                    return resultstring.Replace(func.ToString(), avg.ToString());
                    break;
                case "min":
                    double min = double.MaxValue;
                    foreach (var item2 in allvalue)
                    {
                        if (item2 != null)
                        {
                            if (item2.Contains("."))
                            {
                                min = Math.Min(min, double.Parse(item2, CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                min = Math.Min(min, double.Parse(item2));
                            }
                        }
                    }
                    return resultstring.Replace(func.ToString(), min.ToString());
                    break;
                case "max":
                    double max = double.MinValue;
                    foreach (var item2 in allvalue)
                    {
                        if (item2 != null)
                        {
                            if (item2.Contains("."))
                            {
                                max = Math.Max(max, double.Parse(item2, CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                max = Math.Max(max, double.Parse(item2));
                            }
                        }
                    }
                    return resultstring.Replace(func.ToString(), max.ToString());
                    break;
                case "count":
                    return resultstring.Replace(func.ToString(), allvalue.Count().ToString());
                    break;
                case "countdistinct":
                    return resultstring.Replace(func.ToString(), allvalue.Distinct().Count().ToString());
                    break;
                case "countnull":
                    return resultstring.Replace(func.ToString(), allvalue.Count(x => x == null || x == "").ToString());
                    break;
                case "countnotnull":
                    return resultstring.Replace(func.ToString(), allvalue.Count(x => x != null && x != "").ToString());
                    break;
                case "first":
                    return resultstring.Replace(func.ToString(), allvalue.FirstOrDefault());
                    break;
                case "last":
                    return resultstring.Replace(func.ToString(), allvalue.LastOrDefault());
                    break;

                default:
                    break;
            }
            return resultstring;
        }

        public static string CalculateStringValue(string op, List<string> allvalue, string resultstring, object func)
        {
            switch (op.ToLower())
            {

                case "first":
                    return resultstring.Replace(func.ToString(), allvalue.FirstOrDefault());
                    break;
                case "last":
                    return resultstring.Replace(func.ToString(), allvalue.LastOrDefault());
                    break;
                case "concat":
                    return resultstring.Replace(func.ToString(), string.Join(",", allvalue));
                    break;
                case "minlenght":
                    return resultstring.Replace(func.ToString(), allvalue.Min(x => x.Length).ToString());

                    break;
                case "maxlenght":
                    return resultstring.Replace(func.ToString(), allvalue.Max(x => x.Length).ToString());
                    break;
                case "sumlenght":
                    return resultstring.Replace(func.ToString(), allvalue.Sum(x => x.Length).ToString());
                    break;
                case "avglength":
                    return resultstring.Replace(func.ToString(), allvalue.Average(x => x.Length).ToString());
                    break;


                default:
                    break;
            }
            return resultstring;
        }

        public static string CalculateBoolValue(string op, List<string> allvalue, string resultstring, object func)
        {
            switch (op.ToLower())
            {
                case "all":
                    return resultstring.Replace(func.ToString(), allvalue.All(x => x == "true").ToString());
                    break;
                case "any":
                    return resultstring.Replace(func.ToString(), allvalue.Any(x => x == "true").ToString());
                    break;
                case "allnot":
                    return resultstring.Replace(func.ToString(), allvalue.All(x => x == "false").ToString());
                    break;
                case "anynot":
                    return resultstring.Replace(func.ToString(), allvalue.Any(x => x == "false").ToString());
                    break;
                case "moretrue":
                    return resultstring.Replace(func.ToString(), (allvalue.Count(x => x == "true") > allvalue.Count(x => x == "false")).ToString());
                    break;
                case "morefalse":
                    return resultstring.Replace(func.ToString(), (allvalue.Count(x => x == "true") < allvalue.Count(x => x == "false")).ToString());
                    break;
                case "first":
                    return resultstring.Replace(func.ToString(), allvalue.FirstOrDefault());
                    break;
                case "last":
                    return resultstring.Replace(func.ToString(), allvalue.LastOrDefault());
                    break;




                default:
                    break;
            }
            return resultstring;
        }

    }
}

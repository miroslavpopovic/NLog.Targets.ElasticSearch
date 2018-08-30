﻿using System;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using Newtonsoft.Json;

namespace NLog.Targets.ElasticSearch
{
    internal static class StringExtensions
    {
        public static object ToSystemType(this string field, Type type, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            switch (type.FullName)
            {
                case "System.Boolean":
                    return Convert.ToBoolean(field, formatProvider);
                case "System.Double":
                    return Convert.ToDouble(field, formatProvider);
                case "System.DateTime":
                    return Convert.ToDateTime(field, formatProvider);
                case "System.Int32":
                    return Convert.ToInt32(field, formatProvider);
                case "System.Int64":
                    return Convert.ToInt64(field, formatProvider);
                case "System.Object":
                    return JsonConvert.DeserializeObject<ExpandoObject>(field)
                                      .ReplaceDotInKeys();
                default:
                    return field;
            }
        }

        public static string GetConnectionString(this string name)
        {
            var value = name.GetEnvironmentVariable();
            if (!string.IsNullOrEmpty(value))
                return value;

            var connectionString = ConfigurationManager.ConnectionStrings[name];
            return connectionString?.ConnectionString;
        }

        private static string GetEnvironmentVariable(this string name)
        {
            return string.IsNullOrEmpty(name) ? null : Environment.GetEnvironmentVariable(name);
        }
    }
}
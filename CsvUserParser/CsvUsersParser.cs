using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvUserParser.Models;

namespace CsvUserParser
{
    public class CsvUsersParser
    {
        private readonly ClassMap<UserFileModel> _map;
        private readonly CsvConfiguration _configuration;

        public CsvUsersParser(Dictionary<string, string> columnMaps)
        {
            _map = new ConfigurableClassMap<UserFileModel>(columnMaps);
            _configuration = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        }

        public List<ReadUserModel> Parse(string filePath, FilesImportStatistic importStatistic)
        {
            using (var csv = new CsvReader(new StreamReader(filePath), _configuration))
            {
                csv.Context.RegisterClassMap(_map);

                var users = new List<ReadUserModel>();
                csv.Read();
                csv.ReadHeader();

                csv.ValidateHeader(typeof(UserFileModel));

                while (csv.Read())
                {
                    string currentCsvRow = csv.Context.Parser.RawRecord;
                    try
                    {
                        UserFileModel user = csv.GetRecord<UserFileModel>();
                        users.Add(new ReadUserModel(user, currentCsvRow));

                        importStatistic.TotalCount++;
                    }
                    catch (Exception ex)
                    {
                        string message = $"Can't parse user. Line: '{currentCsvRow}.' {ex.Message}";
                        importStatistic.Errors.Add(message);
                        //LogHolder.MainLog.Error(ex, message);
                    }
                }

                return users;
            }
        }

        public class ConfigurableClassMap<T> : ClassMap<T>
        {
            public ConfigurableClassMap(Dictionary<string, string> overriddenPropertyMaps)
            {
                var modelType = typeof(T);
                var properties = modelType.GetProperties();

                if (IsInvalidPropertyMaps(properties, overriddenPropertyMaps, out string invalidPropertyName))
                {
                    throw new Exception($"Invalid property map. Entity '{modelType.Name}' doesn't have a property with name - '{invalidPropertyName}'");
                }

                foreach (var property in properties)
                {
                    var map = Map(modelType, property);
                    if (overriddenPropertyMaps.TryGetValue(property.Name, out string headerName))
                    {
                        map.Name(headerName);
                    }
                    else
                    {
                        map.Name(property.Name);
                    }

                    object[] attrs = property.GetCustomAttributes(typeof(RequiredAttribute), false);
                    if (attrs.Length == 0)
                    {
                        map.Optional();
                    }
                }
            }

            private bool IsInvalidPropertyMaps(
                PropertyInfo[] properties,
                Dictionary<string, string> overriddenPropertyMaps,
                out string invalidPropertyName)
            {
                foreach (var propertyName in overriddenPropertyMaps.Keys)
                {
                    if (!properties.Any(p => p.Name == propertyName))
                    {
                        invalidPropertyName = propertyName;
                        return true;
                    }
                }
                invalidPropertyName = string.Empty;
                return false;
            }
        }
    }
}

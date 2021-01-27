using CsvHelper;
using CsvHelper.Configuration;
using CsvUserParser.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CsvUserParser
{
    public class CsvUsersParser
    {
        private readonly ClassMap<UserFileModel> _map;
        private readonly CsvConfiguration _configuration;

        public CsvUsersParser(Dictionary<string, string> columnMaps)
        {
            _map = new ReadUserModelMap(columnMaps);
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
        public class ReadUserModelMap : ClassMap<UserFileModel>
        {
            private readonly string[] _requiredFields = new string[] {
                nameof(UserFileModel.ForeignID),
                nameof(UserFileModel.Email)
            };

            public ReadUserModelMap(Dictionary<string, string> columnMaps)
            {
                var modelType = typeof(UserFileModel);
                var properties = modelType.GetProperties();

                if(IsInvalidColumnMaps(properties, columnMaps, out string invalidPropertyName))
                {
                    throw new Exception($"Invalid column map. Entity '{modelType.Name}' doesn't have a property with name - '{invalidPropertyName}'");
                }

                foreach (var property in properties)
                {
                    var map = Map(modelType, property);
                    if (columnMaps.TryGetValue(property.Name, out string headerName))
                    {
                        map.Name(headerName);
                    }
                    else
                    {
                        map.Name(property.Name);
                    }

                    if (!_requiredFields.Contains(property.Name))
                    {
                        map.Optional();
                    }
                }
            }

            private bool IsInvalidColumnMaps(
                PropertyInfo[] properties, 
                Dictionary<string, string> columnMaps, 
                out string invalidPropertyName)
            {
                foreach (var propertyName in columnMaps.Keys)
                {
                    if(!properties.Any(p => p.Name == propertyName))
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

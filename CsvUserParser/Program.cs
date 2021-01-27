using CsvUserParser.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;

namespace CsvUserParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var options = config.GetSection("ParcerOptions").Get<ParcerOptions>();

            string filePath = "DevTestFakeData.csv";
            var fileImportStat = new FilesImportStatistic(filePath);

            var parser = new CsvUsersParser(options.ColumnMaps);
            var users = parser.Parse(filePath, fileImportStat);

            PrintJson(users);
            PrintJson(fileImportStat);
        }

        static void PrintJson(object o)
        {
            Console.WriteLine(JsonConvert.SerializeObject(o, Formatting.Indented));
        }
    }
}

using System;
using System.Collections.Generic;

namespace CsvUserParser.Models
{
    public class FilesImportStatistic
    {
        public string FilePath { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan ElapsedTime => EndTime.HasValue ? EndTime.Value - StartTime : TimeSpan.Zero;

        public int TotalCount { get; set; }
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int UnchangedCount { get; set; }
        public int ValidationErrorsCount { get; set; }
        public int ServerErrorsCount { get; set; }

        public List<string> Errors { get; set; }

        public FilesImportStatistic(string path)
        {
            StartTime = DateTime.Now;
            FilePath = path;
            Errors = new List<string>();
        }
    }
}

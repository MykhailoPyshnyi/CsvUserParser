using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CsvUserParser.Models
{
    public class UserFileModel
    {
        [Required]
        public string ForeignID { get; set; }
        [Required]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
    }

    public class ReadUserModel
    {
        public UserFileModel User { get; set; }
        public string CsvRow { get; set; }
        public string RowHash { get; set; }

        public ReadUserModel(UserFileModel user, string csvRow)
        {
            User = user;
            CsvRow = csvRow;
            RowHash = JsonConvert.SerializeObject(user);
        }

        public SaveUserImportArgs MapToSaveUserImportArgs()
        {
            return new SaveUserImportArgs
            {
                ForeignID = User.ForeignID,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Email = User.Email,
                Position = User.Position
            };
        }
    }
}

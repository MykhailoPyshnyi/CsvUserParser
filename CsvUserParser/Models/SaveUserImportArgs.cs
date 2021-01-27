namespace CsvUserParser.Models
{
    public class SaveUserImportArgs
    {
        public string ForeignID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string Biography { get; set; }
        public string AssistantName { get; set; }
        public string AssistantPhone { get; set; }
        public string AssistantEmail { get; set; }
        public string IconUrl { get; set; }
    }
}

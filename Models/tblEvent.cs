using System.ComponentModel.DataAnnotations;

namespace automationTest.Models
{
    public class tblEvent
    {
        [Key]
        public int EventId { get; set; }
        public string? Mail_Number { get; set; }
        public string? Subject { get; set; }
    }
}

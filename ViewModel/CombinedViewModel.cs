using automationTest.Models;
using System.ComponentModel.DataAnnotations;
namespace automationTest.ViewModel
{
    public class CombinedViewModel
    {
        public List<tblElasticData>? ElasticData { get; set; }
        public List<tblEvent>? EventData { get; set; }
        public List<CombinedModel>? CombinedModel { get; set; }

    }
    //public class CombinedEventData
    //{
    //    [Key]
    //    public int ElasticDataId { get; set; }
    //    public string? To { get; set; }
    //    public string? From { get; set; }
    //    public string? EventType { get; set; }
    //    public DateTime EventDate { get; set; }
    //    public string? Channel { get; set; }
    //    public string? Subject { get; set; }
    //    public string? MessageCategory { get; set; }
    //    public int Quantity { get; set; }
    //    public string? Mail_Number { get; set; }
    //}

}

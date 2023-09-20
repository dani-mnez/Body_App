using MongoDB.Bson;

namespace Web_BodyApp.Data.DTOs
{
    public class HistoricalDataDTO
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string? NutritionalData { get; set; }
        public string? PhysicalData { get; set; }
    }
}

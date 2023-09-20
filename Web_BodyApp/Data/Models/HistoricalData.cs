using Newtonsoft.Json;
using Web_BodyApp.Data.DTOs;
using Web_BodyApp.Data.Models.NutritionData;
using Web_BodyApp.Data.Models.PhysicData;

namespace Web_BodyApp.Data.Models
{
    public class HistoricalData
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public PhysicalData? PhysicalData { get; set; }
        public NutritionalData? NutritionalData { get; set; }

        public HistoricalData GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<HistoricalData>(json);
        }

        public HistoricalDataDTO ToDTO()
        {
            return new HistoricalDataDTO()
            {
                Id = this.Id,
                Date = this.Date,
                PhysicalData = this.PhysicalData?.Id,
                NutritionalData = this.NutritionalData?.Id
            };
        }
    }
}

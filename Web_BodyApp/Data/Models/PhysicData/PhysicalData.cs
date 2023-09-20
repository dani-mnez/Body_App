using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Web_BodyApp.Data.Models.PhysicData
{
    public class PhysicalData
    {
        public string? Id { get; set; }
        public required double Weight { get; set; }
        public BodyMeasure? BodyMeasure { get; set; }
        public required ComputedData Computed { get; set; }

        public PhysicalData GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<PhysicalData>(json);
        }
    }
}

using Newtonsoft.Json;

namespace Web_BodyApp.Data.Models.NutritionData
{
    public class FoodData
    {
        public string? Id { get; set; }
        public string FoodName { get; set; }
        public FoodMacros FoodMacros { get; set; }
        public int FoodCalories { get; set; }

        public FoodData GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<FoodData>(json);
        }
    }
}

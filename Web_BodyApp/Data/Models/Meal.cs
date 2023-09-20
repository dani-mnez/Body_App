using Newtonsoft.Json;
using Web_BodyApp.Data.Models.NutritionData;

namespace Web_BodyApp.Data.Models
{
    public class Meal
    {
        public string? Id { get; set; }

        public string Name { get; set; }

        public int TotalCalories { get; set; } = 0;
        public FoodMacros TotalMealMacros { get; set; }
        public int TotalWeight { get; set; } = 0;

        public Meal GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<Meal>(json);
        }
    }
}

using Newtonsoft.Json;

namespace Web_BodyApp.Data.Models.NutritionData
{
    public class FoodServing
    {
        public double ServingQty { get; set; }
        public int ServingUnit { get; set; }

        public FoodServing GetCopy()
        {
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<FoodServing>(json)!;
        }
    }
}
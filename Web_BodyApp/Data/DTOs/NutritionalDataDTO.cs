using System.Text.Json.Serialization;
using Web_BodyApp.Data.Models.NutritionData;

namespace Web_BodyApp.Data.DTOs
{
    public class NutritionalDataDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; } = null;

        public Dictionary<int, DayTimeIntakesDTO>? DayTimeIntakes { get; set; }

        public int TotalNutDataCalories { get; set; } = 0;
        public required FoodMacros TotalNutDataMacros { get; set; }
    }
}

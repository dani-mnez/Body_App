namespace Web_BodyApp.Data.AssistClasses
{
    public class DeleteStructureData
    {
        // MealIntake
        public bool DeleteMealIntakeList { get; set; } = false;
        public bool DeleteMealIntakeItem { get; set; } = false;

        // FoodIntake
        public bool DeleteFoodIntakeList { get; set; } = false;
        public bool DeleteFoodIntakeItem { get; set; } = false;

        // Intake items
        public int? IntakeIndexToDelete { get; set; } = null;

        // Inner NutritionalData
        public bool DeleteDayTimePair { get; set; } = false;

        // Side actions
        public bool UpdateDayTimeIntakeType { get; set; } = false;
        public bool UpdateDayTimeIntakeCalMacros { get; set; } = false;
    }
}
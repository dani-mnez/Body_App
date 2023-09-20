using API_ASP.NET_Core_Body_App.Models.UserMeal;
using Microsoft.AspNetCore.JsonPatch;
using MongoDB.Driver;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public class MealRepository : IMealRepository
    {
        private readonly IMongoCollection<Meal> _meals;

        public MealRepository(IMongoClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            _meals = database.GetCollection<Meal>("mealsData");
        }

        // Implementación de la interfaz
        // GET
        public async Task<Meal> GetMealByIdAsync(string mealId)
        {
            var filter = Builders<Meal>.Filter.Eq(m => m.Id, mealId);
            return await _meals.Find(filter).FirstOrDefaultAsync();
        }


        // POST
        public async Task AddMealAsync(Meal meal)
        {
            await _meals.InsertOneAsync(meal);
        }

        public async Task<List<Meal>> GetAllListMealByIdAsync(List<string> mealIds)
        {
            var filter = Builders<Meal>.Filter.In(m => m.Id, mealIds);
            return await _meals.Find(filter).ToListAsync();
        }


        // PATCH
        public async Task UpdateMealPropertyByIdAsync(string mealId, JsonPatchDocument<Meal> mealDataPatch)
        {
            var filter = Builders<Meal>.Filter.Eq(m => m.Id, mealId);

            var mealData = await _meals.Find(filter).FirstOrDefaultAsync();
            mealDataPatch.ApplyTo(mealData);

            var updateBuilder = Builders<Meal>.Update;
            var updateDefinition = new List<UpdateDefinition<Meal>>();


            foreach (var operation in mealDataPatch.Operations)
            {
                List<string> propertyComposition = operation.path.Split('/').Skip(1).ToList();
                dynamic? propertyValue = mealData;
                dynamic? nestedProperty = propertyValue;


                for (int i = 0; i < propertyComposition.Count - 1; i++)
                {
                    var property = nestedProperty?.GetType().GetProperty(propertyComposition[i]);
                    if (property != null)
                        nestedProperty = property.GetValue(nestedProperty, null);
                }


                propertyValue = (propertyComposition?.Count > 1)
                    ? nestedProperty?.GetType().GetProperty(propertyComposition.Last())?.GetValue(nestedProperty, null)
                    : typeof(Meal).GetProperty(propertyComposition[0])?.GetValue(mealData, null);


                List<string> lowerCasePropertyComposition = propertyComposition.Select(s => char.ToLowerInvariant(s[0]) + s.Substring(1)).ToList();

                if (propertyValue != null &&
                    (operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Add ||
                    operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Replace))
                {
                    updateDefinition.Add(
                        updateBuilder.Set(
                            string.Join(".", lowerCasePropertyComposition),
                            propertyValue
                        )
                    );

                }
                else if (operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Remove)
                {
                    updateDefinition.Add(
                        updateBuilder.Unset(string.Join(".", lowerCasePropertyComposition))
                    );
                }
            }

            await _meals.UpdateOneAsync(
                filter,
                updateBuilder.Combine(updateDefinition)
            );
        }


        // DELETE
        public async Task DeleteMealByIdAsync(string mealId)
        {
            var filter = Builders<Meal>.Filter.Eq(m => m.Id, mealId);
            await _meals.DeleteOneAsync(filter);
        }
    }
}

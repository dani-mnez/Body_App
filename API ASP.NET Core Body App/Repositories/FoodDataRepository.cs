using API_ASP.NET_Core_Body_App.Models.FoodData;
using API_ASP.NET_Core_Body_App.Models.NutritionData;
using Microsoft.AspNetCore.JsonPatch;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public class FoodDataRepository : IFoodDataRepository
    {
        private readonly IMongoCollection<FoodData> _foodDataCollection;

        public FoodDataRepository(IMongoClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            _foodDataCollection = database.GetCollection<FoodData>("foodData");
        }

        // Implementación de la interfaz
        // GET
        public async Task<FoodData> GetFoodDataByIdAsync(string id)
        {
            var filter = Builders<FoodData>.Filter.Eq(fd => fd.Id, id);
            return await _foodDataCollection.Find(filter).FirstOrDefaultAsync();
        }



        // PATCH
        public async Task UpdateGeneralAttributesAsync(string foodDataId, JsonPatchDocument<FoodData> foodDataPatch)
        {
            var filter = Builders<FoodData>.Filter.Eq(n => n.Id, foodDataId);

            var foodData = await _foodDataCollection.Find(filter).FirstOrDefaultAsync();
            foodDataPatch.ApplyTo(foodData);

            var updateBuilder = Builders<FoodData>.Update;
            var updateDefinition = new List<UpdateDefinition<FoodData>>();


            foreach (var operation in foodDataPatch.Operations)
            {
                List<string> propertyComposition = operation.path.Split('/').Skip(1).ToList();
                object propertyValue = foodData;
                object nestedProperty = propertyValue;

                for (int i = 0; i < propertyComposition.Count - 1; i++)
                {
                    if (nestedProperty is Dictionary<int, DayTimeIntakes> dict)
                    {
                        if (int.TryParse(propertyComposition[i], out int key) && dict.ContainsKey(key))
                            nestedProperty = dict[key];
                    }
                    else if (nestedProperty is List<MealIntake> list)
                    {
                        if (int.TryParse(propertyComposition[i], out int index) && index >= 0 && index < list.Count)
                            nestedProperty = list[index];
                    }
                    else
                    {
                        var property = nestedProperty.GetType().GetProperty(propertyComposition[i]);
                        if (property != null)
                            nestedProperty = property.GetValue(nestedProperty, null);
                    }
                }

                propertyValue = (propertyComposition.Count > 1)
                    ? nestedProperty.GetType().GetProperty(propertyComposition.Last())?.GetValue(nestedProperty, null)
                    : typeof(FoodData).GetProperty(propertyComposition[0])?.GetValue(foodData, null);

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

            await _foodDataCollection.UpdateOneAsync(
                filter,
                updateBuilder.Combine(updateDefinition)
            );
        }



        // POST
        public async Task AddFoodDataAsync(FoodData foodData)
        {
            await _foodDataCollection.InsertOneAsync(foodData);
        }

        public async Task<List<string>> AddMultipleFoodDataAsync(List<FoodData> foodDataList)
        {
            List<string> foodDataIds = new();
            foreach (var foodData in foodDataList)
            {
                foodData.Id = ObjectId.GenerateNewId().ToString();
                foodDataIds.Add(foodData.Id);
            }

            await _foodDataCollection.InsertManyAsync(foodDataList);
            return foodDataIds;
        }

        public async Task<List<FoodData>> GetAllListFoodDataByIdAsync(List<string> foodDataIds)
        {
            var filter = Builders<FoodData>.Filter.In(fd => fd.Id, foodDataIds);
            return await _foodDataCollection.Find(filter).ToListAsync();
        }



        // PUT
        public async Task UpdateFoodDataPropertyByIdAsync(FoodData foodData)
        {
            var filter = Builders<FoodData>.Filter.Eq(fd => fd.Id, foodData.Id);

            var updateDefinitionBuilder = Builders<FoodData>.Update;
            List<UpdateDefinition<FoodData>> updates = new();

            foreach (var property in foodData.GetType().GetProperties())
            {
                if (property.Name == "Id") continue;

                var fieldValue = property.GetValue(foodData);
                if (fieldValue != null) updates.Add(updateDefinitionBuilder.Set(property.Name, fieldValue));
            }

            if (updates.Count > 0)
            {
                var combinedUpdates = updateDefinitionBuilder.Combine(updates);
                await _foodDataCollection.UpdateOneAsync(filter, combinedUpdates);
            }
        }



        // DELETE
        public async Task DeleteFoodDataByIdAsync(string id)
        {
            var filter = Builders<FoodData>.Filter.Eq(fd => fd.Id, id);
            await _foodDataCollection.DeleteOneAsync(filter);
        }
    }
}

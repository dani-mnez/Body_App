using API_ASP.NET_Core_Body_App.Models.NutritionData;
using Microsoft.AspNetCore.JsonPatch;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public class NutritionalDataRepository : INutritionalDataRepository
    {

        private readonly IMongoCollection<NutritionalData> _foodDataRepository;

        public NutritionalDataRepository(IMongoClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            _foodDataRepository = database.GetCollection<NutritionalData>("nutritionalData");
        }



        // GET
        public async Task<NutritionalData> GetNutritionDataByIdAsync(string id)
        {
            var filter = Builders<NutritionalData>.Filter.Eq(nd => nd.Id, id);
            return await _foodDataRepository.Find(filter).FirstOrDefaultAsync();
        }


        // POST
        public async Task AddNutritionDataAsync(NutritionalData nutritionalData)
        {
            await _foodDataRepository.InsertOneAsync(nutritionalData);
        }

        public async Task<List<string>> AddMultipleNutritionalDataAsync(List<NutritionalData> nutritionalDataList)
        {
            List<string> nutDataIds = new();
            foreach (var nutritionalData in nutritionalDataList)
            {
                nutritionalData.Id = ObjectId.GenerateNewId().ToString();
                nutDataIds.Add(nutritionalData.Id);
            }

            await _foodDataRepository.InsertManyAsync(nutritionalDataList);
            return nutDataIds;
        }

        public async Task<List<NutritionalData>> GetAllListNutritionDataByIdAsync(List<string> nutritionDataIds)
        {
            var filter = Builders<NutritionalData>.Filter.In(nd => nd.Id, nutritionDataIds);
            return await _foodDataRepository.Find(filter).ToListAsync();
        }

        public async Task DeleteMultipleNutritionalDataByIdListAsync(List<string> idList)
        {
            var filter = Builders<NutritionalData>.Filter.In(nd => nd.Id, idList);
            await _foodDataRepository.DeleteManyAsync(filter);
        }


        // PATCH
        public async Task UpdateGeneralAttributesAsync(string nutritionalDataId, JsonPatchDocument<NutritionalData> nutDataPatch)
        {
            var filter = Builders<NutritionalData>.Filter.Eq(n => n.Id, nutritionalDataId);

            var nutritionalData = await _foodDataRepository.Find(filter).FirstOrDefaultAsync();
            NutritionalData? originalNutData = nutritionalData.GetCopy();
            nutDataPatch.ApplyTo(nutritionalData);

            var updateBuilder = Builders<NutritionalData>.Update;
            var updateDefinition = new List<UpdateDefinition<NutritionalData>>();


            foreach (var operation in nutDataPatch.Operations)
            {
                List<string> propertyComposition = operation.path.Split('/').Skip(1).ToList();
                // Si es object, en la bdd se serializa mal, debe ser dynamic para poder cambiar de tipo
                // y que lo reconozca el serializador del driver de mongoDB
                dynamic? propertyValue = nutritionalData;
                dynamic? nestedProperty = propertyValue;

                for (int i = 0; i < propertyComposition.Count - 1; i++)
                {
                    if (nestedProperty is Dictionary<string, DayTimeIntakes> dict)
                    {
                        if (int.TryParse(propertyComposition[i], out int key) && dict.ContainsKey($"{key}"))
                            nestedProperty = dict[$"{key}"];
                    }
                    else if (nestedProperty is List<MealIntake> list)
                    {
                        if (int.TryParse(propertyComposition[i], out int index) && index >= 0 && index < list.Count)
                            nestedProperty = list[index];
                    }
                    else if (nestedProperty is List<string> lista)
                    {
                        if (int.TryParse(propertyComposition[i], out int index) && index >= 0 && index < lista.Count)
                            nestedProperty = lista[index];
                    }
                    else
                    {
                        var property = nestedProperty?.GetType().GetProperty(propertyComposition[i]);
                        if (property != null)
                            nestedProperty = property.GetValue(nestedProperty, null);
                    }
                }

                propertyValue = (propertyComposition?.Count > 1)
                    ? nestedProperty?.GetType().GetProperty(propertyComposition.Last())?.GetValue(nestedProperty, null)
                    : typeof(NutritionalData).GetProperty(propertyComposition[0])?.GetValue(nutritionalData, null);


                if (propertyComposition.Count == 2 && propertyComposition[0] == "DayTimeIntakes")
                {
                    if (nestedProperty is Dictionary<string, DayTimeIntakes> dictionary)
                    {
                        if (dictionary.TryGetValue(propertyComposition.Last(), out var dayTimeIntake))
                        {
                            propertyValue = dayTimeIntake;
                        }
                    }
                }


                List<string> lowerCasePropertyComposition = propertyComposition.Select(s => char.ToLowerInvariant(s[0]) + s.Substring(1)).ToList();

                if (propertyValue != null &&
                    (operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Add ||
                    operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Replace))
                {
                    if (propertyComposition.Last() == "FoodIntake")
                    {
                        List<ObjectId>? objIdList = new();
                        foreach (string str in (propertyValue as List<string>)!)
                        {
                            objIdList.Add(ObjectId.Parse(str));
                        }
                        propertyValue = (List<string>?)propertyValue;
                    }
                    else if (propertyComposition.Last() == "MealIntake")
                    {
                        List<MealIntake> mIntakeList = new();
                        foreach (MealIntake mi in (propertyValue as List<MealIntake>)!)
                        {
                            mIntakeList.Add(mi);
                        }
                        propertyValue = mIntakeList;
                    }
                    updateDefinition.Add(
                        updateBuilder.Set(
                            string.Join(".", lowerCasePropertyComposition),
                            propertyValue
                        )
                    );
                }
                else if (operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Remove)
                {
                    if (propertyComposition.Last() == "FoodIntake" || propertyComposition.Last() == "MealIntake")
                    {
                        if (propertyComposition.Last() == "FoodIntake")
                        {
                            //string mealId = originalNutData.DayTimeIntakes[]

                            //CustomMeals.ToArray()[int.Parse(propertyComposition.Last())];
                            //updateDefinition.Add(updateBuilder.Pull(lowerCasePropertyComposition[0], ObjectId.Parse((string)mealId)));
                        }
                        else if (propertyComposition.Last() == "MealIntake")
                        {

                        }

                        //var pullFilter = Builders<User>.Update.PullFilter(
                        //    lowerCasePropertyComposition[0],
                        //    Builders<BsonValue>.Filter.Eq("_id", propertyValue)
                        //);
                        //updateDefinition.Add(pullFilter);
                    }
                    else
                    {
                        updateDefinition.Add(
                            updateBuilder.Unset(string.Join(".", lowerCasePropertyComposition))
                        );
                    }
                }
            }

            await _foodDataRepository.UpdateOneAsync(
                filter,
                updateBuilder.Combine(updateDefinition)
            );
        }


        public async Task UpdateMultipleDocumentsGeneralAttributesAsync(Dictionary<string, JsonPatchDocument<NutritionalData>> nutritionalDataPatches)
        {
            var bulkOps = new List<WriteModel<NutritionalData>>();
            var updateBuilder = Builders<NutritionalData>.Update;

            foreach (var kvp in nutritionalDataPatches)
            {
                var nutritionalDataId = kvp.Key;
                var nutDataPatch = kvp.Value;

                var filter = Builders<NutritionalData>.Filter.Eq(n => n.Id, nutritionalDataId);
                var nutritionalData = await _foodDataRepository.Find(filter).FirstOrDefaultAsync();

                nutDataPatch.ApplyTo(nutritionalData);

                var updateDefinition = new List<UpdateDefinition<NutritionalData>>();
                foreach (var operation in nutDataPatch.Operations)
                {
                    List<string> propertyComposition = operation.path.Split('/').Skip(1).ToList();
                    dynamic? propertyValue = nutritionalData;
                    dynamic? nestedProperty = propertyValue;

                    for (int i = 0; i < propertyComposition.Count - 1; i++)
                    {
                        if (nestedProperty is Dictionary<string, DayTimeIntakes> dict)
                        {
                            if (int.TryParse(propertyComposition[i], out int key) && dict.ContainsKey($"{key}"))
                                nestedProperty = dict[$"{key}"];
                        }
                        else if (nestedProperty is List<MealIntake> list)
                        {
                            if (int.TryParse(propertyComposition[i], out int index) && index >= 0 && index < list.Count)
                                nestedProperty = list[index];
                        }
                        else
                        {
                            var property = nestedProperty?.GetType().GetProperty(propertyComposition[i]);
                            if (property != null)
                                nestedProperty = property.GetValue(nestedProperty, null);
                        }
                    }

                    propertyValue = (propertyComposition.Count > 1)
                        ? nestedProperty?.GetType().GetProperty(propertyComposition.Last())?.GetValue(nestedProperty, null)
                        : typeof(NutritionalData).GetProperty(propertyComposition[0])?.GetValue(nutritionalData, null);

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

                var update = updateBuilder.Combine(updateDefinition);
                var updateOneModel = new UpdateOneModel<NutritionalData>(filter, update) { IsUpsert = false };

                bulkOps.Add(updateOneModel);
            }

            await _foodDataRepository.BulkWriteAsync(bulkOps);
        }



        // PUT
        public async Task UpdateNutritionDataAsync(NutritionalData nutritionDataToUpdate)
        {
            var filter = Builders<NutritionalData>.Filter.Eq(hd => hd.Id, nutritionDataToUpdate.Id);

            // Generamos el objeto update de manera programática
            var updateDefinitionBuilder = Builders<NutritionalData>.Update;
            List<UpdateDefinition<NutritionalData>> updates = new();

            foreach (var property in nutritionDataToUpdate.GetType().GetProperties())
            {
                if (property.Name == "Id") continue;

                var fieldValue = property.GetValue(nutritionDataToUpdate);
                if (fieldValue != null) updates.Add(updateDefinitionBuilder.Set(property.Name, fieldValue));
            }

            if (updates.Count > 0)
            {
                var combinedUpdates = updateDefinitionBuilder.Combine(updates); // Se combinan todas las updates para crear una final
                await _foodDataRepository.UpdateOneAsync(filter, combinedUpdates);
            }
        }


        // DELETE
        public async Task DeleteNutritionDataByIdAsync(string id)
        {
            var filter = Builders<NutritionalData>.Filter.Eq(hd => hd.Id, id);
            await _foodDataRepository.DeleteOneAsync(filter);
        }
    }
}

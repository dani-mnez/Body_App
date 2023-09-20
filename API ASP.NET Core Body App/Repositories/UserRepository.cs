using API_ASP.NET_Core_Body_App.Models;
using Microsoft.AspNetCore.JsonPatch;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            _users = database.GetCollection<User>("users");
        }

        // Implementación de la interfaz
        // GET
        public async Task<User> GetUserByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }


        // POST
        public async Task<User> GetUserByCredentialsAsync(string mail, string password)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Mail, mail),
                Builders<User>.Filter.Eq(u => u.Password, password)
            );
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }


        // PATCH
        public async Task UpdateGeneralAttributesAsync(string userId, JsonPatchDocument<User> userDataPatch)
        {
            var filter = Builders<User>.Filter.Eq(n => n.Id, userId);

            var userData = await _users.Find(filter).FirstOrDefaultAsync();
            User? originalUserData = userData?.GetCopy();
            userDataPatch.ApplyTo(userData);

            var updateBuilder = Builders<User>.Update;
            var updateDefinition = new List<UpdateDefinition<User>>();


            foreach (var operation in userDataPatch.Operations)
            {
                List<string> propertyComposition = operation.path.Split('/').Skip(1).ToList();
                dynamic? propertyValue = userData;
                dynamic? nestedProperty = propertyValue;
                bool isList = false;

                for (int i = 0; i < propertyComposition.Count - 1; i++)
                {
                    if (nestedProperty is List<string> lista)
                    {
                        if (int.TryParse(propertyComposition[i], out int idx) && idx >= 0 && idx < lista.Count)
                            nestedProperty = lista[idx];
                    }
                    else
                    {
                        var property = nestedProperty?.GetType().GetProperty(propertyComposition[i]);
                        if (property != null)
                            nestedProperty = property.GetValue(nestedProperty, null);
                    }
                }

                if (nestedProperty is List<string> list && int.TryParse(propertyComposition.Last(), out int index) && index >= 0 && index < list.Count)
                {
                    propertyValue = list[index];
                    isList = true;
                }
                else
                {
                    propertyValue = (propertyComposition?.Count > 1)
                        ? nestedProperty?.GetType().GetProperty(propertyComposition.Last())?.GetValue(nestedProperty, null)
                        : typeof(User).GetProperty(propertyComposition[0])?.GetValue(userData, null);
                }

                List<string> lowerCasePropertyComposition = propertyComposition.Select(s => char.ToLowerInvariant(s[0]) + s.Substring(1)).ToList();

                if (propertyValue != null &&
                    (operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Add ||
                    operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Replace))
                {
                    if (isList)
                    {
                        updateDefinition.Add(
                            updateBuilder.Push(
                                lowerCasePropertyComposition[0],
                                (lowerCasePropertyComposition[0] == "historicalData" || lowerCasePropertyComposition[0] == "customMeals")
                                ? ObjectId.Parse((string)propertyValue)
                                : propertyValue
                            )
                        );
                    }
                    else
                    {
                        updateDefinition.Add(
                            updateBuilder.Set(
                                string.Join(".", lowerCasePropertyComposition),
                                propertyValue
                            )
                        );
                    }

                }
                else if (operation.OperationType == Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Remove)
                {
                    if (propertyComposition.First() == "CustomMeals" || propertyComposition.First() == "HistoricalData")
                    {
                        if (propertyComposition.First() == "CustomMeals")
                        {
                            string mealId = originalUserData.CustomMeals.ToArray()[int.Parse(propertyComposition.Last())];
                            updateDefinition.Add(updateBuilder.Pull(lowerCasePropertyComposition[0], ObjectId.Parse((string)mealId)));
                        }
                        else if (propertyComposition.First() == "HistoricalData")
                        {
                            string histDataId = originalUserData.HistoricalData.ToArray()[int.Parse(propertyComposition[1])];
                            updateDefinition.Add(updateBuilder.Pull(lowerCasePropertyComposition[0], ObjectId.Parse((string)histDataId)));
                        }
                    }
                    else
                    {
                        updateDefinition.Add(updateBuilder.Unset(string.Join(".", lowerCasePropertyComposition)));
                    }
                }
            }

            await _users.UpdateOneAsync(
                filter,
                updateBuilder.Combine(updateDefinition)
            );
        }


        // DELETE
        public async Task DeleteUserByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _users.DeleteOneAsync(filter);
        }

        public async Task DeleteFromHistoricalDataArrayByIdAsync(string userId, string historicalDataId)
        {
            // OJO No se si funcionaría bien
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var pull = Builders<User>.Update.Pull(u => u.HistoricalData, historicalDataId);
            await _users.UpdateOneAsync(filter, pull);

        }

        public async Task DeleteFromMealArrayByIdAsync(string userId, string mealId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);

            var pull = Builders<User>.Update.Pull(u => u.CustomMeals, mealId);
            await _users.UpdateOneAsync(filter, pull);
        }
    }
}

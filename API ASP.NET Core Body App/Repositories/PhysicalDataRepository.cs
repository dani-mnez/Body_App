using API_ASP.NET_Core_Body_App.Models;
using Microsoft.AspNetCore.JsonPatch;
using MongoDB.Driver;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public class PhysicalDataRepository : IPhysicalDataRepository
    {
        private readonly IMongoCollection<PhysicalData> _physicalData;

        public PhysicalDataRepository(IMongoClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            _physicalData = database.GetCollection<PhysicalData>("physicalData");
        }

        // Implementación de la interfaz
        // GET
        public async Task<PhysicalData> GetPhysicalDataByIdAsync(string id)
        {
            var filter = Builders<PhysicalData>.Filter.Eq(pd => pd.Id, id);
            return await _physicalData.Find(filter).FirstOrDefaultAsync();
        }


        // POST
        public async Task AddPhysicalDataAsync(PhysicalData data)
        {
            await _physicalData.InsertOneAsync(data);
        }

        public async Task<List<PhysicalData>> GetAllPhysicalDataAsync(List<string> physicalDataIDs)
        {
            var filter = Builders<PhysicalData>.Filter.In(pd => pd.Id, physicalDataIDs);
            return await _physicalData.Find(filter).ToListAsync();
        }


        // PATCH
        public async Task UpdatePhysicalDataByIdAsync(string physDataId, JsonPatchDocument<PhysicalData> physDataPatch)
        {
            var filter = Builders<PhysicalData>.Filter.Eq(n => n.Id, physDataId);

            var physicalData = await _physicalData.Find(filter).FirstOrDefaultAsync();
            PhysicalData? originalNutData = physicalData.GetCopy();
            physDataPatch.ApplyTo(physicalData);

            var updateBuilder = Builders<PhysicalData>.Update;
            var updateDefinition = new List<UpdateDefinition<PhysicalData>>();


            foreach (var operation in physDataPatch.Operations)
            {
                List<string> propertyComposition = operation.path.Split('/').Skip(1).ToList();
                dynamic? propertyValue = physicalData;
                dynamic? nestedProperty = propertyValue;

                for (int i = 0; i < propertyComposition.Count - 1; i++)
                {

                    var property = nestedProperty?.GetType().GetProperty(propertyComposition[i]);
                    if (property != null)
                        nestedProperty = property.GetValue(nestedProperty, null);
                }

                propertyValue = (propertyComposition?.Count > 1)
                    ? nestedProperty?.GetType().GetProperty(propertyComposition.Last())?.GetValue(nestedProperty, null)
                    : typeof(PhysicalData).GetProperty(propertyComposition[0])?.GetValue(physicalData, null);


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

            await _physicalData.UpdateOneAsync(
                filter,
                updateBuilder.Combine(updateDefinition)
            );
        }


        // DELETE
        public async Task DeletePhysicalDataByIdAsync(string id)
        {
            var filter = Builders<PhysicalData>.Filter.Eq(pd => pd.Id, id);
            await _physicalData.DeleteOneAsync(filter);
        }
    }
}

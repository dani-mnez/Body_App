using API_ASP.NET_Core_Body_App.Models.HistoricData;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public class HistoricalDataRepository : IHistoricalDataRepository
    {
        private readonly IMongoCollection<HistoricalData> _historicalData;

        public HistoricalDataRepository(IMongoClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            _historicalData = database.GetCollection<HistoricalData>("historicalData");
        }



        // GET
        public async Task<HistoricalData> GetHistoricalDataByIdAsync(string id)
        {
            var filter = Builders<HistoricalData>.Filter.Eq(hd => hd.Id, id);
            return await _historicalData.Find(filter).FirstOrDefaultAsync();
        }



        // POST
        public async Task AddHistoricalDataAsync(HistoricalData historicalData)
        {
            await _historicalData.InsertOneAsync(historicalData);
        }

        public async Task<List<HistoricalData>> GetAllListHistoricalDataByIdAsync(List<string> historicalDataIds)
        {
            var filter = Builders<HistoricalData>.Filter.In(hd => hd.Id, historicalDataIds);
            return await _historicalData.Find(filter).ToListAsync();
        }

        public async Task DeleteMultipleHistoricalDataByIdListAsync(List<string> idList)
        {
            var filter = Builders<HistoricalData>.Filter.In(hd => hd.Id, idList);
            await _historicalData.DeleteManyAsync(filter);
        }



        // PATCH
        public async Task UpdateSingleDocAttributesAsync(string histDataId, JsonPatchDocument<HistoricalData> histDataPatch)
        {
            var filter = Builders<HistoricalData>.Filter.Eq(n => n.Id, histDataId);

            var historicalData = await _historicalData.Find(filter).FirstOrDefaultAsync();
            histDataPatch.ApplyTo(historicalData);

            var updateBuilder = Builders<HistoricalData>.Update;

            var updateDefinition = GenerateHistDataUpdateDefinitions(updateBuilder, historicalData, histDataPatch.Operations);

            await _historicalData.UpdateOneAsync(
                filter,
                updateBuilder.Combine(updateDefinition)
            );
        }

        public async Task UpdateMultipleDocsAttributesAsync(Dictionary<string, JsonPatchDocument<HistoricalData>> histDataPatches)
        {
            var bulkOps = new List<WriteModel<HistoricalData>>();
            var updateBuilder = Builders<HistoricalData>.Update;

            foreach (var kvp in histDataPatches)
            {
                var historicalDataId = kvp.Key;
                var histDataPatch = kvp.Value;

                var filter = Builders<HistoricalData>.Filter.Eq(n => n.Id, historicalDataId);
                var historicalData = await _historicalData.Find(filter).FirstOrDefaultAsync();
                histDataPatch.ApplyTo(historicalData);

                var updateDefinition = GenerateHistDataUpdateDefinitions(updateBuilder, historicalData, histDataPatch.Operations);

                var update = updateBuilder.Combine(updateDefinition);
                var updateOneModel = new UpdateOneModel<HistoricalData>(filter, update) { IsUpsert = false };

                bulkOps.Add(updateOneModel);
            }

            await _historicalData.BulkWriteAsync(bulkOps);
        }

        private List<UpdateDefinition<HistoricalData>> GenerateHistDataUpdateDefinitions(
            UpdateDefinitionBuilder<HistoricalData> updateBuilder,
            HistoricalData historicalData,
            List<Operation<HistoricalData>> operationList
        )
        {
            List<UpdateDefinition<HistoricalData>> updateDefinition = new();

            foreach (var operation in operationList)
            {
                string propertyComposition = operation.path[1..];
                object propertyValue = typeof(HistoricalData).GetProperty(propertyComposition)!.GetValue(historicalData, null)!;

                string lowerCasePropertyComposition = propertyComposition.First().ToString().ToLower() + propertyComposition[1..];

                if (propertyValue != null &&
                    (operation.OperationType == OperationType.Add || operation.OperationType == OperationType.Replace))
                {
                    updateDefinition.Add(
                        updateBuilder.Set(
                            lowerCasePropertyComposition,
                            (lowerCasePropertyComposition == "physicalData" || lowerCasePropertyComposition == "nutritionalData") ? ObjectId.Parse((string)propertyValue) : propertyValue
                        )
                    );
                }
                else if (operation.OperationType == OperationType.Remove)
                {
                    updateDefinition.Add(updateBuilder.Unset(lowerCasePropertyComposition));
                }
            }

            return updateDefinition;
        }



        // DELETE
        public async Task DeleteHistoricalDataByIdAsync(string id)
        {
            var filter = Builders<HistoricalData>.Filter.Eq(hd => hd.Id, id);
            await _historicalData.DeleteOneAsync(filter);
        }
    }
}

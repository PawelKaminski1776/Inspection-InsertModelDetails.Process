using InspectionInsertModelDetails.Channel.Services;
using InspectionInsertModelDetails.Messages.Dtos;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InspectionUserDetails.Channel
{
    public class InsertModelDetailsService : MongoConnect
    {

        public InsertModelDetailsService(string ConnectionString) : base(ConnectionString)
        {
        }

        public async Task<string> InsertModel(ModelDetailsRequest request)
        {
            var database = dbClient.GetDatabase("InspectionAppDatabase");
            var collection = database.GetCollection<BsonDocument>("Inspection_Models");

            var document = new BsonDocument {
                { "Model_URL", request.Model_URL },
                { "InspectionName", request.InspectionName },
                { "status", request.status }
            };

            try
            {
                await collection.InsertOneAsync(document);
                return "Request Successful";
            }
            catch (Exception e)
            {
                return "Failed" + e.Message;
            }
        }

        public async Task<string> CheckIfModelExists(ModelDetailsRequest request)
        {
            var database = dbClient.GetDatabase("InspectionAppDatabase");
            var collection = database.GetCollection<BsonDocument>("Inspection_Models");

            var filter = Builders<BsonDocument>.Filter.Eq("Model_URL", request.Model_URL);

            try
            {
                var modelExists = await collection.Find(filter).AnyAsync();

                return modelExists ? "Model Exists" : "Model Not Found";
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Checking For Model: {e.Message}");
                return $"Error: {e.Message}";
            }
        }

        public async Task<string> UpdateModel(ModelDetailsRequest request)
        {
            var database = dbClient.GetDatabase("InspectionAppDatabase");
            var collection = database.GetCollection<BsonDocument>("Inspection_Models");

            var filter = Builders<BsonDocument>.Filter.Eq("Model_URL", request.Model_URL);
            var update = Builders<BsonDocument>.Update
                .Set("InspectionName", request.InspectionName)
                .Set("status", request.status);

            try
            {
                var result = await collection.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                    return "Model Not Found";

                return result.ModifiedCount > 0 ? "Update Successful" : "No Changes Made";
            }
            catch (Exception e)
            {
                return "Update Failed: " + e.Message;
            }
        }

    }
}
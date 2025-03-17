using MongoDB.Driver;

namespace InspectionInsertModelDetails.Channel.Services
{
    public class MongoConnect
    {
        protected readonly MongoClient dbClient;
        public MongoConnect(string ConnectionString)
        {
            dbClient = new MongoClient(ConnectionString);

        }

    }
}

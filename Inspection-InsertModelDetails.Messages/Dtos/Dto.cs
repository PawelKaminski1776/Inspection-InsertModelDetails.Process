using NServiceBus;

namespace InspectionInsertModelDetails.Messages.Dtos
{

    public class ModelDetailsRequest : IMessage
    {
        public string Model_URL { get; set; }
        public string InspectionName { get; set; }
        public string status { get; set; }

    }

    public class ModelDetailsResponse : IMessage
    {
        public string Message { get; set; }
    }

}

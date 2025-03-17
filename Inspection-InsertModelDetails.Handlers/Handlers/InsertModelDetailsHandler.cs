using InspectionInsertModelDetails.Messages.Dtos;
using InspectionUserDetails.Channel;

namespace InspectionInsertModelDetails.Handlers
{
    public class InsertModelDetailsHandler : IHandleMessages<ModelDetailsRequest>
    {
        private readonly InsertModelDetailsService insertModelDetailsService;
        public InsertModelDetailsHandler()
        {
        }

        public async Task Handle(ModelDetailsRequest message, IMessageHandlerContext context)
        {
            try
            {
                string response = await insertModelDetailsService.CheckIfModelExists(message);

                string mongores = "";

                if (response.Equals("Model Exists"))
                {
                    mongores = await insertModelDetailsService.UpdateModel(message);
                }
                else
                {
                    mongores = await insertModelDetailsService.InsertModel(message);
                }

                await context.Reply(new ModelDetailsResponse { Message = mongores });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while processing the message: {ex.Message}");

                throw;
            }
        }
    }
}

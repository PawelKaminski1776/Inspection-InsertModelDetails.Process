using Microsoft.AspNetCore.Mvc;
using InspectionInsertModelDetails.Messages.Dtos;
using InspectionInsertModelDetails.Controllers.DtoFactory;

namespace InspectionInsertModelDetails.Controllers
{
    [ApiController]
    [Route("Api/InsertModelDetails")]
    public class InsertModelController : BaseController
    {
        public InsertModelController(IMessageSession messageSession, IDtoFactory dtoFactory)
            : base(messageSession, dtoFactory) { }

        [HttpPost("Training-Status")]
        public async Task<IActionResult> AddAccount([FromBody] ModelDetailsRequest dto)
        {
            var Dto = (ModelDetailsRequest)_dtoFactory.UseDto("insertmodeldetailsdto", dto);

            try
            {
                var response = await _messageSession.Request<ModelDetailsResponse>(Dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while processing the request: {ex.Message}");
            }
        }
    }

}

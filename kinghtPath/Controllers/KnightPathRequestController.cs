using Microsoft.AspNetCore.Mvc;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KnightPathService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnightPathRequestController : ControllerBase
    {
        private readonly IAmazonSQS _sqsClient;
        private const string QueueUrl = "https://sqs.<region>.amazonaws.com/<account_id>/knight-path-queue";

        public KnightPathRequestController(IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromQuery] string source, [FromQuery] string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return BadRequest("Please pass source and target positions");
            }

            string operationId = Guid.NewGuid().ToString();

            var message = new
            {
                operationId = operationId,
                source = source,
                target = target
            };

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = QueueUrl,
                MessageBody = JsonConvert.SerializeObject(message)
            };

            await _sqsClient.SendMessageAsync(sendMessageRequest);

            return Ok($"Operation Id {operationId} was created. Please query it to find your results.");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KnightPathService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnightPathResultController : ControllerBase
    {
        private static readonly AmazonDynamoDBClient dynamoDbClient = new AmazonDynamoDBClient();
        private static readonly Table table = Table.LoadTable(dynamoDbClient, "KnightPathResults");

        [HttpGet]
        public async Task<IActionResult> GetResult([FromQuery] string operationId)
        {
            if (string.IsNullOrEmpty(operationId))
            {
                return BadRequest("Please pass the operationId");
            }

            var document = await table.GetItemAsync(operationId);

            if (document == null)
            {
                return NotFound("Operation not found");
            }

            var result = new Dictionary<string, string>
            {
                { "starting", document["starting"] },
                { "ending", document["ending"] },
                { "shortestPath", document["shortestPath"] },
                { "numberOfMoves", document["numberOfMoves"] },
                { "operationId", document["operationId"] }
            };

            return Ok(JsonConvert.SerializeObject(result));
        }
    }
}

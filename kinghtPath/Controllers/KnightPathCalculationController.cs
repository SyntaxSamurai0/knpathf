using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace KnightPathService.Controllers
{
    public class KnightPathCalculationController
    {
        private static readonly AmazonDynamoDBClient dynamoDbClient = new AmazonDynamoDBClient();
        private static readonly Table table = Table.LoadTable(dynamoDbClient, "KnightPathResults");

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var record in evnt.Records)
            {
                var message = JsonConvert.DeserializeObject<Dictionary<string, string>>(record.Body);
                string operationId = message["operationId"];
                string source = message["source"];
                string target = message["target"];

                var result = CalculateKnightPath(source, target);
                result["operationId"] = operationId;
                result["starting"] = source;
                result["ending"] = target;

                var document = new Amazon.DynamoDBv2.DocumentModel.Document();
                foreach (var kvp in result)
                {
                    document[kvp.Key] = kvp.Value;
                }

                await table.PutItemAsync(document);
            }
        }

        private Dictionary<string, string> CalculateKnightPath(string source, string target)
        {
            // Implement the logic to calculate the knight's shortest path and number of moves
            // For simplicity, let's assume the logic is implemented and returns a path and number of moves
            return new Dictionary<string, string>
            {
                { "shortestPath", "A1:C2:E3:D5" },
                { "numberOfMoves", "3" }
            };
        }
    }
}

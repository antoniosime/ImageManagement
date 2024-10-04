using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Drawing;
using System.Net;
using System.Text.Json;

namespace ImageFunctionApp.Functions
{
    public class GetImageFunction
    {
        private readonly ILogger<GetImageFunction> _logger;
        private readonly GetImageVariationHandler _getImageVariationHandler;
        private readonly IQueueService _queueService;

        public GetImageFunction(ILogger<GetImageFunction> logger, GetImageVariationHandler getImageVariationHandler, IQueueService queueService)
        {
            _logger = logger;
            _getImageVariationHandler = getImageVariationHandler;
            _queueService = queueService;
        }

        [Function("GetImage")]
        [OpenApiOperation(operationId: "GetImage", tags: new[] { "name" }, Summary = "Gets the image path", Description = "Gets the image path", Visibility = OpenApiVisibilityType.Important)]

        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Image id", Description = "", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "height", In = ParameterLocation.Query, Required = true, Type = typeof(long), Summary = "Image height", Description = "", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetImageResponse), Summary = "The response", Description = "This returns the response")]
        public async Task<IActionResult> Run([Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req
            , [DurableClient] DurableTaskClient client)
        {

            req.Query.TryGetValue("id", out var id);
            req.Query.TryGetValue("height", out var height);

            var imagePath = await _getImageVariationHandler.Handle(System.Guid.Parse(id), long.Parse(height));

            if(imagePath != null && imagePath != "")
            {
                return new OkObjectResult(new GetImageResponse { ImagePath=imagePath,IsCompleted=true});
            }
            else
            {
                var instanceId = $"{id}_{height}";
                var orchestrationMetadata = await client.GetInstancesAsync(instanceId, getInputsAndOutputs: true);

                if (orchestrationMetadata == null)
                {
                    var messsage = new ResizeImageDto
                    {
                        Id = Guid.Parse(id),
                        Height = long.Parse(height)
                    };
                    await _queueService.AddMessageToQueueAsync("resize-image", JsonSerializer.Serialize(messsage));

                    return new OkObjectResult(new GetImageResponse { ImagePath = "", IsRunning=true });
                }
                else
                {
                    return new OkObjectResult(new GetImageResponse { ImagePath = orchestrationMetadata.SerializedOutput??""
                        ,IsCompleted = orchestrationMetadata.IsCompleted
                    ,IsRunning=orchestrationMetadata.IsRunning});
                }
            }   
        }



    }
}

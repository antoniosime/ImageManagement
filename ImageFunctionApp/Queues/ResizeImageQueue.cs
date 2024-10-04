using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs;
using Azure.Storage.Queues.Models;
using ImageFunctionApp.Functions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Converters;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;

namespace ImageFunctionApp.Queues
{
    public class ResizeImageQueue
    {
        private readonly ILogger<ResizeImageQueue> _logger;

        public ResizeImageQueue(ILogger<ResizeImageQueue> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ResizeImageQueue))]
        public async Task Run([QueueTrigger("resize-image")] string messageData
            , [DurableClient] DurableTaskClient client
            )
        {

            var message = JsonSerializer.Deserialize<ResizeImageDto>(messageData);

            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(ResizeImageFunction)
                , input: message
                , options: new Microsoft.DurableTask.StartOrchestrationOptions
                {
                    InstanceId = $"{message.Id.ToString()}_{message.Height}"
                }, default);

            _logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
        }
    }
}

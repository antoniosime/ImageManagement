using Application.DTOs;
using Application.UseCases;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace ImageFunctionApp.Functions
{
    public  class ResizeImageFunction
    {
        private readonly ResizeImageHandler _resizeImageHandler;

        public ResizeImageFunction(ResizeImageHandler resizeImageHandler)
        {
            _resizeImageHandler = resizeImageHandler;
        }

        [Function(nameof(ResizeImageFunction))]
        public  async Task<string> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var resizeImageDto = context.GetInput<ResizeImageDto>();

            var path = await context.CallActivityAsync<string>(nameof(ResizeImageActivity), resizeImageDto);

            return path;
        }

        [Function(nameof(ResizeImageActivity))]
        public  async Task<string> ResizeImageActivity([ActivityTrigger] ResizeImageDto resizeImageDto, FunctionContext executionContext)
        {
            var url = await _resizeImageHandler.Handle(resizeImageDto.Id,resizeImageDto.Height);

            return url;
        }

        
    }
}

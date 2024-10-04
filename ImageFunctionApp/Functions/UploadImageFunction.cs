using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using ImageFunctionApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ImageFunctionApp.Functions
{
    public class UploadImageFunction
    {
        private readonly UploadImageHandler _uploadImageHandler;
        
        public UploadImageFunction(UploadImageHandler uploadImageHandler)
        {
            _uploadImageHandler = uploadImageHandler;
        }

        [OpenApiOperation(operationId: "UploadImage", tags: new[] { "name" }, Summary = "Upload image", Description = "", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "multipart/form-data", bodyType: typeof(MultiPartFormDataModel), Required = true, Description = "Image")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "", bodyType: typeof(UploadImageResponse), Summary = "The response", Description = "This returns the response")]
        [Function("UploadImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
           // log.LogInformation("Processing image upload request...");

            try
            {
                // Check if a form file is present in the request
                if (!req.HasFormContentType || req.Form.Files.Count == 0)
                {
                    return new BadRequestObjectResult("No file uploaded.");
                }

                // Get the uploaded file
                IFormFile uploadedFile = req.Form.Files[0];

                // Call UploadImageHandler to handle the upload
                var uploadedFilePath = await _uploadImageHandler.Handle(new UploadImageRequest { File=uploadedFile});
                

                // Return the uploaded image id or other details
                return new OkObjectResult(new UploadImageResponse
                {
                    Id=uploadedFilePath.Id,
                });
            }
            catch (Exception ex)
            {
                log.LogError($"An error occurred while uploading the image: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

using Application.Interfaces;
using Application.UseCases;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests
{
    public class UploadImageHandlerTests
    {
        [Fact]
        public async Task HandleUploadAsync_ShouldReturnFilePath_WhenFileIsValid()
        {
            // Arrange
            var mockImageRepository = new Mock<IImageRepository>();
            var mockImageService = new Mock<IImageService>();
            var mockQueueService = new Mock<IQueueService>();

            var imageId = Guid.NewGuid(); 
            var filePath = "test-file-path";
            var fileName = "test.png";

            mockImageService.Setup(f => f.UploadImageAsync(It.IsAny<IFormFile>()))
             .ReturnsAsync(new DTOs.ImageDto
             {
                 Id = imageId,
                 FilePath = filePath,
                 FileName = fileName,
             });

            // Mock the AddImageAsync method to do nothing (since we're not testing it directly)
            mockImageRepository.Setup(r => r.AddImageAsync(It.IsAny<Image>()))
                .Returns(Task.CompletedTask);

            mockQueueService.Setup(r => r.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

            var handler = new UploadImageHandler(mockImageService.Object, mockQueueService.Object);

            // Create a mock file to upload
            var formFile = new Mock<IFormFile>();
            var content = "fake image content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            formFile.Setup(f => f.OpenReadStream()).Returns(ms);
            formFile.Setup(f => f.FileName).Returns(fileName);
            formFile.Setup(f => f.Length).Returns(ms.Length);

            // Act
            var result = await handler.Handle(new DTOs.UploadImageRequest
            {
                File = formFile.Object
            });

            // Assert
            Assert.Equal(filePath, result.FilePath); // Assert that the file path is as expected

            // Assert
            Assert.Equal(fileName, result.FileName); // Assert that the file name is as expected
        }


        [Fact]
        public async Task HandleUploadAsync_ShouldThrowException_WhenFileSaveFails()
        {
            var imageId = Guid.NewGuid();
            var filePath = "test-file-path";
            var fileName = "test.png";

            // Arrange
            var mockImageRepository = new Mock<IImageRepository>();
            var mockImageService = new Mock<IImageService>();
            var mockQueueService = new Mock<IQueueService>();

            mockImageService.Setup(f => f.UploadImageAsync(It.IsAny<IFormFile>()))
             .ThrowsAsync(new Exception("File save failed"));

            // Mock the AddImageAsync method to do nothing (since we're not testing it directly)
            mockImageRepository.Setup(r => r.AddImageAsync(It.IsAny<Image>()))
                .Returns(Task.CompletedTask);

            mockQueueService.Setup(r => r.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

            var handler = new UploadImageHandler(mockImageService.Object, mockQueueService.Object);

            var formFile = new Mock<IFormFile>();
            var ms = new MemoryStream();
            formFile.Setup(f => f.OpenReadStream()).Returns(ms);
            formFile.Setup(f => f.FileName).Returns("test.png");
            formFile.Setup(f => f.Length).Returns(ms.Length);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(new DTOs.UploadImageRequest
            {
                File = formFile.Object
            }));
        }
    }
}

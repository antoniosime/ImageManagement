using Application.Interfaces;
using Application.Services;
using Application.UseCases;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests
{
    public class GetImageVariationHandlerTests
    {
        [Fact]
        public async Task HandleAsync_ShouldReturnThumbnailPath_WhenHeightIs160()
        {
            // Arrange
            var mockImageRepository = new Mock<IImageRepository>();
            var mockImageService = new Mock<IImageService>();

            var imageId= Guid.NewGuid();
            var fileName= "test.png";

            // Simulate getting an image from the repository
            var testImage = new Image { Id = imageId, FileName = fileName, FilePath=$"{imageId}/{fileName}" };
            mockImageRepository.Setup(repo => repo.GetImageByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(testImage);

            // Simulate the file service returning a path for the 160px thumbnail
            mockImageService.Setup(service => service.GetImageVariationAsync(It.IsAny<Guid>(), 160))
                .Returns(Task.FromResult(testImage.FilePath));

            var handler = new GetImageVariationHandler(mockImageService.Object);

            // Act
            var result = await handler.Handle(imageId, 160);

            // Assert
            Assert.Equal(testImage.FilePath, result);
        }
    }
}

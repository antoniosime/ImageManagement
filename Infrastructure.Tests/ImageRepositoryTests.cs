using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tests
{
    public class ImageRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> CreateInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ImageDatabase")
                .Options;
        }

        [Fact]
        public async Task GetImageByIdAsync_ShouldReturnImage_WhenImageExists()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            var context = new ApplicationDbContext(options);

            var id = Guid.NewGuid();

            var image = new Image { Id = id , FileName= "test.png",FilePath = "imagepath.png" };
            await context.Images.AddAsync(image);
            await context.SaveChangesAsync();

            var repository = new ImageRepository(context);

            // Act
            var result = await repository.GetImageByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("test.png", result.FileName);
        }

        [Fact]
        public async Task AddImageAsync_ShouldAddImageToDatabase()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            var context = new ApplicationDbContext(options);

            var repository = new ImageRepository(context);
            var id = Guid.NewGuid();

            var image = new Image { Id = id, FileName = "test.png", FilePath="imagepath.png" };

            // Act
            await repository.AddImageAsync(image);
            var result = await context.Images.FirstOrDefaultAsync(x=>x.Id.ToString()==id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test.png", result.FileName);
        }
    }
}

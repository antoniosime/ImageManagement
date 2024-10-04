using Application.Interfaces;
using Application.Services;
using Application.UseCases;
using AutoFixture;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.FileStorage;
using Infrastructure.Persistence;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Win32;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();


        services.AddTransient<ApplicationDbContext>();

        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));

        // Register services'

        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IFileService>(s=> new BlobStorageService(Environment.GetEnvironmentVariable("AzureWebJobsStorage"),"images"));
        services.AddScoped<IImageRepository, ImageRepository>();


        services.AddScoped<UploadImageHandler>();
        services.AddScoped<GetImageVariationHandler>();
        services.AddScoped<ResizeImageHandler>();

        // Register the AzureQueueService
        services.AddSingleton<IQueueService>(sp =>new AzureQueueService(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));


    })
    .Build();

host.Run();

using Application.Interfaces;
using Azure.Storage.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class AzureQueueService : IQueueService
    {
        private readonly string _connectionString;

        public AzureQueueService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddMessageToQueueAsync(string queueName, string message)
        {
            try
            {
                // Create a QueueClient that points to the queue you want to send a message to
                QueueClient queueClient = new QueueClient(_connectionString, queueName);

                // Ensure the queue exists before adding the message
                await queueClient.CreateIfNotExistsAsync();

                if (queueClient.Exists())
                {
                    var encodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

                    // Send the encoded message to the queue
                    await queueClient.SendMessageAsync(encodedMessage);

                    Console.WriteLine($"Message added to queue: {message}");
                }
                else
                {
                    Console.WriteLine($"Queue '{queueName}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding message to the queue: {ex.Message}");
            }
        }
    }
}

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ABC_Retail.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queue;

        public QueueStorageService(string connectionString, string queueName)
        {
            _queue = new QueueClient(connectionString, queueName);
            _queue.CreateIfNotExists();
        }

        // Send order message to the queue
        public async Task SendMessageAsync(string message)
        {
            if (!string.IsNullOrEmpty(message))
                await _queue.SendMessageAsync(message);
        }

        // Peek messages for debugging
        public async Task<List<string>> PeekMessagesAsync(int maxMessages = 10)
        {
            var result = new List<string>();
            PeekedMessage[] messages = await _queue.PeekMessagesAsync(maxMessages);
            foreach (var msg in messages)
            {
                result.Add(msg.MessageText);
            }
            return result;
        }
    }
}

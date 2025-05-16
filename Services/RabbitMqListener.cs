using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderEventProcessor.Models;
using OrderEventProcessor.Persistence;

namespace OrderEventProcessor.Services
{
    public class RabbitMqListener
    {
        private readonly ApplicationDbContext _dbContext;

        public RabbitMqListener(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task StartListeningAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
                arguments: null);
         

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Detect message type based on the X-MsgType header
                var msgType = ea.BasicProperties.Headers["X-MsgType"]?.ToString();
                if (msgType == "OrderEvent")
                {
                    var orderEvent = JsonSerializer.Deserialize<OrderEvent>(message);
                    await _dbContext.OrderEvents.AddAsync(orderEvent);
                    await _dbContext.SaveChangesAsync();
                    Console.WriteLine($"Order created: {orderEvent.Id}");
                }
                else if (msgType == "PaymentEvent")
                {
                    var paymentEvent = JsonSerializer.Deserialize<PaymentEvent>(message);
                    var order = await _dbContext.OrderEvents
                        .FirstOrDefaultAsync(o => o.Id == paymentEvent.OrderId);

                    if (order != null && order.Total == paymentEvent.Amount)
                    {
                        Console.WriteLine($"Order: {order.Id}, Product: {order.Product}, Total: {order.Total} {order.Currency}, Status: PAID");
                    }
                    else
                    {
                        Console.WriteLine("Payment is not sufficient or order not found.");
                    }
                }
            };

            channel.BasicConsumeAsync(queue: "order-events", autoAck: true, consumer: consumer);

            Console.WriteLine(" [*] Waiting for messages. Press [Enter] to exit.");
            Console.ReadLine();
        }
    }
}

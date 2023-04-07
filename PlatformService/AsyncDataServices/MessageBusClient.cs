using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration config)
        {
            _config = config;

            // Create a connection factory - RabbitMQ Concept.
            var factory = new ConnectionFactory() { 
                HostName = _config["RabbitMQHost"], 
                Port = int.Parse(_config["RabbitMQPort"]) 
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("---> Connected to Message Bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"---> Could not connect to RabbitMQ: {ex.Message}");
            }
        }


        public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
        {
            var message = JsonSerializer.Serialize(platformPublishDto);

            if(_connection.IsOpen)
            {
                Console.WriteLine("---> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }else
            {
                Console.WriteLine("---> RabbitMQ Connection Closed, not sending...");
                //This is where we should implement a retry mechanism.
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger",
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);
            Console.WriteLine($"---> We have sent: {message}");
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("---> RabbitMQ Connection Shutdown");
        }

        //disposeMethod
        public void Dispose()
        {
            Console.WriteLine("---> Message Bus Disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}
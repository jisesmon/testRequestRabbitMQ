using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace testRequestRabbitMQ.BLL;

public interface IAuditProducer
{
    Task PublishAuditLog(RequestAuditLog log);
}

public class RabbitMqAuditProducer : IAuditProducer
{
    private IConfiguration _configuration;
    private readonly ILogger<RabbitMqAuditProducer> _logger;

    //private readonly IConnection _connection;
    //private readonly  IModel _channel;

    public RabbitMqAuditProducer(IConfiguration configuration,
        ILogger<RabbitMqAuditProducer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishAuditLog(RequestAuditLog log)
    {
        //amqp://guest:guest@localhost:5672
        ConnectionFactory factory = new ConnectionFactory() {
            HostName = _configuration["RabbitMQ:HostName"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"],
            VirtualHost= _configuration["RabbitMQ:VirtualHost"]
           };
        _logger.LogInformation("---info---RabbitMqAuditProducer-- " +
                 "HostName:{HostName} ,Port:{Port} ,UserName:{UserName} ,Password:{Password} ,VirtualHost:{VirtualHost}",
                factory.HostName, factory.Port, factory.UserName, factory.Password, factory.VirtualHost);

        // factory.Uri = new Uri("amqp://user:pass@hostName:port/vhost");
        //ok factory.Uri = new Uri("amqp://ali:ali@localhost:5672");

        using IConnection _connection = await factory.CreateConnectionAsync();
        using IChannel _channel = await _connection.CreateChannelAsync();  

        //await _channel.BasicPublish(queueName, exchangeName, routingKey, null);
 
         await _channel.ExchangeDeclareAsync(_configuration["RabbitMQ:ExchangeName"], ExchangeType.Direct, durable: true);
         await _channel.QueueDeclareAsync(_configuration["RabbitMQ:QueueName"] , true, false, false, null);
         await _channel.QueueBindAsync(_configuration["RabbitMQ:QueueName"],
                                       _configuration["RabbitMQ:ExchangeName"], 
                                       _configuration["RabbitMQ:RoutingKey"], null);
 

         
        //QueueDeclarePassiveAsync("audit_logs",)  
        var message = JsonSerializer.Serialize(log);
        var body = Encoding.UTF8.GetBytes(message);
 
          
       await _channel.BasicPublishAsync(exchange: _configuration["RabbitMQ:ExchangeName"], 
                         routingKey: _configuration["RabbitMQ:RoutingKey"],
                         body: body);
    }

    
}
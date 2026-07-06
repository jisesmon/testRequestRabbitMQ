using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;

namespace testRequestRabbitMQ.BLL;


public class AuditLogConsumerService : BackgroundService
{
    private readonly ILogger<AuditLogConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConfiguration _configuration;
  

    public AuditLogConsumerService(IServiceScopeFactory scopeFactory, 
        ILogger<AuditLogConsumerService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
       
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        { 
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                Port = int.Parse(_configuration["RabbitMQ:Port"]),
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"],
               VirtualHost = _configuration["RabbitMQ:VirtualHost"] 
            };

            _logger.LogInformation("---info---AuditLogConsumerService-- " +
             "HostName:{HostName} ,Port:{Port} ,UserName:{UserName} ,Password:{Password} ,VirtualHost:{VirtualHost}",
                factory.HostName, factory.Port, factory.UserName, factory.Password ,factory.VirtualHost);

            // factory.Uri = new Uri("amqp://user:pass@hostName:port/vhost");
            //ok factory.Uri = new Uri("amqp://ali:ali@localhost:5672");  

            IConnection _connection = await factory.CreateConnectionAsync();


              IChannel _channel = await _connection.CreateChannelAsync();


            //await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
            // await _channel.QueueDeclareAsync(queueName, false, false, false, null);
            // await _channel.QueueBindAsync(queueName, exchangeName, routingKey, null);

            await _channel.ExchangeDeclareAsync(_configuration["RabbitMQ:ExchangeName"], ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(_configuration["RabbitMQ:QueueName"], true, false, false, null);
            await _channel.QueueBindAsync(_configuration["RabbitMQ:QueueName"],
                                          _configuration["RabbitMQ:ExchangeName"],
                                          _configuration["RabbitMQ:RoutingKey"], null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            //var consumer = new EventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var log = JsonSerializer.Deserialize<RequestAuditLog>(Encoding.UTF8.GetString(body));

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<TestDbContext>();

                    db.RequestAuditLogs.Add(log);
                    await db.SaveChangesAsync(stoppingToken);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                   // _logger.LogInformation("---info---AuditLogConsumerService--request==>> " + Encoding.UTF8.GetString(body));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "---ERROR ---AuditLogConsumerService---ReceivedAsync()--");
                    // Dead Letter Queue می‌توانید پیاده کنید
                }
            };

            await _channel.BasicConsumeAsync(
                _configuration["RabbitMQ:QueueName"]
                , false, consumer);

            //return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "---ERROR ---@@--AuditLogConsumerService--@@--Execute--");
            //error to LogInformation to debug not to sql--becase hang that  thozen user online  to sever  
            _logger.LogCritical(ex, "---ERROR ---@@--AuditLogConsumerService--@@--Execute--");

        }
    }
}
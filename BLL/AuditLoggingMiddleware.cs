using System.Diagnostics;

namespace testRequestRabbitMQ.BLL;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuditProducer _producer;

    public AuditLoggingMiddleware(RequestDelegate next, IAuditProducer producer)
    {
        _next = next;
        _producer = producer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var audit = new RequestAuditLog
        {
            UserId = context.User.Identity?.Name,
            EntryTime = DateTime.UtcNow,
            EntryUrl = context.Request.Path + context.Request.QueryString,
            StatusCode = context.Response.StatusCode
        };

        var stopwatch = Stopwatch.StartNew();
        context.Items["DATA_RMG"]=  audit ;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            audit.Exception = ex.ToString();
            throw;
        }
        finally
        {
            stopwatch.Stop();
            audit = context.Items["DATA_RMG"] as RequestAuditLog ?? audit;
            audit.ExitTime = DateTime.UtcNow;
            audit.DurationMs = stopwatch.ElapsedMilliseconds;
            audit.StatusCode = context.Response.StatusCode;

            // ارسال به RabbitMQ بدون await (Fire & Forget)
            _producer.PublishAuditLog(audit);
        }
    }
}
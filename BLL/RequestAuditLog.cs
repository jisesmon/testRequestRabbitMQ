namespace testRequestRabbitMQ.BLL;

public class RequestAuditLog
{
    public long Id { get; set; } // = Guid.NewGuid();
    public string? UserId { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public string? EntryUrl { get; set; }
    public string? Exception { get; set; }
    public string? ExceptionMessage { get; set; }
    public int StatusCode { get; set; }
    public long DurationMs { get; set; }
    public DateTime? OrdsEntryTime { get; set; }
    public DateTime? OrdsExitTime { get; set; }
    public long OrdsDurationMs { get; set; }
    public string? OrdsException { get; set; }
    public string? OrdsExceptionMessage { get; set; }
    public int OrdsStatusCode { get; set; }
    public string? OrdsUrl { get; set; } 
}
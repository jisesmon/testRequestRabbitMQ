
using Microsoft.EntityFrameworkCore;

namespace testRequestRabbitMQ.BLL;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options){}
    
    public DbSet<RequestAuditLog> RequestAuditLogs { get; set; }
    
}
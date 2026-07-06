using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using testRequestRabbitMQ.BLL;
namespace testRequestRabbitMQ.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class homeController : ControllerBase
{
   private readonly ILogger<homeController> _logger;
    private TestDbContext _TestDbContext;
    public homeController( ILogger<homeController> logger, TestDbContext testDbContext)
    {
      _logger = logger;
        _TestDbContext = testDbContext;
    }
    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("--------Index---------" + new Random().Next(100));
       var  audit =  HttpContext.Items["DATA_RMG"] as RequestAuditLog ??new RequestAuditLog();
        if (audit != null)
        {
            Thread.Sleep(200);
            audit.OrdsEntryTime = DateTime.UtcNow;
            Thread.Sleep(1000);
            audit.OrdsExitTime = DateTime.UtcNow;
            TimeSpan diff = audit.OrdsExitTime.Value - audit.OrdsEntryTime.Value;
            //_logger.LogInformation("--------Index------TotalMilliseconds---" + diff.TotalMilliseconds);
            //_logger.LogInformation("--------Index------Ticks---" + diff.Ticks);

            audit.OrdsDurationMs = (long)diff.TotalMilliseconds;
            audit.OrdsUrl = "/testUrl";
            HttpContext.Items["DATA_RMG"] = audit;

        }
        return Ok("Helo word"); 
    }

    [HttpGet]
    public IActionResult list()
    {
        _logger.LogInformation("---------list----------" + new Random().Next(100));
        return Ok(_TestDbContext.RequestAuditLogs.ToList());
    }

     
}
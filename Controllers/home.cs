using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
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
    public homeController(ILogger<homeController> logger, TestDbContext testDbContext)
    {
        _logger = logger;
        _TestDbContext = testDbContext;
    }
    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("--------Index---------" + new Random().Next(100));
        var audit = HttpContext.Items["DATA_RMG"] as RequestAuditLog ?? new RequestAuditLog();
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

    [HttpGet]
    public async Task<IActionResult> ping(string host)
    {
        //var ss = await PingHostAsync(host);//SendAdvancedPingAsync(host);
         using var ping = new Ping();
         var reply = await   ping.SendPingAsync(host);
         
        if (reply.Status == IPStatus.Success)
        {
           return Ok($"آدرس: {reply.Address} زمان پاسخ: {reply.RoundtripTime} میلی‌ثانیهطول بافر: {reply.Buffer.Length}");
              
        }
        else
        {
            return BadRequest($"پینگ ناموفق: {reply.Status}");
        }
    }

    public async Task<PingReply> SendAdvancedPingAsync(string host)
    {
        using var ping = new Ping();

        // تنظیمات (TTL و عدم تکه‌تکه شدن)
        var options = new PingOptions
        {
            Ttl = 64,               // حداکثر تعداد هاپ
            DontFragment = true
        };

        // داده ارسالی (مشابه بافر پیش‌فرض ۳۲ بایت)
        byte[] buffer = Encoding.ASCII.GetBytes("PingData");

        // تایم‌اوت ۳ ثانیه‌ای
        int timeout = 3000;

        return await ping.SendPingAsync(host, timeout, buffer, options);
    }

     

}
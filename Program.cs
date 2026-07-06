 using  testRequestRabbitMQ.BLL;
 using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("testRequestRabbitMQ") ;
builder.Services.AddSqlite<TestDbContext>(connectionString);


builder.Services.AddSingleton<IAuditProducer, RabbitMqAuditProducer>();
 builder.Services.AddHostedService<AuditLogConsumerService>();
 
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.addcontrollers(); 
//builder.Services.AddControllersWithViews();
 builder.Services.AddControllers();



var app = builder.Build();

 app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseMiddleware<AuditLoggingMiddleware>();
 
 
 
app.MapControllers();

app.Run();

 
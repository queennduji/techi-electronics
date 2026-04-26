using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Techi.Electronics.MessageBus;
using Techi.Electronics.OrderAPI.Data;
using Techi.Electronics.OrderAPI.Extensions;
using Techi.Electronics.OrderAPI.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//todo: create custom health check for stripe
builder.Services.AddHealthChecks()
    .AddSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"), name: "SQL Database")
     .AddRedis(builder.Configuration.GetConnectionString("AzureRedisConnection"), name: "Redis Cache")
     .AddAzureServiceBusQueue(connectionString: builder.Configuration["ServiceBusConnectionString"],
        queueName: builder.Configuration["TopicAndQueueNames:OrderCreatedTopic"],
        name: "Order Created Topic")
      .AddUrlGroup(
        uri: new Uri("https://api.stripe.com"),
        name: "Stripe API");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddScoped<IMessageBus>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["ServiceBusConnectionString"];

    return new MessageBus(connectionString);
});
builder.Services.AddHttpClient("Product", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.AddAppAuthentication();

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                error = entry.Value.Exception?.Message
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
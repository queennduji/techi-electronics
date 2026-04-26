using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Techi.Electronics.EmailAPI.Data;
using Techi.Electronics.EmailAPI.Extension;
using Techi.Electronics.EmailAPI.Messaging;
using Techi.Electronics.EmailAPI.Services;
using Techi.Electronics.MessageBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHealthChecks()

    // SQL Server
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "SQL Database")

    // Register User Queue
    .AddAzureServiceBusQueue(
        connectionString: builder.Configuration["ServiceBusConnectionString"],
        queueName: builder.Configuration["TopicAndQueueNames:RegisterUserQueue"],
        name: "Register User Queue")

    // Email Shopping Cart Queue
    .AddAzureServiceBusQueue(
        connectionString: builder.Configuration["ServiceBusConnectionString"],
        queueName: builder.Configuration["TopicAndQueueNames:EmailShoppingCartQueue"],
        name: "Email Shopping Cart Queue")

    // Order Created Topic
    .AddAzureServiceBusTopic(
        connectionString: builder.Configuration["ServiceBusConnectionString"],
        topicName: builder.Configuration["TopicAndQueueNames:OrderCreatedTopic"],
        name: "Order Created Topic")

    // Order Created Email Subscription
    .AddAzureServiceBusSubscription(
        connectionString: builder.Configuration["ServiceBusConnectionString"],
        topicName: builder.Configuration["TopicAndQueueNames:OrderCreatedTopic"],
        subscriptionName: builder.Configuration["TopicAndQueueNames:OrderCreated_Email_Subscription"],
        name: "Order Created Email Subscription");

var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddSingleton<IMessageBus>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["ServiceBusConnectionString"];

    return new MessageBus(connectionString);
});

builder.Services.AddSingleton(new EmailService(optionBuilder.Options));

builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.UseAzureServiceBusConsumer();
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

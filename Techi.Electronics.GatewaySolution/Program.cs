using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Techi.Electronics.GatewaySolution.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddAppAuthentication();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
var app = builder.Build();
app.UseResponseCompression();
app.MapGet("/", () => "Hello World!");
app.UseOcelot().GetAwaiter().GetResult();
app.Run();
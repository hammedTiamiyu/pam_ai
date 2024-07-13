using PAMAi.API.Extensions;
using PAMAi.Infrastructure.Identity.Extensions;
using Serilog;
using Serilog.Formatting.Compact;

// Bootstrap logger.
string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        formatter: new RenderedCompactJsonFormatter(),
        path: $"{appDataPath}\\PAMAi\\api-log-.log",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting PAMAi API");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSerilog((services, loggerConfig) =>
    {
        loggerConfig.ReadFrom.Configuration(builder.Configuration);
        loggerConfig.ReadFrom.Services(services);
    });
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddIdentityInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSerilogRequestLogging(options => options.IncludeQueryInRequestPath = true);
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        await app.MigrateDatabasesAsync();
    }
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
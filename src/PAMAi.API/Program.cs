using PAMAi.API.Extensions;
using PAMAi.Infrastructure.Identity.Extensions;
using PAMAi.Infrastructure.Storage.Extensions;
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

    ConfigureServices(builder);
    await ConfigureAndRunAppAsync(builder);
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

void ConfigureServices(WebApplicationBuilder builder)
{
    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddIdentityInfrastructure(builder.Configuration);
    builder.Services.AddStorageInfrastructure(builder.Configuration);
    builder.Services.AddSerilog((services, loggerConfig) =>
    {
        loggerConfig.ReadFrom.Configuration(builder.Configuration);
        loggerConfig.ReadFrom.Services(services);
    });
}

async Task ConfigureAndRunAppAsync(WebApplicationBuilder builder)
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        await app.MigrateDatabasesAsync();
    }

    var seedIdentityDbTask = app.SeedIdentityDatabaseAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSerilogRequestLogging(options => options.IncludeQueryInRequestPath = true);
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    await seedIdentityDbTask;

    await app.RunAsync();
}

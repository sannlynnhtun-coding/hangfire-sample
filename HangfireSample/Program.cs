using CronExpressionDescriptor;
using Hangfire;
using Hangfire.LiteDB;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add Hangfire services.
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseLiteDbStorage("hangfire.db"));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer(serverOptions =>
{
    serverOptions.ServerName = "Hangfire.MinimalAPI.MainServer";
});

builder.Services.AddTransient<IJobService, JobService>();
builder.Services.AddSingleton<HangfireManager>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire");

// API Endpoints
app.MapPost("/jobs/create", (IJobService jobService, [FromBody] JobRequest request) =>
{
    RecurringJob.AddOrUpdate(request.JobId, () => jobService.ExecuteJob(request.Message), request.CronExpression);
    return Results.Created($"/jobs/{request.JobId}", null);
});

app.MapGet("/jobs", () =>
{
    using var connection = JobStorage.Current.GetConnection();
    var recurringJobs = connection.GetRecurringJobs();
    return Results.Ok(recurringJobs);
});

app.MapPut("/jobs/update", (IJobService jobService, [FromBody] JobRequest request) =>
{
    RecurringJob.AddOrUpdate(request.JobId, () => jobService.ExecuteJob(request.Message), request.CronExpression);
    return Results.Ok($"Job '{request.JobId}' has been updated.");
});

app.MapDelete("/jobs/delete/{jobId}", (string jobId) =>
{
    RecurringJob.RemoveIfExists(jobId);
    return Results.Ok($"Job '{jobId}' has been deleted.");
});

app.MapPost("/jobs/stop-all", (HangfireManager hangfireManager) =>
{
    hangfireManager.StopServer();
    return Results.Ok("All Hangfire services stopped.");
});

app.MapPost("/jobs/restart-all", (HangfireManager hangfireManager) =>
{
    hangfireManager.StartServer();
    return Results.Ok("All Hangfire services restarted.");
});

app.MapGet("/jobs/test-cron", (string cronExpression) =>
{
    try
    {
        var description = ExpressionDescriptor.GetDescription(cronExpression, new Options() { Use24HourTimeFormat = true, Locale = "my" });
        return Results.Ok(new { CronExpression = cronExpression, Description = description });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Run();

// DTO for API request
public class JobRequest
{
    public string JobId { get; set; }
    public string Message { get; set; }
    public string CronExpression { get; set; }
}

// Simple job service
public interface IJobService
{
    void ExecuteJob(string message);
}

public class JobService : IJobService
{
    public void ExecuteJob(string message)
    {
        Console.WriteLine($"Executing job: {message} at {DateTime.Now}");
    }
}

// Hangfire Server Manager
public class HangfireManager : IDisposable
{
    private IServiceProvider _serviceProvider;
    private IHostedService _hangfireServer;
    private bool _isServerRunning = true;

    public HangfireManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _hangfireServer = _serviceProvider.GetRequiredService<IHostedService>();
    }

    public void StartServer()
    {
        if (!_isServerRunning)
        {
            _hangfireServer = _serviceProvider.GetRequiredService<IHostedService>();
            _hangfireServer.StartAsync(CancellationToken.None);
            _isServerRunning = true;
        }
    }

    public void StopServer()
    {
        if (_isServerRunning && _hangfireServer != null)
        {
            _hangfireServer.StopAsync(CancellationToken.None);
            _isServerRunning = false;
        }
    }

    public void Dispose()
    {
        StopServer();
    }
}


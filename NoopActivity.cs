using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DurableFunctionLoggingRepro;

public sealed class NoopActivity
{
    private readonly ILogger<NoopActivity> _logger;

    public NoopActivity(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<NoopActivity>();
    }

    [Function(nameof(NoopActivity))]
    public Task Run([ActivityTrigger] int instanceId)
    {
        _logger.LogInformation("Running NoopActivity for instance id {InstanceId}", instanceId);

        return Task.CompletedTask;
    }
}

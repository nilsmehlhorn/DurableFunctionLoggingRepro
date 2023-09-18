using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DurableFunctionLoggingRepro;

public sealed class FailingActivity
{
    private readonly ILogger<FailingActivity> _logger;

    public FailingActivity(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FailingActivity>();
    }

    [Function(nameof(FailingActivity))]
    public Task Run([ActivityTrigger] int instanceId)
    {
        _logger.LogInformation("Running FailingActivity for instance id {InstanceId}", instanceId);

        throw new Exception($"This error was thrown for testing purposes inside FailingActivity.");
    }
}

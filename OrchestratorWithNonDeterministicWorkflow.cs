using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionLoggingRepro;

public sealed class OrchestratorWithNonDeterministicWorkflow
{
    private static bool _isFirstRun = true;

    [Function(nameof(OrchestratorWithNonDeterministicWorkflow))]
    public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var replaySafeLogger = context.CreateReplaySafeLogger<OrchestratorWithFailingWorkflow>();

        replaySafeLogger.LogInformation(
            "Running OrchestratorWithNonDeterministicWorkflow with instance id {InstanceId} which is expected to fail.",
            context.InstanceId);

        if (_isFirstRun)
        {
            _isFirstRun = false;
        }
        else
        {
            await context.CallActivityAsync(nameof(NoopActivity), context.InstanceId);
        }

        replaySafeLogger.LogInformation(
            "Starting timer in OrchestratorWithNonDeterministicWorkflow with instance id {InstanceId}.",
            context.InstanceId);

        await context.CreateTimer(TimeSpan.FromSeconds(value: 2), CancellationToken.None);

        replaySafeLogger.LogInformation(
            "Finished timer in OrchestratorWithNonDeterministicWorkflow with instance id {InstanceId}.",
            context.InstanceId);
    }
}
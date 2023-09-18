using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionLoggingRepro;

public sealed class OrchestratorWithFailingActivity
{
    [Function(nameof(OrchestratorWithFailingActivity))]
    public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var replaySafeLogger = context.CreateReplaySafeLogger<OrchestratorWithFailingActivity>();

        replaySafeLogger.LogInformation(
            "Running OrchestratorWithFailingActivity with instance id {InstanceId} which is expected to fail.",
            context.InstanceId);

        await context.CallActivityAsync(nameof(FailingActivity), context.InstanceId);

        replaySafeLogger.LogInformation(
            "OrchestratorWithFailingActivity with instance id {InstanceId} unexpectedly completed successfully.",
            context.InstanceId);
    }
}
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionLoggingRepro;

public sealed class OrchestratorWithFailingWorkflow
{
    [Function(nameof(OrchestratorWithFailingWorkflow))]
    public static Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var replaySafeLogger = context.CreateReplaySafeLogger<OrchestratorWithFailingWorkflow>();

        replaySafeLogger.LogInformation(
            "Running OrchestratorWithFailingWorkflow with instance id {InstanceId} which is expected to fail.",
            context.InstanceId);

        throw new Exception("This error was thrown for testing purposes inside OrchestratorWithFailingWorkflow.");
    }
}

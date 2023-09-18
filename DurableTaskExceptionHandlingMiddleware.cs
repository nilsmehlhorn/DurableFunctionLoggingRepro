using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionLoggingRepro;

internal sealed class DurableTaskExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<DurableTaskExceptionHandlingMiddleware> _logger;

    public DurableTaskExceptionHandlingMiddleware(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DurableTaskExceptionHandlingMiddleware>();
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (!IsOrchestrationTrigger(context))
        {
            await next(context);

            return;
        }

        try
        {
            await next(context);
        }
        catch (AggregateException e)
        {
            e.Handle(HandleDurableTaskException);
        }
        catch (Exception e)
        {
            HandleDurableTaskException(e);
        }
    }

    private bool HandleDurableTaskException(Exception e)
    {
        if (e is TaskFailedException)
        {
            // Task failed exceptions originate from errors inside activities which
            // are already logged by the framework.
            return false;
        }
        
        _logger.LogError(e, "Error occurred during execution of durable function");

        return true;
    }

    private static bool IsOrchestrationTrigger(FunctionContext context)
    {
        // Code taken and modified from: https://github.com/Azure/azure-functions-durable-extension/blob/2960744a186b768c23ddb487674bcdde2958b0b2/src/Worker.Extensions.DurableTask/DurableTaskFunctionsMiddleware.cs#LL40C15-L40C15
        return context.FunctionDefinition.InputBindings.Values.Any(
            binding => string.Equals(binding.Type, "orchestrationTrigger"));
    }
}
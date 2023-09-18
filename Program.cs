using DurableFunctionLoggingRepro;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults((context, builder) =>
    {
        // Workaround for forwarding workflow exceptions to Application Insights.
        // builder.UseMiddleware<DurableTaskExceptionHandlingMiddleware>();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    }).ConfigureLogging(logging =>
    {
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            var defaultRule = options.Rules.FirstOrDefault(rule =>
            {
                const string loggerProviderName =
                    "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider";
                return rule.ProviderName == loggerProviderName;
            });
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    })
    .Build();

host.Run();
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.Otel;

public static class OtelConfiguration
{
    public static void AddCommonTelemetry(this IServiceCollection services, string serviceName)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder
                    .AddSource(serviceName)
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .ConfigureResource(resource => resource
                        .AddService(serviceName,
                            serviceVersion: "v1", 
                            serviceInstanceId: Environment.MachineName))
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMassTransitInstrumentation()
                    .AddOtlpExporter()
            )
            .WithMetrics(metricsProviderBuilder =>
                metricsProviderBuilder
                    .ConfigureResource(resource => resource
                        .AddService(serviceName, 
                            serviceVersion: "v1",
                            serviceInstanceId: Environment.MachineName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter(InstrumentationOptions.MeterName)
                    .AddOtlpExporter()
            ); 
    }
}

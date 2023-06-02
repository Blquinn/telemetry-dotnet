using Common.Otel;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Common;

public static class Logging
{
    public static void ConfigureLogger(WebApplicationBuilder webApplicationBuilder, string serviceName)
    {
        var builder = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.With(new OpenTelemetryTraceIdEnricher())
            .Enrich.With(new OpenTelemetrySpanIdEnricher());

        Boolean.TryParse(
            Environment.GetEnvironmentVariable("ENABLE_JSON_LOGS")
            , out var enableJson);
        
        if (!webApplicationBuilder.Environment.IsDevelopment() || enableJson)
        {
            // Use structured json logs in staging/production etc.
            builder.WriteTo.Console(new RenderedCompactJsonFormatter());
        }
        else
        {
            // Use more readable text logs in development.
            builder.WriteTo.Console();
        }
        
        builder.WriteTo.Elasticsearch(
            new ElasticsearchSinkOptions(
                new DefaultHttpTransport(
                    new TransportConfiguration(
                        new Uri("http://elastic:changeme@localhost:9200")))));

        Log.Logger = builder.CreateLogger();

        webApplicationBuilder.Host.UseSerilog();
    }
}

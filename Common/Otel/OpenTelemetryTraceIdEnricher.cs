using OpenTelemetry.Trace;
using Serilog.Core;
using Serilog.Events;

namespace Common.Otel;

// from https://github.com/Freheims/Serilog.Enrichers.OpenTelemetry/blob/main/Serilog.Enrichers.OpenTelemetry/OpenTelemetryTraceIdEnricher.cs

public class OpenTelemetryTraceIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Tracer.CurrentSpan.Context.TraceId.ToHexString();
        if (!string.IsNullOrWhiteSpace(traceId) && traceId != "00000000000000000000000000000000")
        {
            var traceIdProperty = new LogEventProperty("TraceId", new ScalarValue(traceId));
            logEvent.AddOrUpdateProperty(traceIdProperty);
        }
    }
}

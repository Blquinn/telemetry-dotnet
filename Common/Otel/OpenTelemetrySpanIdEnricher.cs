using OpenTelemetry.Trace;
using Serilog.Core;
using Serilog.Events;

namespace Common.Otel;

public class OpenTelemetrySpanIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var spanId = Tracer.CurrentSpan.Context.SpanId.ToHexString();
        if (!string.IsNullOrWhiteSpace(spanId) && spanId != "0000000000000000")
        {
            var spanIdProperty = new LogEventProperty("SpanId", new ScalarValue(spanId));
            logEvent.AddOrUpdateProperty(spanIdProperty);
        }
    }
}
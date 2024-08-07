using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Gainsway.Observability;

public static partial class ObservabilityExtensions
{
    public static void AddObservability(
        this WebApplicationBuilder builder,
        string serviceName,
        string commitShortSha
    )
    {
        builder.Services.AddOptions<OtlpExporterOptions>();
        builder
            .Services.AddOpenTelemetry()
            .ConfigureResource(r =>
            {
                r.Clear();

                r.AddService(
                    serviceName: serviceName,
                    serviceVersion: commitShortSha,
                    serviceInstanceId: Environment.MachineName
                );
            })
            .WithLogging()
            .WithMetrics(m =>
            {
                m.AddAspNetCoreInstrumentation();
            })
            .WithTracing(t =>
            {
                t.AddAspNetCoreInstrumentation();
                t.AddHttpClientInstrumentation();
                t.AddConfluentKafkaInstrumentation();
                t.AddNpgsql();
            })
            .UseOtlpExporter();
    }
}

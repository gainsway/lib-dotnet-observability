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
        string commitShortSha,
        Action<OpenTelemetry.Instrumentation.AspNetCore.AspNetCoreTraceInstrumentationOptions>? aspNetCoreInstrumentationOptions =
            null
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
                r.AddEnvironmentVariableDetector();
            })
            .WithLogging(opt => { })
            .WithMetrics(m =>
            {
                m.AddAspNetCoreInstrumentation();
            })
            .WithTracing(t =>
            {
                t.AddAspNetCoreInstrumentation(
                    aspNetCoreInstrumentationOptions
                        ?? (
                            opt =>
                            {
                                opt.Filter = (httpContext) =>
                                {
                                    var path = httpContext.Request.Path.ToString();
                                    return !path.StartsWith("/healthz")
                                        && !path.StartsWith("/metrics");
                                };
                            }
                        )
                );
                t.AddHttpClientInstrumentation();
                t.AddAWSInstrumentation();
                t.AddNpgsql();
            })
            .UseOtlpExporter();
    }
}

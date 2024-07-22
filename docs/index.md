# lib-dotnet-observability

This library add open telemetry configuration extensions for .NET Core Api

## Configuration

1. Install `Gainsway.Observability` package in the target project
  ```sh
  dotnet add package Gainsway.Observability
  ```
2. Register the Observability services in the Infrastructure project:
  ```csharp
  # InfrastructureServiceExtensions.cs 
  var appMetadata = builder.Configuration.GetApplicationMetadata();
  builder.AddObservability(serviceName: appMetadata.ServiceName, commitShortSha: appMetadata.CommitShortSha);
  ```

## Environment variables

> [!NOTE]
> In OpenTelemetry .NET environment variable keys are retrieved using
  `IConfiguration` which means they may be set using other mechanisms such as
  defined in appSettings.json or specified on the command-line.

### Exporter configuration

The [OpenTelemetry
Specification](https://github.com/open-telemetry/opentelemetry-specification/)
defines environment variables which can be used to configure the [OTLP
exporter](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/protocol/exporter.md)
and its associated processor
([logs](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/configuration/sdk-environment-variables.md#batch-logrecord-processor)
&
[traces](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/configuration/sdk-environment-variables.md#batch-span-processor))
or reader
([metrics](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/configuration/sdk-environment-variables.md#periodic-exporting-metricreader)).

* All signals

  The following environment variables can be used to override the default
  values of the `OtlpExporterOptions`:

  | Environment variable          | `OtlpExporterOptions` property        |
  | ------------------------------| --------------------------------------|
  | `OTEL_EXPORTER_OTLP_ENDPOINT` | `Endpoint`                            |
  | `OTEL_EXPORTER_OTLP_HEADERS`  | `Headers`                             |
  | `OTEL_EXPORTER_OTLP_TIMEOUT`  | `TimeoutMilliseconds`                 |
  | `OTEL_EXPORTER_OTLP_PROTOCOL` | `Protocol` (`grpc` or `http/protobuf`)|

* Logs:

  The following environment variables can be used to override the default values
  for the batch processor configured for logging:

  | Environment variable              | `BatchExportLogRecordProcessorOptions` property                         |
  | ----------------------------------| ------------------------------------------------------------------------|
  | `OTEL_BLRP_SCHEDULE_DELAY`        | `ScheduledDelayMilliseconds`                                            |
  | `OTEL_BLRP_EXPORT_TIMEOUT`        | `ExporterTimeoutMilliseconds`                                           |
  | `OTEL_BLRP_MAX_QUEUE_SIZE`        | `MaxQueueSize`                                                          |
  | `OTEL_BLRP_MAX_EXPORT_BATCH_SIZE` | `MaxExportBatchSize`                                                    |

  The following environment variables can be used to override the default values
  of the `OtlpExporterOptions` used for logging when using the [UseOtlpExporter
  extension](#enable-otlp-exporter-for-all-signals):

  | Environment variable                  | `OtlpExporterOptions` property        | UseOtlpExporter | AddOtlpExporter |
  | --------------------------------------| --------------------------------------|-----------------|-----------------|
  | `OTEL_EXPORTER_OTLP_LOGS_ENDPOINT`    | `Endpoint`                            | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_LOGS_HEADERS`     | `Headers`                             | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_LOGS_TIMEOUT`     | `TimeoutMilliseconds`                 | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_LOGS_PROTOCOL`    | `Protocol` (`grpc` or `http/protobuf`)| Supported       | Not supported   |

* Metrics:

  The following environment variables can be used to override the default value
  of the `TemporalityPreference` setting for the reader configured for metrics
  when using OTLP exporter:

  | Environment variable                                | `MetricReaderOptions` property                  |
  | ----------------------------------------------------| ------------------------------------------------|
  | `OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE` | `TemporalityPreference`                         |

  The following environment variables can be used to override the default values
  of the periodic exporting metric reader configured for metrics:

  | Environment variable                                | `PeriodicExportingMetricReaderOptions` property |
  | ----------------------------------------------------| ------------------------------------------------|
  | `OTEL_METRIC_EXPORT_INTERVAL`                       | `ExportIntervalMilliseconds`                    |
  | `OTEL_METRIC_EXPORT_TIMEOUT`                        | `ExportTimeoutMilliseconds`                     |

  The following environment variables can be used to override the default values
  of the `OtlpExporterOptions` used for metrics when using the [UseOtlpExporter
  extension](#enable-otlp-exporter-for-all-signals):

  | Environment variable                  | `OtlpExporterOptions` property        | UseOtlpExporter | AddOtlpExporter |
  | --------------------------------------| --------------------------------------|-----------------|-----------------|
  | `OTEL_EXPORTER_OTLP_METRICS_ENDPOINT` | `Endpoint`                            | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_METRICS_HEADERS`  | `Headers`                             | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_METRICS_TIMEOUT`  | `TimeoutMilliseconds`                 | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_METRICS_PROTOCOL` | `Protocol` (`grpc` or `http/protobuf`)| Supported       | Not supported   |

* Tracing:

  The following environment variables can be used to override the default values
  for the batch processor configured for tracing:

  | Environment variable             | `BatchExportActivityProcessorOptions` property              |
  | ---------------------------------| ------------------------------------------------------------|
  | `OTEL_BSP_SCHEDULE_DELAY`        | `ScheduledDelayMilliseconds`                                |
  | `OTEL_BSP_EXPORT_TIMEOUT`        | `ExporterTimeoutMilliseconds`                               |
  | `OTEL_BSP_MAX_QUEUE_SIZE`        | `MaxQueueSize`                                              |
  | `OTEL_BSP_MAX_EXPORT_BATCH_SIZE` | `MaxExportBatchSize`                                        |

  The following environment variables can be used to override the default values
  of the `OtlpExporterOptions` used for tracing when using the [UseOtlpExporter
  extension](#enable-otlp-exporter-for-all-signals):

  | Environment variable                  | `OtlpExporterOptions` property        | UseOtlpExporter | AddOtlpExporter |
  | --------------------------------------| --------------------------------------|-----------------|-----------------|
  | `OTEL_EXPORTER_OTLP_TRACES_ENDPOINT`  | `Endpoint`                            | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_TRACES_HEADERS`   | `Headers`                             | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_TRACES_TIMEOUT`   | `TimeoutMilliseconds`                 | Supported       | Not supported   |
  | `OTEL_EXPORTER_OTLP_TRACES_PROTOCOL`  | `Protocol` (`grpc` or `http/protobuf`)| Supported       | Not supported   |

### Attribute limits

The [OpenTelemetry
Specification](https://github.com/open-telemetry/opentelemetry-specification/)
defines environment variables which can be used to configure [attribute
limits](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/configuration/sdk-environment-variables.md#attribute-limits).

The following environment variables can be used to configure default attribute
limits:

* `OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT`
* `OTEL_ATTRIBUTE_COUNT_LIMIT`

The following environment variables can be used to configure span limits used
for tracing:

* `OTEL_SPAN_ATTRIBUTE_VALUE_LENGTH_LIMIT`
* `OTEL_SPAN_ATTRIBUTE_COUNT_LIMIT`
* `OTEL_SPAN_EVENT_COUNT_LIMIT`
* `OTEL_SPAN_LINK_COUNT_LIMIT`
* `OTEL_EVENT_ATTRIBUTE_COUNT_LIMIT`
* `OTEL_LINK_ATTRIBUTE_COUNT_LIMIT`

The following environment variables can be used to configure log record limits
used for logging:

* `OTEL_LOGRECORD_ATTRIBUTE_VALUE_LENGTH_LIMIT`
* `OTEL_LOGRECORD_ATTRIBUTE_COUNT_LIMIT`

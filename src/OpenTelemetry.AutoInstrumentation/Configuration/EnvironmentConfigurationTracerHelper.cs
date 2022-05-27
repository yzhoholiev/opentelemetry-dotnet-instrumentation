// <copyright file="EnvironmentConfigurationTracerHelper.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using OpenTelemetry.AutoInstrumentation.Logging;
using OpenTelemetry.Trace;

namespace OpenTelemetry.AutoInstrumentation.Configuration;

internal static class EnvironmentConfigurationTracerHelper
{
    private static readonly Dictionary<TracerInstrumentation, Action<TracerProviderBuilder>> AddInstrumentation = new()
    {
        [TracerInstrumentation.HttpClient] = builder => builder.AddHttpClientInstrumentation(),
        [TracerInstrumentation.AspNet] = builder => builder.AddSdkAspNetInstrumentation(),
        [TracerInstrumentation.SqlClient] = builder => builder.AddSqlClientInstrumentation(),
        [TracerInstrumentation.MongoDB] = builder => builder.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources"),
        [TracerInstrumentation.MassTransit] = builder => builder.AddMassTransitInstrumentation(),
        [TracerInstrumentation.Elasticsearch] = builder => builder.AddElasticsearchClientInstrumentation()
    };

    public static TracerProviderBuilder UseEnvironmentVariables(this TracerProviderBuilder builder, TracerSettings settings, ILogger logger = null)
    {
        builder.SetExporter(settings, logger);

        foreach (var enabledInstrumentation in settings.EnabledInstrumentations)
        {
            if (!AddInstrumentation.TryGetValue(enabledInstrumentation, out var addInstrumentation))
            {
                continue;
            }

            addInstrumentation(builder);

            logger?.Information($"Instrumentation {enabledInstrumentation} added");
        }

        builder.AddSource(settings.ActivitySources.ToArray());

        logger?.Information($"Sources [{string.Join(", ", settings.ActivitySources)}] added");

        foreach (var legacySource in settings.LegacySources)
        {
            builder.AddLegacySource(legacySource);

            logger?.Information($"Legacy source {legacySource} added");
        }

        return builder;
    }

    public static TracerProviderBuilder AddSdkAspNetInstrumentation(this TracerProviderBuilder builder)
    {
#if NET462
        return builder.AddAspNetInstrumentation();
#elif NETCOREAPP3_1_OR_GREATER
        return builder.AddAspNetCoreInstrumentation();
#endif
    }

    private static TracerProviderBuilder SetExporter(this TracerProviderBuilder builder, TracerSettings settings, ILogger logger = null)
    {
        if (settings.ConsoleExporterEnabled)
        {
            builder.AddConsoleExporter();

            logger?.Information("Console exporter added");
        }

        switch (settings.TracesExporter)
        {
            case TracesExporter.Zipkin:
                builder.AddZipkinExporter();

                logger?.Information("Zipkin exporter added");
                break;
            case TracesExporter.Jaeger:
                builder.AddJaegerExporter();

                logger?.Information("Jaeger exporter added");
                break;
            case TracesExporter.Otlp:
#if NETCOREAPP3_1
                if (settings.Http2UnencryptedSupportEnabled)
                {
                    // Adding the OtlpExporter creates a GrpcChannel.
                    // This switch must be set before creating a GrpcChannel/HttpClient when calling an insecure gRPC service.
                    // See: https://docs.microsoft.com/aspnet/core/grpc/troubleshoot#call-insecure-grpc-services-with-net-core-client
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
#endif
                builder.AddOtlpExporter(options =>
                {
                    if (settings.OtlpExportProtocol.HasValue)
                    {
                        options.Protocol = settings.OtlpExportProtocol.Value;
                    }
                });

                logger?.Information("OTLP exporter added");
                break;
            case TracesExporter.None:
                break;
            default:
                throw new ArgumentOutOfRangeException($"Traces exporter '{settings.TracesExporter}' is incorrect");
        }

        return builder;
    }
}

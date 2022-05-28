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
    private static readonly Dictionary<TracerInstrumentation, Action<TraceProviderBuildingContext>> AddInstrumentation = new()
    {
        [TracerInstrumentation.HttpClient] = context => context.Builder.AddHttpClientInstrumentation(),
        [TracerInstrumentation.AspNet] = context => context.AddSdkAspNetInstrumentation(),
        [TracerInstrumentation.SqlClient] = context => context.Builder.AddSqlClientInstrumentation(),
        [TracerInstrumentation.MongoDB] = context => context.Builder.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources"),
        [TracerInstrumentation.MassTransit] = context => context.Builder.AddMassTransitInstrumentation(),
        [TracerInstrumentation.Elasticsearch] = context => context.Builder.AddElasticsearchClientInstrumentation()
    };

    public static TracerProviderBuilder UseEnvironmentVariables(this TracerProviderBuilder builder, TracerSettings settings, ILogger logger = null)
    {
        return new TraceProviderBuildingContext(builder, settings, logger)
            .SetExporter()
            .EnableInstrumentations()
            .AddActivitySources()
            .AddLegacySources()
            .Builder;
    }

    private static TraceProviderBuildingContext AddLegacySources(this TraceProviderBuildingContext context)
    {
        foreach (var legacySource in context.Settings.LegacySources)
        {
            context.Builder.AddLegacySource(legacySource);

            context.Logger?.Information($"Legacy source {legacySource} added");
        }

        return context;
    }

    private static TraceProviderBuildingContext AddActivitySources(this TraceProviderBuildingContext context)
    {
        context.Builder.AddSource(context.Settings.ActivitySources.ToArray());

        context.Logger?.Information($"Sources [{string.Join(", ", context.Settings.ActivitySources)}] added");

        return context;
    }

    private static TraceProviderBuildingContext EnableInstrumentations(this TraceProviderBuildingContext context)
    {
        foreach (var enabledInstrumentation in context.Settings.EnabledInstrumentations)
        {
            if (!AddInstrumentation.TryGetValue(enabledInstrumentation, out var addInstrumentation))
            {
                continue;
            }

            addInstrumentation(context);

            context.Logger?.Information($"Instrumentation {enabledInstrumentation} processed");
        }

        return context;
    }

    private static void AddSdkAspNetInstrumentation(this TraceProviderBuildingContext context)
    {
#if NET462
        context.Builder.AddAspNetInstrumentation();
#elif NETCOREAPP3_1_OR_GREATER
        try
        {
            _ = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyName(new System.Reflection.AssemblyName("Microsoft.AspNetCore.Http.Abstractions"));

            context.Builder.AddAspNetCoreInstrumentation();
        }
        catch
        {
            context.Logger?.Warning("AspNetCore instrumentation not available");
        }
#endif
    }

    private static TraceProviderBuildingContext SetExporter(this TraceProviderBuildingContext context)
    {
        if (context.Settings.ConsoleExporterEnabled)
        {
            context.Builder.AddConsoleExporter();

            context.Logger?.Information("Console exporter added");
        }

        switch (context.Settings.TracesExporter)
        {
            case TracesExporter.Zipkin:
                context.Builder.AddZipkinExporter();

                context.Logger?.Information("Zipkin exporter added");
                break;
            case TracesExporter.Jaeger:
                context.Builder.AddJaegerExporter();

                context.Logger?.Information("Jaeger exporter added");
                break;
            case TracesExporter.Otlp:
#if NETCOREAPP3_1
                if (context.Settings.Http2UnencryptedSupportEnabled)
                {
                    // Adding the OtlpExporter creates a GrpcChannel.
                    // This switch must be set before creating a GrpcChannel/HttpClient when calling an insecure gRPC service.
                    // See: https://docs.microsoft.com/aspnet/core/grpc/troubleshoot#call-insecure-grpc-services-with-net-core-client
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
#endif
                context.Builder.AddOtlpExporter(options =>
                {
                    if (context.Settings.OtlpExportProtocol.HasValue)
                    {
                        options.Protocol = context.Settings.OtlpExportProtocol.Value;
                    }
                });

                context.Logger?.Information("OTLP exporter added");
                break;
            case TracesExporter.None:
                break;
            default:
                throw new ArgumentOutOfRangeException($"Traces exporter '{context.Settings.TracesExporter}' is incorrect");
        }

        return context;
    }

    private sealed class TraceProviderBuildingContext
    {
        public TraceProviderBuildingContext(TracerProviderBuilder builder, TracerSettings settings, ILogger logger)
        {
            Builder = builder ?? throw new ArgumentNullException(nameof(builder));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Logger = logger;
        }

        public TracerProviderBuilder Builder { get; }

        public TracerSettings Settings { get; }

        public ILogger Logger { get; }
    }
}

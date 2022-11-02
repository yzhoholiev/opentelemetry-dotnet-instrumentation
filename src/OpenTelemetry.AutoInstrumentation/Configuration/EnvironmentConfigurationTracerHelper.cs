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
using OpenTelemetry.Trace;

namespace OpenTelemetry.AutoInstrumentation.Configuration;

internal static class EnvironmentConfigurationTracerHelper
{
    private static readonly Dictionary<TracerInstrumentation, Action<TracerProviderBuilder, TracerSettings>> AddInstrumentation = new()
    {
        [TracerInstrumentation.HttpClient] = (builder, _) => builder.AddHttpClientInstrumentation(),
        [TracerInstrumentation.AspNet] = (builder, _) => builder.AddSdkAspNetInstrumentation(),
#if NETCOREAPP3_1_OR_GREATER
        [TracerInstrumentation.SqlClient] = (builder, settings) => builder.AddSqlClientInstrumentation(o =>
        {
            o.SetDbStatementForStoredProcedure = settings.SqlClientAddDbStatement;
            o.SetDbStatementForText = settings.SqlClientAddDbStatement;
        }),
        [TracerInstrumentation.MongoDB] = (builder, _) => builder.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources"),
        [TracerInstrumentation.MySqlData] = (builder, _) => builder.AddSource("OpenTelemetry.Instrumentation.MySqlData"),
        [TracerInstrumentation.StackExchangeRedis] = (builder, _) => builder.AddSource("OpenTelemetry.Instrumentation.StackExchangeRedis"),
#endif
#if NETFRAMEWORK
        [TracerInstrumentation.SqlClient] = (builder, settings) => builder.AddSqlClientInstrumentation(o => o.SetDbStatement = settings.SqlClientAddDbStatement),
#endif
        [TracerInstrumentation.Npgsql] = (builder, _) => builder.AddSource("Npgsql"),
        [TracerInstrumentation.GrpcNetClient] = (builder, _) => builder.AddGrpcClientInstrumentation(options => options.SuppressDownstreamInstrumentation = !Instrumentation.TracerSettings.EnabledInstrumentations.Contains(TracerInstrumentation.HttpClient)),
        [TracerInstrumentation.MassTransit] = (builder, _) => builder.AddMassTransitInstrumentation(),
        [TracerInstrumentation.Elasticsearch] = (builder, _) => builder.AddElasticsearchClientInstrumentation()
    };

    public static TracerProviderBuilder UseEnvironmentVariables(this TracerProviderBuilder builder, TracerSettings settings)
    {
        builder.SetExporter(settings);

        foreach (var enabledInstrumentation in settings.EnabledInstrumentations)
        {
            if (AddInstrumentation.TryGetValue(enabledInstrumentation, out var addInstrumentation))
            {
                addInstrumentation(builder, settings);
            }
        }

        builder.AddSource(settings.ActivitySources.ToArray());
        foreach (var legacySource in settings.LegacySources)
        {
            builder.AddLegacySource(legacySource);
        }

        return builder;
    }

    public static TracerProviderBuilder AddSdkAspNetInstrumentation(this TracerProviderBuilder builder)
    {
#if NET462
        builder.AddAspNetInstrumentation();
#elif NETCOREAPP3_1_OR_GREATER
        builder.AddSource("OpenTelemetry.Instrumentation.AspNetCore");
        builder.AddLegacySource("Microsoft.AspNetCore.Hosting.HttpRequestIn");
#endif

        return builder;
    }

    private static TracerProviderBuilder SetExporter(this TracerProviderBuilder builder, TracerSettings settings)
    {
        if (settings.ConsoleExporterEnabled)
        {
            builder.AddConsoleExporter();
        }

        switch (settings.TracesExporter)
        {
            case TracesExporter.Zipkin:
                builder.AddZipkinExporter();
                break;
            case TracesExporter.Jaeger:
                builder.AddJaegerExporter();
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
                break;
            case TracesExporter.None:
                break;
            default:
                throw new ArgumentOutOfRangeException($"Traces exporter '{settings.TracesExporter}' is incorrect");
        }

        return builder;
    }
}

// <copyright file="EnvironmentConfigurationMetricHelper.cs" company="OpenTelemetry Authors">
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
using OpenTelemetry.Metrics;

namespace OpenTelemetry.AutoInstrumentation.Configuration;

internal static class EnvironmentConfigurationMetricHelper
{
    private static readonly Dictionary<MeterInstrumentation, Action<MeterProviderBuildingContext>> AddMeter = new()
    {
        [MeterInstrumentation.AspNet] = static context => context.AddSdkAspNetInstrumentation(),
        [MeterInstrumentation.HttpClient] = static context => context.Builder.AddHttpClientInstrumentation(),
        [MeterInstrumentation.NetRuntime] = static context => context.Builder.AddRuntimeMetrics(),
    };

    public static MeterProviderBuilder UseEnvironmentVariables(this MeterProviderBuilder builder, MeterSettings settings, ILogger logger = null)
    {
        return new MeterProviderBuildingContext(builder, settings, logger)
            .SetExporter()
            .EnableInstrumentations()
            .AddMeters()
            .Builder;
    }

    private static MeterProviderBuildingContext EnableInstrumentations(this MeterProviderBuildingContext context)
    {
        context.Logger.Information($"Meters [{string.Join(", ", context.Settings.Meters)}] added");

        foreach (var enabledMeter in context.Settings.EnabledInstrumentations)
        {
            if (!AddMeter.TryGetValue(enabledMeter, out var addMeter))
            {
                continue;
            }

            addMeter(context);

            context.Logger.Information($"Meter {enabledMeter} processed");
        }

        return context;
    }

    private static void AddSdkAspNetInstrumentation(this MeterProviderBuildingContext context)
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
            context.Logger.Warning("AspNetCore instrumentation not available");
        }
#endif
    }

    private static MeterProviderBuildingContext SetExporter(this MeterProviderBuildingContext context)
    {
        if (context.Settings.ConsoleExporterEnabled)
        {
            context.Builder.AddConsoleExporter();

            context.Logger.Information("Console exported added");
        }

        switch (context.Settings.MetricExporter)
        {
            case MetricsExporter.Prometheus:
                context.Builder.AddPrometheusExporter(options => { options.StartHttpListener = true; });

                context.Logger.Information("Prometheus exporter added");
                break;
            case MetricsExporter.Otlp:
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

                context.Logger.Information("OTLP exporter added");
                break;
            case MetricsExporter.None:
                break;
            default:
                throw new ArgumentOutOfRangeException($"Metrics exporter '{context.Settings.MetricExporter}' is incorrect");
        }

        return context;
    }

    private static MeterProviderBuildingContext AddMeters(this MeterProviderBuildingContext context)
    {
        context.Builder.AddMeter(context.Settings.Meters.ToArray());

        context.Logger.Information($"Meters [{string.Join(", ", context.Settings.Meters)}] added");

        return context;
    }

    private sealed class MeterProviderBuildingContext : ProviderBuildingContext<MeterProviderBuilder, MeterSettings>
    {
        public MeterProviderBuildingContext(MeterProviderBuilder builder, MeterSettings settings, ILogger logger)
            : base(builder, settings, logger)
        {
        }
    }
}

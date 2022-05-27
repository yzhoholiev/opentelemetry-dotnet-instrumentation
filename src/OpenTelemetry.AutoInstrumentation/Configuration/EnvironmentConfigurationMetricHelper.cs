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
    private static readonly Dictionary<MeterInstrumentation, Action<MeterProviderBuilder>> AddMeters = new()
    {
        [MeterInstrumentation.NetRuntime] = builder => builder.AddRuntimeMetrics(),
    };

    public static MeterProviderBuilder UseEnvironmentVariables(this MeterProviderBuilder builder, MeterSettings settings, ILogger logger = null)
    {
        builder
            .SetExporter(settings, logger)
            .AddMeter(settings.Meters.ToArray());

        logger?.Information($"Meters [{string.Join(", ", settings.Meters)}] added");

        foreach (var enabledMeter in settings.EnabledInstrumentation)
        {
            if (!AddMeters.TryGetValue(enabledMeter, out var addMeter))
            {
                continue;
            }

            addMeter(builder);

            logger?.Information($"Meter {enabledMeter} added");
        }

        return builder;
    }

    private static MeterProviderBuilder SetExporter(this MeterProviderBuilder builder, MeterSettings settings, ILogger logger = null)
    {
        if (settings.ConsoleExporterEnabled)
        {
            builder.AddConsoleExporter();

            logger?.Information("Console exported added");
        }

        return builder;
    }
}

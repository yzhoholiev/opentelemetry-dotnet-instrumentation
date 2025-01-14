// <copyright file="RuntimeTests.cs" company="OpenTelemetry Authors">
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

using System.Threading.Tasks;
using IntegrationTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests;

public class RuntimeTests : TestHelper
{
    public RuntimeTests(ITestOutputHelper output)
        : base("Smoke", output)
    {
        SetEnvironmentVariable("LONG_RUNNING", "true");
        SetEnvironmentVariable("OTEL_METRIC_EXPORT_INTERVAL", "100");
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task SubmitMetrics()
    {
        using var collector = await MockMetricsCollector.Start(Output);
        collector.Expect("OpenTelemetry.Instrumentation.Runtime");

        using var process = StartTestApplication(metricsAgentPort: collector.Port, enableClrProfiler: !IsCoreClr());
        try
        {
            collector.AssertExpectations();
        }
        finally
        {
            process.Kill();
        }
    }
}

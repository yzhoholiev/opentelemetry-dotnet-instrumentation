// <copyright file="NpqsqlTests.cs" company="OpenTelemetry Authors">
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
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests;

[Collection(PostgresCollection.Name)]
public class NpqsqlTests : TestHelper
{
    private const string ServiceName = "TestApplication.Npgsql";
    private readonly PostgresFixture _postgres;

    public NpqsqlTests(ITestOutputHelper output, PostgresFixture postgres)
        : base("Npgsql", output)
    {
        _postgres = postgres;
        SetEnvironmentVariable("OTEL_SERVICE_NAME", ServiceName);
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    [Trait("Containers", "Linux")]
    public async Task SubmitsTraces()
    {
        using var agent = await MockZipkinCollector.Start(Output);

        RunTestApplication(agent.Port, arguments: $"--postgres {_postgres.Port}", enableClrProfiler: !IsCoreClr());
        var spans = await agent.WaitForSpansAsync(1);

        spans.Count.Should().Be(1);
        spans.First().Tags["db.statement"].Should().Be("SELECT 123;");
    }
}

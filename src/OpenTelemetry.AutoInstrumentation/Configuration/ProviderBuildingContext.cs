// <copyright file="ProviderBuildingContext.cs" company="OpenTelemetry Authors">
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

#nullable enable

using System;
using OpenTelemetry.AutoInstrumentation.Logging;

namespace OpenTelemetry.AutoInstrumentation.Configuration;

internal abstract class ProviderBuildingContext<TBuilder, TSettings>
    where TBuilder : class
    where TSettings : Settings
{
    protected ProviderBuildingContext(TBuilder builder, TSettings settings, ILogger logger)
    {
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public TBuilder Builder { get; }

    public TSettings Settings { get; }

    public ILogger Logger { get; }
}

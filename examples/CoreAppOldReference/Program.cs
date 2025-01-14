// <copyright file="Program.cs" company="OpenTelemetry Authors">
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
using Examples.OldReference;

namespace Examples.CoreAppOldReference;

public class Program
{
    public static async Task Main(string[] args)
    {
        await InstrumentedHttpCall.GetAsync("https://www.google.com");
        await InstrumentedHttpCall.GetAsync("http://127.0.0.1:8080/api/mongo");
        await InstrumentedHttpCall.GetAsync("http://127.0.0.1:8080/api/redis");
    }
}

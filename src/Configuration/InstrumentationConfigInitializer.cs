/*
 * Bastille.ID Identity Server
 * (c) Copyright Talegen, LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace Bastille.Id.Core.Configuration
{
    using System.Reflection;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// This class implements a Microsoft Instrumentation Telemetry Initializer instance.
    /// </summary>
    public class InstrumentationConfigInitializer : ITelemetryInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentationConfigInitializer" /> class.
        /// </summary>
        public InstrumentationConfigInitializer()
        {
        }

        /// <summary>
        /// This method is called to initialize the instrumentation telemetry configuration.
        /// </summary>
        /// <param name="telemetry">Contains the telemetry object that is being initialized.</param>
        public void Initialize(ITelemetry telemetry)
        {
            // add the application assembly version to the component version stamp
            telemetry.Context.Component.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
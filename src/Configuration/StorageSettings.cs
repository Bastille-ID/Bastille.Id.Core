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
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains an enumerated list of supported background worker types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StorageType
    {
        /// <summary>
        /// Uses memory to act as a virtual disk storage for unit testing.
        /// </summary>
        Memory,

        /// <summary>
        /// Uses the local web server disk storage for working with and ultimately storing files.
        /// </summary>
        Local,

        /// <summary>
        /// Uses the Azure cloud storage mechanisms for storing files.
        /// </summary>
        Azure
    }

    /// <summary>
    /// Contains storage settings.
    /// </summary>
    public class StorageSettings
    {
        /// <summary>
        /// Gets or sets the type of the storage.
        /// </summary>
        /// <value>The type of the storage.</value>
        public StorageType StorageType { get; set; } = StorageType.Local;

        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        /// <value>The root path.</value>
        public string RootPathUri { get; set; }

        /// <summary>
        /// Gets or sets a dictionary array of configuration settings for an external storage service.
        /// </summary>
        /// <value>
        /// The dictionary contains a key (configuration setting) and value (configuration value) parameter for one or more unique settings for the storage implementation.
        /// </value>
        public Dictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>();
    }
}
﻿/*
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
    using System;

    /// <summary>
    /// This class represents the user and group synchronization settings for the server.
    /// </summary>
    public class SynchronizationSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SynchronizationSettings" /> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; } = "scim_client";

        /// <summary>
        /// Gets or sets the resource URI.
        /// </summary>
        /// <value>The resource URI.</value>
        public Uri ResourceUri { get; set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        /// <value>The secret.</value>
        public string Secret { get; set; }
    }
}
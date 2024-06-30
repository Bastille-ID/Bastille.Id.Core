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

namespace Bastille.Id.Core.Identity
{
    using Duende.IdentityServer.EntityFramework.DbContexts;
    using Duende.IdentityServer.Stores;

    /// <summary>
    /// This class implements the base properties for identity related service classes.
    /// </summary>
    /// <seealso cref="Bastille.Id.Core.BaseServiceContext" />
    public class IdentityServiceContext : BaseServiceContext
    {
        /// <summary>
        /// Gets or sets the client store.
        /// </summary>
        /// <value>The client store.</value>
        public IClientStore ClientStore { get; set; }

        /// <summary>
        /// Gets or sets the resource store.
        /// </summary>
        /// <value>The resource store.</value>
        public IResourceStore ResourceStore { get; set; }

        /// <summary>
        /// Gets or sets the configuration database context.
        /// </summary>
        /// <value>The configuration database context.</value>
        public ConfigurationDbContext ConfigurationDbContext { get; set; }
    }
}
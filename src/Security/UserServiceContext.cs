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

namespace Bastille.Id.Core.Security
{
    using Bastille.Id.Core.Configuration;
    using IdentityServer4.Stores;
    using Talegen.AspNetCore.Web.Extensions;

    /// <summary>
    /// This class contains properties for the user service class to utilize during operation.
    /// </summary>
    public sealed class UserServiceContext : BaseServiceContext
    {
        /// <summary>
        /// Contains The base URL.
        /// </summary>
        private string baseUrl;

        /// <summary>
        /// Gets or sets the profile settings.
        /// </summary>
        /// <value>The profile settings.</value>
        public ProfileSettings ProfileSettings { get; set; }

        /// <summary>
        /// Gets or sets the security service.
        /// </summary>
        /// <value>The security service.</value>
        public SecurityService SecurityService { get; set; }

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
        /// Gets the current user time zone information. If the user has no time zone claim, time zone is set to a default of UTC.
        /// </summary>
        /// <returns>Returns a value indicating the time zone information for the logged-in user.</returns>
        public string CurrentUserTimezone => this.Principal.TimeZone() ?? SecurityDefaults.DefaultUserTimeZone;

        /// <summary>
        /// Gets the current user locale information. If the user has no time zone claim, locale is set to a default English
        /// </summary>
        /// <returns>Returns a value indicating the locale for the logged-in user.</returns>
        public string CurrentUserLocale => this.Principal.Locale() ?? SecurityDefaults.DefaultUserLocale;

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        /// <value>The base URL.</value>
        public string BaseUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.baseUrl))
                {
                    this.baseUrl = string.Format("{0}://{1}", this.HttpContext.Request?.Scheme, this.HttpContext.Request?.Host);
                }

                return this.baseUrl;
            }

            set
            {
                this.baseUrl = value;
            }
        }
    }
}
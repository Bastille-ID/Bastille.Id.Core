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

namespace Bastille.Id.Core
{
    using Bastille.Id.Core.Data;
    using Microsoft.Extensions.Options;
    using Talegen.Common.Core.Errors;

    /// <summary>
    /// Contains an application context.
    /// </summary>
    public class ApplicationContext<TSettings>
        where TSettings : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext{TSettings}" /> class.
        /// </summary>
        /// <param name="settings">Contains an instance of server settings.</param>
        /// <param name="dataContext">Contains the database context.</param>
        /// <param name="errorManager">Contains an instance of error manager.</param>
        public ApplicationContext(IOptionsSnapshot<TSettings> settings, ApplicationDbContext dataContext, IErrorManager errorManager)
        {
            this.Settings = settings?.Value;
            this.DataContext = dataContext;
            this.ErrorManager = errorManager;
        }

        /// <summary>
        /// Gets the server settings.
        /// </summary>
        public TSettings Settings { get; }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        public ApplicationDbContext DataContext { get; }

        /// <summary>
        /// Gets the error manager.
        /// </summary>
        /// <value>The error manager.</value>
        public IErrorManager ErrorManager { get; }

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>The name of the host.</value>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the current user identifier.
        /// </summary>
        /// <value>The current user identifier.</value>
        public string CurrentUserId { get; set; }
    }
}
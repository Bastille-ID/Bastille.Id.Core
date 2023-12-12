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
    using System;
    using System.Security.Claims;
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using Talegen.AspNetCore.Web.Extensions;
    using Bastille.Id.Core.Logging;
    using Bastille.Id.Core.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Caching.Distributed;
    using Talegen.Common.Core.Errors;
    using Talegen.Common.Core.Extensions;
    using Vasont.AspnetCore.RedisClient;
    using Bastille.Id.Core.Extensions;

    /// <summary>
    /// This abstract class provides commonly used properties for service business logic context classes.
    /// </summary>
    public abstract class BaseServiceContext
    {
        /// <summary>
        /// Contains the current user's identity value.
        /// </summary>
        private Guid currentUserId;

        /// <summary>
        /// Contains the current user's user name.
        /// </summary>
        private string currentUserName;

        /// <summary>
        /// Gets or sets the audit log.
        /// </summary>
        /// <value>The audit log.</value>
        public AuditLogService AuditLog { get; set; }

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public ApplicationDbContext DataContext { get; set; }

        /// <summary>
        /// Contains an instance of distributed cache options.
        /// </summary>
        public DistributedCacheEntryOptions CacheEntryOptions { get; } = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(SecurityDefaults.DefaultExpirationMinutes));

        /// <summary>
        /// Gets or sets the principal.
        /// </summary>
        /// <value>The principal.</value>
        public ClaimsPrincipal Principal { get; set; }

        /// <summary>
        /// Gets or sets the user manager.
        /// </summary>
        /// <value>The user manager.</value>
        public UserManager<User> UserManager { get; set; }

        /// <summary>
        /// Gets or sets the cache.
        /// </summary>
        /// <value>The cache.</value>
        public IAdvancedDistributedCache Cache { get; set; }

        /// <summary>
        /// Gets or sets the error manager.
        /// </summary>
        /// <value>The error manager.</value>
        public IErrorManager ErrorManager { get; set; }

        /// <summary>
        /// Gets or sets the HTTP context.
        /// </summary>
        /// <value>The HTTP context.</value>
        public HttpContext HttpContext { get; set; }

        /// <summary>
        /// Gets the current user identity value.
        /// </summary>
        public Guid CurrentUserId
        {
            get
            {
                if (this.currentUserId == Guid.Empty && this.Principal != null)
                {
                    // get subject
                    var id = this.Principal.SubjectId();
                    this.currentUserId = !string.IsNullOrEmpty(id) ? new Guid(id) : this.UserManager.GetUserId(this.Principal).ToGuid();
                }

                return this.currentUserId;
            }
        }

        /// <summary>
        /// Gets the optional current user identifier.
        /// </summary>
        /// <value>The optional current user identifier.</value>
        public Guid? OptionalCurrentUserId
        {
            get
            {
                Guid? result = null;

                if (this.CurrentUserId != Guid.Empty)
                {
                    result = this.CurrentUserId;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the client address.
        /// </summary>
        /// <value>The client address.</value>
        public string ClientAddress => this.HttpContext?.Connection?.RemoteIpAddress.ToString() ?? string.Empty;

        /// <summary>
        /// Gets the current user name.
        /// </summary>
        public string CurrentUserName
        {
            get
            {
                if (string.IsNullOrEmpty(this.currentUserName))
                {
                    this.currentUserName = this.Principal.UserName();
                    if (string.IsNullOrWhiteSpace(this.currentUserName))
                    {
                        this.currentUserName = this.UserManager.GetUserName(this.Principal);
                    }
                }

                return this.currentUserName;
            }
        }
    }
}
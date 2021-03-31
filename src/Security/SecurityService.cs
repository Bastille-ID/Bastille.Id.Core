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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Data.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Core.Errors;
    using Talegen.Common.Models.Security;
    using Vasont.AspnetCore.RedisClient;

    /// <summary>
    /// This class contains security related business logic.
    /// </summary>
    public class SecurityService
    {
        /// <summary>
        /// Contains an instance of the security service context.
        /// </summary>
        private readonly SecurityServiceContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityService" /> class.
        /// </summary>
        /// <param name="context">Contains an instance of the security service context.</param>
        public SecurityService(SecurityServiceContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a user to the main administration role.
        /// </summary>
        /// <param name="userToAdd">The user to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a value indicating success.</returns>
        public async Task<bool> AddToAdminRoleAsync(User userToAdd, CancellationToken cancellationToken)
        {
            string adminKey = string.Format(SecurityDefaults.IsAdminCacheKeyTemplate, userToAdd.Id);

            // remove any cached value
            await this.context.Cache.RemoveAsync(adminKey, cancellationToken);

            // add the user
            IdentityResult result = await this.context.UserManager.AddToRoleAsync(userToAdd, SecurityDefaults.AdministratorRoleName);

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(err =>
                {
                    this.context.ErrorManager.CriticalFormat("{0}: {1}", ErrorCategory.Security, err.Code, err.Description);
                });
            }

            return result.Succeeded;
        }

        /// <summary>
        /// Removes the specified user from the main administration role.
        /// </summary>
        /// <param name="userToRemove">The user to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a value indicating success.</returns>
        public async Task<bool> RemoveFromAdminRoleAsync(User userToRemove, CancellationToken cancellationToken)
        {
            string adminKey = string.Format(SecurityDefaults.IsAdminCacheKeyTemplate, userToRemove.Id);

            // add the user
            IdentityResult result = await this.context.UserManager.RemoveFromRoleAsync(userToRemove, SecurityDefaults.AdministratorRoleName);

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(err =>
                {
                    this.context.ErrorManager.CriticalFormat("{0}: {1}", ErrorCategory.Security, err.Code, err.Description);
                });
            }

            return result.Succeeded;
        }

        /// <summary>
        /// Determines if the current user can read the contents of the specified user identity.
        /// </summary>
        /// <param name="userId">Contains the user identity to read.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a value indicating whether the current user can read the specified user.</returns>
        public async Task<bool> CanReadUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return this.context.CurrentUserId == userId ||
                await this.IsUserAdminAsync(this.context.CurrentUserId, cancellationToken).ConfigureAwait(false) ||
                await this.CanManageUserAsync(userId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// This method is used to retrieve a claim value from the identity.
        /// </summary>
        /// <param name="claimType">Contains the claim value to retrieve.</param>
        /// <returns>Returns the value of the claim if found.</returns>
        public string GetClaimValue(string claimType)
        {
            string result = string.Empty;

            if (this.context.Principal.Identity.IsAuthenticated)
            {
                var claim = this.context.Principal.Claims.FirstOrDefault(c => c.Type.Equals(claimType, StringComparison.InvariantCultureIgnoreCase));

                if (claim != null)
                {
                    result = claim.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to determine if the current user has the permission to manage the specified user.
        /// </summary>
        /// <param name="userId">Contains the identity of the user to manage.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a value indicating whether the current user can manage the details of the specified user.</returns>
        public async Task<bool> CanManageUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return this.context.CurrentUserId == userId
                || await this.context.DataContext.GroupUsers.AnyAsync(ou => ou.UserId == this.context.CurrentUserId && ou.Group.OwnerUserId == this.context.CurrentUserId && ou.Group.Members.Any(u => u.UserId == userId), cancellationToken).ConfigureAwait(false)
                || await this.IsUserAdminAsync(this.context.CurrentUserId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Determines whether the current user can remove the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a value indicating whether the current user can remove the specified user.</returns>
        public async Task<bool> CanRemoveUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            bool currentUserAdminSupport = await this.IsUserAdminAsync(this.context.CurrentUserId);
            bool userToRemoveAdminSupport = await this.IsUserAdminAsync(userId);

            // current user can manage target user and target user is not admin/support or is and current user is admin/support.
            return await CanManageUserAsync(userId, cancellationToken) && (!userToRemoveAdminSupport || currentUserAdminSupport);
        }

        /// <summary>
        /// This method is used to determine if the specified user is an administrator.
        /// </summary>
        /// <param name="userId">Contains the unique user identity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a value indicating whether the user is an administrator.</returns>
        public async Task<bool> IsUserAdminAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            string adminKey = string.Format(SecurityDefaults.IsAdminCacheKeyTemplate, userId);

            // try and get the value out of distributed cache.
            bool? result = await this.context.Cache.GetJsonAsync<bool?>(adminKey, cancellationToken);

            // value wasnt in cache...
            if (!result.HasValue)
            {
                // TODO: check role claim. if failed to get value, get it from the database. find if the current user is in a role named Administrators
                result = await this.context.DataContext.UserRoles
                    .AsNoTracking()
                    .Join(this.context.DataContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { UserRole = ur, Role = r })
                    .AnyAsync(ur => ur.UserRole.UserId == userId && ur.Role.RoleType == RoleTypes.System && ur.Role.Name == SecurityDefaults.AdministratorRoleName, cancellationToken)
                    .ConfigureAwait(false);

                // save whatever the result in distributed cache.
                await this.context.Cache?.SetJsonAsync(adminKey, result, this.context.CacheEntryOptions, cancellationToken);
            }

            return result.Value;
        }

        /// <summary>
        /// This method is used to determine if the specified user is a member of the specified group.
        /// </summary>
        /// <param name="userId">The user identity to match.</param>
        /// <param name="groupId">The group identity to match.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns a value indicating whether the specified user is a member of the specified group.</returns>
        public async Task<bool> CanAccessGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
        {
            return await this.context.DataContext.GroupUsers.AnyAsync(o => o.GroupId == groupId && o.UserId == userId, cancellationToken).ConfigureAwait(false)
                || await this.IsUserAdminAsync(userId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// This method is used to determine if the specified user is a manager of the specified group.
        /// </summary>
        /// <param name="userId">The user identity to match.</param>
        /// <param name="groupId">The group identity to match.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns a value indicating whether the specified user is a manager of the specified group.</returns>
        public async Task<bool> CanManageGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
        {
            return await this.context.DataContext.GroupUsers.AnyAsync(o => o.GroupId == groupId && o.UserId == userId && o.Group.OwnerUserId == userId, cancellationToken).ConfigureAwait(false)
                || await this.IsUserAdminAsync(userId, cancellationToken).ConfigureAwait(false);
        }
    }
}
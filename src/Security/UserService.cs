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
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using Bastille.Id.Core.Extensions;
    using Bastille.Id.Core.Properties;
    using Bastille.Id.Models.Logging;
    using Bastille.Id.Models.Organization;
    using Bastille.Id.Models.Security;
    using IdentityModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Core.Errors;
    using Talegen.Common.Core.Extensions;
    using Talegen.Common.Models.Security.Queries;
    using Talegen.Common.Models.Server.Queries;

    /// <summary>
    /// This class contains methods for querying the application database for user related data and queries.
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// The user service context
        /// </summary>
        private readonly UserServiceContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="context">Contains the user service context.</param>
        public UserService(UserServiceContext context)
        {
            this.context = context;
        }

        #region Public Methods

        #region User Model Methods

        /// <summary>
        /// This method is used to execute a paginated query of user records.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a <see cref="PaginatedQueryResultModel{BastilleUserModel}" /> paginated result model.</returns>
        public async Task<PaginatedQueryResultModel<BastilleUserModel>> BrowseAsync(UserQueryFilterModel filter, CancellationToken cancellationToken)
        {
            IQueryable<BastilleUserModel> query = this.context.DataContext.Users
                .AsNoTracking()
                .Select(u => new BastilleUserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.UserName,
                    TimeZone = u.Claims.Where(c => c.ClaimType == JwtClaimTypes.ZoneInfo).Select(c => c.ClaimValue).FirstOrDefault(),
                    Locale = u.Claims.Where(c => c.ClaimType == JwtClaimTypes.Locale).Select(c => c.ClaimValue).FirstOrDefault(),
                    FirstName = u.Claims.Where(c => c.ClaimType == JwtClaimTypes.GivenName).Select(c => c.ClaimValue).FirstOrDefault(),
                    LastName = u.Claims.Where(c => c.ClaimType == JwtClaimTypes.FamilyName).Select(c => c.ClaimValue).FirstOrDefault(),
                    Picture = u.Claims.Where(c => c.ClaimType == JwtClaimTypes.Picture).Select(c => c.ClaimValue).FirstOrDefault(),
                    IsEmailConfirmed = u.EmailConfirmed,
                    Phone = u.PhoneNumber,
                    LockoutEnd = u.LockoutEnd,
                    Deletable = u.Id == this.context.CurrentUserId,
                    Active = u.Active
                })
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                query = query.Where(g => g.Name.Contains(filter.SearchText) ||
                g.Email.Contains(filter.SearchText) ||
                g.FirstName.Contains(filter.SearchText) ||
                g.LastName.Contains(filter.SearchText) ||
                g.Phone.Contains(filter.SearchText) ||
                g.Locale.Contains(filter.SearchText) ||
                g.TimeZone.Contains(filter.SearchText));
            }

            return await BrowseQueryHelper.ExecutePagedQueryAsync(query, filter, nameof(User.UserName), cancellationToken);
        }

        /// <summary>
        /// Reads the user specified.
        /// </summary>
        /// <param name="userId">Contains the user unique identifier.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="User" /> entity if found.</returns>
        public async Task<User> ReadUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            User userFound = await this.context.DataContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken).ConfigureAwait(false);

            if (userFound == null)
            {
                this.context.ErrorManager.CriticalNotFoundFormat(Resources.ErrorUserIdentityNotFoundText, ErrorCategory.General, userId);
            }

            return userFound;
        }

        /// <summary>
        /// Creates a new user specified.
        /// </summary>
        /// <param name="model">The user model.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="BastilleUserModel" /> created.</returns>
        public async Task<BastilleUserModel> CreateNewUserAsync(BastilleUserModel model, CancellationToken cancellationToken)
        {
            // create a new user record...
            User newUser = new User { UserName = model.Email, Email = model.Email, CreatedDate = DateTime.UtcNow };
            IdentityResult result = await this.context.UserManager.CreateAsync(newUser, model.Password);

            // if successful...
            if (result.Succeeded)
            {
                // set lock out end date
                await this.context.UserManager.SetLockoutEndDateAsync(newUser, model.LockoutEnd);

                // create a new list of claims from model.
                var claims = model.ToClaims(this.context.BaseUrl);

                // add claims to new user record
                result = await this.context.UserManager.AddClaimsAsync(newUser, claims);

                if (result.Succeeded)
                {
                    // Update the admin role if needed.
                    await this.UpdateUserRoleAsync(newUser, model, cancellationToken);

                    // add groups to new user record
                    await this.ProcessGroupsAddedAsync(model, newUser, cancellationToken);

                    // save group changes...
                    if (await this.SaveAsync(newUser, EntityState.Modified, cancellationToken))
                    {
                        // if the save was successful, enqueue the worker to sync user details out to subscribed applications
                        //BackgroundJob.Enqueue(() => this.ProcessUserAdd(newUser, claims.Select(c => new ClaimTypeValuePair { Name = c.Type, Value = c.Value }).ToList(), syncUserType));
                    }
                    else
                    {
                        // warning, groups not added. User will be orphaned. Notify System Admin.
                    }
                }
                else
                {
                    // warning, claims not added.
                }

                // log the completion of the user creation.
                string message = string.Format(Resources.PromptUserCreatedSuccessText, newUser.Email, newUser.Id);
                await this.context.AuditLog.LogAsync(AuditEvent.Profile, AuditResult.Success, string.Empty, message, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                string message = string.Format(Resources.PromptUserCreatedFailedText, newUser.Email);

                // user creation failed. Log why.
                result.Errors.ToList().ForEach(err =>
                {
                    // the user creation command failed. Report errors.
                    this.context.ErrorManager.CriticalFormat("{0}: {1}", ErrorCategory.Application, err.Code, err.Description);
                });

                await this.context.AuditLog.LogAsync(AuditEvent.Profile, AuditResult.Fail, string.Empty, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Updates the user specified.
        /// </summary>
        /// <param name="model">The user model.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="BastilleUserModel" /> model updated.</returns>
        public async Task<BastilleUserModel> UpdateUserAsync(BastilleUserModel model, CancellationToken cancellationToken)
        {
            User updatedUser = await this.context.UserManager.FindByIdAsync(model.Id.ToString());

            if (updatedUser != null)
            {
                // Update this users profile information
                model = await this.UpdateUserFromModelAsync(updatedUser, model);

                // Update this users Claim records
                await this.UpdateUserClaimsAsync(updatedUser, model.ToClaims(this.context.BaseUrl));

                // Update the admin role if needed.
                await this.UpdateUserRoleAsync(updatedUser, model, cancellationToken);

                // Update this users group information
                model = await this.UpdateUserGroupsAsync(model, cancellationToken);

                // save changes to the user...
                if (await this.SaveAsync(updatedUser, EntityState.Modified, cancellationToken))
                {
                    // if the save was successful, enqueue the worker to sync
                    //BackgroundJob.Enqueue(() => this.ProcessUserUpdate(updatedUser, claims.Select(c => new ClaimTypeValuePair { Name = c.Type, Value = c.Value }).ToList(), syncUserType));
                    string message = string.Format(Resources.PromptUserUpdatedSuccessText, updatedUser.Email, updatedUser.Id);
                    await this.context.AuditLog.LogAsync(AuditEvent.Profile, AuditResult.Success, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    string message = string.Format(Resources.PromptUserUpdatedFailedText, updatedUser.Email, updatedUser.Id);
                    await this.context.AuditLog.LogAsync(AuditEvent.Profile, AuditResult.Fail, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken).ConfigureAwait(false);
                    this.context.ErrorManager.Critical(message, ErrorCategory.Application);
                }
            }
            else
            {
                this.context.ErrorManager.CriticalNotFound(Resources.ErrorUserNotFoundText);
            }

            return model;
        }

        /// <summary>
        /// Deletes the user specified.
        /// </summary>
        /// <param name="id">Contains the user unique identifier.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
        {
            User userToDelete = await this.context.UserManager.FindByIdAsync(id.ToString());

            // we found the user to delete...
            if (userToDelete != null)
            {
                // get group user associations to remove user from
                var groupUsers = await this.context.DataContext.GroupUsers
                    .Include(gu => gu.Group)
                    .Where(gu => gu.UserId == id)
                    .ToListAsync(cancellationToken);

                // delete the user from groups
                this.context.DataContext.GroupUsers.RemoveRange(groupUsers);

                // save group removal changes
                if (groupUsers.Count == 0 || await this.SaveAsync(userToDelete, EntityState.Deleted, cancellationToken))
                {
                    // if this was successful... remove the user.
                    IdentityResult result = await this.context.UserManager.DeleteAsync(userToDelete);

                    if (result.Succeeded)
                    {
                        string message = string.Format(Resources.PromptUserDeletedSuccessText, userToDelete.Email, userToDelete.Id);
                        await this.context.AuditLog.LogAsync(AuditEvent.Profile, AuditResult.Success, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken).ConfigureAwait(false);

                        // if the save was successful, enqueue the worker to sync
                        //BackgroundJob.Enqueue(() => this.ProcessUserDelete(id));
                    }
                    else
                    {
                        string message = string.Format(Resources.PromptUserDeletedFailedText, userToDelete.Email, userToDelete.Id);
                        await this.context.AuditLog.LogAsync(AuditEvent.Profile, AuditResult.Fail, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken).ConfigureAwait(false);

                        result.Errors.ToList().ForEach(err =>
                        {
                            this.context.ErrorManager.CriticalFormat("{0}: {1}", ErrorCategory.Application, err.Code, err.Description);
                        });
                    }
                }
            }
            else
            {
                this.context.ErrorManager.CriticalNotFound(Resources.ErrorUserNotFoundText, ErrorCategory.Application);
            }
        }

        #endregion

        #region Models & Claims Methods

        /// <summary>
        /// Builds the claims list for the profile model specified.
        /// </summary>
        /// <param name="id">Contains the user unique identifier.</param>
        /// <param name="model">The profile model.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a list of <see cref="Claim" /> models.</returns>
        public async Task<List<Claim>> BuildClaimsListAsync(Guid id, ProfileModel model, CancellationToken cancellationToken)
        {
            // create new model claims list
            List<Claim> modelClaims = new List<Claim>();

            // get all existing claims for the specified user
            var userClaims = await this.context.DataContext.UserClaims.Where(uc => uc.UserId == id).ToListAsync(cancellationToken).ConfigureAwait(false);

            // remove existing claims for the specified user.
            this.context.DataContext.UserClaims.RemoveRange(this.context.DataContext.UserClaims.Where(uc => uc.UserId == id));

            // for each claim in the model...
            foreach (string claimKey in model.Claims.Keys)
            {
                // add the claims to the database user claims
                modelClaims.Add(new Claim(claimKey, model.Claims[claimKey]));
                this.context.DataContext.UserClaims.Add(new IdentityUserClaim<Guid>
                {
                    ClaimType = claimKey,
                    UserId = id,
                    ClaimValue = model.Claims[claimKey]
                });
            }

            return modelClaims;
        }

        #endregion

        #region User Profile Methods

        /// <summary>
        /// Finds the user profile model for the specified user identifier.
        /// </summary>
        /// <param name="id">Contains the user unique identifier.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a <see cref="ProfileModel" /> if found.</returns>
        public async Task<ProfileModel> FindUserProfileModelAsync(Guid id, CancellationToken cancellationToken)
        {
            ProfileModel model = null;

            // determine if user has permission
            if (await this.context.SecurityService.CanReadUserAsync(id, cancellationToken).ConfigureAwait(false))
            {
                var userFound = await this.ReadUserAsync(id, cancellationToken).ConfigureAwait(false);

                if (userFound != null)
                {
                    model = await this.ConvertToProfileAsync(userFound, cancellationToken).ConfigureAwait(false);
                }
            }
            else if (!await this.UserExistsAsync(id, cancellationToken).ConfigureAwait(false))
            {
                this.context.ErrorManager.CriticalNotFoundFormat(Resources.ErrorUserIdentityNotFoundText, ErrorCategory.General, id);
            }
            else
            {
                this.context.ErrorManager.CriticalForbidden(Resources.ErrorAccessDeniedText, ErrorCategory.Security);
            }

            return model;
        }

        /// <summary>
        /// Returns a value indicating whether the user record exists.
        /// </summary>
        /// <param name="userId">Contains the user identity to find.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a value indicating whether the user exists.</returns>
        public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await this.context.DataContext.Users.AnyAsync(u => u.Id == userId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the profile record.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a value indicating success.</returns>
        public async Task<bool> UpdateProfileAsync(ProfileModel model, CancellationToken cancellationToken)
        {
            // can the current user manage the profile we're seeking to update...
            if (await this.context.SecurityService.CanManageUserAsync(model.UserId.ToGuid(), cancellationToken))
            {
                // continue...
                User updatedUser = await this.ReadUserAsync(model.UserId.ToGuid(), cancellationToken);

                if (updatedUser != null)
                {
                    // Update this users profile information
                    await this.UpdateUserFromProfileAsync(updatedUser, model);

                    // Update this users Claim records
                    if ((await this.UpdateUserClaimsAsync(updatedUser, model.ToClaims(this.context.BaseUrl))).Succeeded)
                    {
                        // if the save was successful, enqueue the worker to sync
                        //BackgroundJob.Enqueue(() => this.ProcessUserUpdate(updatedUser, claims.Select(c => new ClaimTypeValuePair { Name = c.Type, Value = c.Value }).ToList(), syncUserType));
                        string message = string.Format(Resources.PromptProfileUpdatedSuccessfullyText, updatedUser.Email, updatedUser.Id);
                        await this.context.AuditLog.LogAsync(AuditEvent.Profile, AuditResult.Success, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        string message = string.Format(Resources.PromptProfileUpdatedFailText, updatedUser.Email, updatedUser.Id);
                        await this.context.AuditLog.LogAsync(AuditEvent.Profile, AuditResult.Fail, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken).ConfigureAwait(false);
                        this.context.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    this.context.ErrorManager.CriticalNotFound(Resources.ErrorUserNotFoundText);
                }
            }
            else
            {
                // access deined.
                this.context.ErrorManager.CriticalForbidden(Resources.ErrorAccessDeniedText, ErrorCategory.Security);
            }

            return !this.context.ErrorManager.HasCriticalErrors;
        }

        /// <summary>
        /// Changes the profile user password.
        /// </summary>
        /// <param name="userId">Contains the user unique identifier.</param>
        /// <param name="model">The password change model.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a value indicating success.</returns>
        public async Task<bool> ChangeProfilePasswordAsync(Guid userId, ChangePasswordModel model, CancellationToken cancellationToken)
        {
            bool result = false;

            if (await this.context.SecurityService.CanManageUserAsync(userId, cancellationToken))
            {
                User updatedUser = await this.ReadUserAsync(userId, cancellationToken);

                if (updatedUser != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.NewPassword))
                    {
                        // Change the user's password
                        var resetResult = await this.context.UserManager.ResetPasswordAsync(updatedUser, await this.context.UserManager.GeneratePasswordResetTokenAsync(updatedUser), model.NewPassword);
                        result = resetResult?.Succeeded ?? false;
                    }
                }
            }
            else
            {
                this.context.ErrorManager.CriticalForbidden(Resources.ErrorAccessDeniedText, ErrorCategory.Security);
            }

            return result;
        }

        /// <summary>
        /// Converts a user entity to profile model.
        /// </summary>
        /// <param name="user">The user entity to convert.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a new <see cref="ProfileModel" /> model.</returns>
        public async Task<ProfileModel> ConvertToProfileAsync(User user, CancellationToken cancellationToken = default)
        {
            ProfileModel model = new ProfileModel();

            // if the current user was found...
            if (user != null)
            {
                // load user claims
                var claims = await this.context.UserManager.GetClaimsAsync(user);

                if (claims.Any())
                {
                    foreach (Claim claim in claims)
                    {
                        model.Claims.Add(claim.Type, claim.Value);
                    }
                }

                model.UserId = user.Id.ToString();
                model.Email = user.Email;
                model.IsEmailConfirmed = user.EmailConfirmed;

                if (string.IsNullOrWhiteSpace(model.PhoneNumber) && !string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    model.PhoneNumber = user.PhoneNumber;
                }

                model.Organizations = await this.context.DataContext.Organizations
                    .AsNoTracking()
                    .Where(o => o.Groups.Any(g => g.Members.Any(m => m.UserId == user.Id)))
                    .Select(o => new MicroOrganizationModel { OrganizationId = o.OrganizationId, Name = o.Name })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                // get the user's group assignments
                model.Groups = await this.context.DataContext.Groups
                    .AsNoTracking()
                    .Where(o => o.Members.Any(ou => ou.UserId == user.Id))
                    .Select(o => new MinimalGroupModel { GroupId = o.GroupId, Name = o.Name })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                // get the various application user's role assignments
                model.Roles = await this.context.DataContext.UserRoles
                    .AsNoTracking()
                    .Join(this.context.DataContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, ur.RoleId, r.Name })
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.Name)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            return model;
        }

        #endregion

        /// <summary>
        /// This method is used to retrieve all the organizations the user is part of.
        /// </summary>
        /// <param name="userId">Contains the user identity value.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a list of <see cref="Group" /> models the user is part of, if found.</returns>
        public async Task<List<Group>> UserGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            List<Group> results = null;

            results = await this.context.DataContext.Groups
                        .Where(o => this.context.DataContext.GroupUsers.Any(ou => ou.UserId == userId && ou.GroupId == o.GroupId))
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);

            return results;
        }

        #endregion

        #region Protected Save Methods

        /// <summary>
        /// This method is used to wrap the save changes mechanism and report any errors through the response channel.
        /// </summary>
        /// <param name="user">Contains the user record to save.</param>
        /// <param name="entityState">Contains the entity state of the save action.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns a value indicating whether the save changes was successful.</returns>
        protected async Task<bool> SaveAsync(User user, EntityState entityState = EntityState.Modified, CancellationToken cancellationToken = default)
        {
            bool result = false;

            try
            {
                if (user.Id == Guid.Empty)
                {
                    user.Id = Guid.NewGuid();
                }

                switch (entityState)
                {
                    case EntityState.Added:
                        user.CreatedDate = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        user.UpdatedDate = DateTime.UtcNow;
                        break;
                }

                result = await this.context.DataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            }
            catch (DbUpdateException dataEx)
            {
                this.context.ErrorManager.Critical(dataEx, ErrorCategory.Application);
            }

            return result;
        }

        #endregion

        #region Private User Support Methods

        /// <summary>
        /// This method is used to convert a user found in the system to a management model.
        /// </summary>
        /// <param name="userFound">Contains the user entity found.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a new <see cref="BastilleUserModel" /> model.</returns>
        private async Task<BastilleUserModel> ConvertToModelAsync(User userFound, CancellationToken cancellationToken)
        {
            var model = new BastilleUserModel
            {
                Id = userFound.Id,
                Email = userFound.Email,
                LockoutEnd = userFound.LockoutEnd,
                Name = userFound.UserName,
                Groups = await this.context.DataContext.GroupUsers
                    .AsNoTracking()
                    .Where(o => o.UserId == userFound.Id)
                    .Select(o => new GroupModel
                    {
                        GroupId = o.GroupId,
                        Name = o.Group.Name
                    })
                    .ToListAsync(cancellationToken),
                Administrator = this.context.DataContext.UserRoles
                    .Join(this.context.DataContext.Roles, ur => ur.RoleId,
                        role => role.Id,
                        (ur, role) => new { UserRole = ur, Role = role })
                    .Any(ur => ur.UserRole.UserId == userFound.Id && ur.Role.Name == SecurityDefaults.AdministratorRoleName)
            };

            var userClaims = await this.context.DataContext.UserClaims
                .AsNoTracking()
                .Where(uc => uc.UserId == userFound.Id)
                .ToListAsync(cancellationToken);

            model.ParseClaims(userClaims);

            return model;
        }

        /// <summary>
        /// Updates the user from Bastille user model.
        /// </summary>
        /// <param name="updatedUser">The updated user entity.</param>
        /// <param name="model">The model update.</param>
        /// <returns>Returns the <see cref="BastilleUserModel" /> object.</returns>
        private async Task<BastilleUserModel> UpdateUserFromModelAsync(User updatedUser, BastilleUserModel model)
        {
            if (!updatedUser.UserName.Equals(model.Name, StringComparison.OrdinalIgnoreCase))
            {
                updatedUser.UserName = model.Name;
                updatedUser.NormalizedUserName = model.Name.ToUpperInvariant();

                await this.context.UserManager.UpdateNormalizedUserNameAsync(updatedUser);
            }

            if (!updatedUser.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
            {
                // Change the user's email
                await this.context.UserManager.ChangeEmailAsync(updatedUser, model.Email, await this.context.UserManager.GenerateChangeEmailTokenAsync(updatedUser, model.Email));
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                // Change the user's password
                await this.context.UserManager.ResetPasswordAsync(updatedUser, await this.context.UserManager.GeneratePasswordResetTokenAsync(updatedUser), model.Password);
            }

            // Set the lock out end date to the selected date if lockout end date is null, this call will clear the value
            updatedUser.LockoutEnd = !model.LockoutEnd.HasValue || model.LockoutEnd == DateTimeOffset.MinValue
                ? null
                : new DateTime?(model.LockoutEnd.Value.UtcDateTime);

            if (await this.context.UserManager.GetLockoutEnabledAsync(updatedUser) && model.LockoutEnd.HasValue && model.LockoutEnd > DateTimeOffset.MinValue)
            {
                await this.context.UserManager.SetLockoutEndDateAsync(updatedUser, model.LockoutEnd);
            }

            return model;
        }

        /// <summary>
        /// Updates the user from a profile model.
        /// </summary>
        /// <param name="updatedUser">The updated user.</param>
        /// <param name="model">The model.</param>
        /// <returns>Returns the <see cref="ProfileModel" /> object.</returns>
        private async Task<ProfileModel> UpdateUserFromProfileAsync(User updatedUser, ProfileModel model)
        {
            if (this.context.ProfileSettings.AllowUserNameChange)
            {
                switch (this.context.ProfileSettings.IdentifierMethod)
                {
                    case Configuration.LoginIdentifierMethod.Email:
                        if (!updatedUser.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                        {
                            // Change the user's email
                            await this.context.UserManager.ChangeEmailAsync(updatedUser, model.Email, await this.context.UserManager.GenerateChangeEmailTokenAsync(updatedUser, model.Email));
                            updatedUser.UserName = model.Email;
                        }
                        break;

                    case Configuration.LoginIdentifierMethod.UserName:
                    case Configuration.LoginIdentifierMethod.UserNameOrEmail:
                    case Configuration.LoginIdentifierMethod.UserNameOrEmailOrPhone:

                        if (!updatedUser.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase))
                        {
                            updatedUser.UserName = model.UserName;
                            updatedUser.NormalizedUserName = model.UserName.ToUpperInvariant();
                        }

                        break;
                }

                await this.context.UserManager.UpdateNormalizedUserNameAsync(updatedUser);
            }

            return model;
        }

        /// <summary>
        /// Updates the user claims specified.
        /// </summary>
        /// <param name="updatedUser">The user to update.</param>
        /// <param name="newClaims">The new claims to add.</param>
        /// <returns>Returns the <see cref="IdentityResult" /> model.</returns>
        private async Task<IdentityResult> UpdateUserClaimsAsync(User updatedUser, List<Claim> newClaims)
        {
            // remove existing claims
            await this.context.UserManager.RemoveClaimsAsync(updatedUser, await this.context.UserManager.GetClaimsAsync(updatedUser));

            // add claims back
            return await this.context.UserManager.AddClaimsAsync(updatedUser, newClaims);
        }

        /// <summary>
        /// Updates the user groups specified.
        /// </summary>
        /// <param name="model">The user model.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="BastilleUserModel" /> model updated.</returns>
        private async Task<BastilleUserModel> UpdateUserGroupsAsync(BastilleUserModel model, CancellationToken cancellationToken)
        {
            // get the current user organizations from the database for this user
            var groups = await this.context.DataContext.GroupUsers
                .Where(o => o.UserId == model.Id)
                .ToListAsync(cancellationToken);

            // Remove group users for any group this user isn't a part of
            this.context.DataContext.GroupUsers.RemoveRange(groups.Where(o => model.Groups.All(m => m.GroupId != o.GroupId)).ToList());

            if (model.Groups.Any())
            {
                // add any new groups added
                var groupsToAdd = model.Groups
                    .Where(m => groups.All(o => o.GroupId != m.GroupId))
                    .Select(groupToAdd => new GroupUser
                    {
                        GroupId = groupToAdd.GroupId,
                        UserId = model.Id,
                        CreatedDate = DateTime.UtcNow
                    });

                // add groups that currently do not exist in database.
                await this.context.DataContext.GroupUsers.AddRangeAsync(groupsToAdd, cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Updates the user roles specified.
        /// </summary>
        /// <param name="updateUser">The user entity.</param>
        /// <param name="model">The user model.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a value indicating success.</returns>
        private async Task<bool> UpdateUserRoleAsync(User updateUser, BastilleUserModel model, CancellationToken cancellationToken)
        {
            bool result = false;

            // in this application there are only one role. System Admin. All other users are not admins. only group owners can manage groups. For now, there
            // will only ever be one group owner. in the future, perhaps a {group name} manager role can be created, but this may be problematic keeping names
            // in sync.
            bool userInAdminRole = await this.context.UserManager.IsInRoleAsync(updateUser, SecurityDefaults.AdministratorRoleName);

            // add user to
            if (model.Administrator && !userInAdminRole)
            {
                result = await this.context.SecurityService.AddToAdminRoleAsync(updateUser, cancellationToken);
            }
            else if (!model.Administrator && userInAdminRole)
            {
                result = await this.context.SecurityService.RemoveFromAdminRoleAsync(updateUser, cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// This method is used to process adding user groups to a new user
        /// </summary>
        /// <param name="model">Contains the new user model.</param>
        /// <param name="newUser">Contains the new user.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the action task.</returns>
        private async Task ProcessGroupsAddedAsync(BastilleUserModel model, User newUser, CancellationToken cancellationToken)
        {
            // if any groups...
            if (model.Groups.Any())
            {
                // Get the list of group ids sent from the user details page
                var groupIds = model.Groups.Select(o => o.GroupId).ToList();
                var groupsFound = await this.context.DataContext.Groups
                    .Where(o => groupIds.Any(ota => ota == o.GroupId))
                    .ToListAsync(cancellationToken);

                // add user organizations
                newUser.Groups.AddRange(
                    model.Groups.Select(groupToAdd => new GroupUser
                    {
                        Group = groupsFound.FirstOrDefault(o => o.GroupId == groupToAdd.GroupId),
                        GroupId = groupToAdd.GroupId,
                        UserId = newUser.Id,
                        User = newUser,
                        CreatedDate = DateTime.UtcNow
                    }));
            }
        }

        #endregion
    }
}
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

namespace Bastille.Id.Core.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using Bastille.Id.Core.Logging;
    using Bastille.Id.Core.Properties;
    using Bastille.Id.Core.Security;
    using Bastille.Id.Models.Logging;
    using Bastille.Id.Models.Security;
    using IdentityModel;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Core.Errors;
    using Talegen.Common.Models.Security;
    using Talegen.Common.Models.Shared.Queries;

    /// <summary>
    /// This class contains business logic related to organization groups within the database.
    /// </summary>
    /// <seealso cref="ServiceClassBase{ApplicationDbContext}" />
    public class GroupService : ServiceClassBase<ApplicationDbContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupService" /> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorManager">Contains the error manager.</param>
        public GroupService(ApplicationDbContext context, IErrorManager errorManager)
            : base(context, errorManager, new AuditLogService(context))
        {
        }

        /// <summary>
        /// This method is used for paginated querying of the user groups.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the <see cref="PaginatedQueryResultModel{GroupModel}" /> containing the records found.</returns>
        public async Task<PaginatedQueryResultModel<GroupModel>> BrowseAsync(GroupQueryFilterModel filter, CancellationToken cancellationToken)
        {
            IQueryable<GroupModel> query = this.DataContext.Groups
                .AsNoTracking()
                .Include(g => g.Organization)
                .Include(g => g.Owner)
                .Select(g => new GroupModel
                {
                    GroupId = g.GroupId,
                    Name = g.Name
                })
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                query = query.Where(g => g.Name.Contains(filter.SearchText));
            }

            return await BrowseQueryHelper.ExecutePagedQueryAsync(query, filter, nameof(Group.Name), cancellationToken);
        }

        /// <summary>
        /// Reads the Group entity.
        /// </summary>
        /// <param name="id">The group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Group" /> entity if found.</returns>
        public async Task<Group> ReadAsync(Guid id, CancellationToken cancellationToken)
        {
            Group result = await this.DataContext.Groups
                .Include(o => o.Owner)
                .Include(o => o.Organization)
                .FirstOrDefaultAsync(o => o.GroupId == id, cancellationToken);

            if (result == null)
            {
                this.ErrorManager.CriticalNotFoundFormat(Resources.ErrorGroupNotFoundText, ErrorCategory.Application, nameof(Group.GroupId), id);
            }

            return result;
        }

        /// <summary>
        /// Reads the group model.
        /// </summary>
        /// <param name="id">The group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="GroupModel" /> model if found.</returns>
        public async Task<GroupModel> ReadModelAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await this.ReadAsync(id, cancellationToken);
            return entity?.ToModel();
        }

        /// <summary>
        /// Reads the group users.
        /// </summary>
        /// <param name="id">The group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a list of <see cref="GroupUserModel" /> objects if any found.</returns>
        public async Task<List<GroupUserModel>> ReadUsersAsync(Guid id, CancellationToken cancellationToken)
        {
            return await this.DataContext.GroupUsers
                .AsNoTracking()
                .Where(g => g.GroupId == id)
                .OrderBy(g => g.User.UserName)
                .Select(g => new GroupUserModel
                {
                    GroupId = g.GroupId,
                    UserId = g.UserId,
                    UserName = g.User.UserName,
                    FirstName = g.User.Claims.Where(c => c.ClaimType == JwtClaimTypes.GivenName).Select(c => c.ClaimValue).FirstOrDefault(),
                    LastName = g.User.Claims.Where(c => c.ClaimType == JwtClaimTypes.FamilyName).Select(c => c.ClaimValue).FirstOrDefault(),
                    LastLoginDate = g.User.LastLoginDate,
                    Picture = g.User.Claims.Where(c => c.ClaimType == JwtClaimTypes.Picture).Select(c => c.ClaimValue).FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Reads the owner for a group.
        /// </summary>
        /// <param name="id">The group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the <see cref="MinimalUserModel" /> model for the group owner if found.</returns>
        public async Task<MinimalUserModel> ReadOwnerAsync(Guid id, CancellationToken cancellationToken)
        {
            MinimalUserModel result = null;
            User user = await this.DataContext.Groups
                .AsNoTracking()
                .Where(g => g.GroupId == id)
                .Select(u => u.Owner)
                .FirstOrDefaultAsync(cancellationToken);

            if (user != null)
            {
                result = user.ToModel();
            }

            return result;
        }

        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <param name="model">The group model to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the <see cref="GroupModel" /> model of the created record.</returns>
        public async Task<GroupModel> CreateAsync(GroupModel model, CancellationToken cancellationToken)
        {
            Group entity = model.ToEntity();
            AuditResult auditResult = AuditResult.Success;
            string message;

            if (await this.SaveAsync(entity, EntityState.Added, cancellationToken))
            {
                message = string.Format(Resources.PrompGroupCreateSuccessText, model.Name, model.GroupId);

                // refresh and reload from database.
                model = await this.ReadModelAsync(entity.GroupId, cancellationToken);
            }
            else
            {
                message = string.Format(Resources.PromptGroupCreateFailText, model.Name, model.GroupId);
                auditResult = AuditResult.Fail;
                this.ErrorManager.CriticalFormat(message, ErrorCategory.Application, model.Name, model.GroupId);
            }

            await this.AuditLog.LogAsync(AuditEvent.Config, auditResult, string.Empty, message, cancellationToken: cancellationToken);

            return model;
        }

        /// <summary>
        /// Updates the group.
        /// </summary>
        /// <param name="model">The group model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the <see cref="GroupModel" /> model updated.</returns>
        public async Task<GroupModel> UpdateAsync(GroupModel model, CancellationToken cancellationToken)
        {
            Group entity = await this.ReadAsync(model.GroupId, cancellationToken);

            if (entity != null)
            {
                AuditResult auditResult = AuditResult.Success;
                string message;

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.OrganizationId = model.OrganizationId;
                entity.OwnerUserId = model.OwnerUserId;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.ParentGroupId = model.ParentGroupId;
                entity.Active = model.Active;

                // remove organization users from model and send to be updated.
                if (model.Members != null && model.Members.Any())
                {
                    List<GroupUser> groupUsers = model.Members.Select(m => new GroupUser { GroupId = m.GroupId, UserId = m.UserId }).ToList();
                    entity.Members = null;
                    List<GroupUser> currentUsers = this.DataContext.GroupUsers.Where(ou => ou.GroupId == model.GroupId).ToList();
                    this.DataContext.GroupUsers.RemoveRange(currentUsers);
                    this.DataContext.GroupUsers.AddRange(groupUsers);
                }

                if (await this.SaveAsync(entity, EntityState.Modified, cancellationToken))
                {
                    message = string.Format(Resources.PromptGroupUpdateSuccessText, model.Name, model.GroupId);
                    model = entity.ToModel();
                }
                else
                {
                    message = string.Format(Resources.PromptGroupUpdateFailText, model.Name, model.GroupId);
                    auditResult = AuditResult.Fail;
                    this.ErrorManager.CriticalFormat(message, ErrorCategory.Application, model.Name, model.GroupId);
                }

                await this.AuditLog.LogAsync(AuditEvent.Config, auditResult, string.Empty, message, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the group.
        /// </summary>
        /// <param name="id">The group identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            Group entity = await this.ReadAsync(id, cancellationToken);

            if (entity != null)
            {
                AuditResult auditResult = AuditResult.Success;
                string message;

                if (await this.SaveAsync(entity, EntityState.Deleted, cancellationToken))
                {
                    message = string.Format(Resources.PromptGroupDeleteSuccessText, id);
                }
                else
                {
                    message = string.Format(Resources.PromptGroupDeleteFailText, id);
                    auditResult = AuditResult.Fail;
                    this.ErrorManager.CriticalFormat(message, ErrorCategory.Application, id);
                }

                await this.AuditLog.LogAsync(AuditEvent.Config, auditResult, string.Empty, message, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// This method is used to wrap the save changes mechanism and report any errors through the response channel.
        /// </summary>
        /// <param name="entity">Contains the record to save.</param>
        /// <param name="entityState">Contains the entity state of the save action.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns a value indicating whether the save changes was successful.</returns>
        protected async Task<bool> SaveAsync(Group entity, EntityState entityState = EntityState.Modified, CancellationToken cancellationToken = default)
        {
            bool result = false;

            if (await this.ValidateAsync(entity, entityState, cancellationToken))
            {
                try
                {
                    switch (entityState)
                    {
                        case EntityState.Added:
                            entity.CreatedDate = DateTime.UtcNow;
                            await this.DataContext.Groups.AddAsync(entity, cancellationToken);
                            break;

                        case EntityState.Modified:
                            entity.UpdatedDate = DateTime.UtcNow;
                            break;

                        case EntityState.Deleted:
                            this.DataContext.Groups.Remove(entity);
                            break;
                    }

                    result = await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
                }
                catch (DbUpdateException dataEx)
                {
                    this.ErrorManager.Critical(dataEx, ErrorCategory.Application);
                }
            }

            return result;
        }

        /// <summary>
        /// Validates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityState">State of the entity.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a value indicating whether the entity operation is valid.</returns>
        private async Task<bool> ValidateAsync(Group entity, EntityState entityState, CancellationToken cancellationToken)
        {
            if (entityState == EntityState.Added || entityState == EntityState.Modified)
            {
                if ((entityState == EntityState.Added && await this.DataContext.Groups.AnyAsync(g => g.Name == entity.Name && g.OrganizationId == entity.OrganizationId, cancellationToken)) ||
                    (entityState == EntityState.Modified && await this.DataContext.Groups.AnyAsync(g => g.Name == entity.Name && g.OrganizationId == entity.OrganizationId && g.GroupId != entity.GroupId, cancellationToken)))
                {
                    // cannot add or change name to a group name that already exists for the same organization.
                    this.ErrorManager.ValidationFormat(nameof(entity.Name), Resources.ErrorGroupNameExistsText, ErrorCategory.Application, entity.Name);
                }

                if (entity.OwnerUserId == Guid.Empty)
                {
                    this.ErrorManager.Validation(nameof(entity.OwnerUserId), Resources.ErrorGroupSpecifyOwnerText, ErrorCategory.Application);
                }
            }

            return !this.ErrorManager.HasErrors;
        }
    }
}
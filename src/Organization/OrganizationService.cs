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
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using Bastille.Id.Models.Organization;
    using Bastille.Id.Core.Properties;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Core.Errors;
    using Bastille.Id.Core.Logging;
    using System.Linq;
    using Talegen.Common.Models.Shared.Queries;
    using IdentityModel;

    /// <summary>
    /// This class contains business logic related to organizations within the database.
    /// </summary>
    /// <seealso cref="ServiceClassBase{ApplicationDbContext}" />
    public class OrganizationService : ServiceClassBase<ApplicationDbContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationService" /> class.
        /// </summary>
        /// <param name="context">Contains an instance of the application data context.</param>
        /// <param name="errorManager">Contains an instance of an error manager.</param>
        public OrganizationService(ApplicationDbContext context, IErrorManager errorManager)
            : base(context, errorManager, new AuditLogService(context))
        {
        }

        /// <summary>
        /// This method is used to execute a paginated query of organizations.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the <see cref="PaginatedQueryResultModel{OrganizationModel}" /> containing the records found.</returns>
        public async Task<PaginatedQueryResultModel<OrganizationModel>> BrowseAsync(OrganizationQueryFilterModel filter, CancellationToken cancellationToken)
        {
            IQueryable<OrganizationModel> query = this.DataContext.Organizations
                .AsNoTracking()
                .Include(o => o.Owner)
                .Select(o => new OrganizationModel
                {
                    OrganizationId = o.OrganizationId,
                    Name = o.Name,
                    Address1 = o.Address1,
                    Address2 = o.Address2,
                    City = o.City,
                    Country = o.Country,
                    CreatedDate = o.CreatedDate,
                    Description = o.Description,
                    PostalCode = o.PostalCode,
                    Region = o.Region,
                    Slug = o.Slug,
                    UpdatedDate = o.UpdatedDate,
                    Owner = new Models.Security.BastilleMicroUserModel
                    {
                        UserId = o.Owner.Id,
                        Email = o.Owner.Email,
                        UserName = o.Owner.UserName,
                        TimeZone = o.Owner.Claims.Where(c => c.ClaimType == JwtClaimTypes.ZoneInfo).Select(c => c.ClaimValue).FirstOrDefault(),
                        Locale = o.Owner.Claims.Where(c => c.ClaimType == JwtClaimTypes.Locale).Select(c => c.ClaimValue).FirstOrDefault(),
                        FirstName = o.Owner.Claims.Where(c => c.ClaimType == JwtClaimTypes.GivenName).Select(c => c.ClaimValue).FirstOrDefault(),
                        LastName = o.Owner.Claims.Where(c => c.ClaimType == JwtClaimTypes.FamilyName).Select(c => c.ClaimValue).FirstOrDefault(),
                        Picture = o.Owner.Claims.Where(c => c.ClaimType == JwtClaimTypes.Picture).Select(c => c.ClaimValue).FirstOrDefault(),
                    },
                    Active = o.Active
                })
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(g => g.Name.Contains(filter.Search) ||
                g.Address1.Contains(filter.Search) ||
                g.Address2.Contains(filter.Search) ||
                g.Description.Contains(filter.Search) ||
                g.City.Contains(filter.Search) ||
                g.Region.Contains(filter.Search) ||
                g.Country.Contains(filter.Search) ||
                g.PostalCode.Contains(filter.Search) ||
                g.Slug.Contains(filter.Search));
            }

            return await BrowseQueryHelper.ExecutePagedQueryAsync(query, filter, nameof(Group.Name), cancellationToken);
        }

        /// <summary>
        /// Reads the organization specified.
        /// </summary>
        /// <param name="id">The organization identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Organization" /> entity if found.</returns>
        public async Task<Organization> ReadAsync(Guid id, CancellationToken cancellationToken)
        {
            Organization result = await this.DataContext.Organizations
                .Include(o => o.Owner)
                .Include(o => o.Groups)
                .FirstOrDefaultAsync(o => o.OrganizationId == id, cancellationToken);

            if (result == null)
            {
                this.ErrorManager.CriticalNotFoundFormat(Resources.ErrorOrganizationNotFoundText, ErrorCategory.Application, nameof(Organization.OrganizationId), id);
            }

            return result;
        }

        /// <summary>
        /// Reads the organization model specified.
        /// </summary>
        /// <param name="id">The organization identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="OrganizationModel" /> model if found.</returns>
        public async Task<OrganizationModel> ReadModelAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await this.ReadAsync(id, cancellationToken);
            return entity?.ToModel();
        }

        /// <summary>
        /// Creates the organization specified.
        /// </summary>
        /// <param name="model">The organization model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the <see cref="OrganizationModel" /> created.</returns>
        public async Task<OrganizationModel> CreateAsync(OrganizationModel model, CancellationToken cancellationToken)
        {
            Organization entity = model.ToEntity();

            if (await this.SaveAsync(entity, EntityState.Added, cancellationToken))
            {
                // refresh and reload from database.
                model = await this.ReadModelAsync(entity.OrganizationId, cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Updates the organization specified.
        /// </summary>
        /// <param name="model">The organization model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the <see cref="OrganizationModel" /> updated.</returns>
        public async Task<OrganizationModel> UpdateAsync(OrganizationModel model, CancellationToken cancellationToken)
        {
            Organization entity = await this.ReadAsync(model.OrganizationId, cancellationToken);

            if (entity != null)
            {
                if (await this.SaveAsync(entity, EntityState.Modified, cancellationToken))
                {
                    model = entity.ToModel();
                }
            }

            return model;
        }

        /// <summary>
        /// Deletes the organization specified.
        /// </summary>
        /// <param name="id">The organization identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            Organization entity = await this.ReadAsync(id, cancellationToken);

            if (entity != null)
            {
                await this.SaveAsync(entity, EntityState.Deleted, cancellationToken);
            }
        }

        /// <summary>
        /// This method is used to wrap the save changes mechanism and report any errors through the response channel.
        /// </summary>
        /// <param name="entity">Contains the record to save.</param>
        /// <param name="entityState">Contains the entity state of the save action.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns a value indicating whether the save changes was successful.</returns>
        protected async Task<bool> SaveAsync(Organization entity, EntityState entityState = EntityState.Modified, CancellationToken cancellationToken = default)
        {
            bool result = false;

            if (this.Validate(entity, entityState))
            {
                try
                {
                    switch (entityState)
                    {
                        case EntityState.Added:
                            this.DataContext.Organizations.Add(entity);
                            entity.CreatedDate = DateTime.UtcNow;
                            break;

                        case EntityState.Modified:
                            entity.UpdatedDate = DateTime.UtcNow;
                            break;

                        case EntityState.Deleted:
                            this.DataContext.Organizations.Remove(entity);
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
        /// <returns>Returns a value indicating validation success.</returns>
        private bool Validate(Organization entity, EntityState entityState)
        {
            // TODO: validate entity
            return !this.ErrorManager.HasErrors;
        }
    }
}
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
    using System.Linq;
    using Bastille.Id.Core.Data.Entities;
    using Bastille.Id.Models.Organization;
    using Bastille.Id.Models.Security;
    using IdentityModel;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class contains extension methods for organization models.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Converts an entity to model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="OrganizationModel" /> model.</returns>
        public static OrganizationModel ToModel(this Organization entity)
        {
            return new OrganizationModel
            {
                OrganizationId = entity.OrganizationId,
                Active = entity.Active,
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                City = entity.City,
                Country = entity.Country,
                CreatedDate = entity.CreatedDate,
                Description = entity.Description,
                Name = entity.Name,
                PostalCode = entity.PostalCode,
                Region = entity.Region,
                Slug = entity.Slug,
                UpdatedDate = entity.UpdatedDate,
                Owner = entity.Owner != null ? new BastilleMicroUserModel
                {
                    Id = entity.Owner.Id,
                    Email = entity.Owner.Email,
                    FirstName = entity.Owner.Claims.Where(uc => uc.ClaimType == JwtClaimTypes.GivenName).Select(uc => uc.ClaimValue).FirstOrDefault(),
                    LastName = entity.Owner.Claims.Where(uc => uc.ClaimType == JwtClaimTypes.FamilyName).Select(uc => uc.ClaimValue).FirstOrDefault(),
                    Picture = entity.Owner.Claims.Where(uc => uc.ClaimType == JwtClaimTypes.Picture).Select(uc => uc.ClaimValue).FirstOrDefault(),
                    Name = entity.Owner.UserName
                } : null,
                Groups = entity.Groups?.Select(g => new GroupModel
                {
                    GroupId = g.GroupId,
                    Name = g.Name
                }).ToList()
            };
        }

        /// <summary>
        /// Converts to entity value.
        /// </summary>
        /// <param name="model">The model to convert.</param>
        /// <returns>Returns an <see cref="Organization" /> entity.</returns>
        public static Organization ToEntity(this OrganizationModel model)
        {
            return new Organization
            {
                OrganizationId = model.OrganizationId,
                Active = model.Active,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Country = model.Country,
                CreatedDate = model.CreatedDate,
                Description = model.Description,
                Name = model.Name,
                PostalCode = model.PostalCode,
                Region = model.Region,
                Slug = string.IsNullOrWhiteSpace(model.Slug) ? model.Name.Slugify() : model.Slug,
                UpdatedDate = model.UpdatedDate,
                OwnerUserId = model.Owner.Id
            };
        }

        /// <summary>
        /// Converts to model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="GroupModel" /> model.</returns>
        public static GroupModel ToModel(this Group entity)
        {
            return new GroupModel
            {
                GroupId = entity.GroupId,
                Name = entity.Name,
                OwnerUserId = entity.OwnerUserId,
                OrganizationId = entity.OrganizationId,
                ParentGroupId = entity.ParentGroupId,
                Description = entity.Description,
                Active = entity.Active,
                Members = entity.Members.Select(m => new GroupUserModel
                {
                    GroupId = m.GroupId,
                    UserId = m.UserId,
                    FirstName = m.User.Claims.Where(uc => uc.ClaimType == JwtClaimTypes.GivenName).Select(uc => uc.ClaimValue).FirstOrDefault(),
                    LastName = m.User.Claims.Where(uc => uc.ClaimType == JwtClaimTypes.FamilyName).Select(uc => uc.ClaimValue).FirstOrDefault(),
                    Picture = m.User.Claims.Where(uc => uc.ClaimType == JwtClaimTypes.Picture).Select(uc => uc.ClaimValue).FirstOrDefault(),
                    UserName = m.User.UserName,
                    LastLoginDate = m.User.LastLoginDate
                }).ToList()
            };
        }

        /// <summary>
        /// Converts to entity value.
        /// </summary>
        /// <param name="model">The model to convert.</param>
        /// <returns>Returns a <see cref="Group" /> entity.</returns>
        public static Group ToEntity(this GroupModel model)
        {
            return new Group
            {
                GroupId = model.GroupId,
                Name = model.Name,
                OwnerUserId = model.OwnerUserId,
                OrganizationId = model.OrganizationId,
                ParentGroupId = model.ParentGroupId,
                Description = model.Description,
                Members = model.Members.Select(m => new GroupUser { UserId = m.UserId, GroupId = m.GroupId }).ToList(),
                Active = model.Active
            };
        }
    }
}
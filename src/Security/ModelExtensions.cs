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
    using System.Linq;
    using Bastille.Id.Core.Data.Entities;
    using IdentityModel;
    using Talegen.Common.Models.Security;

    /// <summary>
    /// This class contains security model extension methods.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Converts to model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="MinimalUserModel" /> model.</returns>
        public static MinimalUserModel ToModel(this User entity)
        {
            return new MinimalUserModel
            {
                Id = entity.Id,
                Email = entity.Email,
                FirstName = entity.Claims.Where(c => c.ClaimType == JwtClaimTypes.GivenName).Select(c => c.ClaimValue).FirstOrDefault(),
                LastName = entity.Claims.Where(c => c.ClaimType == JwtClaimTypes.FamilyName).Select(c => c.ClaimValue).FirstOrDefault(),
                Locale = entity.Claims.Where(c => c.ClaimType == JwtClaimTypes.Locale).Select(c => c.ClaimValue).FirstOrDefault(),
                TimeZone = entity.Claims.Where(c => c.ClaimType == JwtClaimTypes.ZoneInfo).Select(c => c.ClaimValue).FirstOrDefault(),
                Name = entity.UserName
            };
        }
    }
}
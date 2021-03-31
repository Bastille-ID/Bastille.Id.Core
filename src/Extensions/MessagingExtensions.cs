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

namespace Bastille.Id.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using IdentityModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Messaging.Models;

    /// <summary>
    /// This class contains additional extension methods for working with the messaging queue services.
    /// </summary>
    public static class MessagingExtensions
    {
        /// <summary>
        /// This method is used to convert a standard identity <see cref="User" /> object to a <see cref="MessageUser" /> model.
        /// </summary>
        /// <param name="user">Contains the <see cref="User" /> to convert.</param>
        /// <param name="dataContext">Contains data context for claim lookup.</param>
        /// <param name="absoluteUri">Contains the base URI used for building profile URL if picture claim does not exist.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a new <see cref="MessageUser" /> model.</returns>
        public static async Task<MessageUser> ToMessageUserAsync(this User user, ApplicationDbContext dataContext, string absoluteUri, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (dataContext == null)
            {
                throw new ArgumentNullException(nameof(dataContext));
            }

            var result = new MessageUser
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                UserName = user.UserName,
            };

            List<IdentityUserClaim<Guid>> userClaims = await dataContext.UserClaims
                .AsNoTracking()
                .Where(uc => uc.UserId == user.Id)
                .ToListAsync(cancellationToken);

            result.ParseClaims(userClaims, absoluteUri);

            return result;
        }

        /// <summary>
        /// This method is used to convert claims into <see cref="MessageUser" /> property values.
        /// </summary>
        /// <param name="model">Contains the model to populate.</param>
        /// <param name="userClaims">Contains the claims used to populate the model.</param>
        /// <param name="baseUri">Contains the base address to prefix for a user's default picture URL if none is found or specified.</param>
        public static void ParseClaims(this MessageUser model, List<IdentityUserClaim<Guid>> userClaims, string baseUri = "")
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (userClaims == null)
            {
                throw new ArgumentNullException(nameof(userClaims));
            }

            userClaims.ForEach(claim =>
            {
                switch (claim.ClaimType)
                {
                    case JwtClaimTypes.GivenName:
                        model.FirstName = claim.ClaimValue;

                        break;

                    case JwtClaimTypes.FamilyName:
                        model.LastName = claim.ClaimValue;

                        break;

                    case JwtClaimTypes.ZoneInfo:
                        model.TimeZone = claim.ClaimValue;

                        break;

                    case JwtClaimTypes.Picture:
                        if (!string.IsNullOrEmpty(claim.ClaimValue))
                        {
                            model.PictureUrl = new Uri(claim.ClaimValue);
                        }

                        break;

                    case JwtClaimTypes.Locale:
                        if (!string.IsNullOrWhiteSpace(claim.ClaimValue))
                        {
                            model.Locale = claim.ClaimValue;
                        }

                        break;
                }
            });

            if (model.PictureUrl == null || !userClaims.Exists(x => x.ClaimType == JwtClaimTypes.Picture))
            {
                model.PictureUrl = new Uri(ClaimExtensions.GenerateDefaultPictureUrl(baseUri));
            }
        }
    }
}
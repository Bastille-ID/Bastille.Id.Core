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
    using System.Security.Claims;
    using Bastille.Id.Core.Security;
    using Bastille.Id.Models.Security;
    using IdentityModel;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This class contains extension methods in support of working with claim to model conversions.
    /// </summary>
    public static class ClaimExtensions
    {
        /// <summary>
        /// Converts the specified user model to related JWT claims.
        /// </summary>
        /// <param name="model">The user model to convert to JWT claims.</param>
        /// <param name="baseUri">The base URI to prefix to a default picture URL if no image is specified.</param>
        /// <returns>Returns a list of JWT claims converted from the specified user model.</returns>
        /// <exception cref="System.ArgumentNullException">The exception is thrown if the <paramref name="model" /> is not specified.</exception>
        public static List<Claim> ToClaims(this BastilleUserModel model, string baseUri = "")
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return new List<Claim>
            {
                new Claim(JwtClaimTypes.GivenName, model.FirstName),
                new Claim(JwtClaimTypes.FamilyName, model.LastName),
                new Claim(JwtClaimTypes.Locale, model.Locale),
                new Claim(JwtClaimTypes.ZoneInfo, model.TimeZone),
                new Claim(JwtClaimTypes.PhoneNumber, model.Phone),
                new Claim(JwtClaimTypes.Picture, !string.IsNullOrWhiteSpace(model.Picture) ? model.Picture : GenerateDefaultPictureUrl(baseUri))
            };
        }

        /// <summary>
        /// Converts the specified profile model to related JWT claims.
        /// </summary>
        /// <param name="model">The model to convert.</param>
        /// <param name="baseUri">The base URI to prefix to a default picture URL if no image is specified.</param>
        /// <returns>Returns a list of JWT claims converted from the specified profile model.</returns>
        /// <exception cref="System.ArgumentNullException">The exception is thrown if the <paramref name="model" /> is not specified.</exception>
        public static List<Claim> ToClaims(this ProfileModel model, string baseUri = "")
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return new List<Claim>
            {
                new Claim(JwtClaimTypes.GivenName, model.FirstName),
                new Claim(JwtClaimTypes.FamilyName, model.LastName),
                new Claim(JwtClaimTypes.Locale, model.Locale),
                new Claim(JwtClaimTypes.ZoneInfo, model.TimeZone),
                new Claim(JwtClaimTypes.PhoneNumber, model.PhoneNumber),
                new Claim(JwtClaimTypes.Picture, !string.IsNullOrWhiteSpace(model.PictureUrl) ? model.PictureUrl : GenerateDefaultPictureUrl(baseUri))
            };
        }

        /// <summary>
        /// This method is used to run through the specified user claims and add them to the user model properties.
        /// </summary>
        /// <param name="model">Contains the model to populate.</param>
        /// <param name="userClaims">Contains the claims used to populate the model.</param>
        /// <param name="baseUri">Contains the base address to prefix for a user's default picture URL if none is found or specified.</param>
        public static void ParseClaims(this BastilleUserModel model, List<IdentityUserClaim<Guid>> userClaims, string baseUri = "")
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

                    case JwtClaimTypes.Locale:
                        model.Locale = claim.ClaimValue;
                        break;

                    case JwtClaimTypes.ZoneInfo:
                        model.TimeZone = claim.ClaimValue;
                        break;

                    case JwtClaimTypes.Picture:
                        model.Picture = claim.ClaimValue;
                        break;
                }
            });

            if (string.IsNullOrEmpty(model.Picture) || !userClaims.Exists(x => x.ClaimType == JwtClaimTypes.Picture))
            {
                model.Picture = GenerateDefaultPictureUrl(baseUri);
            }
        }

        /// <summary>
        /// This method is used to generate the default picture address.
        /// </summary>
        /// <param name="baseUri">Contains the base URI for the default picture address.</param>
        /// <returns>Returns the generated default picture address.</returns>
        public static string GenerateDefaultPictureUrl(string baseUri)
        {
            return string.Concat(baseUri, SecurityDefaults.DefaultUserImageName);
        }
    }
}
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
    using System.Security.Claims;
    using Bastille.Id.Core.Security;
    using Bastille.Id.Models.Security;
    using IdentityModel;
    using Microsoft.AspNetCore.Identity;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class contains extension methods in support of working with claim to model conversions.
    /// </summary>
    public static class ClaimExtensions
    {
        /// <summary>
        /// The allowed profile claims.
        /// </summary>
        private static List<string> allowedProfileClaims = new List<string> { JwtClaimTypes.Name, JwtClaimTypes.GivenName, JwtClaimTypes.FamilyName, JwtClaimTypes.WebSite,
            JwtClaimTypes.Address, JwtClaimTypes.BirthDate, JwtClaimTypes.Email, JwtClaimTypes.EmailVerified, JwtClaimTypes.FamilyName, JwtClaimTypes.Gender,
            JwtClaimTypes.Locale, JwtClaimTypes.MiddleName, JwtClaimTypes.NickName, JwtClaimTypes.PhoneNumber, JwtClaimTypes.PhoneNumberVerified, JwtClaimTypes.Picture,
            JwtClaimTypes.ZoneInfo };

        /// <summary>
        /// Gets the allowed profile claims.
        /// </summary>
        /// <value>The allowed profile claims.</value>
        public static List<string> AllowedProfileClaims => allowedProfileClaims;

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

            var results = new List<Claim>();

            if (!string.IsNullOrEmpty(model.FullName))
            {
                results.Add(new Claim(JwtClaimTypes.Name, model.FullName));
            }

            if (!string.IsNullOrEmpty(model.FirstName))
            {
                results.Add(new Claim(JwtClaimTypes.GivenName, model.FirstName));
            }

            if (!string.IsNullOrEmpty(model.LastName))
            {
                results.Add(new Claim(JwtClaimTypes.FamilyName, model.LastName));
            }

            if (!string.IsNullOrEmpty(model.Locale))
            {
                results.Add(new Claim(JwtClaimTypes.Locale, model.Locale));
            }

            if (!string.IsNullOrEmpty(model.TimeZone))
            {
                results.Add(new Claim(JwtClaimTypes.ZoneInfo, model.TimeZone));
            }

            if (!string.IsNullOrEmpty(model.Phone))
            {
                results.Add(new Claim(JwtClaimTypes.PhoneNumber, model.Phone));
            }

            if (string.IsNullOrEmpty(model.Picture))
            {
                model.Picture = GenerateDefaultPictureUrl(baseUri, model.Id.ToString());
            }

            results.Add(new Claim(JwtClaimTypes.Picture, model.Picture));

            return results;
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
            var results = new List<Claim>();

            if (string.IsNullOrEmpty(model.PictureUrl))
            {
                model.PictureUrl = GenerateDefaultPictureUrl(baseUri, model.UserId);
            }

            // return claims that are filtered
            foreach (string key in model.Claims.Keys.Where(k => AllowedProfileClaims.Contains(k)))
            {
                results.Add(new Claim(key, model.Claims[key].ConvertToString()));
            }

            return results;
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
                string subjectId = userClaims.FirstOrDefault(uc => uc.ClaimType == JwtClaimTypes.Subject)?.ClaimValue ?? string.Empty;
                model.Picture = GenerateDefaultPictureUrl(baseUri, subjectId);
            }
        }

        /// <summary>
        /// This method is used to generate the default picture address.
        /// </summary>
        /// <param name="baseUri">Contains the base URI for the default picture address.</param>
        /// <param name="subjectId">Contains the subject id.</param>
        /// <returns>Returns the generated default picture address.</returns>
        public static string GenerateDefaultPictureUrl(string baseUri, string subjectId)
        {
            return string.Concat(baseUri, $"/Picture/{subjectId}");
        }
    }
}
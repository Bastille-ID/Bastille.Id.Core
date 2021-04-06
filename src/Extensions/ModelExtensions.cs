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
    using Bastille.Id.Models.Clients;
    using IdentityServer4.EntityFramework.Mappers;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class contains extensions related to converting objects between model definitions.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Converts the claim entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="ClientClaimModel" /> model.</returns>
        public static ClientClaimModel ToModel(this IdentityServer4.EntityFramework.Entities.ClientClaim entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new ClientClaimModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                Type = entity.Type,
                Value = entity.Value
            };
        }

        /// <summary>
        /// Converts the client scope entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="ClientScopeModel" /> model.</returns>
        public static ClientScopeModel ToModel(this IdentityServer4.EntityFramework.Entities.ClientScope entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new ClientScopeModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                Scope = entity.Scope
            };
        }

        /// <summary>
        /// Converts the client secret entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="ClientScopeModel" /> model.</returns>
        public static ClientSecretModel ToModel(this IdentityServer4.EntityFramework.Entities.ClientSecret entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new ClientSecretModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                Type = entity.Type.ToEnum(converter: (value) =>
                {
                    ClientSecretTypes result;

                    switch (value)
                    {
                        case "JWK":
                            result = ClientSecretTypes.JsonWebKey;
                            break;

                        case "SharedSecret":
                            result = ClientSecretTypes.SharedSecret;
                            break;

                        case "X509CertificateBase64":
                            result = ClientSecretTypes.X509CertificateBase64;
                            break;

                        case "X509Name":
                            result = ClientSecretTypes.X509CertificateName;
                            break;

                        case "X509Thumbprint":
                            result = ClientSecretTypes.X509CertificateThumbprint;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(value));
                    }

                    return result;
                }),
                Description = entity.Description,
                Expiration = entity.Expiration,
                Value = entity.Value
            };
        }

        /// <summary>
        /// Converts the client redirect entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="ClientScopeModel" /> model.</returns>
        public static ClientRedirectUriModel ToModel(this IdentityServer4.EntityFramework.Entities.ClientRedirectUri entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new ClientRedirectUriModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                RedirectUri = new Uri(entity.RedirectUri)
            };
        }

        /// <summary>
        /// Converts the client origin entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="ClientScopeModel" /> model.</returns>
        public static ClientCorsOriginModel ToModel(this IdentityServer4.EntityFramework.Entities.ClientCorsOrigin entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new ClientCorsOriginModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                Origin = entity.Origin
            };
        }

        /// <summary>
        /// Converts the client redirect entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="ClientScopeModel" /> model.</returns>
        public static ClientGrantTypeModel ToModel(this IdentityServer4.EntityFramework.Entities.ClientGrantType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new ClientGrantTypeModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                Type = entity.GrantType.ToEnum(converter: (value) =>
                {
                    ClientGrantTypes result;

                    switch (value)
                    {
                        case "authorization_code":
                            result = ClientGrantTypes.AuthorizationCode;
                            break;

                        case "client_credentials":
                            result = ClientGrantTypes.ClientCredentials;
                            break;

                        case "delegation":
                            result = ClientGrantTypes.Delegation;
                            break;

                        case "device_flow":
                            result = ClientGrantTypes.DeviceFlow;
                            break;

                        case "hybrid":
                            result = ClientGrantTypes.Hybrid;
                            break;

                        case "implicit":
                            result = ClientGrantTypes.Implicit;
                            break;

                        case "password":
                            result = ClientGrantTypes.ResourceOwnerPassword;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(value));
                    }

                    return result;
                })
            };
        }

        /// <summary>
        /// Converts the client logout redirect entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="ClientScopeModel" /> model.</returns>
        public static ClientLogoutRedirectUriModel ToModel(this IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new ClientLogoutRedirectUriModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                LogoutRedirectUri = new Uri(entity.PostLogoutRedirectUri)
            };
        }

        /// <summary>
        /// Converts the client origin entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns a new <see cref="ClientScopeModel" /> model.</returns>
        public static ClientPropertyModel ToModel(this IdentityServer4.EntityFramework.Entities.ClientProperty entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new ClientPropertyModel
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                Key = entity.Key,
                Value = entity.Value
            };
        }

        /// <summary>
        /// Converts to clientmodel.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static ClientModel ToClientModel(this IdentityServer4.EntityFramework.Entities.Client entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var model = entity.ToModel() as ClientModel;
            model.Id = entity.Id;
            return model;
        }
    }
}
﻿/*
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

namespace Bastille.Id.Core.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Data.Entities;
    using Bastille.Id.Core.Extensions;
    using Bastille.Id.Core.Properties;
    using Bastille.Id.Models.Clients;
    using Bastille.Id.Models.Logging;
    using IdentityModel;
    using Duende.IdentityServer.EntityFramework.DbContexts;
    using Duende.IdentityServer.EntityFramework.Entities;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Core.Errors;
    using Talegen.Common.Core.Extensions;
    using Resources = Properties.Resources;

    /// <summary>
    /// This class contains all business logic related to managing clients within the IdP server.
    /// </summary>
    /// <seealso cref="ServiceClassBase{ConfigurationDbContext}" />
    public class ClientService : ServiceClassBase<ConfigurationDbContext>
    {
        /// <summary>
        /// Contains an instance of the client service context.
        /// </summary>
        private readonly ClientServiceContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientService" /> class.
        /// </summary>
        /// <param name="context">Contains an instance of the client service context.</param>
        public ClientService(ClientServiceContext context)
            : base(context.ConfigurationDbContext, context.ErrorManager)
        {
            this.context = context;
        }

        #region Client Record Methods

        /// <summary>
        /// Reads the clients for browsing interfaces.
        /// </summary>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a list of <see cref="BrowseClientModel" /> client models found.</returns>
        public async Task<List<BrowseClientModel>> ReadClientsAsync(CancellationToken cancellationToken)
        {
            return await this.context.ConfigurationDbContext.Clients
                .AsNoTracking()
                .OrderBy(c => c.ClientName)
                .Select(c => new BrowseClientModel
                {
                    Id = c.Id,
                    ClientId = c.ClientId,
                    ClientName = c.ClientName,
                    Description = c.Description,
                    LogoUri = c.LogoUri != null ? new Uri(c.LogoUri) : null,
                    RequireConsent = c.RequireConsent,
                    Enabled = c.Enabled
                })
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Reads the client model by unique client identifier.
        /// </summary>
        /// <param name="clientId">Contains the client unique identifier.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a <see cref="ClientModel" /> if found.</returns>
        public async Task<ClientModel> ReadClientIdAsync(string clientId, CancellationToken cancellationToken)
        {
            int id = await this.context.ConfigurationDbContext.Clients.Where(c => c.ClientId == clientId).Select(c => c.Id).FirstOrDefaultAsync(cancellationToken);

            if (id == 0)
            {
                this.context.ErrorManager.WarningFormat(Resources.ErrorClientNotFoundText, Talegen.Common.Core.Errors.ErrorCategory.Application, nameof(clientId), clientId);
            }

            return id > 0 ? await this.ReadAsync(id, cancellationToken) : null;
        }

        /// <summary>
        /// Reads the Client entity from the repository.
        /// </summary>
        /// <param name="id">Contains the client unique identifier.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a <see cref="Client" /> entity if found.</returns>
        public async Task<Duende.IdentityServer.EntityFramework.Entities.Client> ReadEntityAsync(int id, CancellationToken cancellationToken)
        {
            var client = await this.context.ConfigurationDbContext.Clients.FirstOrDefaultAsync(c => c.Id == id).ConfigureAwait(false);

            if (client == null)
            {
                this.ErrorManager.CriticalNotFoundFormat(Resources.ErrorClientNotFoundText, ErrorCategory.Application, nameof(id), id);
            }

            return client;
        }

        /// <summary>
        /// Reads the client model by record identity.
        /// </summary>
        /// <param name="id">Contains the client record identifier.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a <see cref="ClientModel" /> if found.</returns>
        public async Task<ClientModel> ReadAsync(int id, CancellationToken cancellationToken)
        {
            var client = await this.context.ConfigurationDbContext.Clients
                .AsNoTracking()
                .Select(c => new ClientModel
                {
                    AbsoluteRefreshTokenLifetime = c.AbsoluteRefreshTokenLifetime,
                    AccessTokenLifetime = c.AccessTokenLifetime,
                    AccessTokenTypeValue = c.AccessTokenType,
                    AllowAccessTokensViaBrowser = c.AllowAccessTokensViaBrowser,
                    AllowOfflineAccess = c.AllowOfflineAccess,
                    AllowPlainTextPkce = c.AllowPlainTextPkce,
                    AllowRememberConsent = c.AllowRememberConsent,
                    AlwaysIncludeUserClaimsInIdToken = c.AlwaysIncludeUserClaimsInIdToken,
                    AlwaysSendClientClaims = c.AlwaysSendClientClaims,
                    AuthorizationCodeLifetime = c.AuthorizationCodeLifetime,
                    BackChannelLogoutSessionRequired = c.BackChannelLogoutSessionRequired,
                    BackChannelLogoutUri = c.BackChannelLogoutUri,
                    ClientId = c.ClientId,
                    ClientName = c.ClientName,
                    ClientUri = c.ClientUri,
                    ConsentLifetime = c.ConsentLifetime,
                    Description = c.Description,
                    Enabled = c.Enabled,
                    EnableLocalLogin = c.EnableLocalLogin,
                    Id = c.Id,
                    FrontChannelLogoutSessionRequired = c.FrontChannelLogoutSessionRequired,
                    FrontChannelLogoutUri = c.FrontChannelLogoutUri,
                    IdentityTokenLifetime = c.IdentityTokenLifetime,
                    IncludeJwtId = c.IncludeJwtId,
                    LogoUri = c.LogoUri,
                    ProtocolType = c.ProtocolType,
                    RefreshTokenExpirationValue = c.RefreshTokenExpiration,
                    RefreshTokenUsageValue = c.RefreshTokenUsage,
                    RequireClientSecret = c.RequireClientSecret,
                    RequireConsent = c.RequireConsent,
                    RequirePkce = c.RequirePkce,
                    SlidingRefreshTokenLifetime = c.SlidingRefreshTokenLifetime,
                    UpdateAccessTokenClaimsOnRefresh = c.UpdateAccessTokenClaimsOnRefresh,
                    DeviceCodeLifetime = c.DeviceCodeLifetime,
                    UserSsoLifetime = c.UserSsoLifetime,
                    AllowedScopes = c.AllowedScopes.Select(cs => cs.Scope).ToList(),
                    ClientSecrets = c.ClientSecrets.Select(cs => new Duende.IdentityServer.Models.Secret
                    {
                        Description = cs.Description,
                        Expiration = cs.Expiration,
                        Type = cs.Type,
                        Value = cs.Value
                    }).ToList(),
                    RedirectUris = c.RedirectUris.Select(ruri => ruri.RedirectUri).ToList(),
                    AllowedCorsOrigins = c.AllowedCorsOrigins.Select(aco => aco.Origin).ToList(),
                    AllowedGrantTypes = c.AllowedGrantTypes.Select(agt => agt.GrantType).ToList(),
                    PostLogoutRedirectUris = c.PostLogoutRedirectUris.Select(luri => luri.PostLogoutRedirectUri).ToList()
                })
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (client == null)
            {
                this.context.ErrorManager.WarningFormat(Resources.ErrorClientNotFoundText, Talegen.Common.Core.Errors.ErrorCategory.Application, nameof(id), id);
            }

            return client;
        }

        /// <summary>
        /// Creates a new client record.
        /// </summary>
        /// <param name="model">Contains the client model to store.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientModel" /> of the newly created record</returns>
        public async Task<ClientModel> CreateAsync(ClientModel model, CancellationToken cancellationToken)
        {
            // create new Client entity for save
            Duende.IdentityServer.EntityFramework.Entities.Client entityToCreate = new Duende.IdentityServer.EntityFramework.Entities.Client
            {
                AllowedScopes = new List<ClientScope>(),
                ClientSecrets = new List<ClientSecret>(),
                RedirectUris = new List<ClientRedirectUri>(),
                AllowedCorsOrigins = new List<ClientCorsOrigin>(),
                AllowedGrantTypes = new List<ClientGrantType>(),
                PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>(),
                Properties = new List<ClientProperty>()
            };

            // add the entity's allowed scopes
            foreach (string scopeToAdd in model.AllowedScopes)
            {
                entityToCreate.AllowedScopes.Add(new ClientScope
                {
                    ClientId = model.Id,
                    Scope = scopeToAdd
                });
            };

            foreach (Duende.IdentityServer.Models.Secret secretToAdd in model.ClientSecrets)
            {
                entityToCreate.ClientSecrets.Add(new ClientSecret
                {
                    ClientId = model.Id,
                    Created = DateTime.UtcNow,
                    Description = secretToAdd.Description,
                    Expiration = secretToAdd.Expiration,
                    Type = secretToAdd.Type,
                    Value = secretToAdd.Value.ToSha256()
                });
            }

            foreach (string redirectUriToAdd in model.RedirectUris)
            {
                entityToCreate.RedirectUris.Add(new ClientRedirectUri
                {
                    ClientId = model.Id,
                    RedirectUri = redirectUriToAdd
                });
            }

            // add the entity's cors origins
            foreach (string corsOriginsToAdd in model.AllowedCorsOrigins)
            {
                entityToCreate.AllowedCorsOrigins.Add(new ClientCorsOrigin
                {
                    ClientId = model.Id,
                    Origin = corsOriginsToAdd
                });
            };

            // add the entity's grant types
            foreach (string grantTypesToAdd in model.AllowedGrantTypes)
            {
                entityToCreate.AllowedGrantTypes.Add(new ClientGrantType
                {
                    ClientId = model.Id,
                    GrantType = grantTypesToAdd
                });
            };

            // add the entity's post logout redirect uris
            foreach (string logoutUrisToAdd in model.PostLogoutRedirectUris)
            {
                entityToCreate.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri
                {
                    ClientId = model.Id,
                    PostLogoutRedirectUri = logoutUrisToAdd
                });
            };

            entityToCreate.AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime;
            entityToCreate.AccessTokenLifetime = model.AccessTokenLifetime;
            entityToCreate.AccessTokenType = model.AccessTokenTypeValue;
            entityToCreate.AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser;
            entityToCreate.AllowOfflineAccess = model.AllowOfflineAccess;
            entityToCreate.AllowPlainTextPkce = model.AllowPlainTextPkce;
            entityToCreate.AllowRememberConsent = model.AllowRememberConsent;
            entityToCreate.AlwaysSendClientClaims = model.AlwaysSendClientClaims;
            entityToCreate.AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken;
            entityToCreate.AuthorizationCodeLifetime = model.AuthorizationCodeLifetime;
            entityToCreate.BackChannelLogoutSessionRequired = model.BackChannelLogoutSessionRequired;
            entityToCreate.BackChannelLogoutUri = model.BackChannelLogoutUri;
            entityToCreate.ClientId = model.ClientId;
            entityToCreate.ClientName = model.ClientName;
            entityToCreate.ClientUri = model.ClientUri;
            entityToCreate.ConsentLifetime = model.ConsentLifetime.ZeroToNull();
            entityToCreate.Description = model.Description;
            entityToCreate.DeviceCodeLifetime = model.DeviceCodeLifetime;
            entityToCreate.Enabled = model.Enabled;
            entityToCreate.EnableLocalLogin = model.EnableLocalLogin;
            entityToCreate.FrontChannelLogoutSessionRequired = model.FrontChannelLogoutSessionRequired;
            entityToCreate.FrontChannelLogoutUri = model.FrontChannelLogoutUri;
            entityToCreate.IdentityTokenLifetime = model.IdentityTokenLifetime;
            entityToCreate.IncludeJwtId = model.IncludeJwtId;
            entityToCreate.LogoUri = model.LogoUri;
            entityToCreate.ProtocolType = model.ProtocolType;
            entityToCreate.RefreshTokenExpiration = model.RefreshTokenExpirationValue;
            entityToCreate.RefreshTokenUsage = model.RefreshTokenUsageValue;
            entityToCreate.RequireClientSecret = model.RequireClientSecret;
            entityToCreate.RequireConsent = model.RequireConsent;
            entityToCreate.RequirePkce = model.RequirePkce;
            entityToCreate.SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime;
            entityToCreate.UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh;
            entityToCreate.UserSsoLifetime = model.UserSsoLifetime.ZeroToNull();

            if (await this.SaveClientAsync(entityToCreate, EntityState.Added, cancellationToken))
            {
                string message = string.Format(Resources.PromptClientCreateSuccessText, model.ClientId, model.ClientName);
                await this.context.AuditLog.LogAsync(AuditEvent.Config, AuditResult.Success, this.context.ClientAddress, message, this.context.OptionalCurrentUserId).ConfigureAwait(false);
            }
            else
            {
                string message = string.Format(Resources.PromptClientCreateFailureText, model.ClientId, model.ClientName);
                await this.context.AuditLog.LogAsync(AuditEvent.Config, AuditResult.Fail, this.context.ClientAddress, message, this.context.OptionalCurrentUserId).ConfigureAwait(false);
            }

            return entityToCreate.ToClientModel();
        }

        /// <summary>
        /// Updates an existing client record.
        /// </summary>
        /// <param name="model">Contains the client model to update.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientModel" /> of the updated record.</returns>
        public async Task<ClientModel> UpdateAsync(ClientModel model, CancellationToken cancellationToken)
        {
            // find entity to update
            var entityToUpdate = await this.context.ConfigurationDbContext.Clients
                .Include(a => a.AllowedScopes)
                .Include(a => a.ClientSecrets)
                .Include(a => a.RedirectUris)
                .Include(a => a.AllowedCorsOrigins)
                .Include(a => a.AllowedGrantTypes)
                .Include(a => a.PostLogoutRedirectUris)
                .Include(a => a.Properties)
                .FirstOrDefaultAsync(a => a.Id == model.Id, cancellationToken: cancellationToken);

            if (entityToUpdate != null)
            {
                // update the entity's allowed scopes (add/update/remove)
                UpdateClientScopes(model, entityToUpdate);

                // update the entity's secrets (add/update/remove)
                UpdateClientSecrets(model, entityToUpdate);

                // update the entity's redirect URIs (add/update/remove)
                UpdateClientRedirectUris(model, entityToUpdate);

                // update the entity's cors origins (add/update/remove)
                UpdateClientCorsOrigins(model, entityToUpdate);

                // update the entity's grant types (add/update/remove)
                UpdateClientGrantTypes(model, entityToUpdate);

                // update the entity's logout redirect uris (add/update/remove)
                UpdateClientLogoutRedirectUris(model, entityToUpdate);

                // update the entity's properties (add/update/remove)
                UpdateClientProperties(model, entityToUpdate);

                // update entity properties create entity properties
                entityToUpdate.AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime;
                entityToUpdate.AccessTokenLifetime = model.AccessTokenLifetime;
                entityToUpdate.AccessTokenType = model.AccessTokenTypeValue;
                entityToUpdate.AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser;
                entityToUpdate.AllowOfflineAccess = model.AllowOfflineAccess;
                entityToUpdate.AllowPlainTextPkce = model.AllowPlainTextPkce;
                entityToUpdate.AllowRememberConsent = model.AllowRememberConsent;
                entityToUpdate.AlwaysSendClientClaims = model.AlwaysSendClientClaims;
                entityToUpdate.AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken;
                entityToUpdate.AuthorizationCodeLifetime = model.AuthorizationCodeLifetime;
                entityToUpdate.BackChannelLogoutSessionRequired = model.BackChannelLogoutSessionRequired;
                entityToUpdate.BackChannelLogoutUri = model.BackChannelLogoutUri;
                entityToUpdate.ClientId = model.ClientId;
                entityToUpdate.ClientName = model.ClientName;
                entityToUpdate.ClientUri = model.ClientUri;
                entityToUpdate.ConsentLifetime = model.ConsentLifetime.ZeroToNull();
                entityToUpdate.Description = model.Description;
                entityToUpdate.DeviceCodeLifetime = model.DeviceCodeLifetime;
                entityToUpdate.Enabled = model.Enabled;
                entityToUpdate.EnableLocalLogin = model.EnableLocalLogin;
                entityToUpdate.FrontChannelLogoutSessionRequired = model.FrontChannelLogoutSessionRequired;
                entityToUpdate.FrontChannelLogoutUri = model.FrontChannelLogoutUri;
                entityToUpdate.IdentityTokenLifetime = model.IdentityTokenLifetime;
                entityToUpdate.IncludeJwtId = model.IncludeJwtId;
                entityToUpdate.LogoUri = model.LogoUri;
                entityToUpdate.ProtocolType = model.ProtocolType;
                entityToUpdate.RefreshTokenExpiration = model.RefreshTokenExpirationValue;
                entityToUpdate.RefreshTokenUsage = model.RefreshTokenUsageValue;
                entityToUpdate.RequireClientSecret = model.RequireClientSecret;
                entityToUpdate.RequireConsent = model.RequireConsent;
                entityToUpdate.RequirePkce = model.RequirePkce;
                entityToUpdate.SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime;
                entityToUpdate.UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh;
                entityToUpdate.UserSsoLifetime = model.UserSsoLifetime;

                if (await this.SaveClientAsync(entityToUpdate, EntityState.Modified, cancellationToken))
                {
                    await this.context.AuditLog.LogAsync(AuditEvent.Config, AuditResult.Success, this.context.ClientAddress, string.Format(Resources.PromptClientUpdateSuccessText, model.ClientId, model.ClientName), this.context.OptionalCurrentUserId).ConfigureAwait(false);
                }
                else
                {
                    await this.context.AuditLog.LogAsync(AuditEvent.Config, AuditResult.Fail, this.context.ClientAddress, string.Format(Resources.PromptClientUpdateFailureText, model.ClientId, model.ClientName), this.context.OptionalCurrentUserId).ConfigureAwait(false);
                }
            }
            else
            {
                this.ErrorManager.CriticalNotFound(string.Format(Resources.ErrorClientNotFoundText, nameof(entityToUpdate.Id), entityToUpdate.Id), ErrorCategory.Application);
            }

            return entityToUpdate.ToClientModel();
        }

        /// <summary>
        /// Deletes an existing client record by identity.
        /// </summary>
        /// <param name="id">Contains the unique identifier of the client to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var clientToDelete = await this.ReadEntityAsync(id, cancellationToken);

            if (clientToDelete != null)
            {
                if (await this.SaveClientAsync(clientToDelete, EntityState.Deleted, cancellationToken))
                {
                    await this.context.AuditLog.LogAsync(AuditEvent.Config, AuditResult.Success, this.context.ClientAddress, string.Format(Resources.PromptClientDeleteSuccessText, clientToDelete.ClientId, clientToDelete.ClientName), this.context.OptionalCurrentUserId).ConfigureAwait(false);
                }
                else
                {
                    await this.context.AuditLog.LogAsync(AuditEvent.Config, AuditResult.Fail, this.context.ClientAddress, string.Format(Resources.PromptClientDeleteFailureText, clientToDelete.ClientId, clientToDelete.ClientName), this.context.OptionalCurrentUserId).ConfigureAwait(false);
                }
            }
        }

        #endregion

        #region Client Claims Related Methods

        /// <summary>
        /// Adds a new claim to the specified client.
        /// </summary>
        /// <param name="model">The claim model to add.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the new <see cref="IdentityServer4.Models.ClientClaim" /> model upon success.</returns>
        public async Task<ClientClaimModel> AddClaimAsync(ClientClaimModel model, CancellationToken cancellationToken)
        {
            ClientClaimModel result = model;
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;

                // if scope currently doesnt exist for client...
                if (client.Claims.All(cc => cc.Type != model.Type))
                {
                    // add new claim entity
                    ClientClaim entity = new ClientClaim
                    {
                        Client = client,
                        ClientId = client.Id,
                        Type = model.Type,
                        Value = model.Value
                    };

                    // add new scope
                    client.Claims.Add(entity);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        result = entity.ToModel();
                        message = string.Format(Resources.PromptClientClaimCreateSuccessText, model.Type, model.Value, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientClaimCreateFailText, model.Type, model.Value, model.ClientId);
                        this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientClaimExistsFailText, model.Type, model.ClientId);
                    this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Updates the claim value for the specified client.
        /// </summary>
        /// <param name="model">Contains the claim model.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the updated <see cref="IdentityServer4.Models.ClientClaim" /> model.</returns>
        public async Task<ClientClaimModel> UpdateClaimAsync(ClientClaimModel model, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                var entityToUpdate = client.Claims.FirstOrDefault(cc => cc.Type == model.Type);

                if (entityToUpdate != null)
                {
                    entityToUpdate.Value = model.Value;

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientClaimUpdateSuccessText, model.Type, model.Value, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientClaimUpdateFailText, model.Type, model.Value, model.ClientId);
                        this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientClaimNotFoundFailText, model.Type, model.ClientId);
                    this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the client claim from the client.
        /// </summary>
        /// <param name="id">Contains the client identifier.</param>
        /// <param name="claimType">Contains the claim type identifier to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeleteScopeAsync(int id, string claimType, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(id, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                var entityToDelete = client.Claims.FirstOrDefault(cc => cc.Type == claimType);

                if (entityToDelete != null)
                {
                    client.Claims.Remove(entityToDelete);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientClaimDeleteSuccessText, claimType, id);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientClaimDeleteFailText, claimType, id);
                        this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientClaimNotFoundFailText, claimType, id);
                    this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Client Scope Related Methods

        /// <summary>
        /// Associates the specified scope to the client.
        /// </summary>
        /// <param name="model">Contains the model to add.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns a new <see cref="ClientScope" /> record created.</returns>
        public async Task<ClientScopeModel> AddScopeAsync(ClientScopeModel model, CancellationToken cancellationToken)
        {
            ClientScopeModel result = null;
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;

                // if scope currently doesnt exist for client...
                if (client.AllowedScopes.All(es => es.Scope != model.Scope))
                {
                    // add new scope
                    var entity = new ClientScope
                    {
                        Client = client,
                        ClientId = client.Id,
                        Scope = model.Scope
                    };

                    // add new scope
                    client.AllowedScopes.Add(entity);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        result = entity.ToModel();
                        message = string.Format(Resources.PromptClientScopeCreateSuccessText, model.Scope, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientScopeCreateFailText, model.Scope, model.ClientId);
                        this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientScopeExistsFailText, model.Scope, model.ClientId);
                    this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Updates the client scope to the client.
        /// </summary>
        /// <param name="model">Contains the model to update.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientScope" /> entity that was updated.</returns>
        public async Task<ClientScopeModel> UpdateScopeAsync(ClientScopeModel model, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);
            ClientScope scopeToUpdate = null;

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                scopeToUpdate = client.AllowedScopes.FirstOrDefault(a => a.Id == model.Id);

                if (scopeToUpdate != null)
                {
                    scopeToUpdate.Scope = model.Scope;

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientScopeUpdateSuccessText, model.Scope, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientScopeUpdateFailText, model.Scope, model.ClientId);
                        this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientScopeExistsFailText, model.Scope, model.ClientId);
                    this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the client scope from the client.
        /// </summary>
        /// <param name="id">Contains the client identifier.</param>
        /// <param name="scopeId">Contains the scope identifier to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeleteScopeAsync(int id, int scopeId, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(id, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                ClientScope scopeToDelete = client.AllowedScopes.FirstOrDefault(a => a.Id == scopeId);

                if (scopeToDelete != null)
                {
                    client.AllowedScopes.Remove(scopeToDelete);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientScopeDeleteSuccessText, scopeToDelete.Scope, id);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientScopeDeleteFailText, scopeToDelete.Scope, id);
                        this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientScopeExistsFailText, scopeId, id);
                    this.ErrorManager.CriticalFormat(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Client Secret Related Methods

        /// <summary>
        /// Adds a client secret to the specified client.
        /// </summary>
        /// <param name="model">Contains the secret to add.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the new <see cref="ClientSecret" /> entity added.</returns>
        public async Task<ClientSecretModel> AddSecretAsync(ClientSecretModel model, CancellationToken cancellationToken)
        {
            ClientSecretModel result = null;
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;

                // if scope currently doesnt exist for client...
                if (client.ClientSecrets.All(es => es.Type != model.Type.ToDescription() && es.Value != model.Value))
                {
                    // add new scope
                    var entity = new ClientSecret
                    {
                        Client = client,
                        ClientId = client.Id,
                        Created = DateTime.UtcNow,
                        Description = model.Description,
                        Expiration = model.Expiration,
                        Type = model.Type.ToDescription(),
                        Value = model.Value.ToSha256()
                    };

                    client.ClientSecrets.Add(entity);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        result = entity.ToModel();
                        message = string.Format(Resources.PromptClientSecretCreateSuccessText, model.Type, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientSecretCreateFailText, model.Type, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientSecretExistsFailText, model.Type, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Updates the secret to the specified client.
        /// </summary>
        /// <param name="model">Contains the secret model for updating.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientSecret" /> updated.</returns>
        public async Task<ClientSecretModel> UpdateSecretAsync(ClientSecretModel model, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                var secretToUpdate = client.ClientSecrets.FirstOrDefault(a => a.Id == model.Id);

                if (secretToUpdate != null)
                {
                    secretToUpdate.Expiration = model.Expiration;
                    secretToUpdate.Description = model.Description;
                    secretToUpdate.Value = model.Value.ToSha256();
                    secretToUpdate.Type = model.Type.ToDescription();

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientSecretUpdateSuccessText, model.Type, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientSecretUpdateFailText, model.Type, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientSecretExistsFailText, model.Type, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the secret from the specified client.
        /// </summary>
        /// <param name="id">Contains the client identifier.</param>
        /// <param name="secretId">Contains the secret identifier to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeleteSecretAsync(int id, int secretId, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(id, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                ClientSecret secretToDelete = client.ClientSecrets.FirstOrDefault(a => a.Id == secretId);

                if (secretToDelete != null)
                {
                    client.ClientSecrets.Remove(secretToDelete);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientSecretDeleteSuccessText, secretToDelete.Type, id);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientSecretDeleteFailText, secretToDelete.Type, id);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientSecretExistsFailText, secretId, id);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Client Allowed Redirect Related Methods

        /// <summary>
        /// Adds the redirect to the client specified.
        /// </summary>
        /// <param name="model">Contains the model to add.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the new <see cref="ClientRedirectUri" /> entity added.</returns>
        public async Task<ClientRedirectUriModel> AddRedirectAsync(ClientRedirectUriModel model, CancellationToken cancellationToken)
        {
            ClientRedirectUriModel result = null;
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;

                // if scope currently doesnt exist for client...
                if (client.RedirectUris.All(r => r.RedirectUri != model.RedirectUri.ToString()))
                {
                    // add new scope
                    var entity = new ClientRedirectUri
                    {
                        Client = client,
                        ClientId = client.Id,
                        RedirectUri = model.RedirectUri.ToString()
                    };

                    client.RedirectUris.Add(entity);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        result = entity.ToModel();
                        message = string.Format(Resources.PromptClientRedirectCreateSuccessText, model.RedirectUri, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientRedirectCreateFailText, model.RedirectUri, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientRedirectExistsFailText, model.RedirectUri, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Updates the specified redirect for the client.
        /// </summary>
        /// <param name="model">Contains the redirect model to update.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientRedirectUri" /> updated.</returns>
        public async Task<ClientRedirectUriModel> UpdateRedirectAsync(ClientRedirectUriModel model, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                var redirectToUpdate = client.RedirectUris.FirstOrDefault(a => a.Id == model.Id);

                if (redirectToUpdate != null)
                {
                    redirectToUpdate.RedirectUri = model.RedirectUri.ToString();

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientRedirectUpdateSuccessText, model.RedirectUri, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientRedirectUpdateFailText, model.RedirectUri, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientRedirectExistsFailText, model.RedirectUri, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the redirect from the specified client.
        /// </summary>
        /// <param name="id">Contains the client unique identifier.</param>
        /// <param name="redirectId">Contains the redirect identifier to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeleteRedirectAsync(int id, int redirectId, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(id, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                ClientRedirectUri redirectToDelete = client.RedirectUris.FirstOrDefault(a => a.Id == redirectId);

                if (redirectToDelete != null)
                {
                    client.RedirectUris.Remove(redirectToDelete);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientRedirectDeleteSuccessText, redirectToDelete.RedirectUri, id);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientRedirectDeleteFailText, redirectToDelete.RedirectUri, id);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientRedirectExistsFailText, redirectId, id);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Client Allowed Origin Related Methods

        /// <summary>
        /// Adds the allowed origin to the specified client.
        /// </summary>
        /// <param name="model">Contains the model to add.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientCorsOrigin" /> entity added.</returns>
        public async Task<ClientCorsOriginModel> AddOriginAsync(ClientCorsOriginModel model, CancellationToken cancellationToken)
        {
            ClientCorsOriginModel result = null;
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;

                // if scope currently doesnt exist for client...
                if (client.AllowedCorsOrigins.All(r => r.Origin != model.Origin))
                {
                    // add new scope
                    var entity = new ClientCorsOrigin
                    {
                        Client = client,
                        ClientId = client.Id,
                        Origin = model.Origin
                    };

                    client.AllowedCorsOrigins.Add(entity);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        result = entity.ToModel();
                        message = string.Format(Resources.PromptClientOriginCreateSuccessText, model.Origin, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientOriginCreateFailText, model.Origin, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientOriginExistsFailText, model.Origin, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Updates the allowed origin for the specified client.
        /// </summary>
        /// <param name="model">Contains the allowed origin model to update.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientCorsOrigin" /> entity updated.</returns>
        public async Task<ClientCorsOriginModel> UpdateOriginAsync(ClientCorsOriginModel model, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                var originToUpdate = client.AllowedCorsOrigins.FirstOrDefault(a => a.Id == model.Id);

                if (originToUpdate != null)
                {
                    originToUpdate.Origin = model.Origin;

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientOriginUpdateSuccessText, model.Origin, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientOriginUpdateFailText, model.Origin, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientOriginExistsFailText, model.Origin, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the allowed origin from the client.
        /// </summary>
        /// <param name="id">Contains the client unique identifier.</param>
        /// <param name="originId">Contains the allowed origin identifier to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeleteOriginAsync(int id, int originId, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(id, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                ClientCorsOrigin originToDelete = client.AllowedCorsOrigins.FirstOrDefault(a => a.Id == originId);

                if (originToDelete != null)
                {
                    client.AllowedCorsOrigins.Remove(originToDelete);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientOriginDeleteSuccessText, originToDelete.Origin, id);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientOriginDeleteFailText, originToDelete.Origin, id);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientOriginExistsFailText, originId, id);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Client Grant Type Related Methods

        /// <summary>
        /// Adds the Grant Type to the specified client.
        /// </summary>
        /// <param name="model">Contains the model to add.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientGrantType" /> entity added.</returns>
        public async Task<ClientGrantTypeModel> AddGrantTypeAsync(ClientGrantTypeModel model, CancellationToken cancellationToken)
        {
            ClientGrantTypeModel result = null;
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;

                // if scope currently doesnt exist for client...
                if (client.AllowedGrantTypes.All(r => r.GrantType != model.Type.ToDescription()))
                {
                    // add new scope
                    var entity = new ClientGrantType
                    {
                        Client = client,
                        ClientId = client.Id,
                        GrantType = model.Type.ToDescription()
                    };

                    client.AllowedGrantTypes.Add(entity);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        result = entity.ToModel();
                        message = string.Format(Resources.PromptClientGrantTypeCreateSuccessText, model.Type, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientGrantTypeCreateFailText, model.Type, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientGrantTypeExistsFailText, model.Type, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Updates the Grant Type for the specified client.
        /// </summary>
        /// <param name="model">Contains the Grant Type model to update.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientGrantType" /> entity updated.</returns>
        public async Task<ClientGrantTypeModel> UpdateGrantTypeAsync(ClientGrantTypeModel model, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                var grantTypeToUpdate = client.AllowedGrantTypes.FirstOrDefault(a => a.Id == model.Id);

                if (grantTypeToUpdate != null)
                {
                    grantTypeToUpdate.GrantType = model.Type.ToDescription();

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientGrantTypeUpdateSuccessText, model.Type, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientGrantTypeUpdateFailText, model.Type, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientGrantTypeExistsFailText, model.Type, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the Grant Type from the specified client.
        /// </summary>
        /// <param name="id">Contains the client unqiue identifier.</param>
        /// <param name="grantTypeId">Contains the Grant Type identifier to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeleteGrantTypeAsync(int id, int grantTypeId, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(id, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                ClientGrantType grantTypeToDelete = client.AllowedGrantTypes.FirstOrDefault(a => a.Id == grantTypeId);

                if (grantTypeToDelete != null)
                {
                    client.AllowedGrantTypes.Remove(grantTypeToDelete);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientGrantTypeDeleteSuccessText, grantTypeToDelete.GrantType, id);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientGrantTypeDeleteFailText, grantTypeToDelete.GrantType, id);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientGrantTypeExistsFailText, grantTypeId, id);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Client Logout Redirect Related Methods

        /// <summary>
        /// Adds the Logout Redirect to the specified client.
        /// </summary>
        /// <param name="model">Contains the model to add.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientPostLogoutRedirectUri" /> entity added.</returns>
        public async Task<ClientLogoutRedirectUriModel> AddPostLogoutRedirectAsync(ClientLogoutRedirectUriModel model, CancellationToken cancellationToken)
        {
            ClientLogoutRedirectUriModel result = null;
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;

                // if scope currently doesnt exist for client...
                if (client.PostLogoutRedirectUris.All(r => r.PostLogoutRedirectUri != model.LogoutRedirectUri.ToString()))
                {
                    // add new scope
                    var entity = new ClientPostLogoutRedirectUri
                    {
                        Client = client,
                        ClientId = client.Id,
                        PostLogoutRedirectUri = model.LogoutRedirectUri.ToString()
                    };

                    client.PostLogoutRedirectUris.Add(entity);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        result = entity.ToModel();
                        message = string.Format(Resources.PromptClientLogoutCreateSuccessText, model.LogoutRedirectUri, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientLogoutCreateFailText, model.LogoutRedirectUri, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientLogoutExistsFailText, model.LogoutRedirectUri, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Updates the Logout Redirect for the specified client.
        /// </summary>
        /// <param name="model">Contains the Logout Redirect model to update.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientPostLogoutRedirectUri" /> entity updated.</returns>
        public async Task<ClientLogoutRedirectUriModel> UpdatePostLogoutRedirectTypeAsync(ClientLogoutRedirectUriModel model, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                var logoutToUpdate = client.PostLogoutRedirectUris.FirstOrDefault(a => a.Id == model.Id);

                if (logoutToUpdate != null)
                {
                    logoutToUpdate.PostLogoutRedirectUri = model.LogoutRedirectUri.ToString();

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientLogoutUpdateSuccessText, model.LogoutRedirectUri, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientLogoutUpdateFailText, model.LogoutRedirectUri, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientLogoutExistsFailText, model.LogoutRedirectUri, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the Logout Redirect from the specified client.
        /// </summary>
        /// <param name="id">Contains the client unqiue identifier.</param>
        /// <param name="logoutId">Contains the Logout Redirect identifier to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeletePostLogoutRedirectTypeAsync(int id, int logoutId, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(id, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                ClientPostLogoutRedirectUri logoutToDelete = client.PostLogoutRedirectUris.FirstOrDefault(a => a.Id == logoutId);

                if (logoutToDelete != null)
                {
                    client.PostLogoutRedirectUris.Remove(logoutToDelete);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientLogoutDeleteSuccessText, logoutToDelete.PostLogoutRedirectUri, id);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientLogoutDeleteFailText, logoutToDelete.PostLogoutRedirectUri, id);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientLogoutExistsFailText, logoutId, id);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Client Property Related Methods

        /// <summary>
        /// Adds the Property to the specified client.
        /// </summary>
        /// <param name="model">Contains the Property to add.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientProperty" /> entity added.</returns>
        public async Task<ClientPropertyModel> AddPropertyAsync(ClientPropertyModel model, CancellationToken cancellationToken)
        {
            ClientPropertyModel result = null;
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;

                // if scope currently doesnt exist for client...
                if (client.Properties.All(r => r.Key != model.Key))
                {
                    // add new scope
                    var entity = new ClientProperty
                    {
                        Client = client,
                        ClientId = client.Id,
                        Key = model.Key,
                        Value = model.Value
                    };

                    client.Properties.Add(entity);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        result = entity.ToModel();
                        message = string.Format(Resources.PromptClientPropertyCreateSuccessText, model.Key, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientPropertyCreateFailText, model.Key, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientPropertyExistsFailText, model.Key, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Updates the Property for the specified client.
        /// </summary>
        /// <param name="model">Contains the Property model to update.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the <see cref="ClientProperty" /> updated.</returns>
        public async Task<ClientPropertyModel> UpdatePropertyAsync(ClientPropertyModel model, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(model.ClientId, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                var propertyToUpdate = client.Properties.FirstOrDefault(a => a.Id == model.Id);

                if (propertyToUpdate != null)
                {
                    propertyToUpdate.Key = model.Key;
                    propertyToUpdate.Value = model.Value;

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientPropertyUpdateSuccessText, model.Key, model.ClientId);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientPropertyUpdateFailText, model.Key, model.ClientId);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientPropertyExistsFailText, model.Key, model.ClientId);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }

            return model;
        }

        /// <summary>
        /// Deletes the Property from the specified client.
        /// </summary>
        /// <param name="id">Contains the client unqiue identifier.</param>
        /// <param name="propertyId">Contains the Property identifier to delete.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        public async Task DeletePropertyAsync(int id, int propertyId, CancellationToken cancellationToken)
        {
            var client = await this.ReadEntityAsync(id, cancellationToken);

            if (client != null)
            {
                string message = string.Empty;
                AuditResult auditResult = AuditResult.Success;
                ClientProperty propertyToDelete = client.Properties.FirstOrDefault(a => a.Id == propertyId);

                if (propertyToDelete != null)
                {
                    client.Properties.Remove(propertyToDelete);

                    if (await this.SaveClientAsync(client, EntityState.Modified, cancellationToken))
                    {
                        message = string.Format(Resources.PromptClientPropertyDeleteSuccessText, propertyToDelete.Key, id);
                    }
                    else
                    {
                        auditResult = AuditResult.Fail;
                        message = string.Format(Resources.PromptClientPropertyDeleteFailText, propertyToDelete.Key, id);
                        this.ErrorManager.Critical(message, ErrorCategory.Application);
                    }
                }
                else
                {
                    auditResult = AuditResult.Fail;
                    message = string.Format(Resources.PromptClientPropertyExistsFailText, propertyId, id);
                    this.ErrorManager.Critical(message, ErrorCategory.Application);
                }

                await this.context.AuditLog.LogAsync(AuditEvent.Config, auditResult, this.context.ClientAddress, message, this.context.OptionalCurrentUserId, cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is used to execute an entity state save to the database.
        /// </summary>
        /// <param name="entity">Contains the entity to add, modify, or delete.</param>
        /// <param name="state">Contains the state of the entity to execute on the database.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns a value indicating whether the save execution was successful.</returns>
        private async Task<bool> SaveClientAsync(Duende.IdentityServer.EntityFramework.Entities.Client entity, EntityState state = EntityState.Modified, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            bool result = false;

            if (await this.ValidateAsync(entity, state, cancellationToken))
            {
                switch (state)
                {
                    case EntityState.Added:
                        await this.context.ConfigurationDbContext.Clients.AddAsync(entity, cancellationToken);
                        break;

                    case EntityState.Deleted:
                        this.context.ConfigurationDbContext.Clients.Remove(entity);
                        break;
                }

                result = await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            }

            return result;
        }

        /// <summary>
        /// This method is used to determine if the record meets all business rules during the save method.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="method">The method of record action to validate.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns true if the entity is validated for the state method.</returns>
        private async Task<bool> ValidateAsync(Duende.IdentityServer.EntityFramework.Entities.Client entity, EntityState method, CancellationToken cancellationToken)
        {
            if (method == EntityState.Added || method == EntityState.Modified)
            {
                if (string.IsNullOrWhiteSpace(entity.ClientId))
                {
                    this.ErrorManager.Validation(nameof(Duende.IdentityServer.EntityFramework.Entities.Client.ClientId), ResourceKeys.PromptClientIdRequiredText);
                }
                else if (method == EntityState.Added)
                {
                    // make sure user doesn't already exist
                    if (await this.context.ConfigurationDbContext.Clients.AnyAsync(u => u.ClientId == entity.ClientId, cancellationToken))
                    {
                        this.ErrorManager.ValidationFormat(nameof(User.UserName), ResourceKeys.PromptClientIdExistsText, ErrorCategory.General, entity.ClientId);
                    }
                }
            }

            return !this.ErrorManager.HasErrors;
        }

        /// <summary>
        /// This method is used to update allowed scopes for a client.
        /// </summary>
        /// <param name="model">Contains the model that includes the client scopes.</param>
        /// <param name="entity">Contains the entity that will be updated.</param>
        private static void UpdateClientScopes(ClientModel model, Duende.IdentityServer.EntityFramework.Entities.Client entity)
        {
            // find valid (non-Null) models
            var validScopes = model.AllowedScopes.Where(scope => !string.IsNullOrWhiteSpace(scope)).ToList();

            // get all valid scopes that are not currently associated with entity
            var scopesToAdd = validScopes.Where(scope => entity.AllowedScopes.All(cs => cs.Scope != scope)).ToList();

            foreach (string scope in scopesToAdd)
            {
                entity.AllowedScopes.Add(new ClientScope
                {
                    ClientId = entity.Id,
                    Client = entity,
                    Scope = scope
                });
            }

            // handle removal of scopes not in model but in entity
            var scopesToDelete = entity.AllowedScopes.Where(cs => validScopes.All(scope => scope != cs.Scope)).ToList();

            foreach (var scopeToDelete in scopesToDelete)
            {
                entity.AllowedScopes.Remove(scopeToDelete);
            }
        }

        /// <summary>
        /// This method is used to update secrets for a client.
        /// </summary>
        /// <param name="model">Contains the model that includes the client secrets.</param>
        /// <param name="entity">Contains the entity that will be updated.</param>
        private static void UpdateClientSecrets(ClientModel model, Duende.IdentityServer.EntityFramework.Entities.Client entity)
        {
            // find valid (non-Null) models
            var validSecrets = model.ClientSecrets.Where(m => m != null && !string.IsNullOrWhiteSpace(m.Value)).ToList();

            // secrets to add
            var secretsToAdd = validSecrets.Where(vs => entity.ClientSecrets.All(cs => vs.Type != cs.Type && vs.Value != cs.Value)).ToList();

            foreach (var secret in secretsToAdd)
            {
                entity.ClientSecrets.Add(new ClientSecret
                {
                    Client = entity,
                    ClientId = entity.Id,
                    Created = DateTime.UtcNow,
                    Description = secret.Description,
                    Expiration = secret.Expiration,
                    Type = secret.Type,
                    Value = secret.Value
                });
            }

            // handle deletes
            var secretsToDelete = entity.ClientSecrets.Where(cs => validSecrets.All(vs => vs.Type != cs.Type && vs.Value != cs.Value)).ToList();

            foreach (var secretToDelete in secretsToDelete)
            {
                entity.ClientSecrets.Remove(secretToDelete);
            }
        }

        /// <summary>
        /// This method is used to update redirect URIs for a client.
        /// </summary>
        /// <param name="model">Contains the model that includes the client redirect URIs.</param>
        /// <param name="entity">Contains the entity that will be updated.</param>
        private static void UpdateClientRedirectUris(ClientModel model, Duende.IdentityServer.EntityFramework.Entities.Client entity)
        {
            entity.RedirectUris.Clear();

            foreach (var redirectUriToAdd in model.RedirectUris)
            {
                entity.RedirectUris.Add(new ClientRedirectUri
                {
                    Client = entity,
                    RedirectUri = redirectUriToAdd
                });
            }
        }

        /// <summary>
        /// This method is used to update CORS origins for a client.
        /// </summary>
        /// <param name="model">Contains the model that includes the client CORS origins.</param>
        /// <param name="entity">Contains the entity that will be updated.</param>
        private static void UpdateClientCorsOrigins(ClientModel model, Duende.IdentityServer.EntityFramework.Entities.Client entity)
        {
            entity.AllowedCorsOrigins.Clear();

            foreach (var corsOriginToAdd in model.AllowedCorsOrigins)
            {
                entity.AllowedCorsOrigins.Add(new ClientCorsOrigin
                {
                    Client = entity,
                    Origin = corsOriginToAdd
                });
            }
        }

        /// <summary>
        /// This method is used to update grant types for a client.
        /// </summary>
        /// <param name="model">Contains the model that includes the client grant types.</param>
        /// <param name="entity">Contains the entity that will be updated.</param>
        private static void UpdateClientGrantTypes(ClientModel model, Duende.IdentityServer.EntityFramework.Entities.Client entity)
        {
            entity.AllowedGrantTypes.Clear();

            foreach (var grantTypeToAdd in model.AllowedGrantTypes)
            {
                entity.AllowedGrantTypes.Add(new ClientGrantType
                {
                    Client = entity,
                    GrantType = grantTypeToAdd
                });
            }
        }

        /// <summary>
        /// This method is used to update logout redirect uris for a client.
        /// </summary>
        /// <param name="model">Contains the model that includes the client logout redirect uris.</param>
        /// <param name="entity">Contains the entity that will be updated.</param>
        private static void UpdateClientLogoutRedirectUris(ClientModel model, Duende.IdentityServer.EntityFramework.Entities.Client entity)
        {
            entity.PostLogoutRedirectUris.Clear();

            foreach (var logoutRedirectUriToAdd in model.PostLogoutRedirectUris)
            {
                entity.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri
                {
                    Client = entity,
                    PostLogoutRedirectUri = logoutRedirectUriToAdd
                });
            }
        }

        /// <summary>
        /// This method is used to update properties for a client.
        /// </summary>
        /// <param name="model">Contains the model that includes the client properties.</param>
        /// <param name="entity">Contains the entity that will be updated.</param>
        private static void UpdateClientProperties(ClientModel model, Duende.IdentityServer.EntityFramework.Entities.Client entity)
        {
            entity.Properties.Clear();

            foreach (var propertyToAdd in model.Properties.Keys)
            {
                entity.Properties.Add(new ClientProperty
                {
                    Client = entity,
                    Key = propertyToAdd,
                    Value = model.Properties[propertyToAdd]
                });
            }
        }

        #endregion
    }
}
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

namespace Bastille.Id.Server.Core.Data
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Runtime;
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using Bastille.Id.Core.Properties;
    using Bastille.Id.Core.Security;
    using IdentityModel;
    using IdentityServer4.EntityFramework.DbContexts;
    using Microsoft.AspNetCore.Identity;
    using Talegen.Common.Models.Security;

    /// <summary>
    /// This class contains static methods used to initialize database records
    /// </summary>
    public static class DataInstallHelpers
    {
        /// <summary>
        /// Initializes a user in the database.
        /// </summary>
        /// <param name="appContext">Contains the application database context.</param>
        /// <param name="roleManager">Contains a role manager.</param>
        /// <param name="userManager">Contains a user manager.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        public static async Task InitializeDefaultSecurityDataAsync(ApplicationDbContext appContext, RoleManager<Role> roleManager, UserManager<User> userManager, CancellationToken cancellationToken = default)
        {
            if (appContext == null)
            {
                throw new ArgumentNullException(nameof(appContext));
            }

            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager));
            }

            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            // get a default organization
            var defaultOrganization = appContext.Organizations.FirstOrDefault(o => o.Slug == SecurityDefaults.DefaultOrganizationSlug);

            // find any default system roles...
            var adminRole = appContext.Roles.FirstOrDefault(r => r.Name == SecurityDefaults.AdministratorRoleName);
            var supportRole = appContext.Roles.FirstOrDefault(r => r.Name == SecurityDefaults.SupportRoleName);

            // find a default user.
            var defaultUser = appContext.Users.FirstOrDefault(u => u.Email == SecurityDefaults.DefaultUserEmail);

            // if there are no users we're likely installing...
            if (!appContext.Users.Any())
            {
                // we need to create the default user
                defaultUser = new User { UserName = SecurityDefaults.DefaultUserEmail, Email = SecurityDefaults.DefaultUserEmail };
                await userManager.CreateAsync(defaultUser, SecurityDefaults.DefaultUserPassword);

                if (defaultOrganization == null)
                {
                    // create new organization, making default user the owner.
                    defaultOrganization = new Organization { Name = SecurityDefaults.DefaultOrganizationName, Slug = SecurityDefaults.DefaultOrganizationSlug, OwnerUserId = defaultUser.Id };
                    appContext.Organizations.Add(defaultOrganization);

                    // add default domain for organization users.
                    OrganizationAllowedDomain organizationAllowedDomain = new OrganizationAllowedDomain { Organization = defaultOrganization, Domain = SecurityDefaults.DefaultOrganizationDomain };
                    appContext.OrganizationAllowedDomains.Add(organizationAllowedDomain);

                    // add default group
                    Group defaultGroup = new Group { Name = SecurityDefaults.DefaultOrganizationName, Organization = defaultOrganization, OwnerUserId = defaultUser.Id };
                    appContext.Groups.Add(defaultGroup);

                    // add default user to new default group
                    GroupUser defaultGroupUser = new GroupUser { Group = defaultGroup, User = defaultUser };
                    appContext.GroupUsers.Add(defaultGroupUser);

                    // save organization changes
                    await appContext.SaveChangesAsync(cancellationToken);
                }

                // if not found, create admin role...
                if (adminRole == null)
                {
                    adminRole = new Role { Name = SecurityDefaults.AdministratorRoleName, RoleType = RoleTypes.System };
                    await roleManager.CreateAsync(adminRole);
                }

                // if not found, create support role...
                if (supportRole == null)
                {
                    supportRole = new Role { Name = SecurityDefaults.SupportRoleName, RoleType = RoleTypes.System };
                    await roleManager.CreateAsync(supportRole);
                }

                // add default user to the system administrators role.
                if (!appContext.UserRoles.Any(ur => ur.UserId == defaultUser.Id && ur.RoleId == adminRole.Id))
                {
                    await userManager.AddToRoleAsync(defaultUser, adminRole.Name);
                }

                // add default user to the support role.
                if (!appContext.UserRoles.Any(ur => ur.UserId == defaultUser.Id && ur.RoleId == supportRole.Id))
                {
                    await userManager.AddToRoleAsync(defaultUser, supportRole.Name);
                }
            }

            ////if (!appContext.Claims.Any())
            ////{
            ////    // create available claims
            ////    List<System.Security.Claims.Claim> defaultClaims = new List<System.Security.Claims.Claim>
            ////    {
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimAuditRead, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimIdentityRead, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimIdentityManage, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimProductRead, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimProductManage, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimSubscriptionRead, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimSubscriptionManage, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimSecurityManage, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimOrgRead, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimOrgManage, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimProfileRead, string.Empty),
            ////        new System.Security.Claims.Claim(SecurityDefaults.ClaimProfileManage, string.Empty)
            ////    };

            ////    defaultClaims.ForEach(defaultClaim =>
            ////    {
            ////        appContext.Claims.Add(new Claim { ClaimType = defaultClaim.Type, DefaultValue = defaultClaim.Value });
            ////    });

            ////    appContext.SaveChanges();

            ////    // add claims to admin and support role
            ////    defaultClaims.ForEach(defaultClaim =>
            ////    {
            ////        AsyncHelper.RunSync(() => roleManager.AddClaimAsync(adminRole, defaultClaim));
            ////        AsyncHelper.RunSync(() => roleManager.AddClaimAsync(supportRole, defaultClaim));
            ////    });
            ////}
        }

        public static async Task InitializeDefaultIdentityDataAsync(ConfigurationDbContext configurationDbContext, CancellationToken cancellationToken = default)
        {
            await configurationDbContext.IdentityResources.AddAsync(new IdentityServer4.EntityFramework.Entities.IdentityResource
            {
                Name = "openid",
                DisplayName = "Your user identifier",
                Description = "Your user identifier",
                Enabled = true,
                Required = true,
                Emphasize = false,
                NonEditable = true,
                UserClaims = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.IdentityResourceClaim>
                {
                    new IdentityServer4.EntityFramework.Entities.IdentityResourceClaim { Type = "sub" }
                },
                ShowInDiscoveryDocument = true,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            }, cancellationToken);

            // 
            await configurationDbContext.IdentityResources.AddAsync(new IdentityServer4.EntityFramework.Entities.IdentityResource
            {
                Name = "profile",
                DisplayName = "Your profile information",
                Description = "Your profile information",
                Enabled = true,
                Required = true,
                Emphasize = false,
                NonEditable = true,
                UserClaims = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.IdentityResourceClaim>
                {
                    new IdentityServer4.EntityFramework.Entities.IdentityResourceClaim { Type = "name" },
                    new IdentityServer4.EntityFramework.Entities.IdentityResourceClaim { Type = "email" },
                    new IdentityServer4.EntityFramework.Entities.IdentityResourceClaim { Type = "email_verified" },
                    new IdentityServer4.EntityFramework.Entities.IdentityResourceClaim { Type = "website" },
                    new IdentityServer4.EntityFramework.Entities.IdentityResourceClaim { Type = "address" },
                    new IdentityServer4.EntityFramework.Entities.IdentityResourceClaim { Type = "phone_number" }
                },
                ShowInDiscoveryDocument = true,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            }, cancellationToken);

            // this is the user level scope
            await configurationDbContext.ApiScopes.AddAsync(new IdentityServer4.EntityFramework.Entities.ApiScope
            {
                Name = "basic_user",
                DisplayName = "Basic User Subscriber",
                Description = "Is the user a basic subscriber user.",
                Enabled = true,
                Required = true,
                Emphasize = false,
                ShowInDiscoveryDocument = true,
                UserClaims = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ApiScopeClaim>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiScopeClaim { Type = "user_level" }
                }
            }, cancellationToken);

            // this is a premium user scope
            await configurationDbContext.ApiScopes.AddAsync(new IdentityServer4.EntityFramework.Entities.ApiScope
            {
                Name = "premium_user",
                DisplayName = "Premium User",
                Description = "Is user a premium subscriber.",
                Enabled = true,
                Required = true,
                Emphasize = false,
                ShowInDiscoveryDocument = true,
                UserClaims = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ApiScopeClaim>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiScopeClaim { Type = "premium_user" }
                }
            }, cancellationToken);

            // this is the swept api resource
            await configurationDbContext.ApiResources.AddAsync(new IdentityServer4.EntityFramework.Entities.ApiResource
            {
                Name = "swept_api",
                DisplayName = "Swept API",
                Description = "Swept API",
                Enabled = true,
                ShowInDiscoveryDocument = true,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                UserClaims = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ApiResourceClaim>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiResourceClaim { Type = "user_level" },
                    new IdentityServer4.EntityFramework.Entities.ApiResourceClaim { Type = "premium_user" }
                },
                Scopes = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ApiResourceScope>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiResourceScope { Scope = "user_level" },
                    new IdentityServer4.EntityFramework.Entities.ApiResourceScope { Scope = "premium_user" }
                },
                Secrets = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ApiResourceSecret>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiResourceSecret { Value = "change_me123!".ToSha256() }
                }
            }, cancellationToken);

            // this is the identity-server client that will be used to communicate with the API (e.g., SCIM)
            await configurationDbContext.Clients.AddAsync(new IdentityServer4.EntityFramework.Entities.Client
            {
                ClientId = "identity_server",
                ClientName = "Identity Server",
                Description = "Identity Server",
                ClientSecrets = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientSecret>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientSecret { Value = "change_me123!".ToSha256() }
                },
                AllowedGrantTypes = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientGrantType>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientGrantType { GrantType = "client_credentials" }
                },
                AllowedScopes = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientScope>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "openid" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "profile" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "user_level" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "premium_user" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "swept_api" }
                },
                RedirectUris = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientRedirectUri { RedirectUri = "https://localhost:5001/signin-oidc" }
                },
                PostLogoutRedirectUris = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri { PostLogoutRedirectUri = "https://localhost:5001/signout-callback-oidc" }
                },
                RequirePkce = true,
                RequireClientSecret = true,
                AllowOfflineAccess = false,
                Enabled = true,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                AccessTokenType = (int)IdentityServer4.Models.AccessTokenType.Jwt,
                IdentityTokenLifetime = 300,
                AccessTokenLifetime = 3600,
                AuthorizationCodeLifetime = 300,
                AbsoluteRefreshTokenLifetime = 2592000,
                RefreshTokenUsage = (int)IdentityServer4.Models.TokenUsage.OneTimeOnly,
                RefreshTokenExpiration = (int)IdentityServer4.Models.TokenExpiration.Absolute,
                UpdateAccessTokenClaimsOnRefresh = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowAccessTokensViaBrowser = true,
                FrontChannelLogoutUri = "https://localhost:5001/signout-oidc",
                FrontChannelLogoutSessionRequired = true,
                BackChannelLogoutUri = "https://localhost:5001/signout-oidc",
                BackChannelLogoutSessionRequired = true,
                SlidingRefreshTokenLifetime = 1296000,
                ConsentLifetime = 300,
                EnableLocalLogin = true,
                IncludeJwtId = true,
                AlwaysSendClientClaims = true,
                ClientClaimsPrefix = "client_",
                AllowRememberConsent = true,
                ProtocolType = "oidc",
                RequireConsent = false,
                AllowPlainTextPkce = false,
                RequireRequestObject = false,
                ClientUri = "https://localhost:5001",
                LogoUri = "https://localhost:5001/images/logo.png"

            }, cancellationToken);

            // this is the client that will be used by the web app
            await configurationDbContext.Clients.AddAsync(new IdentityServer4.EntityFramework.Entities.Client
            {
                ClientId = "swept_web_app",
                ClientName = "Swept Web App",
                Description = "Swept Web App",
                ClientSecrets = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientSecret>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientSecret { Value = "change_me123!".ToSha256() }
                },
                AllowedGrantTypes = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientGrantType>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientGrantType { GrantType = "code" }
                },
                AllowedScopes = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientScope>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "openid" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "profile" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "user_level" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "premium_user" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "swept_api" }
                },
                RedirectUris = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientRedirectUri { RedirectUri = "https://localhost:5001/signin-oidc" }
                },
                PostLogoutRedirectUris = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri { PostLogoutRedirectUri = "https://localhost:5001/signout-callback-oidc" }
                },
                RequirePkce = true,
                RequireClientSecret = false,
                AllowOfflineAccess = true,
                Enabled = true,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                AccessTokenType = (int)IdentityServer4.Models.AccessTokenType.Jwt,
                IdentityTokenLifetime = 300,
                AccessTokenLifetime = 3600,
                AuthorizationCodeLifetime = 300,
                AbsoluteRefreshTokenLifetime = 2592000,
                RefreshTokenUsage = (int)IdentityServer4.Models.TokenUsage.OneTimeOnly,
                RefreshTokenExpiration = (int)IdentityServer4.Models.TokenExpiration.Absolute,
                UpdateAccessTokenClaimsOnRefresh = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowAccessTokensViaBrowser = true,
                FrontChannelLogoutUri = "https://localhost:5001/signout-oidc",
                FrontChannelLogoutSessionRequired = true,
                BackChannelLogoutUri = "https://localhost:5001/signout-oidc",
                BackChannelLogoutSessionRequired = true,
                SlidingRefreshTokenLifetime = 1296000,
                ConsentLifetime = 300,
                EnableLocalLogin = true,
                IncludeJwtId = true,
                AlwaysSendClientClaims = true,
                ClientClaimsPrefix = "client_",
                AllowRememberConsent = true,
                ProtocolType = "oidc",
                RequireConsent = false,
                AllowPlainTextPkce = false,
                RequireRequestObject = false,
                ClientUri = "https://localhost:5001",
                LogoUri = "https://localhost:5001/images/logo.png"
            }, cancellationToken);
            
            // this is the client that will be used by the mobile app
            await configurationDbContext.Clients.AddAsync(new IdentityServer4.EntityFramework.Entities.Client
            {
                ClientId = "swept_mobile_android_app",
                ClientName = "Swept Mobile Android App",
                Description = "Swept Mobile Android App",
                ClientSecrets = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientSecret>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientSecret { Value = "change_me123!".ToSha256() }
                },
                AllowedGrantTypes = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientGrantType>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientGrantType { GrantType = "code" }
                },
                AllowedScopes = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientScope>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "openid" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "profile" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "user_level" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "premium_user" },
                    new IdentityServer4.EntityFramework.Entities.ClientScope { Scope = "swept_api" }
                },
                RedirectUris = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientRedirectUri { RedirectUri = "https://localhost:5001/signin-oidc" }
                },
                PostLogoutRedirectUris = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>
                {
                    new IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri { PostLogoutRedirectUri = "https://localhost:5001/signout-callback-oidc" }
                },
                RequirePkce = true,
                RequireClientSecret = false,
                AllowOfflineAccess = true,
                Enabled = true,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                AccessTokenType = (int)IdentityServer4.Models.AccessTokenType.Jwt,
                IdentityTokenLifetime = 300,
                AccessTokenLifetime = 3600,
                AuthorizationCodeLifetime = 300,
                AbsoluteRefreshTokenLifetime = 2592000,
                RefreshTokenUsage = (int)IdentityServer4.Models.TokenUsage.OneTimeOnly,
                RefreshTokenExpiration = (int)IdentityServer4.Models.TokenExpiration.Absolute,
                UpdateAccessTokenClaimsOnRefresh = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowAccessTokensViaBrowser = true,
                FrontChannelLogoutUri = "https://localhost:5001/signout-oidc",
                FrontChannelLogoutSessionRequired = true,
                BackChannelLogoutUri = "https://localhost:5001/signout-oidc",
                BackChannelLogoutSessionRequired = true,
                SlidingRefreshTokenLifetime = 1296000,
                ConsentLifetime = 300,
                EnableLocalLogin = true,
                IncludeJwtId = true,
                AlwaysSendClientClaims = true,
                ClientClaimsPrefix = "client_",
                AllowRememberConsent = true,
                ProtocolType = "oidc",
                RequireConsent = false,
                AllowPlainTextPkce = false,
                RequireRequestObject = false,
                ClientUri = "https://localhost:5001",
                LogoUri = "https://localhost:5001/images/logo.png"
            }, cancellationToken);

            // save changes
            await configurationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
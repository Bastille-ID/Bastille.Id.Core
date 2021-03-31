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
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using Bastille.Id.Core.Security;
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
    }
}
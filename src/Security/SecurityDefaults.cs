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

using Talegen.Common.Core.Extensions;

namespace Bastille.Id.Core.Security
{
    /// <summary>
    /// This class contains security default constants
    /// </summary>
    public static class SecurityDefaults
    {
        /// <summary>
        /// The identity API resource.
        /// </summary>
        public const string ApiResourceNameKey = "bastille-id-api";

        /// <summary>
        /// The scope policy.
        /// </summary>
        public const string ScopePolicy = "ResourceScopePolicy";

        /// <summary>
        /// The backchannel policy.
        /// </summary>
        public const string BackchannelPolicy = "BackchannelPolicy";

        /// <summary>
        /// The client claim backchannel.
        /// </summary>
        public const string ClientClaimBackchannel = "client_backchannel";

        /// <summary>
        /// The default organization slug.
        /// </summary>
        public const string DefaultOrganizationSlug = "talegen";

        /// <summary>
        /// The default organization name.
        /// </summary>
        public const string DefaultOrganizationName = "Talegen";

        /// <summary>
        /// The default organization domain.
        /// </summary>
        public const string DefaultOrganizationDomain = "talegen.com";

        /// <summary>
        /// Contains the support role name.
        /// </summary>
        public const string SupportRoleName = "Support";

        /// <summary>
        /// Contains the administrators role name.
        /// </summary>
        public const string AdministratorRoleName = "Administrators";

        /// <summary>
        /// The default user email.
        /// </summary>
        public const string DefaultUserEmail = "noreply@talegen.com";

        /// <summary>
        /// The default user password.
        /// </summary>
        public const string DefaultUserPassword = "ChangeMe123!";

        /// <summary>
        /// Contains the user default user time zone.
        /// </summary>
        public const string DefaultUserTimeZone = "Etc/UTC";

        /// <summary>
        /// Contains the user default user locale.
        /// </summary>
        public const string DefaultUserLocale = LocaleExtensions.DefaultLanguageCode;

        /// <summary>
        /// Contains the maximum amount of time security information will stay in memory.
        /// </summary>
        public const int DefaultExpirationMinutes = 10;

        /// <summary>
        /// The verify account template name.
        /// </summary>
        public const string VerifyAccountTemplateName = "verify_account";

        /// <summary>
        /// The reset password template name.
        /// </summary>
        public const string ResetPasswordTemplateName = "reset_password";

        /// <summary>
        /// The default user image name.
        /// </summary>
        public const string DefaultUserImageName = "/img/default_user_image.png";

        /// <summary>
        /// The is admin cache key template.
        /// </summary>
        public const string IsAdminCacheKeyTemplate = "{0}_IsAdminFlag";

        #region Public Default Claims

        /// <summary>
        /// The claim all abilities.
        /// </summary>
        public const string ClaimAllRights = "tenant:all";

        /// <summary>
        /// The tenant claim type.
        /// </summary>
        public const string TenantClaimType = "tenant";

        /// <summary>
        /// The claim audit read.
        /// </summary>
        public const string ClaimAuditRead = "tenant:audit:read";

        /// <summary>
        /// The claim identity read.
        /// </summary>
        public const string ClaimIdentityRead = "tenant:identity:read";

        /// <summary>
        /// The claim identity manage.
        /// </summary>
        public const string ClaimIdentityManage = "tenant:identity:manage";

        /// <summary>
        /// The claim product read.
        /// </summary>
        public const string ClaimProductRead = "tenant:product:read";

        /// <summary>
        /// The claim product manage.
        /// </summary>
        public const string ClaimProductManage = "tenant:product:manage";

        /// <summary>
        /// The claim subscription read.
        /// </summary>
        public const string ClaimSubscriptionRead = "tenant:subscription:read";

        /// <summary>
        /// The claim subscription manage.
        /// </summary>
        public const string ClaimSubscriptionManage = "tenant:subscription:manage";

        /// <summary>
        /// The claim security manage.
        /// </summary>
        public const string ClaimSecurityManage = "tenant:security:manage";

        /// <summary>
        /// The claim org read.
        /// </summary>
        public const string ClaimOrgRead = "tenant:org:read";

        /// <summary>
        /// The claim org manage.
        /// </summary>
        public const string ClaimOrgManage = "tenant:org:manage";

        /// <summary>
        /// The claim profile read.
        /// </summary>
        public const string ClaimProfileRead = "tenant:profile:read";

        /// <summary>
        /// The claim profile manage.
        /// </summary>
        public const string ClaimProfileManage = "tenant:profile:manage";

        #endregion
    }
}
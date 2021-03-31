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
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    using Vasont.AspnetCore.RedisClient;

    /// <summary>
    /// This class contains helper methods for working with tenant specific details.
    /// </summary>
    public static class TenantHelpers
    {
        /// <summary>
        /// Contains the tenant cache key template.
        /// </summary>
        public const string TenantCacheKeyTemplate = "Bastille:Tenants:{0}";

        /// <summary>
        /// Finds the tenant information by key in cache and alternatively in the database.
        /// </summary>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the tenant configuration if found.</returns>
        public async static Task<TenantConfig> FindTenantByKeyAsync(string tenantKey, IAdvancedDistributedCache cache, ApplicationDbContext dataContext, CancellationToken cancellationToken = default)
        {
            TenantConfig tenantConfig = null;
            string cacheKey = string.Format(TenantCacheKeyTemplate, tenantKey);

            // check cache first...
            tenantConfig = await cache.GetJsonAsync<TenantConfig>(cacheKey, cancellationToken);

            // if not found...
            if (tenantConfig == null)
            {
                // find tenant and organization information by domain key in the database
                tenantConfig = await dataContext.TenantConfigs
                    .Include(t => t.Organization)
                    .FirstOrDefaultAsync(ap => ap.TenantKey == tenantKey || ap.TenantId.ToString() == tenantKey, cancellationToken);

                if (tenantConfig != null)
                {
                    await cache.SetJsonAsync(cacheKey, tenantConfig, token: cancellationToken);
                }
            }

            return tenantConfig;
        }
    }
}
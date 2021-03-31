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

namespace Bastille.Id.Core.Data
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Core.Extensions;
    using Talegen.Common.Models.Shared.Queries;

    /// <summary>
    /// This class contains page browsing helper methods for paginated queries used throughout the library.
    /// </summary>
    public static class BrowseQueryHelper
    {
        /// <summary>
        /// Executes the paged user query.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="defaultSortColumnName">Default name of the sort column.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="PaginatedQueryResultModel{TModel}" /> object.</returns>
        public static async Task<PaginatedQueryResultModel<TModel>> ExecutePagedQueryAsync<TModel>(IQueryable<TModel> query, PaginatedQueryRequestModel filters, string defaultSortColumnName = "", CancellationToken cancellationToken = default)
            where TModel : class
        {
            query = BuildPagedQuery(filters, query, defaultSortColumnName, cancellationToken);

            PaginatedQueryResultModel<TModel> result = new PaginatedQueryResultModel<TModel>
            {
                Results = await query.ToListAsync(cancellationToken).ConfigureAwait(false)
            };

            // if the query returned less than the limit, and we're on the first page, we can use that count for the component results otherwise, we must run a
            // separate query to determine the total count
            result.TotalCount = filters.Page == 1 && result.Results.Count <= filters.Limit ? result.Results.Count : await query.CountAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// This method is used to build the query LINQ for reuse between retrieval methods in the service class.
        /// </summary>
        /// <param name="filters">Contains the event log filter parameters object.</param>
        /// <param name="baseQuery">Contains the base query used to paginate.</param>
        /// <param name="defaultColumnName">Contains the default column name to sort the paged query results by.</param>
        /// <param name="cancellationToken">Contains the cancellation token.</param>
        /// <returns>Returns an <see cref="IQueryable" /> query class built using optional parameters.</returns>
        private static IQueryable<TModel> BuildPagedQuery<TModel>(PaginatedQueryRequestModel filters, IQueryable<TModel> baseQuery, string defaultColumnName = "", CancellationToken cancellationToken = default)
            where TModel : class
        {
            if (filters.Limit <= 0)
            {
                // we cannot allow an unlimited amount of results, so defaulting to 25 if no or invalid value is passed in
                filters.Limit = 25;
            }

            if (filters.Page <= 0)
            {
                filters.Page = 1;
            }

            if (filters.Sort.Any())
            {
                for (int i = 0; i < filters.Sort.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(filters.Sort[i]))
                    {
                        baseQuery = baseQuery.OrderByName(filters.Sort[i], filters.Dir[i]);
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(defaultColumnName))
            {
                baseQuery = baseQuery.OrderByName(defaultColumnName);
            }

            if (filters.Limit > 0)
            {
                // set paging selection
                baseQuery = filters.Page > 1 ? baseQuery.Skip((filters.Page - 1) * filters.Limit).Take(filters.Limit) : baseQuery.Take(filters.Limit);
            }

            return baseQuery;
        }
    }
}
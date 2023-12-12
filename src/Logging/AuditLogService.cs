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

namespace Bastille.Id.Core.Logging
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Data;
    using Bastille.Id.Core.Data.Entities;
    using Bastille.Id.Models.Logging;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Core.Extensions;
    using Talegen.Common.Models.Server.Queries;

    /// <summary>
    /// This class provides security logging methods to the application.
    /// </summary>
    public class AuditLogService
    {
        /// <summary>
        /// Contains an instance of the <see cref="ApplicationDbContext" /> class.
        /// </summary>
        private readonly ApplicationDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogService" /> class.
        /// </summary>
        /// <param name="context">Contains an instance of the <see cref="ApplicationDbContext" /> class.</param>
        public AuditLogService(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Reads the audit logs asynchronous.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="PaginatedQueryResultModel{AuditLog}" /> result.</returns>
        /// <exception cref="ArgumentNullException">The exception is thrown if the <paramref name="filters" /> parameter is not set.</exception>
        public async Task<PaginatedQueryResultModel<AuditLog>> ReadAuditLogsAsync(AuditLogQueryFilterModel filters, CancellationToken cancellationToken)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            IQueryable<AuditLog> query = this.BuildPagedResultsQuery(filters, cancellationToken);
            return await this.ExecutePagedQuery(query, filters, cancellationToken);
        }

        /// <summary>
        /// This method is used to log a security event within the application data store.
        /// </summary>
        /// <param name="securityEvent">Contains the security event.</param>
        /// <param name="securityResult">Contains the security event result.</param>
        /// <param name="clientAddress">Contains the client IP address.</param>
        /// <param name="message">Contains an optional message.</param>
        /// <param name="userId">Contains an optional user identity that is related to the event.</param>
        /// <param name="location">Contains an optional user location.</param>
        /// <param name="cancellationToken">Contains the cancellation token.</param>
        /// <returns>Returns a value indicating the record was stored.</returns>
        public async Task<bool> LogAsync(AuditEvent securityEvent, AuditResult securityResult, string clientAddress, string message = null, Guid? userId = null, string location = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.context.AuditLogs
                .AddAsync(new AuditLog
                {
                    Event = securityEvent,
                    Result = securityResult,
                    ClientAddress = clientAddress,
                    Message = message,
                    UserId = userId,
                    Location = location
                }, cancellationToken)
                .ConfigureAwait(false);

            return await this.context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
        }

        /// <summary>
        /// This method is used to build the query LINQ for reuse between retrieval methods in the service class.
        /// </summary>
        /// <param name="filters">Contains the event log filter parameters object.</param>
        /// <param name="cancellationToken">Contains the cancellation token.</param>
        /// <returns>Returns an <see cref="IQueryable" /> query class built using optional parameters.</returns>
        private IQueryable<AuditLog> BuildPagedResultsQuery(AuditLogQueryFilterModel filters, CancellationToken cancellationToken)
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

            IQueryable<AuditLog> query = this.context.AuditLogs
                .Include(u => u.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.SearchText))
            {
                query = query.Where(e => (e.User != null && e.User.UserName.Contains(filters.SearchText) || e.User.Email.Contains(filters.SearchText)) || e.Message.Contains(filters.SearchText) || e.Request.Contains(filters.SearchText) || e.ClientAddress.Equals(filters.SearchText));
            }

            filters.Events.ForEach(ev =>
            {
                query = query.Where(q => q.Event == ev);
            });

            filters.Results.ForEach(re =>
            {
                query = query.Where(r => r.Result == re);
            });

            if (filters.Sort.Any())
            {
                for (int i = 0; i < filters.Sort.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(filters.Sort[i]))
                    {
                        query = query.OrderByName(filters.Sort[i], filters.Direction[i]);
                    }
                }
            }
            else
            {
                query = query.OrderByDescending(u => u.EventDateTime);
            }

            if (filters.Limit > 0)
            {
                // set paging selection
                query = filters.Page > 1 ? query.Skip((filters.Page - 1) * filters.Limit).Take(filters.Limit) : query.Take(filters.Limit);
            }

            return query;
        }

        /// <summary>
        /// This method is used to execute the passed query LINQ and return the results as an <see cref="AuditLog" /> list.
        /// </summary>
        /// <param name="query">Contains the LINQ statement to execute.</param>
        /// <param name="filters">Contains the filters used for the query</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the results of the query in an <see cref="PaginatedQueryResultModel{AuditLog}" /> model.</returns>
        private async Task<PaginatedQueryResultModel<AuditLog>> ExecutePagedQuery(IQueryable<AuditLog> query, AuditLogQueryFilterModel filters, CancellationToken cancellationToken)
        {
            PaginatedQueryResultModel<AuditLog> result = new PaginatedQueryResultModel<AuditLog>
            {
                Results = await query.ToListAsync(cancellationToken).ConfigureAwait(false)
            };

            // if the query returned less than the limit, and we're on the first page, we can use that count for the component results otherwise, we must run a
            // separate query to determine the total count
            result.TotalCount = filters.Page == 1 && result.Results.Count <= filters.Limit ? result.Results.Count : query.Count();

            return result;
        }
    }
}
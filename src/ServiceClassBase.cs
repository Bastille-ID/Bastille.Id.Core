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

namespace Bastille.Id.Core
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using Bastille.Id.Core.Logging;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Talegen.Common.Core.Errors;

    /// <summary>
    /// This class implements the base properties and methods for a data service class.
    /// </summary>
    public abstract class ServiceClassBase<TDataContext> where TDataContext : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClassBase{TDataContext}" /> class.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="errorManager">Contains the error manager.</param>
        /// <param name="auditLogService">Contains an optional audit logging service.</param>
        public ServiceClassBase(TDataContext context, IErrorManager errorManager, AuditLogService auditLogService = null)
        {
            this.DataContext = context;
            this.ErrorManager = errorManager;
            this.AuditLog = auditLogService;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public TDataContext DataContext { get; }

        /// <summary>
        /// Gets the error manager.
        /// </summary>
        /// <value>The error manager.</value>
        protected IErrorManager ErrorManager { get; }

        /// <summary>
        /// Gets the audit log.
        /// </summary>
        /// <value>The audit log.</value>
        protected AuditLogService AuditLog { get; }

        /// <summary>
        /// This method is used to execute the data context save changes and catch all validation errors.
        /// </summary>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the number of rows updated on success.</returns>
        protected virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int resultValue = 0;

            try
            {
                resultValue = await (this.DataContext as DbContext).SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException connEx)
            {
                this.ErrorManager.Critical(connEx, ErrorCategory.Application);
            }
            catch (DbUpdateException ex)
            {
                var sqlEx = ex?.InnerException as SqlException;

                this.ErrorManager.Critical(sqlEx?.InnerException ?? ex, ErrorCategory.Application);
            }
            catch (InvalidOperationException invalidEx)
            {
                this.ErrorManager.Critical(invalidEx, ErrorCategory.Application);
            }
            catch (ValidationException validateEx)
            {
                this.ErrorManager.Critical(validateEx, ErrorCategory.Application);
            }
            catch (Exception otherEx)
            {
                this.ErrorManager.Critical(otherEx, ErrorCategory.Application);
            }

            return resultValue;
        }
    }
}
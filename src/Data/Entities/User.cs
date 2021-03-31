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

namespace Bastille.Id.Core.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// This entity class represents a user within the application identity data store.
    /// </summary>
    public class User : IdentityUser<Guid>
    {
        /// <summary>
        /// Gets or sets a date time value when the user last logged-in.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets a list of organization user records.
        /// </summary>
        public List<GroupUser> Groups { get; set; } = new List<GroupUser>();

        /// <summary>
        /// Gets or sets a date time value when the entity was created.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a date time value when the entity was created.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [passwordless enabled].
        /// </summary>
        /// <value><c>true</c> if [passwordless enabled]; otherwise, <c>false</c>.</value>
        public bool PasswordlessEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="User" /> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        /// <value>The claims.</value>
        public virtual List<IdentityUserClaim<Guid>> Claims { get; set; }
    }
}
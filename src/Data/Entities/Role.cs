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
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;
    using Talegen.Common.Models.Security;

    /// <summary>
    /// This class represents a role within the security system.
    /// </summary>
    public class Role : IdentityRole<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role" /> class.
        /// </summary>
        public Role()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the type of the role.
        /// </summary>
        /// <value>The type of the role.</value>
        [Column(TypeName = "varchar(20)")]
        public RoleTypes RoleType { get; set; } = RoleTypes.Defined;

        /// <summary>
        /// Gets or sets a date time value when the entity was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the created by user identifier.
        /// </summary>
        /// <value>The created by user identifier.</value>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets a date time value when the entity was last updated.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the updated by user identifier.
        /// </summary>
        /// <value>The updated by user identifier.</value>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the user update by.
        /// </summary>
        /// <value>The updated by user.</value>
        [ForeignKey(nameof(UpdatedByUserId))]
        public User UpdatedBy { get; set; }
    }
}
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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// This entity class represents an group tenant within the identity server data store.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Gets or sets the unique identity of the group.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>The organization identifier.</value>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets an optional unique identity of the group's parent group.
        /// </summary>
        public Guid? ParentGroupId { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the owner user identifier.
        /// </summary>
        /// <value>The owner user identifier.</value>
        public Guid? OwnerUserId { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public User Owner { get; set; }

        /// <summary>
        /// Gets or sets the group's associated users.
        /// </summary>
        public List<GroupUser> Members { get; set; } = new List<GroupUser>();

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>The organization.</value>
        public Organization Organization { get; set; }

        /// <summary>
        /// Gets or sets the optional parent group.
        /// </summary>
        [ForeignKey("ParentGroupId")]
        public Group ParentGroup { get; set; }

        /// <summary>
        /// Gets or sets a date time value when the entity was created.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
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
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
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

        /// <summary>
        /// Gets or sets a value indicating whether the group is an active user.
        /// </summary>
        public bool Active { get; set; } = true;
    }
}
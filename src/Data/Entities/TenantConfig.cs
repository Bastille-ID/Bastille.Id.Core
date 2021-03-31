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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// This class represents a custom tenant configuration. If found, it will override the default configurations for the identity server. This allows
    /// customized login pages, styles, and requirements for a custom tenant.
    /// </summary>
    public class TenantConfig
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        [Key]
        public Guid TenantId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the tenant key.
        /// </summary>
        /// <value>The tenant key.</value>
        [Required]
        [MaxLength(100)]
        public string TenantKey { get; set; }

        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>The organization identifier.</value>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the logo URL.
        /// </summary>
        /// <value>The logo URL.</value>
        [MaxLength(250)]
        public string LogoUrl { get; set; }

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

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>The organization.</value>
        public Organization Organization { get; set; }
    }
}
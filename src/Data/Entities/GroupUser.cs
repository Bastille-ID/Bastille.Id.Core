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
    /// This entity class represents an group user association within the identity data store.
    /// </summary>
    public class GroupUser
    {
        /// <summary>
        /// Gets or sets the unique identity of the associated group.
        /// </summary>
        [Required]
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets the unique identity of the associated user.
        /// </summary>
        [Required]
        [MaxLength(450)]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the associated group.
        /// </summary>
        public Group Group { get; set; }

        /// <summary>
        /// Gets or sets the associated user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
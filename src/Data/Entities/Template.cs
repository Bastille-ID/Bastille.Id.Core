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
    using Newtonsoft.Json;
    using Talegen.Common.Messaging.Templates;

    /// <summary>
    /// This class represents a content template within the application.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>The template identifier.</value>
        [Key]
        public Guid TemplateId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets an optional organization identifier to make this template organization specific.
        /// </summary>
        /// <value>The organization identifier.</value>
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets a unique identity for the stored content template.
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string TemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the template content type.
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public TemplateType TemplateType { get; set; } = TemplateType.Message;

        /// <summary>
        /// Gets or sets the related language code for the template.
        /// </summary>
        [MaxLength(5)]
        [ForeignKey(nameof(Language))]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the related language navigation property.
        /// </summary>
        public Language Language { get; set; }

        /// <summary>
        /// Gets or sets the image binary data.
        /// </summary>
        public string Content { get; set; }

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
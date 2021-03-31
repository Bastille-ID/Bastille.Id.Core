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
    using Bastille.Id.Models.Notifications;

    /// <summary>
    /// This class represents a notification message withing the application.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Gets or sets the Notification identity.
        /// </summary>
        [Key]
        public Guid NotificationId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>The organization identifier.</value>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the user identity.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the notification date.
        /// </summary>
        [Required]
        public DateTime NotificationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the notification alert value.
        /// </summary>
        [StringLength(10)]
        [Required]
        [Column(TypeName = "varchar(10)")]
        public NotificationMessageAlertStatus Alert { get; set; } = NotificationMessageAlertStatus.None;

        /// <summary>
        /// Gets or sets the notification state.
        /// </summary>
        [StringLength(20)]
        [Required]
        [Column(TypeName = "varchar(20)")]
        public NotificationState State { get; set; } = NotificationState.Unread;

        /// <summary>
        /// Gets or sets the target type.
        /// </summary>
        [StringLength(20)]
        [Required]
        [Column(TypeName = "varchar(20)")]
        public NotificationTarget Target { get; set; } = NotificationTarget.User;

        /// <summary>
        /// Gets or sets the type of notification.
        /// </summary>
        [StringLength(20)]
        [Required]
        [Column(TypeName = "varchar(20)")]
        public NotificationType Type { get; set; } = NotificationType.Message;

        /// <summary>
        /// Gets or sets the model data for the notification message.
        /// </summary>
        [StringLength(500)]
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the model data for the notification message.
        /// </summary>
        [StringLength(1000)]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the model data for the notification message.
        /// </summary>
        public string WebMessage { get; set; }

        /// <summary>
        /// Gets or sets additional optional metadata for the message.
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
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
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// This class represents a time zone in the application data store.
    /// </summary>
    public class TimeZone
    {
        /// <summary>
        /// Gets or sets the unique identity of the time zone.
        /// </summary>
        [Key]
        [MaxLength(100)]
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the short name of the time zone.
        /// </summary>
        [MaxLength(200)]
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the long name of the time zone.
        /// </summary>
        [MaxLength(300)]
        public string LongName { get; set; }

        /// <summary>
        /// Gets or sets the numeric time offset.
        /// </summary>
        [Required]
        public double Offset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if this is the default time zone.
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the time zone is active.
        /// </summary>
        public bool Active { get; set; }
    }
}
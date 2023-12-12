/*
 *
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

namespace Bastille.Id.Core.Configuration
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains an enumerated list of required authentication identifier types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LoginIdentifierMethod
    {
        /// <summary>
        /// User must specify their user name.
        /// </summary>
        UserName,

        /// <summary>
        /// User must specify their e-mail address.
        /// </summary>
        Email,

        /// <summary>
        /// User can specify their username, or email address
        /// </summary>
        UserNameOrEmail,

        /// <summary>
        /// User can specify their username, email address, or phone number
        /// </summary>
        UserNameOrEmailOrPhone
    }

    /// <summary>
    /// This class contains profile related settings.
    /// </summary>
    public class ProfileSettings
    {
        /// <summary>
        /// Gets or sets the identifier method.
        /// </summary>
        /// <value>The identifier method.</value>
        public LoginIdentifierMethod IdentifierMethod { get; set; } = LoginIdentifierMethod.Email;

        /// <summary>
        /// Gets or sets a value indicating whether to allow the call to modify the user name on a profile.
        /// </summary>
        /// <value><c>true</c> if allowing the call to modify the user name on a profile; otherwise, <c>false</c>.</value>
        public bool AllowUserNameChange { get; set; }

        /// <summary>
        /// Gets or sets the profile picture URI base.
        /// </summary>
        /// <value>The profile picture URI base.</value>
        public string ProfilePictureUriBase { get; set; }
    }
}
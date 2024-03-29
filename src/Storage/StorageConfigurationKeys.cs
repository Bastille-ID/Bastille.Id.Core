﻿/*
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

namespace Bastille.Id.Core.Storage
{
    /// <summary>
    /// This class contains storage configuration key names.
    /// </summary>
    public static class StorageConfigurationKeys
    {
        /// <summary>
        /// The azure account name
        /// </summary>
        public const string AzureAccountName = "AzureAccountName";

        /// <summary>
        /// The azure account key
        /// </summary>
        public const string AzureAccountKey = "AzureAccountKey";

        /// <summary>
        /// The AWS service URL.
        /// </summary>
        public const string AwsServiceUrl = "AwsServiceUrl";

        /// <summary>
        /// The AWS bucket name.
        /// </summary>
        public const string AwsBucketName = "AwsBucketName";
    }
}
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

namespace Bastille.Id.Core.Storage
{
    using System;
    using Bastille.Id.Core.Configuration;
    using Bastille.Id.Core.Properties;
    using Talegen.Storage.Net.AzureBlobs;
    using Talegen.Storage.Net.Core;
    using Talegen.Storage.Net.Core.Disk;
    using Talegen.Storage.Net.Core.Memory;

    /// <summary>
    /// This class contains methods for creating a storage service.
    /// </summary>
    public static class StorageFactory
    {
        /// <summary>
        /// Creates the specified storage settings.
        /// </summary>
        /// <param name="storageSettings">The storage settings.</param>
        /// <returns>Returns a new <see cref="IStorageService" /> instance.</returns>
        public static IStorageService Create(StorageSettings storageSettings)
        {
            IStorageService service;

            switch (storageSettings.StorageType)
            {
                case StorageType.Local:
                    service = new LocalStorageService(storageSettings.RootPathUri);
                    break;

                case StorageType.Memory:
                    service = new MemoryStorageService(storageSettings.RootPathUri);
                    break;

                case StorageType.Azure:
                    string accountName = storageSettings.Configuration.ContainsKey(StorageConfigurationKeys.AzureAccountName) ? storageSettings.Configuration[StorageConfigurationKeys.AzureAccountName] : throw new ArgumentException(string.Format(Resources.StorageConfigurationNotSpecifiedErrorText, StorageConfigurationKeys.AzureAccountName));
                    string accountKey = storageSettings.Configuration.ContainsKey(StorageConfigurationKeys.AzureAccountKey) ? storageSettings.Configuration[StorageConfigurationKeys.AzureAccountKey] : throw new ArgumentException(string.Format(Resources.StorageConfigurationNotSpecifiedErrorText, StorageConfigurationKeys.AzureAccountKey));

                    service = new AzureBlobStorageService(new Uri(storageSettings.RootPathUri), accountName, accountKey);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return service;
        }
    }
}
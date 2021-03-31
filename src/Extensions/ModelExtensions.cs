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

namespace Bastille.Id.Core.Extensions
{
    using Bastille.Id.Models.Clients;
    using IdentityServer4.EntityFramework.Mappers;

    /// <summary>
    /// This class contains extensions related to converting objects between model definitions.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Converts to clientmodel.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static ClientModel ToClientModel(this IdentityServer4.EntityFramework.Entities.Client entity)
        {
            var model = entity.ToModel() as ClientModel;
            model.Id = entity.Id;
            return model;
        }
    }
}
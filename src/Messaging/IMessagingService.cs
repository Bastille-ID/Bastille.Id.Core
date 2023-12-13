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
namespace Bastille.Id.Core.Messaging
{
    /// <summary>
    /// This interface is used to interact with the messaging service.
    /// </summary>
    public interface IMessagingService
    {
        /// <summary>
        /// This method is used to publish a message to a specified channel.
        /// </summary>
        /// <param name="channel">Contains the channel name to send the message to.</param>
        /// <param name="message">Contains the message to send to the channel.</param>
        void Publish(string channel, string message);

        /// <summary>
        /// This method is used to publish a model to a specified channel.
        /// </summary>
        /// <typeparam name="T">Contains the model object type.</typeparam>
        /// <param name="channel">Contains the channel name to send the model to.</param>
        /// <param name="model">Contains the model to serialize and send to the channel.</param>
        void Publish<T>(string channel, T model);
    }
}

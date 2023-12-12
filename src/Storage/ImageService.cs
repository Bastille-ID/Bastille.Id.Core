namespace Bastille.Id.Core.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Bastille.Id.Core.Configuration;
    using Talegen.AspNetCore.Web.Extensions;
    using Talegen.Storage.Net.Core;

    /// <summary>
    /// This class contains methods for interacting with stored images.
    /// </summary>
    public class ImageService
    {
        /// <summary>
        /// The storage service.
        /// </summary>
        private readonly IStorageService storageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService" /> class.
        /// </summary>
        /// <param name="storageSettings">The storage settings.</param>
        public ImageService(StorageSettings storageSettings)
        {
            this.storageService = StorageFactory.Create(storageSettings);
        }

        /// <summary>
        /// Reads the image.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="scaleSize">Size of the scale.</param>
        /// <param name="defaultImageNamePath">The default image name path.</param>
        /// <returns>Returns the image bytes</returns>
        /// <exception cref="Exception"></exception>
        public byte[] ReadImage(Guid userId, int scaleSize = 0, string defaultImageNamePath = "default.jpg")
        {
            string imagesWorkspacePath = Path.Combine("Images", userId + ".jpg");
            string defaultImagePath = Path.Combine("Images", defaultImageNamePath);
            byte[] imageBytes;

            if (this.storageService.FileExists(imagesWorkspacePath))
            {
                imageBytes = this.storageService.ReadBinaryFile(imagesWorkspacePath);
            }
            else if (this.storageService.FileExists(defaultImagePath))
            {
                imageBytes = this.storageService.ReadBinaryFile(defaultImagePath);
            }
            else
            {
                throw new Exception();
            }

            return scaleSize > 0 && scaleSize <= 128 ? imageBytes.CreateThumbnail(scaleSize, scaleSize) : imageBytes;
        }
    }
}
using System;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Localization;
using Volo.Abp;
using Volo.Abp.Content;

namespace Unseal.Extensions;

public static class FileExtension
{
    extension(IServiceProvider serviceProvider)
    {
        public async Task<string> UploadFileAsync(IRemoteStreamContent streamContent)
        {
            var stringLocalizer = serviceProvider.GetRequiredService<IStringLocalizer<UnsealResource>>();
            try
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var cloudinaryOptions = configuration.GetSection(nameof(Cloudinary)).Get<Models.Options.Cloudinary>();
                var account = new Account(
                    cloudinaryOptions.CloudName,
                    cloudinaryOptions.ApiKey,
                    cloudinaryOptions.ApiSecret
                );
                var cloudinary = new Cloudinary(account);
                using var stream = streamContent.GetStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(streamContent.FileName, stream),
                    Folder = cloudinaryOptions.Folder,
                    Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error is null)
                {
                    var fileUrl = uploadResult.SecureUrl.ToString();
                    return fileUrl;
                }

                throw new UserFriendlyException(stringLocalizer[ExceptionCodes.File.UploadError]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new UserFriendlyException(stringLocalizer[ExceptionCodes.File.UploadError]);
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var cloudinaryOptions = configuration.GetSection(nameof(Cloudinary)).Get<Models.Options.Cloudinary>();
            var account = new Account(
                cloudinaryOptions.CloudName,
                cloudinaryOptions.ApiKey,
                cloudinaryOptions.ApiSecret
            );
            var cloudinary = new Cloudinary(account);
            var publicId = ExtractPublicIdFromUrl(fileUrl);

            if (string.IsNullOrEmpty(publicId)) return ;

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Auto
            };

            var deletionResult = await cloudinary.DestroyAsync(deletionParams);
        }
    }

    private static string ExtractPublicIdFromUrl(string url)
    {
        try
        {
            var splitUrl = url.Split("/upload/");
            if (splitUrl.Length < 2) return null;

            var pathAfterUpload = splitUrl[1];
            var firstSlashIndex = pathAfterUpload.IndexOf('/');
            var publicIdWithExtension = pathAfterUpload.Substring(firstSlashIndex + 1);

            var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
            return lastDotIndex > 0 
                ? publicIdWithExtension.Substring(0, lastDotIndex) 
                : publicIdWithExtension;
        }
        catch
        {
            return null;
        }
    }
}
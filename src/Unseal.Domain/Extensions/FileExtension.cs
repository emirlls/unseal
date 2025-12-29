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
    public static async Task<string> UploadFileAsync(this IServiceProvider serviceProvider,
        IRemoteStreamContent streamContent)
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
}
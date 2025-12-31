using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using QRCoder;

namespace Unseal.Extensions;

public static class QrCodeExtension
{
    extension(IServiceProvider serviceProvider)
    {
        public async Task<string> GenerateQrCodeAsync(Guid id)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator
                .CreateQrCode(id.ToString(), QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var bitmap = qrCode.GetGraphic(20);

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            var  bytes = ms.ToArray();
            var base64Image = Convert.ToBase64String(bytes);
            return base64Image;
        }
    }
}
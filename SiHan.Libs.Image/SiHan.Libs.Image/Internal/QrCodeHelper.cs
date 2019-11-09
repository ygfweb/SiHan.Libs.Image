using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using ZXing.SkiaSharp;

namespace SiHan.Libs.Image.Internal
{
    internal static class QrCodeHelper
    {
        public static string Read(Stream imageFile)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }
            using (var stream = new SKManagedStream(imageFile))
            {
                using (SKBitmap bitmap = SKBitmap.Decode(stream))
                {
                    if (bitmap == null)
                    {
                        return "";
                    }
                    else
                    {
                        ZXing.SkiaSharp.BarcodeReader reader = new ZXing.SkiaSharp.BarcodeReader();
                        Result result = reader.Decode(bitmap);
                        if (result != null)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="contents">待生成二维码的文字</param>
        /// <param name="logoImage">logo图片</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="disableMargin">是否禁止生成边界</param>
        /// <returns></returns>
        public static byte[] GeneratorQrImage(string contents, Stream logoImage = null, int width = 360, int height = 360, bool disableMargin = true)
        {
            if (string.IsNullOrEmpty(contents))
            {
                return null;
            }
            //本文地址：http://www.cnblogs.com/Interkey/p/qrcode.html
            EncodingOptions options = null;
            BarcodeWriter writer = null;
            options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = width,
                Height = height,
                ErrorCorrection = ErrorCorrectionLevel.H,
            };
            if (disableMargin)
            {
                options.Margin = 0;
            }
            writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = options;
            // 获取二维码图片
            using (SKBitmap qrBitmap = writer.Write(contents))
            {
                if (qrBitmap == null)
                {
                    throw new ImageException("生成二维码失败");
                }
                using (SKSurface surface = SKSurface.Create(new SKImageInfo(qrBitmap.Width, qrBitmap.Height)))
                {
                    SKCanvas canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);
                    // 绘制二维码
                    canvas.DrawBitmap(qrBitmap, 0, 0);
                    if (logoImage != null && logoImage.Length > 0)
                    {
                        using (SKManagedStream managedStream = new SKManagedStream(logoImage))
                        {
                            using (SKBitmap logoBitmap = SKBitmap.Decode(managedStream))
                            {
                                if (logoBitmap != null)
                                {
                                    int deltaHeight = qrBitmap.Height - logoBitmap.Height;
                                    int deltaWidth = qrBitmap.Width - logoBitmap.Width;
                                    canvas.DrawBitmap(logoBitmap, deltaWidth / 2, deltaHeight / 2);
                                }
                            }
                        }
                    }
                    using (SKImage image = surface.Snapshot())
                    {
                        using (SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                        {
                            return data.ToArray();
                        }
                    }
                }
            }
        }
    }
}

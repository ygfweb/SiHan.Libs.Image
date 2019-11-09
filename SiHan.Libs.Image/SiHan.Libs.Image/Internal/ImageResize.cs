using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SiHan.Libs.Image.Internal
{
    internal class ImageResize
    {
        /// <summary>
        /// 重新调整图片的尺寸
        /// </summary>
        /// <param name="imageStream">源图片流</param>
        /// <param name="maxSize">最大尺寸</param>
        /// <param name="isSameRate">是否保存相同比率</param>
        /// <returns></returns>
        public static byte[] Resize(Stream imageStream, ImageSize maxSize, bool isSameRate = false)
        {
            using (SKManagedStream managedStream = new SKManagedStream(imageStream)) //包装流
            {
                using (SKBitmap bitmap = SKBitmap.Decode(managedStream)) // 将流加载到图片中
                {
                    if (bitmap == null)
                    {
                        throw new ImageException("传递的参数不是有效图片");
                    }

                    ImageSize sourceSize = new ImageSize(bitmap.Width, bitmap.Height);
                    ImageSize newSize = maxSize;
                    if (isSameRate)
                    {
                        newSize = ImageSize.GetSameRateSize(sourceSize, maxSize);
                    }

                    using (SKBitmap newBitmap = bitmap.Resize(new SKImageInfo(newSize.Width, newSize.Height), SKFilterQuality.High))
                    {
                        if (newBitmap == null)
                        {
                            throw new ImageException("调整图片尺寸失败");
                        }

                        using (SKImage image = SKImage.FromBitmap(newBitmap))
                        {
                            using (SKData p = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                            {
                                return p.ToArray();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 裁剪图片
        /// </summary>
        public static byte[] Crop(Stream imageStream, ImageSize cropSize, int quality = 100)
        {
            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            if (cropSize == null)
            {
                throw new ArgumentNullException(nameof(cropSize));
            }

            using (SKManagedStream managedStream = new SKManagedStream(imageStream))
            {
                using (SKBitmap bitmap = SKBitmap.Decode(managedStream))
                {
                    if (bitmap == null)
                    {
                        throw new ImageException("传递的参数不是有效图片");
                    }

                    ImageSize sourceSize = new ImageSize(bitmap.Width, bitmap.Height);
                    if (sourceSize.Height < cropSize.Height || sourceSize.Width < cropSize.Width)
                    {
                        throw new ImageException("源图片的尺寸不能小于裁剪尺寸");
                    }

                    using (SKSurface surface = SKSurface.Create(new SKImageInfo(cropSize.Width, cropSize.Height)))
                    {
                        SKCanvas canvas = surface.Canvas;
                        canvas.Clear(SKColors.White);
                        int deltaHeight = cropSize.Height - sourceSize.Height;
                        int deltaWidth = cropSize.Width - sourceSize.Width;
                        canvas.DrawBitmap(bitmap, deltaWidth / 2, deltaHeight / 2);
                        using (SKImage image = surface.Snapshot())
                        {
                            using (SKData data = image.Encode(SKEncodedImageFormat.Jpeg, quality))
                            {
                                return data.ToArray();
                            }
                        }
                    }
                }
            }
        }
    }
}

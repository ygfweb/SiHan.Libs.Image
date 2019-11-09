using SiHan.Libs.Image.Internal;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiHan.Libs.Image
{
    /// <summary>
    /// 图片文件包装器
    /// </summary>
    public class ImageWrapper
    {
        private byte[] ImageBytes { get; set; }
        /// <summary>
        /// 使用图片内容创建对象实例
        /// </summary>
        /// <param name="imageBytes"></param>
        public ImageWrapper(byte[] imageBytes)
        {
            if (imageBytes == null)
            {
                throw new ArgumentNullException(nameof(imageBytes));
            }

            ImageBytes = imageBytes;
        }

        /// <summary>
        /// 通过图片文件创建对象实例
        /// </summary>
        public static ImageWrapper CreateFromFile(FileInfo imageFile)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

            if (!imageFile.Exists)
            {
                throw new FileNotFoundException("指定的图片文件不存在", imageFile.FullName);
            }

            byte[] bytes = File.ReadAllBytes(imageFile.FullName);
            return new ImageWrapper(bytes);
        }

        /// <summary>
        /// 通过验证码创建对象实例
        /// </summary>
        public static ImageWrapper CreateByVerifyCode(string code, int height = 38, int width = 120)
        {
            ImageVerifyCode verifyCode = new ImageVerifyCode(code, height, width);
            byte[] bytes = verifyCode.BuildImage();
            return new ImageWrapper(bytes);
        }

        /// <summary>
        /// 创建内存流
        /// </summary>
        public MemoryStream CreateMemoryStream()
        {
            return new MemoryStream(this.ImageBytes.ToArray());
        }

        /// <summary>
        /// 将图片保存为文件
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveToFile(string filePath)
        {
            File.WriteAllBytes(filePath, this.ImageBytes);
        }

        /// <summary>
        /// 调整图片的尺寸
        /// </summary>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="isSameRate">是否保持比率</param>
        /// <returns></returns>
        public ImageWrapper Resize(int maxWidth, int maxHeight, bool isSameRate = false)
        {
            using (var stream = new MemoryStream(this.ImageBytes.ToArray()))
            {
                ImageSize imageSize = new ImageSize(maxWidth, maxHeight);
                byte[] newBytes = ImageResize.Resize(stream, imageSize, isSameRate);
                return new ImageWrapper(newBytes);
            }
        }

        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="quality">图片质量</param>
        /// <returns></returns>
        public ImageWrapper Crop(int width, int height, int quality = 100)
        {
            using (var stream = new MemoryStream(this.ImageBytes.ToArray()))
            {
                ImageSize imageSize = new ImageSize(width, height);
                byte[] bytes = ImageResize.Crop(stream, imageSize, quality);
                return new ImageWrapper(bytes);
            }
        }

        /// <summary>
        /// 是否是真实图片
        /// </summary>
        public bool IsReallyImage()
        {
            using (var memoryStream = new MemoryStream(this.ImageBytes.ToArray()))
            {
                using (var stream = new SKManagedStream(memoryStream)) //对流进行包装
                {
                    using (SKBitmap bitmap = SKBitmap.Decode(stream))
                    {
                        if (bitmap != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 识别二维码
        /// </summary>
        public string QRCodeDecoder()
        {
            using (var memoryStream = new MemoryStream(this.ImageBytes.ToArray()))
            {
                return QrCodeHelper.Read(memoryStream);
            }
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="text">文字</param>
        /// <param name="logoImage">logo图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="disableMargin">禁止白边（默认为true）</param>
        /// <returns></returns>
        public static ImageWrapper CreateFormQRCodeEncoder(string text, ImageWrapper logoImage = null, int width = 360, int height = 360, bool disableMargin = true)
        {
            if (logoImage == null)
            {
                byte[] bytes = QrCodeHelper.GeneratorQrImage(text, null, width, height, disableMargin);
                return new ImageWrapper(bytes);
            }
            else
            {
                using (MemoryStream stream = logoImage.CreateMemoryStream())
                {
                    byte[] bytes = QrCodeHelper.GeneratorQrImage(text, stream, width, height, disableMargin);
                    return new ImageWrapper(bytes);
                }
            }
        }
    }
}

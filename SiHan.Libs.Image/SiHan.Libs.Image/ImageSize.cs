using System;
using System.Collections.Generic;
using System.Text;

namespace SiHan.Libs.Image
{
    /// <summary>
    /// 图片尺寸
    /// </summary>
    public class ImageSize
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }

        public ImageSize(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ImageException("设置的图片尺寸不是正确尺寸");
            }
            Width = width;
            Height = height;
        }


        /// <summary>
        /// 获取相同比率的图片尺寸
        /// </summary>
        /// <param name="sourceSize">原图片尺寸</param>
        /// <param name="maxSize">重设最大的图片尺寸</param>
        /// <returns>返回新的相同比率尺寸</returns>
        public static ImageSize GetSameRateSize(ImageSize sourceSize, ImageSize maxSize)
        {
            int newWidth = sourceSize.Width;
            int newHeight = sourceSize.Height;
            if (sourceSize.Width > maxSize.Width)
            {
                newWidth = maxSize.Width;
                newHeight = (newWidth * sourceSize.Height) / sourceSize.Width;
            }
            if (newHeight > maxSize.Height)
            {
                newHeight = maxSize.Height;
                newWidth = (newHeight * sourceSize.Width) / sourceSize.Height;
            }
            return new ImageSize(newWidth, newHeight);
        }
    }
}

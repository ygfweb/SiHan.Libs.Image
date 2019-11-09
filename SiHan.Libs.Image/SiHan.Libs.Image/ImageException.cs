using System;
using System.Collections.Generic;
using System.Text;

namespace SiHan.Libs.Image
{
    /// <summary>
    /// 图片处理异常
    /// </summary>
    public class ImageException: Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public ImageException(string msg) : base(msg)
        {

        }
    }
}

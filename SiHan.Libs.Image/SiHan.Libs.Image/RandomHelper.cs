using System;
using System.Collections.Generic;
using System.Text;

namespace SiHan.Libs.Image
{
    internal class RandomHelper
    {
        /// <summary>
        /// 获得随机整数
        /// </summary>
        public static int GetIntNumber(int min, int max)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            return r.Next(min, max);
        }
    }
}

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SiHan.Libs.Image.Internal
{
    /// <summary>
    /// 验证码图片
    /// </summary>
    internal class ImageVerifyCode
    {
        /// <summary>
        /// 高度
        /// </summary>
        private int Height { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        private int Width { get; set; }
        /// <summary>
        /// 验证码文字
        /// </summary>
        private string Code { get; set; }

        private byte[] Font { get; set; }

        public ImageVerifyCode(string code, int height = 38, int width = 120)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            this.Font = SiHan.Libs.Image.Properties.Resources.aispec;
            Height = height;
            Width = width;
            this.Code = code;
        }

        /// <summary>
        /// 生成验证码图片
        /// </summary>
        public byte[] BuildImage()
        {
            SKImageInfo imageInfo = new SKImageInfo(this.Width, this.Height);
            // 创建绘图缓冲区
            using (SKSurface surface = SKSurface.Create(imageInfo))
            {
                SKCanvas canvas = surface.Canvas; // 获取画板
                canvas.Clear(SKColors.White); // 将背景设置为白色
                this.CreateBackground(canvas); // 绘制渐变背景
                this.CreateText(canvas); // 绘制文本
                this.CreateCurve(canvas); // 绘制干扰线             
                using (SKImage image = surface.Snapshot())
                {
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        return data.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// 绘制渐变背景
        /// </summary>
        private void CreateBackground(SKCanvas canvas)
        {
            // 创建画笔
            using (SKPaint paint = new SKPaint())
            {
                SKRect rect = new SKRect(0, 0, this.Width, this.Height); // 创建绘制区域
                List<SKColor> colors = new List<SKColor>();
                colors.Add(this.GetRandomColor(200, 230));
                colors.Add(this.GetRandomColor(200, 230));
                paint.Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(this.Width, this.Height), colors.ToArray(), new float[] { 0, 1 }, SKShaderTileMode.Repeat);
                canvas.DrawRect(rect, paint);
            }
        }
        /// <summary>
        /// 绘制字符
        /// </summary>
        private void CreateChar(SKCanvas canvas, string c, int index)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.TextSize = 25; // 字体大小
                paint.IsAntialias = true; // 开启抗锯齿
                paint.Color = SKColors.Red;
                byte[] font = this.Font;
                int angle = RandomHelper.GetIntNumber(-15, 15); //旋转角度
                using (MemoryStream stream = new MemoryStream(font))
                {
                    paint.Typeface = SKTypeface.FromStream(stream); // 设置字体
                    SKRect textSize = new SKRect();
                    paint.MeasureText(c, ref textSize);
                    float margin = 5;
                    float charWidth = (Width - margin) / Code.Length;
                    float charX = charWidth * index + margin;
                    float charY = Height - (Height - textSize.Size.Height) / 2;
                    List<SKColor> colors = new List<SKColor>();
                    colors.Add(this.GetRandomColor(0, 200));
                    colors.Add(this.GetRandomColor(0, 200));
                    paint.Shader = SKShader.CreateLinearGradient(new SKPoint(charX, Height / 2), new SKPoint(charX + textSize.Width, Height / 2), colors.ToArray(), new float[] { 0, 1 }, SKShaderTileMode.Repeat);

                    canvas.RotateDegrees(angle, charX, charY);
                    canvas.DrawText(c, charX, charY, paint);
                    canvas.RotateDegrees(-angle, charX, charY);
                }
            }
        }


        /// <summary>
        /// 创建验证码文本
        /// </summary>
        /// <param name="canvas"></param>
        private void CreateText(SKCanvas canvas)
        {
            int length = this.Code.Length;
            for (int i = 0; i < length; i++)
            {
                this.CreateChar(canvas, this.Code[i].ToString(), i);
            }
        }

        /// <summary>
        /// 绘制干扰线
        /// </summary>
        private void CreateCurve(SKCanvas canvas)
        {
            SKPoint p1 = new SKPoint(0, RandomHelper.GetIntNumber(0, this.Height));
            SKPoint p2 = new SKPoint(RandomHelper.GetIntNumber(0, Width), RandomHelper.GetIntNumber(0, Height));
            SKPoint p3 = new SKPoint(RandomHelper.GetIntNumber(0, Width), RandomHelper.GetIntNumber(0, Height));
            SKPoint p4 = new SKPoint(Width, RandomHelper.GetIntNumber(0, Height));
            using (SKPath path = new SKPath())
            {
                path.MoveTo(p1);
                path.CubicTo(p2, p3, p4);
                List<SKColor> colors = new List<SKColor>();
                colors.Add(this.GetRandomColor(0, 200));
                colors.Add(this.GetRandomColor(0, 200));
                using (SKPaint paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(this.Width, this.Height), colors.ToArray(), new float[] { 0, 1 }, SKShaderTileMode.Repeat);
                    paint.StrokeWidth = 2;
                    paint.IsAntialias = true;
                    canvas.DrawPath(path, paint);
                }
            }
        }

        /// <summary>
        /// 获取随机颜色
        /// </summary>
        private SKColor GetRandomColor(byte min, byte max)
        {
            byte r = Convert.ToByte(RandomHelper.GetIntNumber(min, max));
            byte g = Convert.ToByte(RandomHelper.GetIntNumber(min, max));
            byte b = Convert.ToByte(RandomHelper.GetIntNumber(min, max));
            return new SKColor(r, g, b);
        }
    }
}

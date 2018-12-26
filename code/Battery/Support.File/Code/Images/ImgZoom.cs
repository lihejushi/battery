using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Support.File.Code.Images
{
    /// <summary>
    ///  图片压缩
    /// </summary>
    public class ImgZoom
    {
        public static void ConstrainImg(string imgPath, string savePath, int width, int height)
        {
            if (!System.IO.File.Exists(imgPath)) return; //文件不存在
            //large图
            string dire = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dire)) Directory.CreateDirectory(dire);
            //读取原始图片
            Image image = Image.FromFile(imgPath);
            //生成缩略原图
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.None; //设置高质量,低速度呈现平滑程度
            g.InterpolationMode = InterpolationMode.Low;   //设置高质量插值法 
            g.CompositingQuality = CompositingQuality.HighSpeed; //质量低

            g.Clear(Color.Transparent);   //清空画布并以透明背景色填充
            g.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            try
            {
                //以指定格式保存图片
                bitmap.Save(savePath);
                //XLog.XTrace.WriteLine("ImgZoom：{0}", savePath);
            }
            catch (Exception ex)
            {
                //XLog.XTrace.WriteLine("ImgZoom：{0} \r\n {1} \r\n {2}", imgPath, ex.Message, ex.StackTrace);
            }
            finally
            {
                //释放资源
                if (g != null) g.Dispose();
                if (bitmap != null) bitmap.Dispose();
                if (image != null) image.Dispose();
            }
        }

        #region 获取ImageFlags
        private static string GetImageFlags(System.Drawing.Image img)
        {
            ImageFlags FlagVals = (ImageFlags)Enum.Parse(typeof(ImageFlags), img.Flags.ToString());
            return FlagVals.ToString();
        }
        #endregion
        #region 判断CMYK
        private static bool IsCMYK(System.Drawing.Image img)
        {
            string FlagVals = GetImageFlags(img);
            if ((FlagVals.IndexOf("Ycck") > -1) || (FlagVals.IndexOf("Cmyk") > -1))
                return true;
            else
                return false;
        }
        #endregion
        #region 压缩图片
        /// <summary>
        ///  转换CMYK格式至RGB
        /// </summary>
        /// <param name="rate">（小于0.5低质量），（大于1高质量），（0.5至1之间，一般质量）</param>
        private static Bitmap ConvertCMYK(Bitmap bmp, double rate)
        {
            rate = rate > 1.0 ? 1.0 : rate;
            rate = rate < 0.5 ? 0.5 : rate;
            Bitmap tmpBmp = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(tmpBmp);
            g.Clear(Color.White);
            if (rate == 0.5)//低质量
            {
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
            if (rate == 1.0)//高质量
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            }
            else//一般质量
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            }
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            // 将CMYK图片重绘一遍,此时GDI+自动将CMYK格式转换为RGB了
            g.DrawImage(bmp, rect);
            Bitmap returnBmp = new Bitmap(tmpBmp);
            g.Dispose();
            tmpBmp.Dispose();
            bmp.Dispose();
            return returnBmp;
        }
        #endregion
    }
}
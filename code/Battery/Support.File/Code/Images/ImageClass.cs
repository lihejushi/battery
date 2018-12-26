using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Support.File.Code.Images
{
    public class ImageClass
    {
        /// <summary>
        /// 填充模式
        /// </summary>
        /// <remarks></remarks>
        public enum FillMode
        {
            /// <summary>
            /// 平铺
            /// </summary>
            /// <remarks></remarks>
            Title = 0,
            /// <summary>
            /// 居中
            /// </summary>
            /// <remarks></remarks>
            Center = 1,
            /// <summary>
            /// 拉伸
            /// </summary>
            /// <remarks></remarks>
            Struk = 2,
            /// <summary>
            /// 缩放
            /// </summary>
            /// <remarks></remarks>
            Zoom = 3
        }

        /// <summary>
        /// 将指向图像按指定的填充模式绘制到目标图像上
        /// </summary>
        /// <param name="SourceBmp">要控制填充模式的源图</param>
        /// <param name="TargetBmp">要绘制到的目标图</param>
        /// <param name="_FillMode">填充模式</param>
        /// <remarks></remarks>
        public static void Image_FillRect(Bitmap SourceBmp, Bitmap TargetBmp, FillMode _FillMode, ImageFormat imgFormat)
        {
            try
            {
                switch (_FillMode)
                {
                    case FillMode.Title:
                        using (TextureBrush Txbrus = new TextureBrush(SourceBmp))
                        {
                            Txbrus.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
                            using (Graphics G = Graphics.FromImage(TargetBmp))
                            {
                                //G.Clear(Color.FromArgb(0, 0, 0, 0));
                                GraphicsClear(SourceBmp, imgFormat, G);
                                G.FillRectangle(Txbrus, new Rectangle(0, 0, TargetBmp.Width - 1, TargetBmp.Height - 1));
                            }
                        }

                        break;
                    case FillMode.Center:
                        using (Graphics G = Graphics.FromImage(TargetBmp))
                        {
                            int xx = (TargetBmp.Width - SourceBmp.Width) / 2;
                            int yy = (TargetBmp.Height - SourceBmp.Height) / 2;
                            //G.Clear(Color.FromArgb(0, 0, 0, 0));
                            GraphicsClear(SourceBmp, imgFormat, G);
                            G.DrawImage(SourceBmp, new Rectangle(xx, yy, SourceBmp.Width, SourceBmp.Height), new Rectangle(0, 0, SourceBmp.Width, SourceBmp.Height), GraphicsUnit.Pixel);
                        }

                        break;
                    case FillMode.Struk:
                        using (Graphics G = Graphics.FromImage(TargetBmp))
                        {
                            //G.Clear(Color.FromArgb(0, 0, 0, 0));
                            GraphicsClear(SourceBmp, imgFormat, G);
                            G.DrawImage(SourceBmp, new Rectangle(0, 0, TargetBmp.Width, TargetBmp.Height), new Rectangle(0, 0, SourceBmp.Width, SourceBmp.Height), GraphicsUnit.Pixel);
                        }

                        break;
                    case FillMode.Zoom:
                        double tm = 0.0;
                        int W = SourceBmp.Width;
                        int H = SourceBmp.Height;
                        if (W > TargetBmp.Width)
                        {
                            tm = TargetBmp.Width / SourceBmp.Width;
                            W = Convert.ToInt32(W * tm);
                            H = Convert.ToInt32(H * tm);
                        }
                        if (H > TargetBmp.Height)
                        {
                            tm = TargetBmp.Height / H;
                            W = Convert.ToInt32(W * tm);
                            H = Convert.ToInt32(H * tm);
                        }
                        using (Bitmap tmpBP = new Bitmap(W, H))
                        {
                            using (Graphics G2 = Graphics.FromImage(tmpBP))
                            {
                                G2.Clear(Color.FromArgb(0, 0, 0, 0));
                                G2.DrawImage(SourceBmp, new Rectangle(0, 0, W, H), new Rectangle(0, 0, SourceBmp.Width, SourceBmp.Height), GraphicsUnit.Pixel);
                                using (Graphics G = Graphics.FromImage(TargetBmp))
                                {
                                    //G.Clear(Color.FromArgb(0, 0, 0, 0));
                                    int xx = (TargetBmp.Width - W) / 2;
                                    int yy = (TargetBmp.Height - H) / 2;
                                    GraphicsClear(SourceBmp, imgFormat, G);
                                    G.DrawImage(tmpBP, new Rectangle(xx, yy, W, H), new Rectangle(0, 0, W, H), GraphicsUnit.Pixel);
                                }
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 拉伸图片
        /// </summary>
        /// <param name="bmp">原始图片</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        public static MemoryStream ResizeImage(Bitmap sourceBmp, int newW, int newH, ImageFormat imgFormat)
        {
            try
            {
                Bitmap bap = new Bitmap(newW, newH);

                Image_FillRect(sourceBmp, bap, FillMode.Struk, imgFormat);
                MemoryStream ms2 = new MemoryStream();
                bap.Save(ms2, imgFormat);

                bap.Dispose();
                //sourceBmp.Dispose( );
                return ms2;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 清理笔刷北极你给
        /// </summary>
        /// <param name="sourceBmp"></param>
        /// <param name="imgFormat"></param>
        /// <param name="G"></param>
        public static void GraphicsClear(Bitmap sourceBmp, ImageFormat imgFormat, Graphics G)
        {
            if (sourceBmp.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png) &&
                    imgFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
            {
                G.Clear(Color.White);
            }
            else
            {
                G.Clear(Color.FromArgb(0, 0, 0, 0));
            }
        }

        /// <summary>
        /// 处理图片
        /// </summary>
        /// <param name="oripath">源地址</param>
        /// <param name="pngPath">保存地址</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        static void processorPng(string oripath, string pngPath, int width, int height)
        {
            Image imgPhoto = Image.FromFile(oripath);
            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmPhoto.MakeTransparent();
            Graphics gbmPhoto = Graphics.FromImage(bmPhoto);
            gbmPhoto.Clear(Color.Transparent); //使用透明背景
            gbmPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, width, height), new Rectangle(0, 0, imgPhoto.Width, imgPhoto.Height), GraphicsUnit.Pixel);
            bmPhoto.Save(pngPath, ImageFormat.Png);

            gbmPhoto.Dispose();
            bmPhoto.Dispose();
            imgPhoto.Dispose();
        }

        public static byte[] CompressionImage(Stream fileStream, long quality)
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromStream(fileStream))
            {
                using (Bitmap bitmap = new Bitmap(img))
                {
                    ImageCodecInfo CodecInfo = GetEncoder(img.RawFormat);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, CodecInfo, myEncoderParameters);
                        myEncoderParameters.Dispose();
                        myEncoderParameter.Dispose();
                        return ms.ToArray();
                    }
                }
            }
        }


        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
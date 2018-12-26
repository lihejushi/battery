using ImageMagick;
using Support.File.Code.Config;
using Support.File.Code.Images;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Support.File.Code
{
    public class FileService
    {
        IFileServiceConfig Config = null;
        public FileService(IFileServiceConfig config)
        {
            Config = config;
        }

        public void CropImg(HttpContext context)
        {
            try
            {
                string SaveDire1 = context.Request["SaveDire1"];
                string SaveDire2 = context.Request["SaveDire2"];

                int PointX = Convert.ToInt32(context.Request["x"]);
                int PointY = Convert.ToInt32(context.Request["y"]);
                int CutWidth = Convert.ToInt32(context.Request["w"]);
                int CutHeight = Convert.ToInt32(context.Request["h"]);
                int PicWidth = Convert.ToInt32(context.Request["pw"]);
                int PicHeight = Convert.ToInt32(context.Request["ph"]);
                int targetWidth = Convert.ToInt32(context.Request["tw"]);
                int targetHeight = Convert.ToInt32(context.Request["th"]);

                string PicExt = context.Request["ext"];
                ImageFormat PicFormat = ImageFormat.Jpeg;

                if (PicExt == "png")
                    PicFormat = ImageFormat.Png;

                Tuple<string, string> PathTuple = Config.CropImgConfig(SaveDire1, SaveDire2, Convert.ToString(context.Request["PicPath"]));

                using (MagickImage image = new MagickImage(PathTuple.Item1))
                {
                    //图片裁剪逻辑
                    //1.宽高一致时，图像不作处理
                    //2.宽高任意一项小于目标尺寸时，图像放大后裁剪
                    //3.宽高都大于目标尺寸时，图像不放大，把坐标和选取宽高放大
                    //if (image.Width > targetWidth && image.Height > targetHeight)
                    //{
                    //    //int newPointX = Convert.ToInt32((Convert.ToDouble(image.Width) / Convert.ToDouble(PicWidth)) * PointX);
                    //    //int newPointY = Convert.ToInt32((Convert.ToDouble(image.Height) / Convert.ToDouble(PicHeight)) * PointY);

                    //    //int newWidth = Convert.ToInt32((Convert.ToDouble(image.Width) / Convert.ToDouble(PicWidth)) * CutWidth);
                    //    //int newHeight = Convert.ToInt32((Convert.ToDouble(image.Height) / Convert.ToDouble(PicHeight)) * CutHeight);

                    //    //image.Crop(new MagickGeometry(newPointX, newPointY, newWidth, newHeight));
                    //    //image.Resize(new MagickGeometry(targetWidth, targetHeight) { IgnoreAspectRatio = true });
                    //    image.Crop(new MagickGeometry(PointX, PointY, CutWidth, CutHeight));
                    //    image.Resize(new MagickGeometry(targetWidth, targetHeight) { IgnoreAspectRatio = true });
                    //}
                    //else if (image.Width < targetWidth || image.Height < targetHeight)
                    //{
                    //    //int newWidth = Convert.ToInt32((Convert.ToDouble(PicWidth) / Convert.ToDouble(CutWidth)) * targetWidth);
                    //    //int newHeight = Convert.ToInt32((Convert.ToDouble(PicHeight) / Convert.ToDouble(CutHeight)) * targetHeight);
                    //    //image.Resize(new MagickGeometry(newWidth, newHeight) { IgnoreAspectRatio = true });
                    //    //image.Crop(new MagickGeometry(PointX * newWidth / PicWidth, PointY * newHeight / PicHeight, targetWidth, targetHeight));
                    //    image.Crop(new MagickGeometry(PointX, PointY, CutWidth, CutHeight));
                    //    image.Resize(new MagickGeometry(targetWidth, targetHeight) { IgnoreAspectRatio = true });
                    //}
                    //else if (image.Width == targetWidth)
                    //{

                    //}
                    if (image.Width == targetWidth && image.Height == targetHeight)
                    {
                    }
                    else
                    {
                        double scale = (Convert.ToDouble(image.Width) / Convert.ToDouble(PicWidth));
                        PointX = Convert.ToInt32(scale * PointX);
                        PointY = Convert.ToInt32(scale * PointY);
                        CutWidth = Convert.ToInt32(scale * CutWidth);
                        CutHeight = Convert.ToInt32(scale * CutHeight);

                        if (PointX + CutWidth > image.Width)
                        {
                            PointX = 0;
                            CutWidth = image.Width;
                        }
                        if (PointY + CutHeight > image.Height)
                        {
                            PointY = 0;
                            CutHeight = image.Height;
                        }

                        image.Crop(new MagickGeometry(PointX, PointY, CutWidth, CutHeight));
                        image.Resize(new MagickGeometry(targetWidth, targetHeight) { IgnoreAspectRatio = true });
                    }

                    //image.Crop(new MagickGeometry(PointX, PointY, CutWidth, CutHeight));
                    //image.Resize(new MagickGeometry(targetWidth, targetHeight) { IgnoreAspectRatio = true });

                    MemoryStream msteam = new MemoryStream();
                    image.Write(msteam, PicFormat == ImageFormat.Jpeg ? MagickFormat.Jpeg : MagickFormat.Png);
                    // 生成文件名，并创建目录
                    string FileName = Md5Hash32(msteam.GetBuffer()) + (PicFormat == ImageFormat.Png ? ".png" : ".jpg");
                    string CropImgPath = PathTuple.Item2 + FileName;
                    string dire = Path.GetDirectoryName(CropImgPath);
                    if (!Directory.Exists(dire))
                        Directory.CreateDirectory(dire);

                    using (Stream localFile = new FileStream(CropImgPath, FileMode.OpenOrCreate))
                    {
                        localFile.Write(msteam.ToArray(), 0, (int)msteam.Length);
                    }

                    Output(context, Config.CropImgReturnResult(CropImgPath, PathTuple.Item2, FileName));
                    return;
                }

                //// 获取原图
                //string PicPath = PathTuple.Item1;
                //Bitmap sourceBmp = new Bitmap( PicPath );

                //Image zoomPhoto = Image.FromStream(ImageClass.ResizeImage(
                //    sourceBmp,
                //    Convert.ToInt32((Convert.ToDouble(PicWidth) / Convert.ToDouble(CutWidth)) * targetWidth),
                //    Convert.ToInt32((Convert.ToDouble(PicHeight) / Convert.ToDouble(CutHeight)) * targetHeight), PicFormat
                //));
                ////zoomPhoto.Save("test_1816635655e0f4a99be8206eb21ca7c3.jpg", ImageFormat.Jpeg);

                //// 开始处理
                //Bitmap bmPhoto = new Bitmap( targetWidth, targetHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb );

                //Graphics gbmPhoto = Graphics.FromImage( bmPhoto );
                //gbmPhoto.SmoothingMode = SmoothingMode.HighQuality; //设置高质量,低速度呈现平滑程度
                //gbmPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;   //设置高质量插值法 
                //gbmPhoto.CompositingQuality = CompositingQuality.HighQuality; //质量低
                ////gbmPhoto.Clear( Color.Transparent );
                ////gbmPhoto.Clear(Color.FromArgb(0, 0, 0, 0)); //使用白色背景z
                //ImageClass.GraphicsClear(sourceBmp, PicFormat, gbmPhoto);
                //gbmPhoto.DrawImage( zoomPhoto, new Rectangle( 0, 0, targetWidth, targetHeight ),
                //    PointX * zoomPhoto.Width / PicWidth,
                //    PointY * zoomPhoto.Height / PicHeight,
                //    targetWidth, targetHeight, GraphicsUnit.Pixel
                //);

                //MemoryStream msteam = new MemoryStream( );
                //bmPhoto.Save(msteam, PicFormat);

                //// 生成文件名，并创建目录
                //string FileName = Md5Hash32(msteam.GetBuffer()) + (PicFormat == ImageFormat.Png ? ".png" : ".jpg");
                //string CropImgPath = PathTuple.Item2 + FileName;
                //string dire = Path.GetDirectoryName( CropImgPath );
                //if( !Directory.Exists( dire ) )
                //    Directory.CreateDirectory( dire );
                //// 保存图片
                //gbmPhoto.Save( );
                //gbmPhoto.Dispose( );
                ////bmPhoto.GetThumbnailImage(bmPhoto.Width, bmPhoto.Height, () => { return false; }, IntPtr.Zero).Save(CropImgPath, ImageFormat.Jpeg);
                ////bmPhoto.MakeTransparent( Color.Empty );
                //if(PicFormat == ImageFormat.Jpeg)
                //{
                //    JpegSave(100, CropImgPath, bmPhoto);
                //}
                //else
                //{
                //    bmPhoto.Save(CropImgPath, PicFormat);
                //}

                //msteam.Dispose( );                
                //bmPhoto.Dispose( );
                //zoomPhoto.Dispose( );
                //sourceBmp.Dispose( );

                //XLog.XTrace.WriteLine( "剪切成功：{0}", CropImgPath );
                //Output(context, Config.CropImgReturnResult(CropImgPath, PathTuple.Item2, FileName));
            }
            catch (Exception ex)
            {
                XLog.XTrace.WriteLine("剪切图片失败：{0}\r\n{1}", ex.Message, ex.StackTrace);
                Output(context, "Error");
            }
        }

        private bool JpegSave(int height, string dFile, Bitmap ob)
        {
            //XLog.XTrace.WriteLine("Jpeg图片保存");
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = height;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    //XLog.XTrace.WriteLine("Jpeg图片保存");
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    ob.Save(dFile, ImageFormat.Jpeg);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
            }
        }

        public void ConstrainImg(HttpContext context)
        {
            string SaveDire1 = context.Request["SaveDire1"];
            string SaveDire2 = context.Request["SaveDire2"];
            string SaveFileName = context.Request["SaveFileName"];
            string SourceFileName = context.Request["SourceFileName"];
            int TargetWidth = Convert.ToInt32(context.Request["w"]);
            int TargetHeight = Convert.ToInt32(context.Request["h"]);
            Tuple<string, string> PathTuple = Config.MoveFile(SaveDire1, SaveDire2, SourceFileName, SaveFileName);
            // 获取原始文件
            string SourceFilePath = PathTuple.Item1;
            // 目标路径
            string SaveFilePath = PathTuple.Item2;
            if (System.IO.File.Exists(SourceFilePath))
            {
                ImgZoom.ConstrainImg(SourceFilePath, SaveFilePath, TargetWidth, TargetHeight);
                Output(context, "Success");
            }
            else
            {
                Output(context, "Error"); return;
            }
        }

        public void MoveFile(HttpContext context)
        {
            string SaveDire1 = context.Request["SaveDire1"];
            string SaveDire2 = context.Request["SaveDire2"];
            string SaveFileName = context.Request["SaveFileName"];
            string SourceFileName = context.Request["SourceFileName"];
            try
            {
                Tuple<string, string> PathTuple = Config.MoveFile(SaveDire1, SaveDire2, SourceFileName, SaveFileName);

                // 获取原始文件
                string SourceFilePath = PathTuple.Item1;
                // 目标路径
                string SaveFilePath = PathTuple.Item2;
                // 是否清除原文件夹内所有文件
                bool ClearAllFile = !string.IsNullOrEmpty(context.Request["ClearAllFile"]);
                // 目标文件夹，目录清除
                string dire = Path.GetDirectoryName(SaveFilePath);
                if (Directory.Exists(dire) == false)
                    Directory.CreateDirectory(dire);
                else if (ClearAllFile == true)
                {
                    Directory.Delete(dire, true);
                    Directory.CreateDirectory(dire);
                }
                System.IO.File.Copy(SourceFilePath, SaveFilePath, true);
                XLog.XTrace.WriteLine("转移成功：{0} 至  {1} 清除目录：{2}", SourceFilePath, SaveFilePath, ClearAllFile);
                Output(context, "Success");
            }
            catch (Exception ex)
            {
                XLog.XTrace.WriteLine("{0}   {1}  {2}  {3}", SaveDire1, SaveFileName, SaveDire2, SourceFileName);
                XLog.XTrace.WriteLine("文件转移失败：{0}\r\n{1}", ex.Message, ex.StackTrace);
                Output(context, "Error");
            }
        }

        public void UploadFile(HttpContext context)
        {
            string SaveDire1 = context.Request["SaveDire1"];
            //if(string.IsNullOrEmpty(SaveDire1)) SaveDire1 = DateTime.Now.ToString("yyyyMMdd");
            string SaveFileName = context.Request["SaveFileName"];
            bool CoverFile = !string.IsNullOrEmpty(context.Request["CoverFile"]);

            int FileLengs, BytePosition, ByteWriteLeng, BytePostLeng; //文件总长度，偏移量，写长度，发送长度            
            byte[] BufferPost; //post 的文件流

            FileLengs = Convert.ToInt32(context.Request["FileLengs"]);
            BytePosition = Convert.ToInt32(context.Request["BytePosition"]);
            ByteWriteLeng = Convert.ToInt32(context.Request["ByteWriteLeng"]);
            BytePostLeng = context.Request.ContentLength;
            //判断Post流长度（是否有丢包，长度为0）
            if (BytePostLeng == 0 || (BytePostLeng < ByteWriteLeng))
            {
                Output(context, "Error"); return;
            }
            if (BytePosition == 0)
                XLog.XTrace.WriteLine("{0}：{1}", "传递原文件", FileLengs);
            //读取文件流，完整文件存储路径
            BufferPost = context.Request.BinaryRead(BytePostLeng);
            string SavePath = Config.UploadFileConfig(SaveDire1, SaveFileName);
            try
            {
                //创建目录
                string dire = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(dire))
                    Directory.CreateDirectory(dire);
                else if (BytePosition == 0 && CoverFile == false && System.IO.File.Exists(SavePath) == true)
                {
                    XLog.XTrace.WriteLine("原文件存在：" + SavePath);
                    Output(context, "$FileExists$" + Config.UploadFileReturnResult(SavePath, SaveDire1, SaveFileName));
                    return;
                }
                else if (BytePosition == 0 && CoverFile == true) System.IO.File.Delete(SavePath);

                SaveFile(SavePath, BufferPost, BytePosition, ByteWriteLeng);
                Output(context, Config.UploadFileReturnResult(SavePath, SaveDire1, SaveFileName));
            }
            catch (Exception ex)
            {
                XLog.XTrace.WriteLine("{0} \r\n {1}", ex.Message, ex.StackTrace);
                Output(context, "Error");
            }
        }

        #region protected
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="SavePath"></param>
        /// <param name="BufferPost"></param>
        /// <param name="BytePosition"></param>
        /// <param name="ByteWriteLeng"></param>
        protected void SaveFile(string SavePath, byte[] BufferPost, int BytePosition, int ByteWriteLeng)
        {
            FileStream fStream = null;
            BinaryWriter binaryWriter = null;
            try
            {
                fStream = new FileStream(SavePath, FileMode.OpenOrCreate);
                fStream.Position = BytePosition;
                fStream.Write(BufferPost, 0, ByteWriteLeng);
                XLog.XTrace.WriteLine("{0}：{1} - {2}", SavePath, BytePosition, ByteWriteLeng);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (binaryWriter != null) binaryWriter.Close();
                if (fStream != null) fStream.Close();
            }
        }

        /// <summary>
        ///  Hash值（32位）
        /// </summary>
        protected static string Md5Hash32(byte[] bytes)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(bytes);
            // 32位
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
                sb.Append(b.ToString("x"));
            return sb.ToString();
        }

        // 输出结果
        protected void Output(HttpContext context, string state)
        {
            context.Response.Clear();
            context.Response.Write(state.Replace('\\', '/'));
        }
        #endregion
    }
}
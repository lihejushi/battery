using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using System.Drawing.Imaging;
namespace Battery.Framework.Tools
{
    public class QrCodeHelper
    {
        /// <summary>  
        /// 生成二维码图片  
        /// </summary>  
        /// <param name="codeNumber">要生成二维码的字符串</param>       
        /// <param name="size">大小尺寸</param>  
        /// <returns>二维码图片</returns>  
        public byte[] Create_ImgCode(string codeNumber)
        {
            //创建二维码生成类  
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            ////设置编码模式  
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            ////设置编码测量度  
             qrCodeEncoder.QRCodeScale  = 3;
            ////设置编码版本  
            qrCodeEncoder.QRCodeVersion = 0;
            ////设置编码错误纠正  
            //qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //生成二维码图片  
            MemoryStream codeMs = new System.IO.MemoryStream();
            qrCodeEncoder.Encode(codeNumber,Encoding.GetEncoding("gb2312")).Save(codeMs, ImageFormat.Png);
            byte[] buffer = codeMs.ToArray();
            return buffer;
        }
        private Bitmap Getlogo(string path)
        {
            //Bitmap newBmp = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/wx/images/logo.png"));//获取图片对象
            Bitmap newBmp = new Bitmap(System.Web.HttpContext.Current.Server.MapPath(path));//获取图片对象
            Bitmap bmp = new Bitmap(newBmp, 40, 40);//缩放
            return bmp;
        }
        public byte[] GetQr(string content, string path)
        {
            //生成二维码
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            //qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //qrCodeEncoder.QRCodeScale = 6;
            //qrCodeEncoder.QRCodeVersion = 7;
            //qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
            MemoryStream oCodeMs = new System.IO.MemoryStream();
            qrCodeEncoder.Encode(content, Encoding.UTF8).Save(oCodeMs, ImageFormat.Png);
            Bitmap bCode = new Bitmap(oCodeMs);
            Bitmap bLogo = Getlogo(path); //获取logo图片对象
            int Y = bCode.Height;
            int X = bCode.Width;
            Point point = new Point(X / 2 - 20, Y / 2 - 20);
            //logo图片绘制到二维码上，这里将简单计算一下logo所在的坐标                      
            Graphics g = Graphics.FromImage(bCode);//创建一个画布                     
            g.DrawImage(bLogo, point);//将logo图片绘制到二维码图片上
            g.Dispose();

            MemoryStream nCodeMs = new System.IO.MemoryStream();
            bCode.Save(nCodeMs, ImageFormat.Png);
            byte[] buffer = nCodeMs.ToArray();

            bLogo.Dispose();
            bCode.Dispose();
            oCodeMs.Close();
            nCodeMs.Close();
            System.Drawing.Bitmap image = bCode;
            return buffer;
        }
    }
}
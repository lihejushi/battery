using System;
using System.Security.Cryptography;
using System.Text;

namespace XT.MVC.Core.Encrypt
{
    /// <summary>
    ///  MD5 非对称加密
    /// </summary>
    public class MD5Helper
    {
        /// <summary>
        ///  Hash值（16位）
        /// </summary>
        public static string Md5Hash16( byte[] bytes )
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider( );
            byte[] data = md5Hasher.ComputeHash( bytes );
            // 16位
            StringBuilder sb = new StringBuilder( );
            for(int i = 4; i < 12; i++)
                sb.Append( data[i].ToString( "x2" ) );
            return sb.ToString( );
        }

        /// <summary>
        ///  Hash值（32位）
        /// </summary>
        public static string Md5Hash32( string input )
        {
            return Md5Hash32( Encoding.Default.GetBytes( input ) );
        }
        /// <summary>
        ///  Hash值（32位）
        /// </summary>
        public static string Md5Hash32( byte[] bytes )
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider( );
            byte[] data = md5Hasher.ComputeHash( bytes );
            // 32位
            StringBuilder sb = new StringBuilder( );
            foreach(byte b in data) sb.Append( b.ToString( "x2" ) );
            return sb.ToString( );
        }
    }//class end
}

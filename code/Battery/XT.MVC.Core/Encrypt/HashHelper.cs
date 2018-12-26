using System;
using System.Text;
using System.Security.Cryptography;

namespace XT.MVC.Core.Encrypt
{
    /// <summary>
    /// Hash散列值对称加密
    /// </summary>
    public class HashHelper
    {
        #region fields
        private HashAlgorithm mhash;
        #endregion

        public HashHelper( )
        {
        }

        #region private methods
        /// <summary>
        /// 生成随机Salt值.
        /// </summary>
        /// <returns>Salt值,8位</returns>
        private string CreateSalt( )
        {
            byte[] bytSalt = new byte[8];
            RNGCryptoServiceProvider rng;
            rng = new RNGCryptoServiceProvider( );
            rng.GetBytes( bytSalt );
            return Convert.ToBase64String( bytSalt );
        }
        private HashAlgorithm SetHash( string HashType )
        {
            if(HashType == "SHA1")
                return new SHA1CryptoServiceProvider( );
            else
                return new MD5CryptoServiceProvider( );
        }
        private void EncryptPaul( )
        {
            byte[] bytValue;
            byte[] bytHash;
            SHA1CryptoServiceProvider SHA1;
            SHA1 = new SHA1CryptoServiceProvider( );
            // Convert the original string to array of Bytes
            bytValue = System.Text.Encoding.UTF8.GetBytes( "Paul" );
            // Compute the Hash, returns an array of Bytes
            bytHash = SHA1.ComputeHash( bytValue );
            SHA1.Clear( );
            // Return a base 64 encoded string of the Hash value
            System.Diagnostics.Debug.WriteLine( Convert.ToBase64String( bytHash ) );
        }
        private void EncryptPaulMD5( )
        {
            byte[] bytValue;
            byte[] bytHash;
            MD5CryptoServiceProvider MD5;
            MD5 = new MD5CryptoServiceProvider( );
            // Convert the original string to array of Bytes
            bytValue = System.Text.Encoding.UTF8.GetBytes( "Paul" );
            // Compute the Hash, returns an array of Bytes
            bytHash = MD5.ComputeHash( bytValue );
            MD5.Clear( );
            // Return a base 64 encoded string of the Hash value
            System.Diagnostics.Debug.WriteLine( Convert.ToBase64String( bytHash ) );
        }
        #endregion

        #region public methods
        /// <summary>
        /// 散列加密字符串,返回加Salt值的加密字符串
        /// </summary>
        /// <param name="Value">要加密的字符串</param>
        /// <param name="HashType">散列算法种类,SHA1或MD5</param>
        /// <param name="Salt">生成密码时Salt为"",自动生成;验证密码时,传入Salt值</param>
        /// <returns>加Salt值的加密字符串,Salt值在字符串的最后8位</returns>
        public string HashString(string Value, string HashType, string Salt)
        {
            byte[] bytValue;
            byte[] bytHash;
            string salt = "";
            if(Salt == "")
                salt = CreateSalt( );
            else
                salt = Salt;

            mhash = SetHash( HashType );
            // Convert the original string to array of Bytes
            string strTemp = Value + salt;
            bytValue = System.Text.Encoding.UTF8.GetBytes( strTemp );
            // Compute the Hash, returns an array of Bytes
            bytHash = mhash.ComputeHash( bytValue );
            mhash.Clear( );
            // Return a base 64 encoded string of the Hash value
            return Convert.ToBase64String( bytHash ) + salt;
        }

        public string HashString(string Value)
        {
            return HashString(Value, "SHA1", "");
        }

        public string HashString(string Value, string Salt)
        {
            return HashString(Value, "SHA1", Salt);
        }
        #endregion
    }//class end
}

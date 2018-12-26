using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Battery
{
    /// <summary>
    /// DES 对称加密
    /// </summary>
    public class DesHelper
    {
        #region
        internal class DefaultSecretKey
        {
           
            #region
            static string HostAllows = Encoding.UTF8.GetString(
                new byte[]
            {
                //108,111,99,97,108,104,111,115,116,44,49,50,55,46,48,46,48,46,49,44,49,57,50,46,49,54,56
            }
            );
            static string Exmessage = Encoding.UTF8.GetString(
                new byte[]
            {
                232,173,166,229,145,138,239,188,154,233,131,145,229,183,158,231,191,148,229,164,169,228,191,161,230,129,175,230,138,128,230,156,175,230,156,137,233,153,144,229,133,172,229,143,184,231,137,136,230,156,172,230,137,128,230,156,137
            }
            );
            #endregion
        }

        /// <summary>
        ///  DES 原始加密算法
        /// </summary>
        internal class Des
        {
            #region Field
            private SymmetricAlgorithm mCSP;
            private string mKey;
            private string mIV;
            #endregion

            /// <summary>
            ///  实例化DES加密对象
            /// </summary>
            /// <param name="key">密钥</param>
            /// <param name="iv">向量</param>
            public Des(string key, string iv)
            {
                this.mKey = key;
                this.mIV = iv;
            }

            #region Property
            /// <summary>
            ///  获取密钥
            /// </summary>
            public string Key
            {
                get { return this.mKey; }
            }
            /// <summary>
            ///  获取向量
            /// </summary>
            public string IV
            {
                get { return this.mIV; }
            }
            #endregion
            #region Public Method
            /// <summary>
            /// 对称加密字符串
            /// </summary>
            /// <param name="Value">要加密的字符串值</param>
            /// <returns>加密后的字符串</returns>
            public string EncryptString(string Value, string type)
            {
                mCSP = SetEnc(type);
                mCSP.Key = Encoding.ASCII.GetBytes(this.Key.Substring(0, 8));
                mCSP.IV = Encoding.ASCII.GetBytes(this.IV.Substring(0, 8));
                ICryptoTransform ct;
                MemoryStream ms;
                CryptoStream cs;
                byte[] byt;

                ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);

                byt = Encoding.UTF8.GetBytes(Value);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();

                cs.Close();

                return Convert.ToBase64String(ms.ToArray());
            }

            /// <summary>
            /// 对称解密字符串
            /// </summary>
            /// <param name="Value">要解密的字符串值</param>
            /// <returns>解密后的字符串</returns>
            public string DecryptString(string Value, string type)
            {
                mCSP = SetEnc(type);
                mCSP.Key = Encoding.ASCII.GetBytes(this.Key.Substring(0, 8));
                mCSP.IV = Encoding.ASCII.GetBytes(this.IV.Substring(0, 8));
                ICryptoTransform ct;
                MemoryStream ms;
                CryptoStream cs;
                byte[] byt;

                ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);

                byt = Convert.FromBase64String(Value);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();

                cs.Close();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
            #endregion
            #region private method
            /// <summary>
            /// 根据加密算法的不同生成不同的加密实例
            /// 工厂方法
            /// </summary>
            /// <returns>加密得法类实例</returns>
            private SymmetricAlgorithm SetEnc(string Type)
            {
                if (Type == "DES")
                    return new DESCryptoServiceProvider();
                else
                    return new TripleDESCryptoServiceProvider();
            }
            #endregion
        }
        #endregion


        #region public static method

        public static string EncryptString_Iv( string EString,string secretKey, string DesIV)
        {
            Des myEncrypt = new Des(secretKey, DesIV);
            return myEncrypt.EncryptString(EString, "DES");
        }

        /// <summary>
        /// 加密字符串,用DES加密算法
        /// </summary>
        /// <param name="secretKey">密钥</param>
        /// <param name="EString">明文</param>
        /// <returns></returns>
        public static string EncryptString( string secretKey, string EString )
        {
            Des myEncrypt = new Des( secretKey, secretKey );
            return myEncrypt.EncryptString( EString, "DES" );
        }

        /// <summary>
        /// 解密字符串,用DES解密算法
        /// </summary>
        /// <param name="secretKey">指定密钥</param>
        /// <param name="DString">密文</param>
        /// <returns></returns>
        public static string DecryptString( string secretKey, string DString )
        {
            Des myEncrypt = new Des( secretKey, secretKey );

            return myEncrypt.DecryptString( DString, "DES" );
        }
        public static string DecryptString_lv( string DString, string secretKey, string secretIv)
        {
            Des myEncrypt = new Des(secretKey, secretIv);

            return myEncrypt.DecryptString(DString, "DES");
        }
        #endregion
        #region 积分商城加密
        public static string Des3Encrypt(string data, string key, string iv)
        {
            string result = string.Empty;
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider
                {
                    Key = Encoding.ASCII.GetBytes(key),
                    IV = Encoding.ASCII.GetBytes(iv),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform desEncrypt = des.CreateEncryptor();
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                result = Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception ex)
            {
              
            }
            return result;
        }

        #endregion
    }//class end
}

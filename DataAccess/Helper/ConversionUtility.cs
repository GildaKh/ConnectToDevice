using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace DeviceServer.Helper
{
    public static class ConversionUtility
    {
        public static byte[] Str2Bcd(String s, bool padLeft, byte[] d, int offset)
        {
            int len = s.Length;
            d = new byte[(len + 1) >> 1];
            int start = (((len & 1) == 1) && padLeft) ? 1 : 0;
            for (int i = start; i < len + start; i++)
            {
                d[offset + (i >> 1)] |= (byte)((GetDigit((byte)s[i - start])) << ((i & 1) == 1 ? 0 : 4));
            }
            return d;
        }
        public static byte[] Str2Bcd(String s, bool padLeft)
        {
            int len = s.Length;
            byte[] d = new byte[(len + 1) >> 1];
            return Str2Bcd(s, padLeft, d, 0);
        }
        public static string BcdToString(byte[] b, int offset, int len, bool padLeft)
        {
            StringBuilder d = new StringBuilder(len);
            int shift;
            char c;
            int start = (((len & 1) == 1) && padLeft) ? 1 : 0;
            for (int i = start; i < len + start; i++)
            {
                shift = ((i & 1) == 1 ? 0 : 4);
                c = (char)(((b[offset + (i >> 1)] >> shift) & 0x0F) + '0');
                if (c == 'd')
                    c = '=';
                d.Append(char.ToUpper(c));
            }
            return d.ToString();
        }
        public static byte GetDigit(byte b)
        {
            if ((b >= '0' && b <= '9') || b == '=')
                return (byte)(b - (byte)'0');
            if (b >= 'A' && b <= 'F')
                return (byte)(10 + b - 'A');
            return 0;
        }
        public static byte[] CalculateMac(byte[] cryptedString, byte[] key)
        {
            byte[] buffer = new byte[8];
            byte[] buffer2;

            if (key == null || cryptedString == null)
                return null;

            try
            {
                buffer2 = DesDecrypt(cryptedString, key);
                Array.Copy(buffer2, (buffer2.Length / 8 - 1) * 8, buffer, 0, 8);

                return buffer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static byte[] DesEncrypt(byte[] cryptedString, byte[] key)
        {
            byte[] b;
            if (key.Length == 8)
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider() { Padding = PaddingMode.Zeros };
                b = cryptoProvider.CreateEncryptor(key, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }).TransformFinalBlock(cryptedString, 0, cryptedString.Length);
            }
            else
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider() { Padding = PaddingMode.Zeros };
                b = cryptoProvider.CreateEncryptor(key, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }).TransformFinalBlock(cryptedString, 0, cryptedString.Length);
            }
            return b;
        }
        public static byte[] DesDecrypt(byte[] cryptedString, byte[] key)
        {
            //DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider() { Padding = PaddingMode.Zeros};
            byte[] b;
            if (key.Length == 8)
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider() { Padding = PaddingMode.Zeros };
                b = cryptoProvider.CreateDecryptor(key, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }).TransformFinalBlock(cryptedString, 0, cryptedString.Length);
            }
            else
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider() { Padding = PaddingMode.Zeros };
                b = cryptoProvider.CreateDecryptor(key, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }).TransformFinalBlock(cryptedString, 0, cryptedString.Length);
            }
            return b;

        }
        public static T Deserialize<T>(byte[] param)
        {
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                return (T)br.Deserialize(ms);
            }
        }
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        public static string Sha256(string data)
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));
            return crypto.Aggregate(hash, (current, theByte) => current + theByte.ToString("x2"));
        }
        public static byte[] StringToByteWithLen(int len, string data)
        {
            var tmp = new byte[10000];
            byte[] pp = Encoding.GetEncoding(1256).GetBytes(data);                        //data
            byte[] output = new byte[pp.Length + len];                                      //output
            byte[] lenByte = Encoding.ASCII.GetBytes(pp.Length.ToString());       //len data
            var startAt = len - lenByte.Length;
            Buffer.BlockCopy(lenByte, 0, output, startAt, lenByte.Length);                            //copy len data to beginning of output
            Buffer.BlockCopy(pp, 0, output, len, pp.Length);

            return output;
        }
        public static int ByteToInt(byte[] data)
        {
            if (data == null)
                return 0;
            try
            {
                int length = 0;
                var len = Encoding.GetEncoding(1256).GetString(data).Replace("\0", "");
                int.TryParse(len, out length);

                return length;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static byte[] IntToByte(int len, int data)
        {
            byte[] pp = Encoding.GetEncoding(1256).GetBytes(data.ToString());  
            byte[] output = new byte[len];                                      //output
            var startAt = len - pp.Length;
            Buffer.BlockCopy(pp, 0, output, startAt, pp.Length);                //copy len data to beginning of output
            return output;
        }
    }
}

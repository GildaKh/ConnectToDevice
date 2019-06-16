using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceServer.Helper;
using Utility;

namespace DeviceServer.Model
{
    public class PersonInfo
    {
        internal Int64 Id { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Code { get; set; }
        internal DateTime CreateTime { get; set; }
        private long GetPersonInfo()
        {
            try
            {
                if (ConfigUtility.ConnectionString == null)
                    return -1;
                using (var helper = new SqlHelper(ConfigUtility.ConnectionString))
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Name", Name),
                        new SqlParameter("@FamilyName", FamilyName),
                        new SqlParameter("@UserName", UserName),
                        new SqlParameter("@Password", Password),
                        new SqlParameter("@Code", Code)
                    };

                    var foundedData = helper.FetchDataTable("sp_GetPersonInfo", parameters, CommandType.StoredProcedure);
                    if (foundedData != null)
                    {
                        long id = 0;
                        long.TryParse(foundedData.Rows[0]["Id"].ToString(), out id);
                        return id;
                    }
                    return -1;
                }
            }
             catch (Exception ex)
            {
                return -1;
            }
        }
        internal bool Unpack(byte[] rawMsg)
        {

            try
            {
                byte[] lenByte = new byte[2];
                int i = 0;
                Name = unPackField(rawMsg, 2, i, out i);
                FamilyName = unPackField(rawMsg, 2, i, out i);
                UserName = unPackField(rawMsg, 2, i, out i);
                Password = unPackField(rawMsg, 2, i, out i);
                var tmpCode = unPackFieldBCD(rawMsg, 2, i, out i);
                int code = 0;
                int.TryParse(tmpCode, out code);
                Code = code;

                var destArr = new byte[1000];
                Array.Copy(rawMsg, 0, destArr, 0, i);
                var b = ConversionUtility.CalculateMac(destArr, Keys.KeyMac);
                var mac = new byte[8];
                Array.Copy(rawMsg, i, mac, 0, 8);

                for (int j = 0; j < 8; j++)
                {
                    if (mac[j] != b[j])
                        return false;
                }
                Array.Copy(b, 0, destArr, i, b.Length);
                i += b.Length;

                Id = GetPersonInfo();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string unPackField(byte[] rawMsg, int msgLen, int start, out int end)
        {
            byte[] lenByte = new byte[msgLen];
            end = start;
            Array.Copy(rawMsg, start, lenByte, 0, msgLen);
            int length = ConversionUtility.ByteToInt(lenByte);

            byte[] tmp = new byte[length];
            end += msgLen;
            Array.Copy(rawMsg, msgLen + start, tmp, 0, length);
            end += length;
            return Encoding.GetEncoding(1256).GetString(tmp);
        }
        private string unPackFieldBCD(byte[] rawMsg, int msgLen, int start, out int end)
        {
            byte[] lenByte = new byte[msgLen];
            end = start;
            Array.Copy(rawMsg, start, lenByte, 0, msgLen);
            int length = ConversionUtility.ByteToInt(lenByte);

            byte[] tmp = new byte[length];
            end += msgLen;
            Array.Copy(rawMsg, msgLen + start, tmp, 0, length);
            end += length;
            return ConversionUtility.BcdToString(tmp, 0, length*2, false);
        }
    }
}

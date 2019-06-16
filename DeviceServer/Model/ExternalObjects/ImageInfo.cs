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
    public class ImageInfo
    {
        public ImageInfo()
        {
            IsSuccessful = false;
        }
        internal Int64 Id { get; set; }
        public Image Image { get; set; }
        internal byte[] ImageByte { get; set; }
        internal PersonInfo PersonInfo { get; set; }
        public string Description { get; set; }
        internal bool IsSuccessful { get; set; }

        public ImageInfo GetImage(Int64 personId)
        {
            try
            {
                using (var helper = new SqlHelper(ConfigUtility.ConnectionString))
                {
                    var parameters = new List<SqlParameter> {new SqlParameter("@PersonInfoId", personId)};

                    var foundedData = helper.FetchDataTable("sp_GetImageInfo", parameters, CommandType.StoredProcedure);
                    if (foundedData != null)
                    {
                        var imageByte = foundedData.Rows[0]["Image"] as byte[];
                        Image img = null;
                        if (imageByte != null)
                            using (var ms = new MemoryStream(imageByte))
                            {
                                img = Image.FromStream(ms);
                            }

                        ImageInfo image = new ImageInfo()
                        {
                            Image = img,
                            IsSuccessful = true,
                            Description = foundedData.Rows[0]["Description"].ToString()

                        };
                        image.ImageByte = new byte[imageByte.Length];
                        Array.Copy(imageByte, image.ImageByte, imageByte.Length);
                        return image;
                    }
                    return new ImageInfo()
                    {
                        Description = "Information provided is not valid",
                    };
                }
            }
            catch (Exception ex)
            {
                return new ImageInfo()
                {
                    Description = "Exception: "+ ex.Message
                };
            }
        }
    }
}

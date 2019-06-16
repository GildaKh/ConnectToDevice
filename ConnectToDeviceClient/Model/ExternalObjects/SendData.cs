using ConnectToDevice.Infrastructure.Helper;
using ConnectToDevice.Model.Channel;
using ConnectToDevice.Model.Interfaces;
using ConnectToDevice.Model.Types;
using DeviceServer.Helper;
using System;
using System.IO;

namespace ConnectToDevice.Model
{
    public class SendData : IPersonnel
    {
        #region Constructor
        public SendData(string name, string fName, string userName, string pass, int code)
        {
            Name = new LLCHAR(10, name);
            FamilyName = new LLCHAR(10, fName);
            UserName = new LLCHAR(10, userName);
            Password = new ILPASSWORD(10, pass);
            Code = new ILNUMBER(10, code);
        }
        #endregion

        #region Properties
        internal LLCHAR Name { get; set; }
        internal LLCHAR FamilyName { get; set; }
        internal LLCHAR UserName { get; set; }
        internal ILPASSWORD Password { get; set; }
        internal ILNUMBER Code { get; set; }
        #endregion

        #region Interface Implementation
        LLCHAR IPersonnel.Name
        {
            get
            {
                return Name;
            }
        }

        LLCHAR IPersonnel.FamilyName
        {
            get
            {
                return FamilyName;
            }
        }

        LLCHAR IPersonnel.UserName
        {
            get
            {
                return UserName;
            }
        }

        ILPASSWORD IPersonnel.Password
        {
            get
            {
                return Password;
            }
        }

        ILNUMBER IPersonnel.Code
        {
            get
            {
                return Code;
            }
        }
        byte[] IPersonnel.Pack()
        {
            return Pack();
        }
        #endregion

        #region Methods
        internal byte[] Pack()
        {
            // foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            // {
            //     propertyInfo.PropertyType.GetMethod("Pack").Invoke(propertyInfo.GetValue(this), null);
            // }

            byte[] destArr = new byte[2000];
            var b = Name.Pack(this.Name.Value);
            var i = b.Length;
            Array.Copy(b, destArr, i);

            b = FamilyName.Pack(this.FamilyName.Value);
            Array.Copy(b, 0, destArr, i, b.Length);
            i += b.Length;

            b = UserName.Pack(this.UserName.Value);
            Array.Copy(b, 0, destArr, i, b.Length);
            i += b.Length;

            b = Password.Pack(this.Password.Value);
            Array.Copy(b, 0, destArr, i, b.Length);
            i += b.Length;

            b = Code.Pack(this.Code.Value.ToString());
            Array.Copy(b, 0, destArr, i, b.Length);
            i += b.Length;

            b = ConversionUtility.CalculateMac(destArr, Keys.KeyMac);
            Array.Copy(b, 0, destArr, i, b.Length);
            i += b.Length;

            var tmp = new byte[i];
            Array.Copy(destArr, tmp, i);
            return tmp;
        }
       
        #endregion
    }
}

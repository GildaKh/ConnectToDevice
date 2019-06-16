using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeviceServer.Model;

namespace DeviceSimulation
{
    public partial class FrmServer : Form
    {
        #region Properties

        private ReceiveDataManagement _receiveDataManagement;
        ImageInfo _imageInfo;

        #endregion
        public FrmServer()
        {
            InitializeComponent();
            txtIP.Text = "192.168.1.4";
            txtPort.Text = "1197";
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            if (_receiveDataManagement == null)
            {
                _receiveDataManagement = new ReceiveDataManagement();
                _receiveDataManagement.ResultReceived += _receiveDataManagement_ResultReceived;
                _receiveDataManagement.ImageFound += _receiveDataManagement_ImageFound; 
            }
            int port = 0;
            Int32.TryParse(txtPort.Text, out port);
            if (string.IsNullOrEmpty(txtPort.Text) || port <= 0)
            {
                MessageBox.Show("Define a valid port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtIP.Text))
            {
                MessageBox.Show("Define a valid IP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _receiveDataManagement.Initialization(txtIP.Text, port);
            txtStatus.Text = "Start Listenin. Waiting for client.....";
            picUnsigned.Image = null;
            txtDescription.Text = string.Empty;
            txtName.Text = string.Empty; 
            txtFName.Text = string.Empty; 
            txtUserName.Text = string.Empty;
            txtCode.Text = string.Empty; 
        }

        private void _receiveDataManagement_ImageFound(ImageInfo image)
        {
            _imageInfo = image;
            if (image == null)
            {
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(() =>
                    {
                        txtStatus.Text = "No Image Found";
                    }));
                else
                {
                    txtStatus.Text = "No Image Found";
                }
            }
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(() =>
                {
                    txtStatus.Text = "Image Found.";
                    txtDescription.Text = image.Description;
                    picUnsigned.Image = image.Image;
                }));
            else
            {
                txtStatus.Text = "Image Found.";
                txtDescription.Text = image.Description;
                picUnsigned.Image = image.Image;
            }

        }

        void _receiveDataManagement_ResultReceived(PersonInfo person)
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(() =>
                {
                    txtStatus.Text = "Information Received.";
                    txtFName.Text = person.FamilyName;
                    txtName.Text = person.Name;
                    txtUserName.Text = person.UserName;
                    txtCode.Text = person.Code.ToString();
                }));
            else
            {
                txtStatus.Text = "Information Received.";
                txtFName.Text = person.FamilyName;
                txtName.Text = person.Name;
                txtUserName.Text = person.UserName;
                txtCode.Text = person.Code.ToString();
            }          
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(_receiveDataManagement == null)
            {
                txtStatus.Text = "First Start Listening";
                return;
            }
            txtStatus.Text = "Sendind data.....";
            _receiveDataManagement.Send(_imageInfo);

        }
    }
}

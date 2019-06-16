using ConnectToDevice.Model;
using ConnectToDevice.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectToDevice.Infrastructure.Helper;

namespace ConnectToDeviceTestApp
{
    public partial class FrmConnecToDevice : Form
    {
        #region Properties
        ConnectToDeviceApi _connectToDevice;
        private AsyncType _asyncType;
        private ConnectionLink _connectionLink;
        #endregion
        public FrmConnecToDevice()
        {
            InitializeComponent();
            InitializeForm();
            LoadComPorts();
        }
        private void InitializeForm()
        {
            txtName.Text = "Gilda";
            txtFName.Text = "Khosravi";
            txtUserName.Text = "Gilda_kh";
            txtPassword.Text = "123456";
            txtCode.Text = "1230";
            txtBaudRate.Text = "19200";
            txtIP.Text = "192.168.1.4";
            txtPort.Text = "1197";
            LoadComPorts();

            cmbConnectionLink.DataSource = Enum.GetValues(typeof(ConnectionLink))
                                            .Cast<ConnectionLink>()
                                            .Select(v => v.ToString())
                                            .ToList();
            cmbConnectionLink.SelectedIndex = 0;

            cmbAsyncType.DataSource = Enum.GetValues(typeof(AsyncType))
                                            .Cast<AsyncType>()
                                            .Select(v => v.ToString())
                                            .ToList();
            cmbAsyncType.SelectedIndex = 0;

            if (_connectToDevice == null)
                _connectToDevice = new ConnectToDeviceApi();

            //Assign Events
            _connectToDevice.TransactionCompleted += ConnectToDeviceOnTransactionCompleted;

            txtStatus.Text = "Ready.";

        }
        private void LoadComPorts()
        {
            string defaultPort = "COM7";

            cmbComPort.DataSource = System.IO.Ports.SerialPort.GetPortNames();

            if (((string[])cmbComPort.DataSource).Any())
                cmbComPort.SelectedIndex = 0;

            if (((string[])cmbComPort.DataSource).Any(s => s == defaultPort))
                cmbComPort.SelectedItem = defaultPort;
        }

        private void ConnectToDeviceOnTransactionCompleted(ReceivedData receivedData)
        {
            DataReceivedCompletely(receivedData);
        }

        private void DataReceivedCompletely(ReceivedData receivedData)
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(() =>
                {
                    txtStatus.Text = "Transaction Completed.";
                    if (receivedData != null)
                    {
                        txtDescrition.Text = receivedData.Description;
                        picReceived.Image = receivedData.Image;
                    }
                }));
            else
            {
                txtStatus.Text = "Transaction Completed.";
                if (receivedData != null)
                {
                    txtDescrition.Text = receivedData.Description;
                    picReceived.Image = receivedData.Image;
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtDescrition.Text = string.Empty;
            //txtBaudRate.Text = string.Empty;
            //txtCode.Text = string.Empty;
            //txtFName.Text = string.Empty;
            //txtName.Text = string.Empty;
            //txtPassword.Text = string.Empty;
            //txtStatus.Text = string.Empty;
            //txtUserName.Text = string.Empty;
            picReceived.Image = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCode.Text) || string.IsNullOrEmpty(txtUserName.Text) ||
               string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtName.Text) ||
               string.IsNullOrEmpty(txtFName.Text))
            {
                txtStatus.Text = "Please Fill All Fields";
                return;
            }
            if(_connectToDevice == null)
            {
                txtStatus.Text = "Please try Again";
                return;
            }
            if (_connectionLink == ConnectionLink.Com)
            {
                SerialPort selectedPort = null;
                int baudRate = 0;
                Int32.TryParse(txtBaudRate.Text, out baudRate);
                if (string.IsNullOrEmpty(txtBaudRate.Text) || baudRate<=0)
                {
                    MessageBox.Show("Define a valid BaudRate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (SerialPort.GetPortNames().Any(p => p == cmbComPort.SelectedItem.ToString()))
                    selectedPort = new SerialPort((string)cmbComPort.SelectedItem);
                if (selectedPort == null)
                {
                    MessageBox.Show("There is no selected Port in configurations.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                string port = selectedPort.PortName.Replace("COM", "");
                _connectToDevice.Initialization(_asyncType, int.Parse(port), baudRate);

            }
            if (_connectionLink == ConnectionLink.Lan)
            {
                if (string.IsNullOrEmpty(txtIP.Text))
                {
                    MessageBox.Show("There is no value for IP in configurations.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                 int port = 0;
                Int32.TryParse(txtPort.Text, out port);
                if (string.IsNullOrEmpty(txtPort.Text) || port<=0)
                {
                    MessageBox.Show("Define a valid Port.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                _connectToDevice.Initialization(_asyncType, txtIP.Text, port);

            }
            int code = 0;
            int.TryParse(txtCode.Text, out code);
            SendData data = new SendData(txtName.Text, txtFName.Text, txtUserName.Text, txtPassword.Text, code);
            var receivedData = _connectToDevice.Send(data);
            if (_asyncType == AsyncType.Sync)
            {
                DataReceivedCompletely(receivedData);                
            }
        }

        private void cmbAsyncType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem.ToString() == AsyncType.Async.ToString())
                _asyncType = AsyncType.Async;
            else if (comboBox != null && comboBox.SelectedItem.ToString() == AsyncType.Sync.ToString())
                _asyncType = AsyncType.Sync;
        }

        private void cmbConnectionLink_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem.ToString() == ConnectionLink.Com.ToString())
            {
                _connectionLink = ConnectionLink.Com;
                cmbComPort.Enabled = true;
                btnRefresh.Enabled = true;
                txtIP.Enabled = false;
                txtPort.Enabled = false;
                txtBaudRate.Enabled = true;
            }
            else if (comboBox != null && comboBox.SelectedItem.ToString() == ConnectionLink.Lan.ToString())
            {
                _connectionLink = ConnectionLink.Lan;
                cmbComPort.Enabled = false;
                btnRefresh.Enabled = false;
                txtIP.Enabled = true;
                txtPort.Enabled = true;
                txtBaudRate.Enabled = false;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadComPorts();
        }
    }
}

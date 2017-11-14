using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Torrent_Parser
{
    public partial class SettingsForm : Form
    {
        CConfigMng oConfigMng;
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            button1_Click(sender, e);
        }

        private void useRemoteCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Enabled = useRemoteCheckBox.Checked;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            oConfigMng = ((SearchForm)this.Owner).oConfigMng;
            oConfigMng.LoadConfig();

            typeComboBox.Text = oConfigMng.Config.RemoteType;
            useRemoteCheckBox.Checked = oConfigMng.Config.RemoteEnabled;
            serverTextBox.Text = oConfigMng.Config.RemoteServer;
            portTextBox.Text = oConfigMng.Config.RemotePort.ToString();
            usernameTextBox.Text = oConfigMng.Config.RemoteUsername;
            passwordTextBox.Text = oConfigMng.Config.RemotePassword;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            oConfigMng.Config.RemoteType = typeComboBox.Text;
            oConfigMng.Config.RemoteEnabled= useRemoteCheckBox.Checked;
            oConfigMng.Config.RemoteServer= serverTextBox.Text;
            oConfigMng.Config.RemotePort = Convert.ToInt32(portTextBox.Text);
            oConfigMng.Config.RemoteUsername= usernameTextBox.Text;
            oConfigMng.Config.RemotePassword=passwordTextBox.Text;
            oConfigMng.SaveConfig();
            this.DialogResult = DialogResult.OK;
        }
        
    }
}

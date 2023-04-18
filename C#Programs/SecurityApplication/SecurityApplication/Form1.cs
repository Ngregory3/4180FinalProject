using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace SecurityApplication
{
    public partial class FormSecurityApp : Form
    {
        //Set up text variables
        private bool initComplete = false;
        private int brightnessValue = 0;
        private int volumeValue = 0;

        public FormSecurityApp()
        {
            InitializeComponent();
        }

        private void buttonDisarm_Click(object sender, EventArgs e)
        {
            if (initComplete)
            {
                bool result = Program.sendMessage(Program.disarmMessage);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            //Set server IP using text box
            Program.ipServer = textBoxIPAddress.Text;
            bool result = Program.openConnection();
            if (!result)
            {
                labelConnectionStatus.Text = "Not Connected";
                labelErrorMessage.Text = Program.connectionErrorMessage;
            }
            else
            {
                labelConnectionStatus.Text = "Connected";
                labelErrorMessage.Text = "";
                initComplete = true;
            }
        }

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            if (initComplete)
            {
                bool result = Program.sendMessage(Program.activateToggleMessage);
            }
        }

        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
            if (initComplete)
            {
                brightnessValue = trackBarBrightness.Value;
            }
        }

        private void trackBarBrightness_MouseUp(object sender, EventArgs e)
        {
            if (initComplete)
            {
                String message = Program.brightnessMessage + brightnessValue.ToString();
                bool result = Program.sendMessage(message);
            }
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            if (initComplete)
            {
                volumeValue = trackBarVolume.Value;
            }
        }

        private void trackBarVolume_MouseUp(object sender, EventArgs e)
        {
            if (initComplete)
            {
                String message = Program.volumeMessage + volumeValue.ToString();
                bool result = Program.sendMessage(message);
            }
        }
    }
}

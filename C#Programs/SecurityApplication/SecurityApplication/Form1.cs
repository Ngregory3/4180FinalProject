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
using System.Threading;

namespace SecurityApplication
{
    public partial class FormSecurityApp : Form
    {
        //Set up text variables
        private bool initComplete = false;
        private int brightnessValue = 0;
        private int volumeValue = 0;
        private static Mutex mutex = new Mutex();

        public FormSecurityApp()
        {
            InitializeComponent();
        }

        private void setupOnConnect()
        {
            //Check if triggered
            Console.WriteLine("Check Triggered");
            String result;
            result = Program.requestMessage(Program.requestTriggeredMessage);
            if (result.Equals("#T11"))
            {
                changeStatus(alarmState.TRIGGERED);
                checkBoxActive.Checked = true;
            } else
            {
                //Set if active
                Console.WriteLine("Check Armed");
                result = Program.requestMessage(Program.requestActiveMessage);
                Console.WriteLine("Result: {0}", result);
                if (result.Equals("#A11"))
                {
                    //Armed
                    Console.WriteLine("Armed");
                    changeStatus(alarmState.ARMED);
                    checkBoxActive.Checked = true;
                }
            }

            Console.WriteLine("Check Volume");
            result = Program.requestMessage(Program.requestVolumeMessage);
            Console.WriteLine("Result: {0}", result);
            if (result.Equals("#M00"))
            {
                //Volume on
                trackBarVolume.Value = 1;
            }

            Console.WriteLine("Check Brightness");
            result = Program.requestMessage(Program.requestBrightnessMessage);
            Console.WriteLine("Result: {0}", result);
            Console.WriteLine("Brightness: {0}", result);
            int brightnessValue = Int32.Parse((result[2]).ToString());
            trackBarBrightness.Value = brightnessValue;
        }

        private void changeStatus(alarmState newState)
        {
            Color c;
            switch (newState)
            {
                case alarmState.ARMED:
                    c = Color.Blue;
                    break;
                case alarmState.DISARMED:
                    c = Color.Green;
                    break;
                case alarmState.TRIGGERED:
                    c = Color.Red;
                    break;
                default:
                    c = Color.Gray;
                    break;
            }
            mutex.WaitOne();
            textBoxAlarmStatus.Text = newState.ToString();
            textBoxAlarmStatus.BackColor = c;
            mutex.ReleaseMutex();
        }

        private void buttonDisarm_Click(object sender, EventArgs e)
        {
            if (initComplete)
            {
                bool result = Program.sendMessage(Program.disarmMessage);
                //triggerFlag = 1;
                //Thread.Sleep(1000);
                //Program.clearBuffer();
                if (checkBoxActive.Checked)
                {
                    Console.WriteLine("Armed changed");
                    changeStatus(alarmState.ARMED);
                }
                else
                {
                    Console.WriteLine("Disarmed Changed");
                    changeStatus(alarmState.DISARMED);
                }
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
                setupOnConnect();
                Program.startTriggerThread();
                initComplete = true;
            }
        }

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            if (initComplete)
            {
                if (checkBoxActive.Checked)
                {
                    Console.WriteLine("Armed");
                    String message = Program.activateToggleMessage + "1";
                    Program.sendMessage(message);
                    changeStatus(alarmState.ARMED);
                } 
                else if (!checkBoxActive.Checked)
                {
                    Console.WriteLine("Disarmed");
                    String message = Program.activateToggleMessage + "0";
                    Program.sendMessage(message);
                    changeStatus(alarmState.DISARMED);

                }
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

        public void alarmTriggered()
        {
            if (initComplete)
            {
                Console.WriteLine("Set to triggered");
                changeStatus(alarmState.TRIGGERED);
            }
        }

        public void connectionFailed()
        {
            labelConnectionStatus.Text = "Not Connected";
            labelErrorMessage.Text = Program.connectionErrorMessage;
            initComplete = false;
        }

        private void textBoxAlarmStatus_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Program.closeConnection();
            labelConnectionStatus.Text = "Not Connected";
            labelErrorMessage.Text = "Disconnected from Device";
            initComplete = false;
        }
    }
}

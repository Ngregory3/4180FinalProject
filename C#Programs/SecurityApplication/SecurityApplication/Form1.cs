using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace SecurityApplication
{
    public partial class FormSecurityApp : Form
    {
        private bool activeConnection = false;
        private int brightnessValue = 0;
        private int volumeValue = 0;
        private static Mutex alarmStatusMutex = new Mutex();

        public FormSecurityApp()
        {
            InitializeComponent();
        }

        //Called to update the status of the GUI to match the alarm system once a TCP connection has been established
        private void setupOnConnect()
        {
            //Check the current status of the alarm system, and update the GUI accordingly
            Console.WriteLine("Check Triggered");
            String result;
            result = Program.requestMessage(Program.requestTriggeredMessage);
            if (result.Equals("#T11"))
            {
                //Alarm is triggered
                changeStatus(alarmState.TRIGGERED);
                checkBoxActive.Checked = true;
            } else
            {
                Console.WriteLine("Check Armed");
                result = Program.requestMessage(Program.requestActiveMessage);
                Console.WriteLine("Result: {0}", result);
                if (result.Equals("#A11"))
                {
                    //Alarm is armed
                    Console.WriteLine("Armed");
                    changeStatus(alarmState.ARMED);
                    checkBoxActive.Checked = true;
                }
            }
            //Alarm is disarmed if neither triggered or armed

            Console.WriteLine("Check Volume");
            result = Program.requestMessage(Program.requestVolumeMessage);
            Console.WriteLine("Result: {0}", result);
            if (result.Equals("#M00"))
            {
                //Volume is on
                trackBarVolume.Value = 1;
            }

            Console.WriteLine("Check Brightness");
            result = Program.requestMessage(Program.requestBrightnessMessage);
            Console.WriteLine("Result: {0}", result);
            Console.WriteLine("Brightness: {0}", result);
            //Set brightness value to PWM output for LED
            int brightnessValue = Int32.Parse((result[2]).ToString());
            trackBarBrightness.Value = brightnessValue;
        }

        //Called to change the alarm status text box
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
            alarmStatusMutex.WaitOne();
            textBoxAlarmStatus.Text = newState.ToString();
            textBoxAlarmStatus.BackColor = c;
            alarmStatusMutex.ReleaseMutex();
        }

        private void buttonDisarm_Click(object sender, EventArgs e)
        {
            if (activeConnection)
            {
                bool result = Program.sendMessage(Program.disarmMessage);
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
            bool result = Program.openConnection(); //Creates a TCP connection to the device
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
                activeConnection = true;
            }
        }

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            if (activeConnection)
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

        //Keeps track of the change in values for brightness of the LED
        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
            if (activeConnection)
            {
                brightnessValue = trackBarBrightness.Value;
            }
        }

        //Only sends updated brightness value to LED when the user lets go of the track bar 
        private void trackBarBrightness_MouseUp(object sender, EventArgs e)
        {
            if (activeConnection)
            {
                String message = Program.brightnessMessage + brightnessValue.ToString();
                bool result = Program.sendMessage(message);
            }
        }

        //Keeps track of the change in volume of the speaker
        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            if (activeConnection)
            {
                volumeValue = trackBarVolume.Value;
            }
        }

        //Only sends updated volume value to the speaker when the user lets go of the track bar
        private void trackBarVolume_MouseUp(object sender, EventArgs e)
        {
            if (activeConnection)
            {
                String message = Program.volumeMessage + volumeValue.ToString();
                bool result = Program.sendMessage(message);
            }
        }

        //Called by Program.cs when a trigger is detected by the alarm system
        public void alarmTriggered()
        {
            if (activeConnection)
            {
                Console.WriteLine("Set to triggered");
                changeStatus(alarmState.TRIGGERED);
            }
        }

        public void connectionFailed()
        {
            labelConnectionStatus.Text = "Not Connected";
            labelErrorMessage.Text = Program.connectionErrorMessage;
            activeConnection = false;
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Program.closeConnection();
            labelConnectionStatus.Text = "Not Connected";
            labelErrorMessage.Text = "Disconnected from Device";
            activeConnection = false;
        }
    }
}

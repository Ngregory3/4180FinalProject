using System;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;

namespace SecurityApplication
{
    public enum alarmState {
        ARMED,
        DISARMED,
        TRIGGERED
    }

    public class Program
    {
        //TCP Connection
        public static String ipServer = "";
        public static Int32 port = 61000;
        public static TcpClient client;
        public static NetworkStream stream;
        public static String connectionErrorMessage = "";
        private static bool connectionOpened = false;

        //Message Headers
        public static String ackMessage= "!CCC";
        public static String testMessage = "!XXX";
        public static String activateToggleMessage = "!A";
        public static String disarmMessage = "!D";
        public static String brightnessMessage = "!B";
        public static String volumeMessage = "!M";
        public static String triggeredMessage = "!!!!";

        //Message Request Headers
        public static String requestActiveMessage = "#A";
        public static String requestBrightnessMessage = "#B";
        public static String requestVolumeMessage = "#M";
        public static String requestTriggeredMessage = "#T";

        //Message Buffer
        private static String buffer = "XX";

        private static FormSecurityApp guiForm;

        private static Thread triggeredThread;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormSecurityApp form = new FormSecurityApp();
            guiForm = form;
            Application.Run(form);
        }

        public static bool openConnection()
        {
            try {
                client = new TcpClient(ipServer, port);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(testMessage);
                stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                data = new Byte[256];
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                if (!responseData.Equals(ackMessage))
                {
                    Console.WriteLine("Incorrect data sent back: {0}", responseData);
                }

                Console.WriteLine("Successful connection opened");
                connectionErrorMessage = "";
                connectionOpened = true;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                //Issue with IP address
                Console.WriteLine("ArgumentNullException: {0}", ex);
                connectionErrorMessage = ex.Message;
                connectionOpened = false;
                return false;
            }
            catch (SocketException ex)
            {
                //Issue with socket connection
                Console.WriteLine("SocketException: {0}", ex);
                connectionErrorMessage = ex.Message;
                connectionOpened = false;
                return false;
            }
        }

        public static void startTriggerThread()
        {
            triggeredThread = new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                checkTriggered();
            });
            triggeredThread.Start();
        }

        public static bool closeConnection()
        {
            try
            {
                stream.Close();
                client.Close();
                triggeredThread.Abort();
                connectionOpened = false;
                return true;
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
                return false;
            }
        }

        public static bool sendMessage(String message)
        {
            if (connectionOpened)
            {
                String messageWithBuffer = message + buffer;
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(messageWithBuffer);
                stream.Write(data, 0, data.Length);

                return true;
            } else
            {
                connectionErrorMessage = "There is no connection open";
                Console.WriteLine("No Connection");
                return false;
            }
        }

        public static String requestMessage(String message)
        {
            if (connectionOpened)
            {
                //Send request message
                String messageWithBuffer = message + buffer;
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(messageWithBuffer);
                stream.Write(data, 0, data.Length);

                //Recieve response message
                data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                return responseData;
            }
            else
            {
                connectionErrorMessage = "There is no connection open";
                Console.WriteLine("No Connection");
                return "";
            }
        }

        //Thread function created to check if alarm has been triggered
        public static bool checkTriggered()
        {
            Console.WriteLine("Running check triggered");
            Byte[] data = new Byte[256];
            int i;
            String input;
            while (true)
            {
                try
                {
                    while ((i = stream.Read(data, 0, data.Length)) != 0)
                    {
                        input = System.Text.Encoding.ASCII.GetString(data, 0, i);
                        if (input.Contains(triggeredMessage))
                        {
                            //Change status on GUI
                            Console.WriteLine("Received Triggered Status");
                            guiForm.Invoke(new MethodInvoker(guiForm.alarmTriggered));
                        }
                        Thread.Sleep(500);
                    }
                } catch (Exception e)
                {
                    //If problem with connection, retry on next loop
                }
                Thread.Sleep(1000); //Run no more than once per second
            }
        }
    }
}

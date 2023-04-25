using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SecurityApplication
{

    public enum alarmState {
        ARMED,
        DISARMED,
        TRIGGERED
    }

    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static event EventHandler AlarmTriggered;

        public static String ipServer = "";
        public static Int32 port = 61000;
        public static TcpClient client;
        public static NetworkStream stream;
        public static String connectionErrorMessage = "";

        public static String ackMessage= "!CCC";
        public static String testMessage = "!XXX";
        public static String activateToggleMessage = "!A";
        public static String disarmMessage = "!D";
        public static String brightnessMessage = "!B";
        public static String volumeMessage = "!M";
        public static String triggeredMessage = "!!!!";

        public static String requestActiveMessage = "#A";
        public static String requestBrightnessMessage = "#B";
        public static String requestVolumeMessage = "#M";
        public static String requestTriggeredMessage = "#T";

        private static String buffer = "XX";
        private static bool connectionOpened = false;

        private static FormSecurityApp guiForm;

        private static Thread triggeredThread;
        private static Thread canaryThread;

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
                // Prefer a using declaration to ensure the instance is Disposed later.
                client = new TcpClient(ipServer, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(testMessage);

                // Get a client stream for reading and writing.
                stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                // Receive the server response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                if (!responseData.Equals(ackMessage))
                {
                    Console.WriteLine("Incorrect data sent back: {0}", responseData);
                }

                // Explicit close is not necessary since TcpClient.Dispose() will be
                // called automatically.
                Console.WriteLine("Successful connection opened");
                connectionErrorMessage = "";
                connectionOpened = true;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("ArgumentNullException: {0}", ex);
                connectionErrorMessage = ex.Message;
                connectionOpened = false;
                return false;
            }
            catch (SocketException ex)
            {
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
            //May need to add acknowledge checks when there is time
            if (connectionOpened)
            {
                String messageWithBuffer = message + buffer;
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(messageWithBuffer);

                // Send the message to the connected TcpServer.
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
            //May need to add acknowledge checks when there is time
            if (connectionOpened)
            {
                String messageWithBuffer = message + buffer;
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(messageWithBuffer);

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
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

        public static void clearBuffer()
        {
            Byte[] data = new Byte[256];
            stream.Read(data, 0, data.Length);
        }

        //Want to set this up as an interrupt basically calling this whenever something is sent
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
                            Console.WriteLine("Received Triggered Status");
                            guiForm.Invoke(new MethodInvoker(guiForm.alarmTriggered));
                        }
                        Thread.Sleep(500);
                    }
                } catch (Exception e)
                {
                    //Do nothing
                }
            }
        }

        public static void checkConnection()
        {
            Console.WriteLine("Running connection Canary");
            while (true)
            {
                Thread.Sleep(10000);
                try
                {
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(testMessage);
                    stream.Write(data, 0, data.Length);

                    // Receive the server response.
                    data = new Byte[256];
                    String responseData = String.Empty;
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

/*                    if (!responseData.Equals(ackMessage))
                    {
                        Console.WriteLine("Connection failed");
                        guiForm.Invoke(new MethodInvoker(guiForm.connectionFailed));
                        closeConnection();
                        return;
                    }*/
                } catch (Exception e)
                {
                    Console.WriteLine("Connection failed {0}", e.Message);
                    guiForm.Invoke(new MethodInvoker(guiForm.connectionFailed));
                    closeConnection();
                    return;
                }
            }
        }
    }
}

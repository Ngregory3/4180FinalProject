using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace SecurityApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static String ipServer = "";
        public static Int32 port = 61000;
        public static TcpClient client;
        public static NetworkStream stream;
        public static String connectionErrorMessage = "";

        public static String ackMessage= "!CCC";
        public static String testMessage = "!XX";
        public static String activateToggleMessage = "!A";
        public static String disarmMessage = "!D";
        public static String brightnessMessage = "!B";
        public static String volumeMessage = "!V";
        public static String triggeredMessage = "!!!!";

        private static String buffer = "XXXXXX";
        private static bool connectionOpened = false;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormSecurityApp());

/*            while (true)
            {
                checkTriggered();
                //Need to add wait here!!
            }*/
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

        public static bool closeConnection()
        {
            try
            {
                stream.Close();
                client.Close();
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
                return false;
            }
        }

        //Want to set this up as an interrupt basically calling this whenever something is sent
        public static bool checkTriggered()
        {
            return false;
        }
    }
}

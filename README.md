# Dorm Security System
## Team Members
William Diamond, Nathaniel Gregory
## Project Description
This project is a wifi enabled Mbed and Raspberry Pi Zero W based dorm security device that can be remotely controlled using a Windows C# desktop application.<br>

[Video Demo](https://www.youtube.com/watch?v=cLvQkAxkBHs)
# Parts List
mbed LPC1768<br>
Raspberry Pi Zero W<br>
Pi Cobbler<br>
Male to male Mini-Micro USB Cable<br>
5V 2A AC adapter with breadboard compatible jack<br>
SparkFun Class D Audio Amp Breakout - TPA2005D1<br>
Speaker<br>
USB-to-TTL Serial Cable<br>
RGB LED<br>
3x330 Ohm Resistor<br>
VL53L0X LIDAR TOF Sensor<br>
# Wiring
<img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/6F6EC5D8-5CD3-403F-A610-D143A668B1D5.jpeg" width="350"><br>

## Class D Audio Amplifier

| TPA2005D1 | mbed | Speaker | External 5V Power |
|-----------|------|---------|-------------------|
| OUT+      |      | +       |                   |
| OUT-      |      | -       |                   |
| PWR+      |      |         | +5V               |
| PWR-      | GND  |         | GND               |
| IN+       | p24  |         |                   |
| IN-       | GND  |         | GND               |

## LIDAR TOF Sensor

Note: A ribbon cable can be used to allow better placement of the LIDAR sensor.

| VL53L0X | mbed       |
|---------|------------|
| VIN     | VOUT +3.3V |
| GND     | GND        |
| SCL     | p27        |
| SDA     | p28        |
| XSHUT   | p26        |
| GPIO    | NC         |

## RGB Warning LED

WARNING: Each Mbed pin should have a 330 Ohm resistor between it and the LED pins.

| RGB LED | mbed |
|---------|------|
| RED     | p21  |
| GND     | GND  |
| GREEN   | p22  |
| BLUE    | p23  |

## Raspberry Pi Zero W

Note: Use the Mini-Micro USB cable to connect the Pi to the mbed.

| Pi Zero W Cobbler | mbed | USB-TTL Cable | External 5V Power |
|-------------------|------|---------------|-------------------|
| 17                | p5   |               |                   |
| TXD               |      | RXD           |                   |
| RXD               |      | TXD           |                   |
| GND               | GND  | GND           | GND               |
| x2 5V             |      |               | 5V                |

# Code
## Mbed Code<br>
The Mbed code takes in input from the LIDAR sensor to determine if an intruder has entered the room. The mbed also controls the RGB LED output and speaker based on the current state of the alarm system and the settings given by the C# application. Finally, the Mbed will send a signal to the Rasbperry Pi when the alarm system is triggered to alert a Node-Red email system. The Mbed is connected to the Rasbperry Pi through a Serial connection, which will send and recieve messages to and from the C# Application.<br>

**MbedSecuritySystem.cpp**<br>
Once compiled and running on the Mbed, the Mbed code will set the speaker period to 500 Hz.<br>
Three threads are created:<br>
* tripWire Thread<br>
  The tripWire Thread tracks if the LIDAR has sensed an object that is between 120 and 600 mm for 3 cycles or longer.
* alarmLED Thread<br>
  The alarmLED Thread changes the RGB LED based on the current state. The brightness (PWM output) of the LED is determined by inputs from the C# Application.
  * Triggered: RED and Flashing (0.2 second period)
  * Armed: BLUE and ON
  * Disarmed: GREEN and ON<br>
  <img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/LEDStatusLights.png" width="350"><br>
* alarmSound Thread<br>
  The alarmSound Thread sets the PWM output of the speaker to 0.5 if triggered, and 0 otherwise. If the system is muted, then the speaker output will always be set to 0.

The main Thread checks the Serial input from the Rasbpery Pi. If the C# application sends a request (#), the Mbed will check the current state of the requested sensor or output and send it back. If a command (!) is recieved by the Mbed, then the device will update the status accordingly (See TCP Command Syntax).
The main thread will also check if the sensor has been triggered and send a message to the client to update the C# applicaton GUI.

## Raspberry Pi Code
The Raspberry Pi Zero W only has two functions in the alarm system: managing the TCP service between the Mbed and C# Application and running the Node-Red service to alert the system user via email.

**PiTCPServer.c**<br>
The main function creates and opens a TCP server for users to connect to on port 61000. Whenever a client requests a connection, the server will accept it and start a thread for it. The thread will begin passing TCP traffic between the client and the Mbed over the USB Virtual Com Port in sets of 4 characters.

**EmailTriggerFlow.json**<br>
This flow will need to be imported and deployed in Node-Red on the Raspberry Pi Zero W. The flow contains an Input Pin, Trigger, Narrowband, Function Block, and Email Send block.

<img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/NodeRedFlowDiagram.JPG" width="350"><br>

The input pin is used to trigger the email. The Mbed will send a short pulse to the Rasbperry Pi each time the alarm system is triggered. This pulse is sent for less than a second, allowing the Trigger to only fire once for each trigger event. The Narrowband block only fires a pulse when the trigger is set high, so that there isn't a second email sent when the signal pulse is set to 0. The function block sets up the header and body of the email, and finally the email block will send the email to the user.

Outlook seems to be the only email that is still compatiable with the Node-Red email functionality. The user will need to input their Outlook userID and password into the email block and then Deploy the flow to Node-Red on the Pi. Example email sent to Outlook:

<img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/EmailWarning.PNG" width="350"><br>


## C# Application
The Windows Form C# application connects to the Raspberry Pi Zero W through a TCP connection. As long as the application is on the same Wi-Fi as the Raspberry Pi, it can connect given an IP address.

**Program.cs**<br>
This file controls the TCP connection side of the applicaton, including initiating connection with the device and sending/receiving messages. Program.cs also contains all message headers (commands and requests), port information (61000), and a triggeredThread that checks for incoming trigger messages from the Mbed.

**Form1.cs**<br>
This holds the design information for the application GUI and the event handler functions for user interactions. Non helper functions:
* setUpOnConnect: requests information on the current state of the system using Program.cs and updates the GUI accordingly
* changeStatus: changes the color and text of the status textbox. Uses a mutex to avoid crashing the system if multiple instances of the function get called at once.
* alarmTriggered: called by Program.cs when the triggeredThread detects a triggered message from the Mbed.

Form1.cs does not directly interact with the TCP connection and requires the Program.cs file to send and recieve messages.

## GUI Functionality
* Connection
  * IP Address Text Box: Input IP address of Raspbery Pi
  * Connect Button: Attempts connection to TCP client using the given IP address
  * Status Label: Shows the current status of the C# application (Connected/Not Connected)
  * Status Error Message: Shows error message if one arises
* Device Use
  * Volume Track Bar: Changes volume to on or off
  * Brightness Track Bar: Changes brightness to any value between 0 and 9
  * Active Checkbox: Arms the device if checked, disarms the device if unchecked
  * Disarm Button: Turns off the trigger if the device has been triggerd. The system will return to the Armed state if still armed, or disarmed state if the checkbox is unchecked.
  * Status TextBox: Shows the current state of the system.
    * Triggered: RED
    * Armed: BLUE
    * Disarmed: GREEN
  * Disconnect Button: Disconnects the application from the TCP client. Requires clicking the Connect button to reconnect to the device.

<img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/DisarmedScreenshot.JPG" width="350">
<img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/ArmedScreenshot.JPG" width="350">
<img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/TriggeredScreenshot.JPG" width="350"><br>

# TCP Command Syntax
The C# client and Mbed communicate via a TCP to Serial passthrough server on the Pi. Whenever the server receives 4 characters from either the Mbed or the Pi, it forwards it to the other side. When the C# Application sends a request, both the request and response from the Mbed start with a # character. When a command is sent to change the state of either the Mbed or the Pi, the message starts with a ! and does not require a response.

| Command    | Use                 |
|--------    |---------------------|
| !AXX       | Toggle Alarm Active |
| !DXX       | Disarm              |
| !B0X - !B9X| Brightness level    |
| !MXX       | Mute Speaker        |
| !!!!       | Alarm Triggered     |

| Request    | Returns         |
|--------    |-----------------|
| #AXX       | Alarm Status    |
| #BXX       | Brightness level|
| #MXX       | Mute Status     |
| #TXX       | Trigger Status  |

The character X signifies buffer characters and has no meaning. Responses will be identical to requests but the X buffer characters will instead contain the status requested.

**Example 1)** C# App Requesting Active Status from Mbed when device is armed:<br>
| Send      | Recieve   |
|-----------|-----------|
| #AXX      | #A11      |

**Example 2)** C# App Setting Brightness of LED to maximum value:<br>
| Send      | Recieve   |
|-----------|-----------|
| !B9X      |           |

**Example 3)** C# App Receiving Triggered Status from Mbed:<br>
| Send      | Recieve   |
|-----------|-----------|
|           | !!!!      |

# How to start Software on the Device
1. Compile and add Mbed Code
  * Import needed libraries into Keil Studio mbed 2.0 project
    * [Mbed RTOS](https://os.mbed.com/users/mbed_official/code/mbed-rtos/#5713cbbdb706a1e02938701baf0a466463b41ada)
    * [X_NUCLEO_53L0A1](https://developer.mbed.org/teams/ST/code/X_NUCLEO_53L0A1/#27d3d95c8593)
  * Compile and send to Mbed
2. Configure Pi
  * Transfer PiTCPServer.c to the Pi and compile it using `gcc PiTCPServer.c -lpthread`
  * Install Node Red using your preferred method (if not already installed)
  * Connect to the same wifi network as your PC
  * Use `ifconfig` and make note of the Pi's IP address
  * Run both the TCP server and Node Red using `sudo ./a.out & node-red && fg`
  * If the server is running correctly it should display  `Waiting for connection request...`
  * Using a PC browser, go to the IP address at port 1880
    * Example: '170.43.118.20:1880'
  * Import the EmailTriggerFlow.json file into Node-Red
    * Open the send email block
    * Put in your email address, userID, and password<br>
    <img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/EmailFlowProperties.JPG" width="350"><br>
  * Deploy the EmailTriggerFlow to the Pi
3. C# Application
  * Open the SecurityApplicaton.sln solution file in Microsoft Visual Studio
  * Click "Start" to run the application
  * The default application GUI should appear until a connection is made<br>
  <img src="https://raw.githubusercontent.com/Ngregory3/4180FinalProject/main/Images/StartupScreen.JPG" width="350"><br>
  

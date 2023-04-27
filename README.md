# Dorm Security System
## Team Members
William Diamond, Nathaniel Gregory
## Project Description
This project is a wifi enabled Mbed and Raspberry Pi Zero W based dorm security device that can be remotely controled using a Windows C# desktop application.
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

## Class D audio amplifier

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

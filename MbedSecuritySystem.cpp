#include "mbed.h"
#include "XNucleo53L0A1.h"
#include <stdio.h>
#include "rtos.h"
Serial pi(USBTX,USBRX);
DigitalOut shdn(p26);
PwmOut led(p23);
PwmOut speaker(p24);

volatile bool triggered = false;
volatile bool armed = false;
volatile float brightness = 1.0;

#define VL53L0_I2C_SDA   p28
#define VL53L0_I2C_SCL   p27
static XNucleo53L0A1 *board=NULL;

void alarmLED(void const *args) {
    while(1) {
        if (triggered) {
            led = 1 * brightness;
            Thread::wait(100);
            led = 0;
        } else {
            led = 0;
        }
        Thread::wait(100);
    }
}

void alarmSound(void const *args) {
    while(1) {
        if (triggered) {
            //speaker = 0.5;
        } else {
            speaker = 0;
        }
        Thread::wait(500);
    }
}

void tripWire(void const *args) {
    int triggerCount;
    int status;
    uint32_t distance;
    DevI2C *device_i2c = new DevI2C(VL53L0_I2C_SDA, VL53L0_I2C_SCL);
    board = XNucleo53L0A1::instance(device_i2c, A2, D8, D2);

    wait(0.1);
    shdn = 1;
    wait(0.1);
    status = board->init_board();
    while (status) {
        status = board->init_board();
    }
    while (1) {
        status = board->sensor_centre->get_distance(&distance);
        if (status == VL53L0X_ERROR_NONE) {
            if (distance > 120 && distance < 600 && armed) {
                triggerCount++;
            } else {
                triggerCount = 0;
            }
            if (triggerCount > 3) {
                triggered = true;
            }
        }
    }
}

int main()
{
    speaker.period(1/340.0);
    Thread t1(tripWire);
    Thread t2(alarmLED);
    Thread t3(alarmSound);

    char buffer[4];
    while (1) {
        if (triggered && pi.writable()) {
            pi.putc('!');
            pi.putc('!');
            pi.putc('!');
            pi.putc('!');
        }
        if (pi.readable()) {
            buffer[0] = pi.getc();
            buffer[1] = pi.getc();
            buffer[2] = pi.getc();
            buffer[3] = pi.getc();

            if (buffer[0] == '!') {
                switch(buffer[1]) {
                    case 'A':
                        armed = true;
                        break;
                    case 'D':
                        armed = false;
                        triggered = false;
                        break;
                    case 'B':
                        switch(buffer[2]) {
                            case '0':
                                brightness = 0;
                                break;
                            case '1':
                                brightness = 0.2;
                                break;
                            case '2':
                                brightness = 0.3;
                                break;
                            case '3':
                                brightness = 0.4;
                                break;
                            case '4':
                                brightness = 0.5;
                                break;
                            case '5':
                                brightness = 0.6;
                                break;
                            case '6':
                                brightness = 0.7;
                                break;
                            case '7':
                                brightness = 0.8;
                                break;
                            case '8':
                                brightness = 0.9;
                                break;
                            case '9':
                                brightness = 1.0;
                                break;
                        }
                }
            }
        }
    }
}
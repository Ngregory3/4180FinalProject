#include "mbed.h"
#include "XNucleo53L0A1.h"
#include <stdio.h>
#include "rtos.h"
Serial pi(USBTX,USBRX);
DigitalOut shdn(p26);
PwmOut red(p21);
PwmOut green(p22);
PwmOut blue(p23);
PwmOut speaker(p24);
DigitalOut piTrigger(p5);

volatile bool triggered = false;
volatile bool armed = false;
volatile float brightness = 1.0;
volatile bool muted = true;
volatile bool triggerSent = false;

#define VL53L0_I2C_SDA   p28
#define VL53L0_I2C_SCL   p27
static XNucleo53L0A1 *board=NULL;

void alarmLED(void const *args) {
    while(1) {
        if (triggered) {
            blue = 0;
            green = 0;
            red = 1.0 * brightness;
            Thread::wait(100);
            red = 0;
        } else if (armed) {
            red = 0;
            blue = 1.0 * brightness;
            green = 0;
        } else {
            red = 0;
            green = 1.0 * brightness;
            blue = 0;
        }
        Thread::wait(100);
    }
}

void alarmSound(void const *args) {
    while(1) {
        if (triggered && !muted) {
            speaker = 0.5;
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
            if (triggerCount > 3 && !triggered) {
                triggered = true;
                triggerSent = false;
                piTrigger = 1;
                Thread::wait(200);
                piTrigger = 0;
            }
        }
    }
}

int main()
{
    speaker.period(1/500.0);
    Thread t1(tripWire);
    Thread t2(alarmLED);
    Thread t3(alarmSound);
    piTrigger = 0;

    char buffer[4];
    while (1) {
        if (pi.readable()) {
            buffer[0] = pi.getc();
            buffer[1] = pi.getc();
            buffer[2] = pi.getc();
            buffer[3] = pi.getc();

            if (buffer[0] == '!') {
                switch(buffer[1]) {
                    case 'A':
                        switch (buffer[2]) {
                            case '0':
                                armed = false;
                                triggered = false;
                                break;
                            case '1':
                                armed = true;
                                triggered = false;
                                break;
                        }
                        break;
                    case 'D':
                        triggered = false;
                        break;
                    case 'M':
                        muted = !muted;
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
            if (buffer[0] == '#') {
                switch (buffer[1]) {
                    case 'T':
                        if (triggered) {
                            pi.putc('#');
                            pi.putc('T');
                            pi.putc('1');
                            pi.putc('1');
                        } else {
                            pi.putc('#');
                            pi.putc('T');
                            pi.putc('0');
                            pi.putc('0');
                        }
                        break;
                    case 'A':
                        if (armed) {
                            pi.putc('#');
                            pi.putc('A');
                            pi.putc('1');
                            pi.putc('1');
                        } else {
                            pi.putc('#');
                            pi.putc('A');
                            pi.putc('0');
                            pi.putc('0');
                        }
                        break;
                    case 'M':
                        if (muted) {
                            pi.putc('#');
                            pi.putc('M');
                            pi.putc('1');
                            pi.putc('1');
                        } else {
                            pi.putc('#');
                            pi.putc('M');
                            pi.putc('0');
                            pi.putc('0');
                        }
                        break;
                    case 'B':
                        int b = 0;
                        if (brightness > 0) {
                            b = static_cast<int>(brightness * 10.0 - 1);
                        }
                        b += 48;
                        pi.putc('#');
                        pi.putc('B');
                        pi.putc(static_cast<char>(b));
                        pi.putc(static_cast<char>(b));
                        break;
                }
            }
        }
        if (triggered && pi.writable() && !triggerSent) {
            pi.putc('!');
            pi.putc('!');
            pi.putc('!');
            pi.putc('!');
            triggerSent = true;
        }
        Thread::wait(300);
    }
}

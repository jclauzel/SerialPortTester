# SerialPortTester
SerialPortTester utility for CSpect and UARTReplacement

SerialPortTester is a simple utility that allows testing serial port opening such as an ESP or PI for CSpect.

The primary goal why this tool was written is to be able to test from command line (outside of CSpect) if UARTReplacement.dll can connect as done by CSpect but under the exact same conditions (using .net stack ...)

I wrote the tool because if there is a failure opening the port under CSpect there is no error message written or logged, most time what you will see is a hang when running DEMOS\ESP\wifi.bas when issuing first ATE command. 

This happens because the port failed opening and the thread sits there waiting for no reply.

In this case:

- First step is to check on Windows in Device Manager what is the allocated COM Port number or on Linux MacOS ls /dev/tty.*
- Second step is to check you have installed the latest drivers for your USB Serial Port device!

This simple test code code is largley inspired from Robin Verhagen-Guest: https://github.com/Threetwosevensixseven/CSpectPlugins/wiki/UART-Replacement big kudos to him!

Usage:
------
 SerialPortTester [COM Port]
or
 SerialPortTester [COM Port] [BaudSpeed]

Example 1 : SerialPortTester COM1
 - This will try to open and close Serial Port COM1, there is no need out of the box to set the speed or tweak the serial port speed in Windows device manager
   
     If you get something like: 'Success! Opened & closed port COM1 at baud speed 115384.!' it clearly means the port can be opened and that should be all your need. Otherwise check the Exception and Stack spewed in case of failure. Once the utility can connect configure UARTReplacement.dll.config accordingly and you should be good to go.
   
     Note: opening the port is unfortunately not the complete end to end requirement, bytes need to be sent back and forth but at least with this you know your SerialPort ESP can be accessed properly.
   
Example 2 : SerialPortTester COM1 115200
 - This will try to open and close Serial Port COM1 at specific speed of 115200.

Example 3 : mono SerialPortTester /dev/tty.wchusbserial710
 - This will try to open and close Serial Port tty.wchusbserial710 on Linux or MacOS using mono.
   
     As above this is all what should be needed under Linux or MacOS If you get something like : 'Success! Opened & closed port'... the port can be accessed properly.
   

You may want to check out as well: https://remysharp.com/2021/09/09/working-with-the-esp-in-cspect

Requirements
------------
.NET Framework v4.5.2, On MacOS and Linux through mono

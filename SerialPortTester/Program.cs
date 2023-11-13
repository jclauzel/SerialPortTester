using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;


namespace SerialPortTester
{
    internal class Program
    {
        private static System.IO.Ports.SerialPort port;
        private static int clock = 27000000; // CSpect defaults to HDMI timings
        private static int prescaler = 234;  // Next baud defaults to 115200 (more accurately 115384 with integer division)

        public static int Baud
        {
            get
            {
                try
                {
                    if (prescaler == 0)
                        return 0;
                    return Convert.ToInt32(Math.Truncate((Convert.ToDecimal(clock) / prescaler)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SerialPort.Baud Exception:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    return 0;
                }
            }
        }

        static public void SerialPort(string PortName, int baudspeed=0)
        {
            int BaudRate = 0;
            
            try
            {
                port = new System.IO.Ports.SerialPort();

                port.PortName = PortName;
                
                int baud = Baud ;
                if (baudspeed>0)
                    {  baud = baudspeed; }

                port.BaudRate = baud > 0 ? baud : 115200;
                BaudRate = port.BaudRate;
                port.Parity = System.IO.Ports.Parity.None;
                port.DataBits = 8;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.Handshake = System.IO.Ports.Handshake.None;

                Console.WriteLine("Trying to Open() port {0} at baud speed {1}...", PortName, BaudRate);

                port.Open();
                Console.WriteLine("Port Opened ...");
                port.Close();
                Console.WriteLine("Port Closed ...");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success! Opened & closed port {0} at baud speed {1}.", PortName, BaudRate);
                Console.ForegroundColor = ConsoleColor.White;

            }
            catch (Exception ex)
            {
                port = null;
                Console.WriteLine("Failed Opening port! SerialPort Exception: ");
                //Console.WriteLine("Port:{0} , BaudRate:{1}" , PortName, BaudRate);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed opening port {0} at baud speed {1}.", PortName, BaudRate);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void Esp_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            var port = sender as System.IO.Ports.SerialPort;
            if (port != null && port.BytesToRead > 0)
            {
                var bytes = new byte[port.BytesToRead];
                port.Read(bytes, 0, bytes.Length);
                foreach (Byte b in bytes)
                {
                    Console.Write(b.ToString());
                }
            }
        }
        static void Main(string[] args)
        {
            string COMPort = "COM1";
            int BaudSpeed = 0;
            int desiredBaudSpeed = 0;
            string BaudSpeedRate = "";

            Console.WriteLine("SerialPortTester v1.2.");
            Console.WriteLine("----------------------");

            if (args.Length >=1 && !string.IsNullOrEmpty(args[0]))
            {
                COMPort = args[0];
                if (args.Length == 2 && !string.IsNullOrEmpty(args[1]))
                {
                    BaudSpeedRate = args[1];
                    bool convertStatus = int.TryParse(BaudSpeedRate, out BaudSpeed);
                    if (convertStatus)
                    { desiredBaudSpeed = BaudSpeed ; }
                }
            }
            else
            {
                Console.WriteLine("SerialPortTester is a simple utility that allows testing serial port opening such as an ESP or PI.");
                Console.WriteLine("The primary goal why this tool was written is to able to test from command line if UARTReplacement.dll can connect as done by CSpect but under the exact same conditions (using .net stack ...) ");
                Console.WriteLine(@"I wrote the tool because if there is a failure opening the port under CSpect there is no error message written or logged, most time what you will see is a hang when running DEMOS\ESP\wifi.bas when issuing first ATE command. This happens because the port failed opening and the thread sits there waiting for no reply.");
                Console.WriteLine("First step is to check on Windows in Device Manager what is the allocated COM Port number or on Linux MacOS ls /dev/tty.*");
                Console.WriteLine("Second step is to check you have installed the latest drivers for your USB Serial Port device!");
                Console.WriteLine(@"This simple test code code is largley inspired from Robin Verhagen-Guest: https://github.com/Threetwosevensixseven/CSpectPlugins/wiki/UART-Replacement big kudos to him!");
                Console.WriteLine("");
                Console.WriteLine("Usage:");
                Console.WriteLine("------");
                Console.WriteLine(" SerialPortTester <COM Port>");
                Console.WriteLine("or");
                Console.WriteLine(" SerialPortTester <COM Port> <BaudSpeed>");
                Console.WriteLine("");
                Console.WriteLine("Example 1 : SerialPortTester COM1");
                Console.WriteLine(" - This will try to open and close Serial Port COM1, there is no need out of the box to set the speed or tweak the serial port speed in Windows device manager");
                Console.WriteLine("     If you get something like : 'Success! Opened & closed port COM1 at baud speed 115384.!' it clearly means the port can be opened and that should be all your need.");
                Console.WriteLine("     Note: opening the port is unfortunatly not the complete end to end requirement, bytes needs to be sent back and forth but at least with this you know your SerialPort ESP can be accessed properly.");
                Console.WriteLine("Example 2 : SerialPortTester COM1 115200");
                Console.WriteLine(" - This will try to open and close Serial Port COM1 at specific speed of 115200.");
                Console.WriteLine(@"Example 3 : mono SerialPortTester /dev/tty.wchusbserial710");
                Console.WriteLine(" - This will try to open and close Serial Port tty.wchusbserial710 on Linux or MacOS using mono.");
                Console.WriteLine("     As above this is all what should be needed under Linux or MacOS If you get something like : 'Success! Opened & closed port'... the port can be accessed properly.");
                Console.WriteLine("");
                Console.WriteLine(@"You may want to checkout as well: https://remysharp.com/2021/09/09/working-with-the-esp-in-cspect");


                System.Environment.Exit(0);
            }

            SerialPort(COMPort, desiredBaudSpeed);

            Console.WriteLine("Exiting...");
        }
    }
}

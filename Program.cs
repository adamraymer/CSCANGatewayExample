using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kvaser.CanLib;

/*
This is a basic example of how to read a message from one device, change the ID and possibly change message contents, then send the message 
back on the bus
*/

namespace GatewayExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //A channel is a connection to the bus, or in this case, a channel is an actual device connected to the PC, 
            //whether by USB, PCI (express or other), or Ethernet. It could also be a Virtual device if no physical devices are found
            //(and the proper flag is cet in canOpenCHannel
            int channel1 = 0;
            int channel2 = 1;
          
            int handle1;
            int handle2;
            
            byte[] msg1 = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            Canlib.canInitializeLibrary();

            //these handles are connected to the two separate channels/devices. By being set to canOPEN_ACCEPT_VIRTUAL, they will also accept vitual devices if 
            // no physical device is found
            handle1 = Canlib.canOpenChannel(channel1, Canlib.canOPEN_ACCEPT_VIRTUAL);
            handle2 = Canlib.canOpenChannel(channel2, Canlib.canOPEN_ACCEPT_VIRTUAL);

            //set bus on
            Canlib.canSetBusParams(handle1, Canlib.canBITRATE_500K, 0, 0, 0, 0);
            Canlib.canBusOn(handle1);

            Canlib.canSetBusParams(handle2, Canlib.canBITRATE_500K, 0, 0, 0, 0);
            Canlib.canBusOn(handle2);

            

            int retID;
            byte[] msg2 = new byte[8];
            int retDlc;
            int retFlag;
            long time;
            Canlib.canStatus Stat;

            //loop through communication simulation; the number after handle name is the ID for the message

            //This is a fairly simple loop. handle 1 writes a message to the bus. handle3 reads the message. It then
            //changes the ID and writes the message with the new ID back to the bus. Using a few more handles, this could also be 
            // done to read from one bus, change the ID or message contents, and write to another bus (using a channel connection to eh second bus)
            for (int i = 200; i < 210; i++)
            {
                Stat = Canlib.canWriteWait(handle1, 100, msg1, 8, 0, 1000);
                Console.WriteLine("handle1 Write = " + Stat.ToString());
                Stat = Canlib.canReadWait(handle2, out retID, msg2, out retDlc, out retFlag, out time, 1000);
                Console.WriteLine("handle2 Read = " + retID.ToString());
                readAndRepack(handle2, msg2, i);
                Stat = Canlib.canReadWait(handle1, out retID, msg2, out retDlc, out retFlag, out time, 1000);
                Console.WriteLine("handle1 Read = " + retID.ToString());

            }

            //stop process
            Console.ReadLine();
        }

        static void readAndRepack(int handle, byte[] msg, int ID)
        {
            Canlib.canStatus Stat;
            Stat = Canlib.canWriteWait(handle, ID, msg, 8, 0, 1000);
            Console.WriteLine("Handle " + (handle + 1 ).ToString() + " Write = " + ID.ToString() + " Code = " + Stat.ToString());

        }
    }
}

using System;

namespace Love
{
    class Program
    {
        static void Main(string[] args)
        {
            PacketCapture.CapturePackets();
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
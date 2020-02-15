using System;
using System.IO;
using System.Runtime.InteropServices;
using SharpPcap;
using SharpPcap.Npcap;

namespace Love
{
    public class GrabInterface
    {
        public static ICaptureDevice MainDevice()
        {
            var devFile = @$"{Environment.CurrentDirectory}/device.ini";
            Console.WriteLine(devFile);
            var devices = CaptureDeviceList.Instance;
            if (!File.Exists(devFile))
            {
                Console.WriteLine("Please select your device, I'll try and remember it in the future.\n");
                for (var i = 0; i < devices.Count; i++)
                {
                    Console.WriteLine($"{i}) {devices[i].Name}");
                }

                var input = Convert.ToInt32(Console.ReadLine());
                using (StreamWriter outputFile =
                    new StreamWriter(Path.Combine(Environment.CurrentDirectory, "device.ini")))
                {
                    outputFile.WriteLine(devices[input].Name);
                }

                return devices[input];
            }
            else
            {
                foreach (var dev in devices)
                {
                    if (dev.Name == File.ReadAllText(devFile))
                        return dev;
                }
            }
            // Fails to find get first
            return devices[0];
        }
    }
}
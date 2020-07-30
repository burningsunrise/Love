using System;
using System.Net.NetworkInformation;
using SharpPcap;

namespace Love
{
    public class GrabInterface
    {
        public static ICaptureDevice MainDevice()
        {
            var devices = CaptureDeviceList.Instance;
            
            foreach(NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var device in devices)
                    {
                        //Console.WriteLine($"{ni.Description} | {device.Description} | {ni.Id} | {device.Name}");
                        if (Environment.OSVersion.Platform == PlatformID.Unix && ni.Name == device.Name && ni.Name != "lo")
                        {
                            Console.WriteLine($"{ni.Description}");
                            return device;
                        }
                        if (!ni.Description.Contains("Loopback") && @$"\Device\NPF_{ni.Id}" == device.Name)
                        {
                            return device;
                        }
                    }
                }  
            }

            return devices[0];
        }
    }
}
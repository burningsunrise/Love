using System;
using SharpPcap;
using SharpPcap.Npcap;

namespace Love
{
    public class GrabInterface
    {
        public static ICaptureDevice MainDevice()
        {
            var devices = CaptureDeviceList.Instance;
            foreach (var dev in devices)
            {
                if (dev.Name.ToLower()[0] == 'e')
                    return dev;
            }
            
            // Fails to find get first
            return devices[0];
        }
    }
}
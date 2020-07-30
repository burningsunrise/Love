using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;

namespace Love
{
    public class PacketCapture
    {
        public static int PacketLimit { get; set; }
        public static List<PhysicalAddress> SourceMac { get; set; } = new List<PhysicalAddress>();
        public static PhysicalAddress LocalMac { get; set; }

        public static void CapturePackets()
        {
            var device = GrabInterface.MainDevice();
            // Wait for the device to become available
            Thread.Sleep(1000);
            device.OnPacketArrival += device_OnPacketArrival;

            var readTimeoutMilliseconds = 1000;
            if (device is NpcapDevice)
            {
                var nPcap = device as NpcapDevice;
                nPcap.Open(OpenFlags.DataTransferUdp | OpenFlags.NoCaptureLocal,
                    readTimeoutMilliseconds);
            }
            else if (device is LibPcapLiveDevice)
            {
                var livePcapDevice = device as LibPcapLiveDevice;
                livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            }
            else
            {
                throw new InvalidOperationException("Unknown device type of " + device.GetType());
            }
            
            Console.WriteLine("-- Started Capture");
            LocalMac = device.MacAddress;
            device.StartCapture();
            // Wait for 'Enter' from the user.
            Console.ReadLine();
            // Stop the capturing process
            device.StopCapture();
            Console.WriteLine("-- Capture stopped.");

        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;

            var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            if (packet is EthernetPacket)
            {
                var eth = ((EthernetPacket) packet);
                if (!SourceMac.Contains(eth.SourceHardwareAddress) && !Equals(eth.SourceHardwareAddress, LocalMac))
                {
                    Console.WriteLine(string.Join(":", eth.SourceHardwareAddress.GetAddressBytes().Select(x => x.ToString("X2"))));
                    SourceMac.Add(eth.SourceHardwareAddress);
                }
            }
            var tcpPacket = packet.Extract<TcpPacket>();
            if (tcpPacket != null)
            {
                var ipPacket = (IPPacket) tcpPacket.ParentPacket;
                var srcIp = ipPacket.SourceAddress;
                var dstIp = ipPacket.DestinationAddress;
                int srcPort = tcpPacket.SourcePort;
                int dstPort = tcpPacket.DestinationPort;
                // Console.WriteLine($"SrcIp: {srcIp} | DstIp: {dstIp} | SrcPort: {srcPort} | DstPort: {dstPort}");
            }
        }
    }
}
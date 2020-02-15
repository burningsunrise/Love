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
        public static int PacketLimit { get; set; } = 0;
        public static List<PhysicalAddress> SourceMac { get; set; } = new List<PhysicalAddress>();
        public static PhysicalAddress LocalMac { get; set; }

        public static void CapturePackets()
        {
            var device = GrabInterface.MainDevice();
            Console.WriteLine(device.Name);
            device.OnPacketArrival += new PacketArrivalEventHandler(
                device_OnPacketArrival);

            var readTimeoutMilliseconds = 1000;
            if (device is NpcapDevice)
            {
                var nPcap = device as NpcapDevice;
                nPcap.Open(SharpPcap.Npcap.OpenFlags.DataTransferUdp | SharpPcap.Npcap.OpenFlags.NoCaptureLocal,
                    readTimeoutMilliseconds);
            }
            else if (device is LibPcapLiveDevice)
            {
                var livePcapDevice = device as LibPcapLiveDevice;
                livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            }
            else
            {
                throw new InvalidOperationException("Unknown device type of " + device.GetType().ToString());
            }
            
            Console.WriteLine("-- Started Capture");
            LocalMac = device.MacAddress;
            device.StartCapture();
            while (SourceMac.Count < 1)
            {
                Thread.Sleep(1000);
            }
            Cleanup(device);
        }

        private static void Cleanup(ICaptureDevice device)
        {
            foreach (var mac in SourceMac)
                Console.WriteLine(string.Join(":", (from z in mac.GetAddressBytes() select z.ToString("X2")).ToArray()));
            device.StopCapture();
            Console.WriteLine("-- Capture stopped.");
            //Console.WriteLine(device.Statistics.ToString());
            device.Close();
            
        }
        
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;

            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            if (packet is PacketDotNet.EthernetPacket)
            {
                PacketLimit++;
                var eth = ((PacketDotNet.EthernetPacket) packet);
                if (!SourceMac.Contains(eth.SourceHardwareAddress) && !Equals(eth.SourceHardwareAddress, LocalMac))
                    SourceMac.Add(eth.SourceHardwareAddress);
            }
            var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
            if (tcpPacket != null)
            {
                var ipPacket = (PacketDotNet.IPPacket) tcpPacket.ParentPacket;
                var srcIp = ipPacket.SourceAddress;
                var dstIp = ipPacket.DestinationAddress;
                int srcPort = tcpPacket.SourcePort;
                int dstPort = tcpPacket.DestinationPort;
                // Console.WriteLine($"SrcIp: {srcIp} | DstIp: {dstIp} | SrcPort: {srcPort} | DstPort: {dstPort}");
            }
        }
    }
}
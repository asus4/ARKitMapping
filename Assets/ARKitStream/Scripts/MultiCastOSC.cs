using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityOSC;

namespace AppKit
{
    public class MultiCastOSC : MonoBehaviour
    {
        public string address = "225.6.7.8";

        [Range(1024, IPEndPoint.MaxPort)]
        public int port = 9765; // default

        public event Action<OSCMessage> OnMessage;

        IPEndPoint remote;
        IPEndPoint local;
        UdpClient client;

        void Start()
        {
            Application.runInBackground = true;
            IPAddress addr = IPAddress.Parse(address);

            if (!IsMulticastAddress(addr))
            {
                Debug.LogWarning($"{address} is not Multicast address.");
                address = "225.6.7.8";
            }

            remote = new IPEndPoint(IPAddress.Parse(address), port);
            local = new IPEndPoint(IPAddress.Any, port);
            client = new UdpClient(local);

            client.JoinMulticastGroup(addr);
        }

        void OnDestroy()
        {
            client.Close();
        }

        void Update()
        {
            if (client == null)
            {
                return;
            }

            while (client.Available > 0)
            {
                // Parse value
                byte[] bytes = client.Receive(ref local);
                if (OnMessage == null)
                {
                    continue;
                }

                var packet = OSCPacket.Unpack(bytes);
                if (packet.IsBundle())
                {
                    foreach (OSCMessage msg in packet.Data)
                    {
                        OnMessage(msg);
                    }
                }
                else
                {
                    OnMessage(packet as OSCMessage);
                }
            }

        }

        public void Send(OSCPacket packet)
        {
            var data = packet.BinaryData;
            client.Send(data, data.Length, remote);
        }

        static bool IsMulticastAddress(IPAddress addr)
        {
            // 224.0.0.0 - 239.255.255.255
            byte[] bytes = addr.GetAddressBytes();
            return (bytes[0] & 0xF0) == 0xE0;
        }


    }
}

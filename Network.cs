using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


class Network
{
    UdpClient udpClient;
    readonly int hostListenPort = 9239;
    public int PlayerId { get; private set; }

    IPEndPoint opponent;
    public Network(bool isHost)
    {
        udpClient = new UdpClient(isHost ? hostListenPort : 0);
        Console.WriteLine("listening on port " + (isHost ? hostListenPort : 0));
    }

    public void Connect(string ip)
    {
        PlayerId = 2;
        opponent = new IPEndPoint(IPAddress.Parse(ip), hostListenPort);
        udpClient.Send(new byte[] {}, 0, opponent);

        Console.WriteLine("waiting for opponent");
        udpClient.Receive(ref opponent);
        
    }

    public void WaitForConnection()
    {
        PlayerId = 1;

        opponent = new IPEndPoint(IPAddress.Any, hostListenPort);

        Console.WriteLine("waiting for opponent");
        udpClient.Receive(ref opponent);
        udpClient.Send(new byte[] { 0 }, 0, opponent);
    }

    public int GetOpponentMove()
    {
        return (int)udpClient.Receive(ref opponent)[0];
    }

    public void SendMove(int move)
    {
        udpClient.Send(new byte[] { (byte)move }, 1, opponent);
    }

    public void Dispose()
    {
        udpClient.Dispose();
    }
}


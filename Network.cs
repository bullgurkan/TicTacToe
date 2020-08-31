using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Network
{
    NetworkStream stream;
    readonly int hostListenPort = 9239;
    private Queue<int> movesToSend;
    private Queue<int> movesToRecive;
    public int PlayerId { get; private set; }
    Byte[] bytes = new Byte[128];


    bool running = true;

    Thread thread;
    private static readonly Object obj = new Object();
    public Network(bool isHost)
    {
        //tcpClient = new UdpClient(isHost ? hostListenPort : 0);
        movesToSend = new Queue<int>();
        movesToRecive = new Queue<int>();
    }

    public void Connect(string ip)
    {
        PlayerId = 2;
        try
        {
            stream = new TcpClient(ip, hostListenPort).GetStream();
        }
        catch (Exception)
        {
            Environment.Exit(0);
            throw;
        }
       


        //opponent = new IPEndPoint(IPAddress.Parse(ip), hostListenPort);
        stream.Write(bytes, 0, 128);
        stream.Flush();

        Console.WriteLine("waiting for opponent");
        stream.Read(bytes, 0, 128);

    }

    public void WaitForConnection()
    {
        PlayerId = 1;

        TcpListener tcpServer = new TcpListener(IPAddress.Parse("127.0.0.1"), hostListenPort);
        tcpServer.Start();

        Console.WriteLine("waiting for opponent");

        stream = tcpServer.AcceptTcpClient().GetStream();

        stream.Read(bytes, 0, 128);

        stream.Write(bytes, 0, 128);
        stream.Flush();

        //tcpClient.Receive(ref opponent);
        //tcpClient.Send(new byte[] { 0 }, 0, opponent);
    }

    public int GetOpponentMove()
    {
        bool lockWasTaken = false;
        var temp = obj;
        try
        {
            Monitor.Enter(temp, ref lockWasTaken);



            if (movesToRecive.Count > 0)
                return movesToRecive.Dequeue();
            else
                return -1;

        }
        finally
        {
            if (lockWasTaken)
            {
                Monitor.Exit(temp);
            }
        }
    }

    public void SendMove(int move)
    {
        bool lockWasTaken = false;
        var temp = obj;
        try
        {
            Monitor.Enter(temp, ref lockWasTaken);

            movesToSend.Enqueue(move);
        }
        finally
        {
            if (lockWasTaken)
            {
                Monitor.Exit(temp);
            }
        }
    }



    public void Start()
    {
        if (thread == null)
        {
            thread = new Thread(new ThreadStart(Update));
            thread.Start();
        }
    }
    public void Stop()
    {
        bool lockWasTaken = false;
        var temp = obj;
        try
        {
            Monitor.Enter(temp, ref lockWasTaken);

            running = false;

        }
        finally
        {
            if (lockWasTaken)
            {
                Monitor.Exit(temp);
            }
        }
    }

    private void Update()
    {
        bool isSending = PlayerId == 1;
        int opponentMove = -1;
        Dictionary<byte, int> counter = new Dictionary<byte, int>();
        while (true)
        {

            if (!isSending)
            {
                stream.Read(bytes, 0, 128);
                foreach (byte b in bytes)
                {
                    if(!counter.ContainsKey(b))
                    {
                        counter.Add(b, 0);
                    }
                    counter[b]++;
                }

                
                opponentMove = (int)counter.Max().Key;
                
                counter.Clear();
                isSending = true;
            }

            bool lockWasTaken = false;
            var temp = obj;
            try
            {
                Monitor.Enter(temp, ref lockWasTaken);

                if (isSending && movesToSend.Count > 0)
                {

                    byte move = (byte)movesToSend.Dequeue();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = move;
                    }

                    stream.Write(bytes, 0, 128);
                    stream.Flush();
                    isSending = false;
                }



                if (opponentMove != -1)
                {
                    movesToRecive.Enqueue(opponentMove);
                }

                if (!running)
                    break;

            }
            finally
            {
                if (lockWasTaken)
                {
                    Monitor.Exit(temp);
                }
            }
            Thread.Sleep(1);
        }

        //used to make the other client go to send mode so that it can exit

        bytes[0] = (byte)movesToSend.Dequeue();
        stream.Write(bytes, 0, 128);
        stream.Flush();

        //tcpClient.Dispose();
    }
}


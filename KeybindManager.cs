using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

class KeybindingManager
{
    private Dictionary<ConsoleKey, Move> keybinds;
    private Queue<Move> actions;
    private bool shouldStop = false;
    private static readonly Object obj = new Object();
    Thread thread;



    public enum Move
    {
        Right, Left, Up, Down, Select
    }

    public KeybindingManager()
    {
        keybinds = new Dictionary<ConsoleKey, Move>();
        actions = new Queue<Move>();
    }

    public void InitDefualtKeybinds()
    {
        AddKeybind(ConsoleKey.RightArrow, Move.Right);
        AddKeybind(ConsoleKey.D, Move.Right);
        AddKeybind(ConsoleKey.LeftArrow, Move.Left);
        AddKeybind(ConsoleKey.A, Move.Left);
        AddKeybind(ConsoleKey.UpArrow, Move.Up);
        AddKeybind(ConsoleKey.W, Move.Up);
        AddKeybind(ConsoleKey.DownArrow, Move.Down);
        AddKeybind(ConsoleKey.S, Move.Down);
        AddKeybind(ConsoleKey.Spacebar, Move.Select);
    }

    public void AddKeybind(ConsoleKey key, Move actrion)
    {
        keybinds.Add(key, actrion);
    }

    public void Start()
    {
        if(thread == null)
        {
            thread = new Thread(new ThreadStart(Update));
            thread.Start();
        }
        
    }

    public Move? GetMove()
    {

        bool lockWasTaken = false;
        var temp = obj;
        try
        {
            Monitor.Enter(temp, ref lockWasTaken);
            if (actions.Count > 0)
                return actions.Dequeue();
            else
                return null;
        }
        finally
        {
            if (lockWasTaken)
            {
                Monitor.Exit(temp);
            }
        }
    }

    public void ShutDown()
    {
        bool lockWasTaken = false;
        var temp = obj;
        try
        {
            Monitor.Enter(temp, ref lockWasTaken);
            shouldStop = true;
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
        while (true)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (keybinds.ContainsKey(key))
            {
                bool lockWasTaken = false;
                var temp = obj;
                try
                {
                    Monitor.Enter(temp, ref lockWasTaken);

                    if (shouldStop)
                        return;

                    actions.Enqueue(keybinds[key]);
                }
                finally
                {
                    if (lockWasTaken)
                    {
                        Monitor.Exit(temp);
                    }
                }
            }
        }

    }
}


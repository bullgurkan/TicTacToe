using System;
using System.Collections.Generic;
using System.Text;


public class GameManager
{
    int endState;
    Network network;
    KeybindingManager keybindingManager;
    public void PreGame()
    {
        keybindingManager = new KeybindingManager();
        keybindingManager.InitDefualtKeybinds();
        

        // 127.0.0.1
        Console.WriteLine("play on network? y for yes, n for no");

        if (Console.ReadLine() == "y")
        {
            Console.WriteLine("h for host, c for client");
            bool isHost = Console.ReadLine() == "h";
            network = new Network(isHost);

            if (isHost)
                network.WaitForConnection();
            else
            {
                Console.WriteLine("input ip to connect to");
                network.Connect(Console.ReadLine());
            }
               
            
        }

        
    }
    public void RunGame()
    { 

        World world = new World(20, 4);
        InputHandler input = new InputHandler(world);
        keybindingManager.Start();

        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

        Console.CursorVisible = false;

        world.DrawWorldWholeWorld(input.HoveredTile);

        //if no one has won keep playing
        while (endState < 1)
        {
            if(network == null || network.PlayerId == (int)world.CurrentPlayer)
            {
                endState = input.Update(world, network, keybindingManager);
            }      
            else
            {
                int opponentMove = network.GetOpponentMove();
                if (world.InBounds(opponentMove))
                {
                    endState = world.MakeMove(opponentMove);
                    input.HoveredTile = opponentMove;
                }
                   
            }

            world.DrawHoveredPos(input.HoveredTile);

            System.Threading.Thread.Sleep(1);
        }

        network?.Dispose();

        PostGame();
    }

    private void PostGame()
    {
        Console.Clear();

        Console.WriteLine();

        Console.ForegroundColor = endState == 1 ? ConsoleColor.Green : ConsoleColor.Red;
        Console.Write($"Player {endState} ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("is the ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("Winner\n");

        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Press Spacebar to end close");

        keybindingManager.ShutDown();
        
    }
}


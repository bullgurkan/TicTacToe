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

        World world = new World(20, 5);
        InputHandler input = new InputHandler(world);
        keybindingManager.Start();
        network?.Start();

        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

        Console.CursorVisible = false;

        world.DrawWorldWholeWorld(input.HoveredTile, input.HoveredTile);

        int opponentHoveredTile = input.HoveredTile;

        //if no one has won keep playing
        while (endState < 1)
        {
            if (network == null)
            {
                endState = input.Update(world, network, keybindingManager, true);
                world.DrawHoveredPos(input.HoveredTile, input.HoveredTile);
            }
            else
            {
                endState = input.Update(world, network, keybindingManager, network.PlayerId == (int)world.CurrentPlayer);

                if (endState == 0 && network.PlayerId != (int)world.CurrentPlayer)
                {
                    int move = network.GetOpponentMove();
                     
                    if (world.InBounds(move))
                    {
                        opponentHoveredTile = move;
                        endState = world.MakeMove(opponentHoveredTile);
                    }

                }

                world.DrawHoveredPos(network.PlayerId == 1 ? input.HoveredTile : opponentHoveredTile, network.PlayerId == 2 ? input.HoveredTile : opponentHoveredTile);
            }



            System.Threading.Thread.Sleep(1);
        }

        network?.Stop();

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


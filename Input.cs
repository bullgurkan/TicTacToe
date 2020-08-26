using System;
using System.Collections.Generic;
using System.Text;


class Input
{
    public int HoveredTile { get; set; }
    private Dictionary<ConsoleKey, Action> keybinds;


    public enum Action
    {
        Right, Left, Up, Down, Select
    }

    public Input(World world)
    {
        HoveredTile = world.WorldWidth * world.WorldWidth / 2 + world.WorldWidth / 2;
        keybinds = new Dictionary<ConsoleKey, Action>();
    }

    public void InitDefualtKeybinds()
    {
        AddKeybind(ConsoleKey.RightArrow, Action.Right);
        AddKeybind(ConsoleKey.D, Action.Right);
        AddKeybind(ConsoleKey.LeftArrow, Action.Left);
        AddKeybind(ConsoleKey.A, Action.Left);
        AddKeybind(ConsoleKey.UpArrow, Action.Up);
        AddKeybind(ConsoleKey.W, Action.Up);
        AddKeybind(ConsoleKey.DownArrow, Action.Down);
        AddKeybind(ConsoleKey.S, Action.Down);
        AddKeybind(ConsoleKey.Spacebar, Action.Select);
    }

    public void AddKeybind(ConsoleKey key, Action actrion)
    {
        keybinds.Add(key, actrion);
    }

    public int Update(World world, Network net)
    {
        switch (keybinds[Console.ReadKey().Key])
        {
            case Action.Right:
                if (HoveredTile % world.WorldWidth < world.WorldWidth - 1)
                {
                    HoveredTile++;
                }
                break;
            case Action.Left:
                if (HoveredTile % world.WorldWidth > 0)
                {
                    HoveredTile--;
                }
                break;
            case Action.Up:
                if (HoveredTile / world.WorldWidth > 0)
                {
                    HoveredTile -= world.WorldWidth;
                }
                break;
            case Action.Down:
                if (HoveredTile / world.WorldWidth < world.WorldWidth - 1)
                {
                    HoveredTile += world.WorldWidth;
                }
                break;
            case Action.Select:
                if (net != null)
                {
                    net.SendMove(HoveredTile);
                }
                return world.MakeMove(HoveredTile);
            default:
                break;
        }
        return 0;
    }
}


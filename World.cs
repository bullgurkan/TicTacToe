using System;
using System.Collections.Generic;
using System.Text;


class World
{

    public enum Tile
    {
        Empty, Player1, Player2
    }

    readonly Tile[] world;
    public int WorldWidth { get; private set; }
    readonly int winningRowLength;
    int prevHoveredTile;

    public Tile CurrentPlayer { get; private set; }

    public World(int worldWidthIn, int winningRowLengthIn)
    {
        WorldWidth = worldWidthIn;
        winningRowLength = winningRowLengthIn;
        world = new Tile[WorldWidth * WorldWidth];
        CurrentPlayer = Tile.Player1;
    }

    /// <summary>
    /// Takes the position as x + y * width and makes the move on the world
    /// </summary>
    /// <returns>
    /// a exit code where -1 move is invalid, 0 move is succsessful, 1 Player1 won, 2 Player2 won
    /// </returns>
    public int MakeMove(int pos)
    {
        if (InBounds(pos) && world[pos] == Tile.Empty)
        {
            world[pos] = CurrentPlayer;

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        CurrentPlayer = CurrentPlayer == Tile.Player1 ? Tile.Player2 : Tile.Player1;
                        return 0;
                    }
                    if (CheckLine(pos, x + y * WorldWidth))
                    {
                        return (int)CurrentPlayer;
                    }
                }
            }

            return 0;
        }
        else
        {
            return -1;
        }



    }

    /// <summary>
    /// Counts the tiles in a line from origin towards dir and origin towards -dir until it finds an empty or enemy tile
    /// </summary>
    /// <returns>
    /// returns total tiles in the line is >=  winningRowLength
    /// </returns>
    private bool CheckLine(int origin, int dir)
    {
        int rowLength = 1;
        for (int mult = -1; mult < 2; mult += 2)
        {
            for (int i = 1; i < winningRowLength; i++)
            {

                if (InBounds(origin + dir * i * mult) && world[origin + dir * i * mult] == CurrentPlayer)
                    rowLength++;
                else
                    break;
            }
        }

        return rowLength >= winningRowLength;

    }

    public bool InBounds(int pos)
    {
        return pos >= 0 && pos < world.Length;
    }


    /// <summary>
    /// ReDraws the whole world
    /// </summary>
    public void DrawWorldWholeWorld(int hoveredTile)
    {
        Console.SetCursorPosition(0, 0);
        for (int y = 0; y < WorldWidth; y++)
        {
            for (int x = 0; x < WorldWidth; x++)
            {
                DrawTile(x, y, hoveredTile);
            }
            Console.WriteLine();
        }
        prevHoveredTile = hoveredTile;

    }

    /// <summary>
    /// Replaces the tile at the previous hovered pos and draws the tile at the new hovered pos
    /// </summary>
    public void DrawHoveredPos(int hoveredTile)
    {

        int x = prevHoveredTile % WorldWidth;
        int y = prevHoveredTile / WorldWidth;
        Console.SetCursorPosition(x * 3, y);

        DrawTile(x, y, hoveredTile);

        x = hoveredTile % WorldWidth;
        y = hoveredTile / WorldWidth;
        Console.SetCursorPosition(x * 3, y);
        DrawTile(x, y, hoveredTile);

        prevHoveredTile = hoveredTile;

        Console.SetCursorPosition(0, WorldWidth);

    }

    /// <summary>
    /// Draws a tile at x, y with the border color of white or the color of the player if it's hovered and the middle color the color of the player who controls the tile
    /// </summary>
    private void DrawTile(int x, int y, int hoveredTile)
    {
        ConsoleColor borderColor = x + y * WorldWidth == hoveredTile ? (CurrentPlayer == Tile.Player1 ? ConsoleColor.Green : ConsoleColor.Red) : ConsoleColor.White; ;
        Console.ForegroundColor = borderColor;
        Console.Write("[");
        switch (world[x + y * WorldWidth])
        {
            case Tile.Empty:
                Console.Write(" ");
                break;
            case Tile.Player1:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("X");
                break;
            case Tile.Player2:
                Console.ForegroundColor = ConsoleColor.Red; 
                Console.Write("O");
                break;
            default:
                break;
        }

        Console.ForegroundColor = borderColor;
        Console.Write("]");
    }
}
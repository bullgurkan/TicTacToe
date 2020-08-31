using System;
using System.Collections.Generic;
using System.Text;
using static KeybindingManager;

class InputHandler
{
    public int HoveredTile { get; set; }

    public InputHandler(World world)
    {
        HoveredTile = world.WorldWidth * world.WorldWidth / 2 + world.WorldWidth / 2;
    }

    public int Update(World world, Network net, KeybindingManager keybindingManager, bool allowSelect)
    {
        Move? move = keybindingManager.GetMove();
        while (move != null)
        {
            switch (move)
            {
                case Move.Right:
                    if (HoveredTile % world.WorldWidth < world.WorldWidth - 1)
                    {
                        HoveredTile++;
                    }
                    break;
                case Move.Left:
                    if (HoveredTile % world.WorldWidth > 0)
                    {
                        HoveredTile--;
                    }
                    break;
                case Move.Up:
                    if (HoveredTile / world.WorldWidth > 0)
                    {
                        HoveredTile -= world.WorldWidth;
                    }
                    break;
                case Move.Down:
                    if (HoveredTile / world.WorldWidth < world.WorldWidth - 1)
                    {
                        HoveredTile += world.WorldWidth;
                    }
                    break;
                case Move.Select:
                    if (allowSelect)
                    {
                        int endState = world.MakeMove(HoveredTile);
                        if (net != null && endState != -1)
                        {
                            net.SendMove(HoveredTile);
                        }
                        return endState;
                    }
                    break;
                default:
                    break;
            }

            move = keybindingManager.GetMove();
        }
            
        return 0;
    }
}


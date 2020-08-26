using System;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {

            GameManager gm = new GameManager();
            gm.PreGame();
            gm.RunGame();

        }
    }
}

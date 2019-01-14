using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Timers;
using DoubleBuffer;

namespace GameOfLife
{
    class Program
    {
        private static System.Timers.Timer aTimer = new System.Timers.Timer();
        private static int consoleSizeChangedPrev;
        private static GameOfLife game;

        static void Main(string[] args)
        {

            game = new GameOfLife(100, 50);            
            game.StartGame();

            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 50;
            aTimer.Enabled = true;
            aTimer.AutoReset = false;

            Console.ReadKey();
        }


        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            int w = Console.WindowWidth;
            int h = Console.WindowHeight;
            if ( (w ^ h) != consoleSizeChangedPrev) // check if the console windows was resized
            {
                consoleSizeChangedPrev = (w ^ h);

                // Re draw game board with the new size
                game.DrawGameBoard(w,h);
                game.ConsoleBuffer.Draw("Press any key to quit.", 0, game.ConsoledMinimumHeight - 1, 15);
            }

            game.Play();           

            aTimer.Enabled = true;
        }

        
    }
}

using System;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DoubleBuffer;

namespace GameOfLife
{
    class Program
    {
        static void Main(string[] args)
        {

            int width = 100;
            int height = 50;

            Cell[,] gameBoardCells = new Cell[width, height];
            Cell.SetGameBoardSize(width, height);

            // Init all cells of game board
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    gameBoardCells[i, j] = new Cell(i, j);
                }
            }

            List<Cell> clown = new List<Cell>()
            {
                new Cell(0,0),
                new Cell(0,1),
                new Cell(0,2),
                new Cell(2,0),
                new Cell(2,1),
                new Cell(2,2),
                new Cell(1,2)
            };

            List<Cell> clown2 = new List<Cell>()
            {
                new Cell( 0, 0),
                new Cell( 1,-1),
                new Cell(-1,-1),
                new Cell( 1,-2),
                new Cell(-1,-2),
                new Cell( 1,-3),
                new Cell(-1,-3)
            };

            List<Cell> hInvert = new List<Cell>()
            {
                new Cell(-1,-1),
                new Cell(-1, 0),
                new Cell(-1, 1),
                new Cell( 0, 0),
                new Cell( 1, 0),
                new Cell( 1, 1),
                new Cell( 1, 2)
            };

            List<Cell> flower = new List<Cell>()
            {
                new Cell(-1,-1),
                new Cell( 0,-1),
                new Cell( 1,-1),
                new Cell( 0, 0),
                new Cell(-2, 0),
                new Cell( 2, 0),
                new Cell(-1, 2),
                new Cell( 0, 2),
                new Cell( 1 , 2)
            };

            List<List<Cell>> sprites = new List<List<Cell>>()
            {
                clown,
                clown2,
                hInvert,
                flower
            };

            Cell.AddSprite(clown, width / 2, height / 2, gameBoardCells);

            // Configure console with double buffer

            Console.SetWindowSize(Math.Max(width + 3, 60), height + 4);
            ConsoleDoubleBuffer consoleBuffer = new ConsoleDoubleBuffer(Console.WindowWidth, Console.WindowHeight, Console.WindowWidth, Console.WindowHeight);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            Console.CursorVisible = false;            

            // Display borders
            for (int i = 0; i < width+3; i++)
            {
                consoleBuffer.Draw("*", i, 1 , 2);
                consoleBuffer.Draw("*", i, height+2, 2);
            }

            for (int i = 2; i < height+3; i++)
            {
                consoleBuffer.Draw("*", 0, i, 2);
                consoleBuffer.Draw("*", width+2, i, 2);
            }

            // Infinity loop...
            int generation = 1;
            int generationWithGod = 0;
            EndOfEvolutionDetection endOfEvolutionDetection = new EndOfEvolutionDetection();
            Random r = new Random();
            while (true)
            {
                Thread.Sleep(50);
                int population = 0;
                string cellsString = "";
                foreach (Cell c in gameBoardCells)
                {
                    c.Update();
                    string s = " ";
                    if( c.IsAlive() == true )
                    {
                        s = "#";
                        population++;
                    }

                    consoleBuffer.Draw(s, c.PosX + 1, c.PosY + 2, 15);
                    cellsString += s;
                }

                // Check if it it is the end of cycle

                endOfEvolutionDetection.AddCycle(cellsString);
                bool needGod = (population == 0 || endOfEvolutionDetection.EvolutionEnding() == true);

                if ( needGod == true )
                {
                    generationWithGod++;
                    // all cells are dead or not evolutate  
                    int i = r.Next(0, width);
                    int j = r.Next(0, height);
                    if (population < 10 || generationWithGod > 150 || r.Next(0,50) == 2)
                    {
                        if( Cell.AddSprite(sprites[ r.Next(0, sprites.Count) ], i, j, gameBoardCells) == false)
                        {
                            endOfEvolutionDetection.ClearHistory();
                        }
                    }
                    else
                    {
                        gameBoardCells[i, j].Reborn();
                    }
                }
                else
                {
                    generationWithGod = 0;
                }

                consoleBuffer.Draw( String.Format("  generation: {0}   population: {1:0000}  {2} ", 
                                                    generation++, 
                                                    population, 
                                                    needGod ? String.Format("God in action ({0})", generationWithGod) : "                       " ), 
                                    0, 0, 15);

                consoleBuffer.Print();
                
                foreach(Cell c in gameBoardCells)
                {
                    c.Cycle(gameBoardCells);
                }                
            }

        }
    }
    class Cell
    {
        static int _width;
        static int _height;

        static public void SetGameBoardSize(int width, int height )
        {
            _width = width;
            _height = height;
        }

        public static bool AddSprite(List<Cell> sprite, int x, int y, Cell[,] gameBoardCells)
        {
            bool truncated = false;
            foreach (Cell c in sprite)
            {
                int i = Math.Min(Math.Max(x + c.PosX, 0), _width - 1);
                int j = Math.Min(Math.Max(y + c.PosY, 0), _height - 1);
                truncated = (truncated == false) && (i != x + c.PosX || j != y + c.PosY);

                gameBoardCells[i, j].Reborn();
            }

            return truncated;
        }

        public int PosX { get; private set; }
        public int PosY { get; private set; }
        private bool _alive;
        private bool _nextAlive;

        public bool IsAlive() { return _alive; }

        public Cell(int x, int y)
        {
            PosX = x;
            PosY = y;
            _nextAlive = _alive = false;
        }

        public void Update()
        {
            _alive = _nextAlive;
        }

        public void Reborn()
        {
            _alive = _nextAlive = true;
        }        

        public void Cycle(Cell[,] gameBoardCells)
        {
            int neighboursAliveCounter = 0;
            for (int i = PosX - 1; i < PosX + 2; i++)
            {
                for (int j = PosY - 1; j <PosY + 2; j++)
                {
                    if(i != PosX || j != PosY) // skip current cell
                    {
                        if(i < 0 || j < 0 || i >= _width ||  j >= _height) // check if neighbour exists...
                        {
                            //aliveCounter++;
                        }
                        else if ( gameBoardCells[i,j]._alive == true)
                        {
                            neighboursAliveCounter++;
                        }
                    }
                }
            }

            if( neighboursAliveCounter == 3 && _alive == false)
            {
                _nextAlive = true;
            }
            else if( (neighboursAliveCounter == 2 || neighboursAliveCounter == 3) && _alive == true)
            {
                _nextAlive = true;
            }
            else
            {
                _nextAlive = false;
            }
        }

    }

    class EndOfEvolutionDetection
    {
        public EndOfEvolutionDetection(int historyLength = 100, int minimumDuplicateKeys = 2)
        {
            _historyLength = historyLength;
            _minimumDuplicateKeys = minimumDuplicateKeys;
        }

        private List<string> _historyHashs = new List<string>();
        private int _historyLength;
        private int _minimumDuplicateKeys;
        private MD5 _md5 = MD5.Create();

        public void AddCycle(string cellsString)
        {
           
            byte[] hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(cellsString));
            _historyHashs.Add(BitConverter.ToString(hash));

            while(_historyHashs.Count > _historyLength)
            {
                _historyHashs.RemoveAt(0);
            }
        }

        public bool EvolutionEnding()
        {
            var duplicateKeys = _historyHashs.GroupBy(x => x)
                        .Where(group => group.Count() > _minimumDuplicateKeys)
                        .Select(group => group.Key).ToList();

            return duplicateKeys.Count() > 0;
        }

        public void ClearHistory()
        {
            _historyHashs.Clear();
        }
    }
}

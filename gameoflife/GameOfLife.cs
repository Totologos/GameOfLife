using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using DoubleBuffer;

namespace GameOfLife
{
    class GameOfLife
    {

        public int GameBoardWidth { get; private set; }
        public int GameBoardHeight { get; private set; }

        public int ConsoledMinimumWidth { get { return GameBoardWidth + 6; } }
        public int ConsoledMinimumHeight { get { return GameBoardHeight + 4; } }

        private Cell[,] gameBoardCells;
        public ConsoleDoubleBuffer ConsoleBuffer { get; private set; }

        private EndOfEvolutionDetection endOfEvolutionDetection = new EndOfEvolutionDetection();

        private Random r = new Random();

        private int generation = 1;

        private int generationWithGod = 0;

        /// <summary>
        /// Create game of life object (constructor)
        /// The cells number are specify in parameter (gameBoardWidth x gameBoardHeight)
        /// </summary>
        /// <param name="gameBoardWidth">number of cells in width</param>
        /// <param name="gameBoardHeight">number of cells in height</param>
        public GameOfLife(int gameBoardWidth, int gameBoardHeight)
        {
            GameBoardWidth = gameBoardWidth;
            GameBoardHeight = gameBoardHeight;            

            gameBoardCells = new Cell[GameBoardWidth, GameBoardHeight];
            Cell.SetGameBoardSize(GameBoardWidth, GameBoardHeight);

            for (int i = 0; i < GameBoardWidth; i++)
            {
                for (int j = 0; j < GameBoardHeight; j++)
                {
                    gameBoardCells[i, j] = new Cell(i, j);
                }
            }
        }

        /// <summary>
        /// Start game: setup console windows size in adequation with board game size
        /// Draw the game board and add a Sprite in the middle of game board
        /// </summary>
        public void StartGame()
        {
            Console.SetWindowSize(ConsoledMinimumWidth, ConsoledMinimumHeight);
            DrawGameBoard(GameBoardWidth, GameBoardHeight);
            Cell.AddSprite(Sprites.Clown, GameBoardWidth / 2, GameBoardHeight / 2, gameBoardCells);
        }



        /// <summary>
        /// Each time his method is called, a new cell generation was created and displayed
        /// </summary>
        public void Play()
        {
            int population = 0;
            string cellsString = "";
            foreach (Cell c in gameBoardCells)
            {
                c.Update();
                string s = " ";
                if (c.IsAlive() == true)
                {
                    s = "#";
                    population++;
                }

                ConsoleBuffer.Draw(s, c.PosX + 1, c.PosY + 2, 15);
                cellsString += s;
            }

            // Check if it it is the end of cycle
            endOfEvolutionDetection.AddCycle(cellsString);
            bool needGod = (population == 0 || endOfEvolutionDetection.EvolutionEnding() == true);

            if (needGod == true)
            {
                generationWithGod++;
                // all cells are dead or not evolutate  
                int i = r.Next(0, GameBoardWidth);
                int j = r.Next(0, GameBoardHeight);
                if (population < 10 || generationWithGod > 150 || r.Next(0, 50) == 2)
                {
                    if (Cell.AddSprite(Sprites.Alls[r.Next(0, Sprites.Alls.Count)], i, j, gameBoardCells) == false)
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

            ConsoleBuffer.Draw(String.Format("  generation: {0}   population: {1:0000}  {2} ",
                                                generation++,
                                                population,
                                                needGod ? String.Format("God in action ({0})", generationWithGod) : "                       "),
                                0, 0, 15);

            ConsoleBuffer.Print();

            foreach (Cell c in gameBoardCells)
            {
                c.Cycle(gameBoardCells);
            }
        }

        public void DrawGameBoard(int consoleWidth, int consoleHeight)
        {
            Console.Clear();
            Console.CursorVisible = false;
            ConsoleBuffer = new ConsoleDoubleBuffer(Math.Max(consoleWidth, ConsoledMinimumWidth), Math.Max(consoleHeight, ConsoledMinimumHeight));
            // Init all cells of game board
            for (int i = 0; i < GameBoardWidth + 3; i++)
            {
                ConsoleBuffer.Draw("*", i, 1, 2);
                ConsoleBuffer.Draw("*", i, GameBoardHeight + 2, 2);
            }

            for (int i = 2; i < GameBoardHeight + 3; i++)
            {
                ConsoleBuffer.Draw("*", 0, i, 2);
                ConsoleBuffer.Draw("*", GameBoardWidth + 2, i, 2);
            }
        }
    }

    class Cell
    {
        static int _width;
        static int _height;

        static public void SetGameBoardSize(int width, int height)
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
                for (int j = PosY - 1; j < PosY + 2; j++)
                {
                    if (i != PosX || j != PosY) // skip current cell
                    {
                        if (i < 0 || j < 0 || i >= _width || j >= _height) // check if neighbour exists...
                        {
                            //aliveCounter++;
                        }
                        else if (gameBoardCells[i, j]._alive == true)
                        {
                            neighboursAliveCounter++;
                        }
                    }
                }
            }

            if (neighboursAliveCounter == 3 && _alive == false)
            {
                _nextAlive = true;
            }
            else if ((neighboursAliveCounter == 2 || neighboursAliveCounter == 3) && _alive == true)
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

            while (_historyHashs.Count > _historyLength)
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

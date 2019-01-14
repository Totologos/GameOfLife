using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Sprites
    {
        public readonly static List<Cell> Clown = new List<Cell>()
        {
            new Cell(0,0),
            new Cell(0,1),
            new Cell(0,2),
            new Cell(2,0),
            new Cell(2,1),
            new Cell(2,2),
            new Cell(1,2)
        };

        public readonly static List<Cell> Clown2 = new List<Cell>()
        {
            new Cell( 0, 0),
            new Cell( 1,-1),
            new Cell(-1,-1),
            new Cell( 1,-2),
            new Cell(-1,-2),
            new Cell( 1,-3),
            new Cell(-1,-3)
        };

        public readonly static List<Cell> HInvert = new List<Cell>()
        {
            new Cell(-1,-1),
            new Cell(-1, 0),
            new Cell(-1, 1),
            new Cell( 0, 0),
            new Cell( 1, 0),
            new Cell( 1, 1),
            new Cell( 1, 2)
        };

        public readonly static List<Cell> Flower = new List<Cell>()
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

        public readonly static List<List<Cell>> Alls = new List<List<Cell>>()
        {
            Clown,
            Clown2,
            HInvert,
            Flower

        };
    }
}

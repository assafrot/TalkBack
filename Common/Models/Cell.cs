using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Cell
    {
        public int NumOfCheckers { get; set; }
        public CheckerColor Color { get; set; }

        public Cell(int checkers = 0, CheckerColor color = CheckerColor.None)
        {
            NumOfCheckers = checkers;
            Color = color;
        }

        public bool IsEmpty()
        {
            return NumOfCheckers == 0;
        }

        public bool IsEdible(CheckerColor eaterColor)
        {
            if (NumOfCheckers != 1)
                return false;
            return eaterColor != Color;
        }

        public bool CanAddChecker(CheckerColor playerColor)
        {
            if (Color != playerColor && NumOfCheckers > 1)
                return false;
            return true;
        }
    }
}

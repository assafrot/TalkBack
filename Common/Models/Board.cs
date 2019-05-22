namespace Common.Models
{
    public class Board
    {

        public Cell[] GameBoard { get; set; }

        public Board()
        {
            GameBoard = new Cell[26];
            Init();
            //TestInit();
        }

        private void Init()
        {
            GameBoard[24] = new Cell(2, CheckerColor.Black);
            GameBoard[13] = new Cell(5, CheckerColor.Black);
            GameBoard[8] = new Cell(3, CheckerColor.Black);
            GameBoard[6] = new Cell(5, CheckerColor.Black);
            //eaten
            GameBoard[25] = new Cell(0, CheckerColor.Black);

            GameBoard[1] = new Cell(2, CheckerColor.White);
            GameBoard[12] = new Cell(5, CheckerColor.White);
            GameBoard[17] = new Cell(3, CheckerColor.White);
            GameBoard[19] = new Cell(5, CheckerColor.White);
            //eaten
            GameBoard[0] = new Cell(0, CheckerColor.White);

            for (int i = 0; i < GameBoard.Length; i++)
            {
                if (GameBoard[i] == null)
                    GameBoard[i] = new Cell();
            }
        }

        private void TestInit()
        {
            GameBoard[6] = new Cell(5, CheckerColor.Black);
            GameBoard[7] = new Cell(1, CheckerColor.Black);

            GameBoard[23] = new Cell(1, CheckerColor.White);
            GameBoard[24] = new Cell(1, CheckerColor.White);

            GameBoard[25] = new Cell(2, CheckerColor.Black);
            GameBoard[0] = new Cell(0, CheckerColor.White);

            for (int i = 0; i < GameBoard.Length; i++)
            {
                if (GameBoard[i] == null)
                    GameBoard[i] = new Cell();
            }
        }

        public bool GotEaten(CheckerColor color)
        {
            if (CheckerColor.Black == color && GameBoard[25].NumOfCheckers > 0)
                return true;
            if (CheckerColor.White == color && GameBoard[0].NumOfCheckers > 0)
                return true;
            return false;
        }
    }
}

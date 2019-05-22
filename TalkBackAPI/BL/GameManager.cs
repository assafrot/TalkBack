using Common.Models;
using System;

namespace TalkBackAPI.BL
{
    public class GameManager
    {

        Random rnd = new Random();
        public Board Board { get; set; }
        public string BlackUser { get; set; }
        public string WhiteUser { get; set; }
        public string Turn { get; set; }
        public TurnStatus TurnStatus { get; set; }
        public int[] Moves { get; set; }

        public GameManager(string user1, string user2)
        {
            BlackUser = user1;
            WhiteUser = user2;
            Turn = user2;
            Board = new Board();
            Moves = new int[] { 0, 0, 0, 0 };
        }

        public int[] Roll()
        {
            int[] res = new int[] { 0, 0 };
            if (TurnStatus != TurnStatus.RollDice)
                return res;
            int dice1 = rnd.Next(1, 6);
            int dice2 = rnd.Next(1, 6);
            Moves[0] = dice1;
            Moves[1] = dice2;
            //if double - 4 turns.
            if (dice1 == dice2)
            {
                Moves[2] = dice1;
                Moves[3] = dice1;
            }
            else
            {
                Moves[2] = 0;
                Moves[3] = 0;
            }
            res = new int[] { dice1, dice2 };
            if (!CanPlay())
                TurnStatus = TurnStatus.EndTurn;
            else
                TurnStatus = TurnStatus.Move;
            return res;
        }


        public bool PlayMove(int source, int target)
        {
            //check which color is current player
            if (TurnStatus != TurnStatus.Move)
                return false;
            CheckerColor color;
            if (Turn == BlackUser)
                color = CheckerColor.Black;
            else
                color = CheckerColor.White;
            //check if can move eaten.
            if (Board.GotEaten(color))
            {
                if (!CanPlayEaten(color))
                {
                    TurnStatus = TurnStatus.EndTurn;
                    return false;
                }
                //wrong move - don't end turn.
                if ((color == CheckerColor.Black && source != 25) || (color == CheckerColor.White && source != 0))
                    return false;
            }
            //if no eaten - check if can move in board.
            else if (!CanPlayInBoard(color))
            {
                TurnStatus = TurnStatus.EndTurn;
                return false;
            }
            //have legel moves on board - now, all other conditions
            if (AreAvailableMoves() && IsMoveExist(source, target, out int move)
                && CanMove(color, source, target))
            {
                PlayMove(source, target, color, move);
                return true;
            }
            return false;
        }

        //if can play - reach here
        private void PlayMove(int source, int target, CheckerColor color, int move)
        {
            Board.GameBoard[source].NumOfCheckers--;
            if (Board.GameBoard[source].NumOfCheckers == 0 && source != 0 && source != 25)
                Board.GameBoard[source].Color = CheckerColor.None;
            if (Board.GameBoard[target].IsEdible(color))
            {
                if (color == CheckerColor.Black)
                    Board.GameBoard[0].NumOfCheckers++;
                else
                    Board.GameBoard[25].NumOfCheckers++;
                Board.GameBoard[target].NumOfCheckers = 0;
            }
            if (Board.GameBoard[target].NumOfCheckers == 0)
                Board.GameBoard[target].Color = color;
            Board.GameBoard[target].NumOfCheckers++;
            Moves[move] = 0;
            if (!AreAvailableMoves() || !CanPlay())
            {
                TurnStatus = TurnStatus.EndTurn;
            }
        }
        //checks for the min value in dice for playing the move.
        public bool ThrowChecker(int source)
        {
            int distance;
            if (Turn == BlackUser && CanThrowCheckers(CheckerColor.Black))
                distance = source;
            else
                distance = 25 - source;

            int move = 7;
            int index = 5;
            for (int i = 0; i < Moves.Length; i++)
            {
                if (Moves[i] >= distance && Moves[i] < move)
                {
                    move = Moves[i];
                    index = i;
                }
            }
            // if a real move exist on dice.
            if (move < 7)
            {
                Moves[index] = 0;
                Board.GameBoard[source].NumOfCheckers--;
                if (Board.GameBoard[source].NumOfCheckers == 0)
                    Board.GameBoard[source].Color = CheckerColor.None;
                if (!AreAvailableMoves())
                {
                    TurnStatus = TurnStatus.EndTurn;
                }
                return true;
            }
            return false;
        }

        public bool CanThrowCheckers(CheckerColor color)
        {
            int start = 7;
            int end = 24;
            if (color == CheckerColor.White)
            {
                start -= 6;
                end -= 6;
            }
            for (int i = start; i <= end; i++)
            {
                if (Board.GameBoard[i].Color == color) return false;
            }
            return true;
        }

        private bool CanPlayInBoard(CheckerColor color)
        {
            if (CanThrowCheckers(color))
            {
                if (color == CheckerColor.Black)
                {
                    for (int i = 1; i < 7; i++)
                    {
                        if (CanThrow(i)) return true;
                    }
                }
                else
                {
                    for (int i = 19; i < 25; i++)
                    {
                        if (CanThrow(i)) return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Moves.Length; i++)
                {
                    for (int j = 1; j < Board.GameBoard.Length - 1; j++)
                    {
                        if (Board.GameBoard[j].Color == color && color == CheckerColor.Black &&
                           CanMove(color, j, j - Moves[i]))
                            return true;
                        if (Board.GameBoard[j].Color == color && color == CheckerColor.White &&
                            CanMove(color, j, j + Moves[i]))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool CanPlayEaten(CheckerColor color)
        {
            for (int i = 0; i < Moves.Length; i++)
            {
                if (Moves[i] != 0 && CanMoveEaten(color, Moves[i])) return true;
            }

            return false;
        }

        private bool CanPlay()
        {
            CheckerColor color;
            if (Turn == BlackUser)
                color = CheckerColor.Black;
            else
                color = CheckerColor.White;
            if (Board.GotEaten(color))
            {
                return CanPlayEaten(color);
            }
            else
            {
                return CanPlayInBoard(color);
            }
        }

        public void SwitchTurns()
        {
            if (Turn == BlackUser)
                Turn = WhiteUser;
            else
                Turn = BlackUser;
            TurnStatus = TurnStatus.RollDice;
        }

        private bool CanMoveEaten(CheckerColor color, int steps)
        {
            if (CheckerColor.Black == color)
            {
                if (Board.GameBoard[25 - steps].IsEmpty() || Board.GameBoard[25 - steps].IsEdible(color)
                    || Board.GameBoard[25 - steps].Color == CheckerColor.Black) return true;
            }

            if (CheckerColor.White == color)
            {
                if (Board.GameBoard[steps].IsEmpty() || Board.GameBoard[steps].IsEdible(color)
                    || Board.GameBoard[steps].Color == CheckerColor.White)
                    return true;
            }
            return false;
        }

        private bool CanMove(CheckerColor color, int source, int target)
        {
            //out of range
            if (target < 1 || target > 24)
                return false;
            //right player
            if (color != Board.GameBoard[source].Color)
                return false;
            //wrong direction
            if ((color == CheckerColor.Black && source <= target)
                || (color == CheckerColor.White && target <= source))
                return false;
            //If source is empty cell
            if (Board.GameBoard[source].IsEmpty())
                return false;
            //Target is occupied
            if (!Board.GameBoard[target].CanAddChecker(color))
                return false;
            return true;
        }

        private bool CanThrow(int source)
        {
            int distance;
            if (Turn == BlackUser && CanThrowCheckers(CheckerColor.Black))
                distance = source;
            else
                distance = 25 - source;
            for (int i = 0; i < Moves.Length; i++)
            {
                if (Moves[i] >= distance)
                    return true;
            }
            return false;
        }
        //Available moves on dice
        private bool AreAvailableMoves()
        {
            for (int i = 0; i < Moves.Length; i++)
            {
                if (Moves[i] != 0) return true;
            }
            return false;
        }

        private bool IsMoveExist(int source, int target, out int diceResult)
        {
            diceResult = 0;
            int steps = Math.Abs(source - target);
            for (int i = 0; i < Moves.Length; i++)
            {
                if (Moves[i] == steps)
                {
                    diceResult = i;
                    return true;
                }
            }
            return false;
        }

        public bool IsWinner()
        {
            CheckerColor color;
            if (Turn == BlackUser)
                color = CheckerColor.Black;
            else
                color = CheckerColor.White;
            for (int i = 0; i < Board.GameBoard.Length; i++)
            {
                if (Board.GameBoard[i].Color == color && Board.GameBoard[i].NumOfCheckers > 0)
                    return false;
            }
            TurnStatus = TurnStatus.GameOver;
            return true;
        }
    }
}
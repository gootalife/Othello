using NUnit.Framework;
using Othello.Lib;
using System.Drawing;

namespace Othello.Test {
    public class Tests {
        private OthelloGame othello;
        [SetUp]
        public void Setup() {
            othello = new OthelloGame();
        }

        [Test]
        public void InitTest() {
            var board = new Cell[8, 8];
            for(int y = 0; y < board.GetLength(0); y++) {
                for(int x = 0; x < board.GetLength(1); x++) {
                    board[y, x] = new Cell {
                        State = BoardState.EMPTY,
                        Point = new Point(x, y)
                    };
                }
            }
            board[3, 3].State = board[4, 4].State = BoardState.WHITE;
            board[3, 4].State = board[4, 3].State = BoardState.BLACK;
            Assert.AreEqual(othello.Board.SelectOnBoard(cell => cell.State), board.SelectOnBoard(cell => cell.State));
            Assert.AreEqual(othello.Board.SelectOnBoard(cell => cell.Point.X), board.SelectOnBoard(cell => cell.Point.X));
            Assert.AreEqual(othello.Board.SelectOnBoard(cell => cell.Point.Y), board.SelectOnBoard(cell => cell.Point.Y));
        }
    }
}
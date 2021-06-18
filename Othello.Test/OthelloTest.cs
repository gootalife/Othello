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
            board.ForEachOnBoard(pos => {
                board[pos.Y, pos.X] = new Cell {
                    State = BoardState.EMPTY,
                    Point = new Point(pos.X, pos.Y)
                };
            });
            board[3, 3].State = board[4, 4].State = BoardState.WHITE;
            board[3, 4].State = board[4, 3].State = BoardState.BLACK;
            Assert.AreEqual(othello.Board, board);
        }

        [Test]
        public void InputTest() {
            var board = new Cell[8, 8];
            board.ForEachOnBoard(pos => {
                board[pos.Y, pos.X] = new Cell {
                    State = BoardState.EMPTY,
                    Point = new Point(pos.X, pos.Y)
                };
            });
            board[3, 3].State = board[4, 4].State = BoardState.WHITE;
            board[3, 4].State = board[4, 3].State = BoardState.BLACK;
            Assert.AreEqual(othello.Board, board);
        }


    }
}
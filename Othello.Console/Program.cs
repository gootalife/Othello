using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using Othello.Lib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CSL = System.Console;

namespace Othello.Console {
    public class Program : ConsoleAppBase {
        private const string esc = "\u001b";

        public static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
        }

        public void Run() {
            var othello = new OthelloGame();
            while(true) {
                ClearConsole();
                othello.SetCanPut();
                PrintBoard(othello.Board, othello.Player);
                // 置ける場所があるなら置く
                if (!othello.PassJudge()) {
                    var pos = GetPosition(othello.Board);
                    othello.Put(pos);
                    othello.TurnOver(pos);
                } else if(othello.OpponentPassed) {
                    CSL.WriteLine("ゲーム終了\n");
                    othello.EndGame();
                } else {
                    CSL.WriteLine("パスします\n");
                    othello.Pass();
                    continue;
                }
                if(othello.IsOver) {
                    break;
                }
                othello.EndTurn();
            }
            var winner = othello.Judge();
            if(winner is Player.BLACK) {
                CSL.Write("黒");
                CSL.ForegroundColor = ConsoleColor.Black;
                CSL.BackgroundColor = ConsoleColor.Green;
                CSL.Write(GetStateString(BoardState.BLACK));
                CSL.ResetColor();
                CSL.WriteLine("の勝ちです。");
            } else if(winner is Player.WHITE) {
                CSL.Write("黒");
                CSL.ForegroundColor = ConsoleColor.Black;
                CSL.BackgroundColor = ConsoleColor.Green;
                CSL.Write(GetStateString(BoardState.BLACK));
                CSL.ResetColor();
                CSL.WriteLine("の勝ちです。");
            } else {
                CSL.WriteLine("引き分けです。");
            }
            CSL.ReadKey();
        }

        private static void ClearConsole() {
            CSL.Write($"{esc}[2J");
            CSL.Write($"{esc}[1;1H");
        }

        private static void PrintBoard(Cell[,] board, Player player) {
            CSL.BackgroundColor = ConsoleColor.Black;
            CSL.ForegroundColor = ConsoleColor.White;
            CSL.Write("   ");
            for(int x = 0; x < board.GetLength(0); x++) {
                CSL.Write($"{x,2}");
            }
            CSL.WriteLine();
            CSL.Write("  +");
            for(int x = 0; x < board.GetLength(0); x++) {
                CSL.Write("--");
            }
            CSL.WriteLine();
            for(int y = 0; y < board.GetLength(0); y++) {
                CSL.ForegroundColor = ConsoleColor.White;
                CSL.Write($"{y,2}|");
                CSL.ForegroundColor = ConsoleColor.Black;
                CSL.BackgroundColor = ConsoleColor.Green;
                for(int x = 0; x < board.GetLength(1); x++) {
                    CSL.Write(GetStateString(board[y, x].State));
                }
                CSL.BackgroundColor = ConsoleColor.Black;
                CSL.WriteLine();
            }
            CSL.ResetColor();
            CSL.WriteLine();
            if(player == Player.BLACK) {
                CSL.Write("黒");
                CSL.ForegroundColor = ConsoleColor.Black;
                CSL.BackgroundColor = ConsoleColor.Green;
                CSL.Write(GetStateString(BoardState.BLACK));
                CSL.BackgroundColor = ConsoleColor.Black;
            } else if(player == Player.WHITE) {
                CSL.Write("白");
                CSL.ForegroundColor = ConsoleColor.Black;
                CSL.BackgroundColor = ConsoleColor.Green;
                CSL.Write(GetStateString(BoardState.WHITE));
                CSL.BackgroundColor = ConsoleColor.Black;
            }
            CSL.ResetColor();
            CSL.WriteLine("の番です。");
            CSL.WriteLine();
        }

        private static Point GetPosition(Cell[,] board) {
            IEnumerable<int> input;
            var point = new Point();
            var err = "";
            while(true) {
                try {
                    CSL.WriteLine(err);
                    CSL.Write("座標を入力[x y]>");
                    input = CSL.ReadLine().Split(' ').Select(elem => int.Parse(elem));
                    if(input.Count() != 2) {
                        throw new Exception("引数の数が不正です。");
                    }
                    point = new Point(input.ElementAt(0), input.ElementAt(1));
                    if(board[point.Y, point.X].State != BoardState.CAN_PUT) {
                        throw new Exception("そこには置けません。");
                    }
                    break;
                } catch(Exception e) {
                    err = e.Message;
                    CSL.Write($"{esc}[2F");
                    CSL.Write($"{esc}[0J");
                    continue;
                }
            }
            return point;
        }

        private static string GetStateString(BoardState boardState) {
            string str = "";
            switch(boardState) {
                case BoardState.BLACK:
                    str = "●";
                    break;
                case BoardState.WHITE:
                    str = "○";
                    break;
                case BoardState.CAN_PUT:
                    str = "☆";
                    break;
                case BoardState.EMPTY:
                    str = "□";
                    break;
            }
            return str;
        }
    }
}

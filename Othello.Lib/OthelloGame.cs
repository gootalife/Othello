using Othello.Lib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Othello.Lib {
    public class OthelloGame {
        public const int BOARD_SIZE = 8;
        public Cell[,] Board { get; private set; }
        public bool IsOver { get; private set; }
        public bool OpponentPassed { get; private set; }
        //  1:黒(先攻)
        // -1:白(後攻)
        public Player Player { get; private set; }

        public OthelloGame() {
            IsOver = false;
            OpponentPassed = false;
            Player = Player.BLACK;
            Board = new Cell[BOARD_SIZE, BOARD_SIZE];
            for(int y = 0; y < Board.GetLength(0); y++) {
                for(int x = 0; x < Board.GetLength(1); x++) {
                    Board[y, x] = new Cell {
                        State = BoardState.EMPTY,
                        Point = new Point(x, y)
                    };
                }
            }

            Board[3, 3].State = Board[4, 4].State = BoardState.WHITE;
            Board[3, 4].State = Board[4, 3].State = BoardState.BLACK;
        }

        /// <summary>
        /// ターン終了処理
        /// </summary>
        public void EndTurn() {
            Player = Player == Player.BLACK ? Player.WHITE : Player.BLACK;
        }

        /// <summary>
        /// ゲーム終了処理
        /// </summary>
        public void EndGame() {
            IsOver = true;
        }

        /// <summary>
        /// パス判定
        /// </summary>
        /// <returns></returns>
        public bool PassJudge() {
            return !Board.WhereOnBoard(cell => cell.State is BoardState.EMPTY or BoardState.CAN_PUT).Any();
        }

        /// <summary>
        /// パス時の処理
        /// </summary>
        public void Pass() {
            OpponentPassed = true;
        }

        /// <summary>
        /// 勝者を判定する
        /// </summary>
        /// <returns></returns>
        public Player? Judge() {
            var p1 = Board.WhereOnBoard(cell => cell.State is BoardState.BLACK).Length;
            var p2 = Board.WhereOnBoard(cell => cell.State is BoardState.WHITE).Length;
            Player? winner = null;
            if(p1 > p2) {
                winner = Player.BLACK;
            } else if(p2 > p1) {
                winner = Player.WHITE;
            }
            return winner;
        }

        /// <summary>
        /// 指定した座標に現在のプレイヤーの石を置く
        /// </summary>
        /// <param name="pos"></param>
        public void Put(Point pos) {
            OpponentPassed = false;
            Board[pos.Y, pos.X].State = Player == Player.BLACK ? BoardState.BLACK : BoardState.WHITE;
        }

        /// <summary>
        /// 8方向を探索して裏返す
        /// </summary>
        /// <param name="point"></param>
        public void TurnOver(Point point) {
            for(int dy = -1; dy <= 1; dy++) {
                for(int dx = -1; dx <= 1; dx++) {
                    var line = new List<Cell>();
                    if(dx == 0 && dy == 0) { continue; }
                    for(int i = 1; i < BOARD_SIZE; i++) {
                        var py = point.Y + dy * i;
                        var px = point.X + dx * i;
                        if(px < 0 || py < 0 || px >= BOARD_SIZE || py >= BOARD_SIZE) {
                            continue;
                        }
                        var c = Board[py, px];
                        if(c.State is BoardState.EMPTY or BoardState.CAN_PUT) {
                            // 空が間にあるなら探索終了
                            break;
                        } else if((int)c.State == (int)Player) {
                            // 自分の石が来たとき探索終了
                            line.Add(Board[py, px]);
                            break;
                        } else {
                            // 相手の石が来たなら列に追加
                            line.Add(Board[py, px]);
                        }
                    }
                    // 列が空なら次の探索へ
                    if(!line.Any()) {
                        continue;
                    }
                    // 列の最後が自分の石なら列を自分の石にする
                    if(line.Last().State == (Player == Player.BLACK ? BoardState.BLACK : BoardState.WHITE)) {
                        line.ForEach(cell => Board[cell.Point.Y, cell.Point.X].State = Player == Player.BLACK ? BoardState.BLACK : BoardState.WHITE);
                    }
                }
            }
        }

        /// <summary>
        /// 石を置ける場所を設定する
        /// </summary>
        public void SetCanPut() {
            Board = Board.SelectOnBoard(cell => {
                if(cell.State == BoardState.CAN_PUT) {
                    cell.State = BoardState.EMPTY;
                }
                return cell;
            });
            var canPutCells = Board.WhereOnBoard(cell => CheckCanPut(Board[cell.Point.Y, cell.Point.X]));
            canPutCells.ForEach(cell => Board[cell.Point.Y, cell.Point.X].State = BoardState.CAN_PUT);
        }

        /// <summary>
        /// 石を置けるか判定する
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool CheckCanPut(Cell cell) {
            var result = false;
            // 空のセルについてのみ判定を継続する
            if(cell.State == BoardState.EMPTY) {
                for(int dy = -1; dy <= 1; dy++) {
                    // 置けることが既にわかっているなら終了
                    if(result) {
                        break;
                    }
                    for(int dx = -1; dx <= 1; dx++) {
                        if(dx == 0 && dy == 0) { continue; }
                        if(CheckLine(cell, dx, dy)) {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 一直線上に相手の石が続き、自分の石があるかどうか
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private bool CheckLine(Cell cell, int dx, int dy) {
            var result = false;
            var lineFlag = false;
            for(int i = 1; i < BOARD_SIZE; i++) {
                var py = cell.Point.Y + dy * i;
                var px = cell.Point.X + dx * i;
                if(px < 0 || py < 0 || px >= BOARD_SIZE || py >= BOARD_SIZE) {
                    continue;
                }
                var c = Board[py, px];
                if(c.State is BoardState.EMPTY or BoardState.CAN_PUT) {
                    // 空が間にあるならfalse
                    result = false;
                    break;
                } else if((int)c.State == (int)Player) {
                    // 自分の石が来たとき探索終了。間に相手の色があるならtrue
                    if(lineFlag) {
                        result = true;
                    }
                    break;
                } else {
                    // 相手の石が来たならラインフラグを立てて継続
                    lineFlag = true;
                }
            }
            return result;
        }
    }
}

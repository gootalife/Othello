using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello.Lib {
    public static class ArrayExtensions {
        public static void ForEach<T>(this T[] array, Action<T> action) {
            for(int i = 0; i < array.Length; i++) {
                action(array[i]);
            }
        }
        public static void ForEachOnBoard<T>(this T[,] cells, Action<T> action) {
            for(int y = 0; y < cells.GetLength(0); y++) {
                for(int x = 0; x < cells.GetLength(1); x++) {
                    action(cells[y, x]);
                }
            }
        }

        public static void ForEachOnAround<T>(this T[,] cells, Action<Point> action) {
            for(int dy = -1; dy <= 1; dy++) {
                for(int dx = -1; dx <= 1; dx++) {
                    if(dx == 0 && dy == 0) { continue; }
                    action(new Point(dx, dy));
                }
            }
        }

        public static TResult[,] SelectOnBoard<T, TResult>(this T[,] cells, Func<T, TResult> func) {
            var newCells = new TResult[cells.GetLength(0), cells.GetLength(1)];
            for(int y = 0; y < cells.GetLength(0); y++) {
                for(int x = 0; x < cells.GetLength(1); x++) {
                    newCells[y, x] = func(cells[y, x]);
                }
            }
            return newCells;
        }

        public static TResult[,] SelectOnAround<T, TResult>(this T[,] cells, Func<T, TResult> func) {
            var newCells = new TResult[cells.GetLength(0), cells.GetLength(1)];
            for(int dy = -1; dy <= 1; dy++) {
                for(int dx = -1; dx <= 1; dx++) {
                    if(dx == 0 && dy == 0) { continue; }
                    newCells[dy, dy] = func(cells[dy, dy]);
                }
            }
            return newCells;
        }

        public static T[] WhereOnBoard<T>(this T[,] cells, Func<T, bool> func) {
            var list = new List<T>();
            for(int y = 0; y < cells.GetLength(0); y++) {
                for(int x = 0; x < cells.GetLength(1); x++) {
                    if(func(cells[y, x])) {
                        list.Add(cells[y, x]);
                    }
                }
            }
            return list.ToArray();
        }

        public static T[] WhereOnAround<T>(this T[,] cells, Func<T, bool> func) {
            var list = new List<T>();
            for(int dy = -1; dy <= 1; dy++) {
                for(int dx = -1; dx <= 1; dx++) {
                    if(dx == 0 || dy == 0) {
                        continue;
                    }
                    if(func(cells[dy, dx])) {
                        list.Add(cells[dy, dx]);
                    }
                }
            }
            return list.ToArray();
        }
    }
}

using NUnit.Framework;
using Othello.Lib;
using System.Drawing;
using System.Reflection;

namespace Othello.Console.Test {
    public class Tests {
        private OthelloGame othello;
        [SetUp]
        public void Setup() {
        }

        [TestCase(BoardState.BLACK, "��")]
        [TestCase(BoardState.WHITE, "��")]
        [TestCase(BoardState.CAN_PUT, "��")]
        [TestCase(BoardState.EMPTY, "��")]
        public void InputTest(BoardState boardState, string result) {
            var p = new Program();
            var method = p.GetType().GetMethod("GetStateString", BindingFlags.NonPublic | BindingFlags.Static);
            var str = (string)method.Invoke(null, new object[] { boardState });
            Assert.AreEqual(str, result);
        }


    }
}
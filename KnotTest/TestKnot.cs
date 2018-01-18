using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnotThatFast.Models;
using System.Collections.Generic;

namespace KnotTest
{
    [TestClass]
    public class TestKnot
    {
        [TestMethod]
        public void TestIsGaussCodeCorrectOK()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3 });
            PrivateObject obj = new PrivateObject(knot);
            var test = obj.Invoke("IsGaussCodeCorrect");
            Assert.AreEqual(test, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestIsGaussCodeCorrectException()
        {
            Knot knot = new Knot(new List<int>() { 1, 3, -1, 2, -3 });
        }

        [TestMethod]
        public void TestUnknotIsOK()
        {
            Knot knot = new Knot();
            PrivateObject obj = new PrivateObject(knot);
            var test = obj.Invoke("IsGaussCodeCorrect");
            Assert.AreEqual(test, true);
        }


        [TestMethod]
        public void TestSolve1OK()
        {
            //Figure eight with an extra twist
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 4, -4, 5, -2, 1, -5, 3 });
            Knot solved = new Knot(new List<int>() { -1, 2, -3, 5, -2, 1, -5, 3 });
            knot = Knot.Solve(knot);
            Assert.AreEqual(solved, knot);
        }

        [TestMethod]
        public void TestSolve2OK()
        {
            //Figure eight with an extra twist
            Knot knot = new Knot(new List<int>() { 1, -2, 3, 4, 5, 6, -6, -5, -4, -1, 2, -3 });
            Knot solved = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3 });
            knot = Knot.Solve(knot);
            Assert.AreEqual(solved, knot);
        }

        [TestMethod]
        public void TestSolveHardUnknot()
        {
            Knot knot = new Knot(new List<int>() { 4, -1, 2, 5, 8, -9, 10, -7, 6, -3, 1, -2, 3, -4, -5, -6, 7, -8, 9, -10});
            Knot unknot = new Knot();
            knot = Knot.Solve(knot);
            Assert.AreEqual(knot, unknot);
        }

        [TestMethod]
        public void TestEqualKnotsOK()
        {
            Knot k1 = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3 });
            Knot k2 = new Knot(new List<int>() { -3, 1, -2, 3, -1, 2 });
            Assert.AreEqual(k1, k2);
        }

        [TestMethod]
        public void TestEqualKnotsFail()
        {
            Knot k1 = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3 });
            Knot k2 = new Knot(new List<int>() { -2, 1, -3, 3, -1, 2 });
            Assert.AreNotEqual(k1, k2);
        }

        [TestMethod]
        public void TestMove1IsOK()
        {
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 4, -4, 5, -2, 1, -5, 3 });
            PrivateObject obj = new PrivateObject(knot);
            int? test = (int?)obj.Invoke("getPositionForMove1");
            if (test.HasValue)
                Assert.AreEqual(test, 3);
            else
                Assert.Fail();
        }

        [TestMethod]
        public void TestMove2IsOK()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, 4, 5, 6, -6, -5, -4, -1, 2, -3 });
            PrivateObject obj = new PrivateObject(knot);
            Tuple<int, int> test = (Tuple<int, int>)obj.Invoke("getPositionsForMove2");
            if (test != null)
            {
                Assert.AreEqual(test.Item1, 3);
                Assert.AreEqual(test.Item2, 7);
            }
            else
                Assert.Fail();
        }
    }
}

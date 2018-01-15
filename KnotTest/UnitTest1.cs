using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnotThatFast.Models;
using System.Collections.Generic;

namespace KnotTest
{
    [TestClass]
    public class UnitTest
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
        public void TestSolveOK()
        {
            //Figure eight with an extra twist
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 4, -4, 5, -2, 1, -5, 3 });
            Knot solved = new Knot(new List<int>() { -1, 2, -3, 4, -2, 1, -4, 3 });
            knot = Knot.Solve(knot);
            Assert.AreEqual(solved, knot);
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
    }
}

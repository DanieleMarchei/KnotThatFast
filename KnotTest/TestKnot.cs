﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnotThatFast.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        public void TestSolveHardUnknot1()
        {
            Knot knot = new Knot(new List<int>() { 4, -1, 2, 5, 8, -9, 10, -7, 6, -3, 1, -2, 3, -4, -5, -6, 7, -8, 9, -10 });
            knot = Knot.Solve(knot);
            Assert.IsTrue(knot.IsUnknot);
        }

        [TestMethod]
        public void TestSolveHardKnot1()
        {
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 1, -2, 3, -4, 5, -6, 7, 8, 4, -5, 6, -7, -8 });
            Knot solved = new Knot(new List<int>() { -1, 2, -3, 1, -2, 3, -4, 5, -6, 4, -5, 6 });
            knot = Knot.Solve(knot);
            Assert.AreEqual(solved, knot);
        }

        [TestMethod]
        public void TestIsValidTangleTrue1()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3, 4, -4 });
            Tangle tangle = new Tangle(new int[] { 4, 3 });
            PrivateObject obj = new PrivateObject(knot);
            bool isValid = (bool)obj.Invoke("IsValidTangle", tangle);
            Assert.AreEqual(true, isValid);
        }

        [TestMethod]
        public void TestIsValidTangleTrue2()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3, 4, -4 });
            Tangle tangle = new Tangle(new int[] { 3, 4 });
            PrivateObject obj = new PrivateObject(knot);
            bool isValid = (bool)obj.Invoke("IsValidTangle", tangle);
            Assert.AreEqual(true, isValid);
        }

        [TestMethod]
        public void TestIsValidTangleFalse1()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3, 4, -4 });
            Tangle tangle = new Tangle(new int[] { 4, 2 });
            PrivateObject obj = new PrivateObject(knot);
            bool isValid = (bool)obj.Invoke("IsValidTangle", tangle);
            Assert.AreEqual(false, isValid);
        }

        [TestMethod]
        public void TestIsValidTangleFalse2()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3, 4, -4 });
            Tangle tangle = new Tangle(new int[] { 2, 4 });
            PrivateObject obj = new PrivateObject(knot);
            bool isValid = (bool)obj.Invoke("IsValidTangle", tangle);
            Assert.AreEqual(false, isValid);
        }

        [TestMethod]
        public void TestIsValidTangleFalse3()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3, 4, -4 });
            Tangle tangle = new Tangle(new int[] { 1, 3 });
            PrivateObject obj = new PrivateObject(knot);
            bool isValid = (bool)obj.Invoke("IsValidTangle", tangle);
            Assert.AreEqual(false, isValid);
        }

        [TestMethod]
        public void TestIsValidTangleFalse4()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3, 4, -4 });
            Tangle tangle = new Tangle(new int[] { 3, 1 });
            PrivateObject obj = new PrivateObject(knot);
            bool isValid = (bool)obj.Invoke("IsValidTangle", tangle);
            Assert.AreEqual(false, isValid);
        }

        [TestMethod]
        public void TestEqualKnotsOK()
        {
            Knot k1 = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3 });
            Knot k2 = new Knot(new List<int>() { -3, 1, -2, 3, -1, 2 });
            Assert.AreEqual(true, k1 == k2);
        }

        [TestMethod]
        public void TestEqualKnotsFail()
        {
            Knot k1 = new Knot(new List<int>() { 1, -2, 3, -1, 2, -3 });
            Knot k2 = new Knot(new List<int>() { -2, 1, -3, 3, -1, 2 });
            Assert.AreNotEqual(true, k1 == k2);
        }

        [TestMethod]
        public void TestMove1IsOK()
        {
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 4, -4, 5, -2, 1, -5, 3 });
            PrivateObject obj = new PrivateObject(knot);
            int? test = (int?)obj.Invoke("getPositionForReductionMove1");
            if (test.HasValue)
                Assert.AreEqual(4, test);
            else
                Assert.Fail();
        }

        [TestMethod]
        public void TestMove2IsOK()
        {
            Knot knot = new Knot(new List<int>() { 1, -2, 3, 4, 5, 6, -6, -5, -4, -1, 2, -3 });
            PrivateObject obj = new PrivateObject(knot);
            int[] test = (int[])obj.Invoke("getPositionsForReductionMove2");
            
            if (test != null)
            {
                Assert.AreEqual(4, test[0]);
                Assert.AreEqual(5, test[1]);
            }
            else
                Assert.Fail();
        }

        [TestMethod]
        public void TestKnotTangleIsContiguousTrue()
        {
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 1, -2, 3, -4, 5, -6, 7, 8, 4, -5, 6, -7, -8 });
            PrivateObject obj = new PrivateObject(knot);
            Tangle t = new Tangle(new int[] { 1, 2, 3 });
            bool test = (bool)obj.Invoke("IsContiguousTangle", t);
            Assert.AreEqual(true, test);
        }

        [TestMethod]
        public void TestKnotTangleIsContiguousFalse()
        {
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 1, -2, 3, -4, 5, -6, 7, 8, 4, -5, 6, -7, -8 });
            PrivateObject obj = new PrivateObject(knot);
            Tangle t = new Tangle(new int[] { 5, 6, 7 });
            bool test = (bool)obj.Invoke("IsContiguousTangle", t);
            Assert.AreEqual(false, test);
        }

        [TestMethod]
        public void TestKnotEndsContiguousTangle()
        {
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 1, -2, 3, -4, 5, -6, 7, 8, 4, -5, 6, -7, -8 });
            PrivateObject obj = new PrivateObject(knot);
            Tangle t = new Tangle(new int[] { 1, 2, 3 });
            int[] test = (int[])obj.Invoke("GetTangleEnds", t);
            Assert.AreEqual(2, test.Length);
            Assert.AreEqual(15, test[0]);
            Assert.AreEqual(6, test[1]);
        }

        [TestMethod]
        public void TestKnotEndsNotContiguousTangle()
        {
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 1, -2, 3, -4, 5, -6, 7, 8, 4, -5, 6, -7, -8 });
            PrivateObject obj = new PrivateObject(knot);
            Tangle t = new Tangle(new int[] { 5, 6, 7 });
            int[] test = (int[])obj.Invoke("GetTangleEnds", t);
            Assert.AreEqual(4, test.Length);
            Assert.AreEqual(6, test[0]);
            Assert.AreEqual(10, test[1]);
            Assert.AreEqual(11, test[2]);
            Assert.AreEqual(15, test[3]);
        }

        [TestMethod]
        public void TestFactorEasy1()
        {
            Knot knot = new Knot(new List<int>() { -1, 2, -3, 1, -2, 3, -4, 5, -6, 4, -5, 6, -7, 8, -9, 10, -8, 7, -10, 9 });
            int[] expected = new int[3] {3, 3, 4};
            int[] factors = Knot.Factorize(knot);
            bool isEqual = Enumerable.SequenceEqual(expected, factors);
            Assert.AreEqual(true, isEqual);
        }
    }
}

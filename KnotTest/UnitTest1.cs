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
    }
}

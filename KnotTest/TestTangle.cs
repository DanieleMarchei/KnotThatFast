using System;
using System.Collections.Generic;
using KnotThatFast.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KnotTest
{
    [TestClass]
    public class TestTangle
    {
        [TestMethod]
        public void TestTangleHashOK()
        {
            Tangle t1 = new Tangle( new int[] { 2, -1, 3 });
            Tangle t2 = new Tangle(new int[] { 1, -2, -3 });
            Tangle t3 = new Tangle(new int[] { -1, 3, 2 });
            Tangle t4 = new Tangle(new int[] { -1, -4, 3, 2, 4 });

            int h1 = t1.Hash();
            int h2 = t2.Hash();
            int h3 = t3.Hash();
            int h4 = t4.Hash();

            Assert.AreEqual(h1, h2);
            Assert.AreEqual(h2, h3);
            Assert.AreEqual(h3, h4);
        }
    }
}

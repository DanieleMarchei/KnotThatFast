using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnotThatFast.Extensions;

namespace KnotTest
{
    [TestClass]
    public class TestMovableCircularList
    {
        [TestMethod]
        public void TestCircularOK()
        {
            MovableCircularList<int> movable = new MovableCircularList<int>() { 1, 2, 3, 4 };
            Assert.AreEqual(1, movable[movable.Count]);
        }

        [TestMethod]
        public void TestMovable1OK()
        {
            MovableCircularList<int> movable = new MovableCircularList<int>() { 1, 2, 3, 4 };
            movable.Move(0, 2);
            Assert.AreEqual(2, movable[0]);
            Assert.AreEqual(3, movable[1]);
            Assert.AreEqual(1, movable[2]);
            Assert.AreEqual(4, movable[3]);

        }

        [TestMethod]
        public void TestMovable2OK()
        {
            MovableCircularList<int> movable = new MovableCircularList<int>() { 1, 2, 3, 4 };
            movable.Move(0, 6);
            Assert.AreEqual(2, movable[0]);
            Assert.AreEqual(3, movable[1]);
            Assert.AreEqual(1, movable[2]);
            Assert.AreEqual(4, movable[3]);
        }

        [TestMethod]
        public void TestMovableEqualsOK()
        {
            MovableCircularList<int> m1 = new MovableCircularList<int>() { 1, 2, 3, 4 };
            MovableCircularList<int> m2 = new MovableCircularList<int>() { 2, 3, 4, 1 };
            MovableCircularList<int> m3 = new MovableCircularList<int>() { 3, 4, 1, 2 };
            MovableCircularList<int> m4 = new MovableCircularList<int>() { 4, 1, 2, 3 };

            Assert.AreEqual(m1, m2);
            Assert.AreEqual(m2, m3);
            Assert.AreEqual(m3, m4);
        }

        [TestMethod]
        public void TestMovableSetOK()
        {
            MovableCircularList<int> m1 = new MovableCircularList<int>() { 1, 2, 3, 4 };
            m1[2] = 5;
            Assert.AreEqual(5, m1[2]);
        }
    }
}

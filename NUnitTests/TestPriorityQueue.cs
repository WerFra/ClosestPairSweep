using ClosestPairSweep;
using NUnit.Framework;
using System;

namespace NUnitTests {
    [TestFixture]
    public class TestPriorityQueue {

        [Test]
        public void TestInt() {
            var lcl_Queue = new PriorityQueue<int, int>();
            lcl_Queue.Push((4, 4));
            lcl_Queue.Push((9, 9));
            lcl_Queue.Push((8, 8));
            lcl_Queue.Push((2, 2));
            lcl_Queue.Push((0, 0));
            lcl_Queue.Push((5, 5));
            System.Console.WriteLine(lcl_Queue);
            Assert.AreEqual(0, lcl_Queue.Pop());
            Assert.AreEqual(2, lcl_Queue.Pop());
            Assert.AreEqual(4, lcl_Queue.Pop());
            Assert.AreEqual(5, lcl_Queue.Pop());
            Assert.AreEqual(8, lcl_Queue.Pop());
            Assert.AreEqual(9, lcl_Queue.Pop());
        }

        [Test]
        public void TestReal() {
            var lcl_Queue = new PriorityQueue<double, double>();
            lcl_Queue.Push((187, 187));
            lcl_Queue.Push((179, 179));
            lcl_Queue.Push((398, 398));
            lcl_Queue.Push((393, 393));
            lcl_Queue.Push((635, 635));
            lcl_Queue.Push((625, 625));
            lcl_Queue.Push((240, 240));
            lcl_Queue.Push((611, 611));
            lcl_Queue.Push((513, 513));
            lcl_Queue.Push((395, 395));
            System.Console.WriteLine(lcl_Queue);
            Assert.AreEqual(179, lcl_Queue.Pop());
            Assert.AreEqual(187, lcl_Queue.Pop());
            Assert.AreEqual(240, lcl_Queue.Pop());
            Assert.AreEqual(393, lcl_Queue.Pop());
            Assert.AreEqual(395, lcl_Queue.Pop());
            Assert.AreEqual(398, lcl_Queue.Pop());
            Assert.AreEqual(513, lcl_Queue.Pop());
            Assert.AreEqual(611, lcl_Queue.Pop());
            Assert.AreEqual(625, lcl_Queue.Pop());
            Assert.AreEqual(635, lcl_Queue.Pop());
        }

        [Test]
        public void TestEmpty() {
            var lcl_Queue = new PriorityQueue<int, int>();
            Assert.True(lcl_Queue.Empty);
            lcl_Queue.Push(1, 1);
            Assert.False(lcl_Queue.Empty);
        }

        [Test]
        public void TestCount() {
            var lcl_Queue = new PriorityQueue<int, int>();
            Assert.AreEqual(0, lcl_Queue.Count);
            lcl_Queue.Push(1, 1);
            Assert.AreEqual(1, lcl_Queue.Count);
            lcl_Queue.Pop();
            Assert.AreEqual(0, lcl_Queue.Count);
        }

        [Test]
        public void TestThrowOnEmpty() {
            var lcl_Queue = new PriorityQueue<int, int>();
            Assert.Throws<InvalidOperationException>(() => lcl_Queue.Pop());
        }
    }
}
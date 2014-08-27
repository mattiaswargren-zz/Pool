// Copyright (c) 2014 Mattias Wargren
using NUnit.Framework;
using System;

namespace pool.tests
{
	[TestFixture()]
	public class TestIntPool
	{
		[Test()]
		public void TestCreation()
		{
			var pool = new Pool<int>();
			Assert.AreEqual(0, pool.size);
			Assert.AreEqual(0, pool.free);
		}

		[Test()]
		public void TestWarmup()
		{
			var pool = new Pool<int>();
			pool.Warmup(3);
			Assert.AreEqual(3, pool.size);
			Assert.AreEqual(3, pool.free);
		}

		[Test()]
		public void TestFetchEmpty()
		{
			var pool = new Pool<int>();
			Assert.AreEqual(0, pool.size);
			var item = pool.Fetch();
			Assert.AreEqual(0, item);
			Assert.AreEqual(1, pool.size);
		}

		[Test]
		public void TestCustomAllocate()
		{
			var pool = new Pool<int>();
			pool.Allocate = () => {
				return 42;
			};
			var item = pool.Fetch();
			Assert.AreEqual(1, pool.size);
			Assert.AreEqual(0, pool.free);
			Assert.AreEqual(42, item);
		}

		[Test]
		public void TestReturnItem()
		{
			var pool = new Pool<int>();
			pool.Warmup(10);

			Assert.AreEqual(10, pool.size);
			Assert.AreEqual(10, pool.free);

			var item = pool.Fetch();

			Assert.AreEqual(9, pool.free);

			pool.Return(item);

			Assert.AreEqual(10, pool.free);
		}

		[Test]
		public void TestReturnItemNotInPool()
		{
			var pool = new Pool<int>();

			Assert.AreEqual(0, pool.size);

			pool.Return(1);
			pool.Return(2);
			pool.Return(3);

			Assert.AreEqual(3, pool.size);
			Assert.AreEqual(3, pool.free);

			var item = pool.Fetch();

			Assert.AreEqual(3, pool.size);
			Assert.AreEqual(2, pool.free);
		}
	}
}


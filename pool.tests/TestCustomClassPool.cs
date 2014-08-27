// Copyright (c) 2014 Mattias Wargren
using NUnit.Framework;
using System;

namespace pool.tests
{
	[TestFixture()]
	public class TestCustomClassPool
	{

		[Test()]
		public void TestCreation()
		{
			var pool = new Pool<CustomPoolItem>();
			Assert.AreEqual(0, pool.size);
		}

		[Test()]
		public void TestFetchWithoutAllocate()
		{
			var pool = new Pool<CustomPoolItem>();
			var item = pool.Fetch();

			Assert.IsNull(item);
		}

		[Test()]
		public void TestFetchWithAllocate()
		{
			var pool = new Pool<CustomPoolItem>();
			pool.Allocate = () => {
				return new CustomPoolItem();
			};

			var item = pool.Fetch();

			Assert.IsNotNull(item);
		}

		[Test()]
		public void TestFetchWithAllocateAndFetch()
		{
			var pool = new Pool<CustomPoolItem>();
			pool.Allocate = () => {
				return new CustomPoolItem();
			};

			pool.OnFetch = (pItem) => {
				pItem.Fetch();
			};

			pool.OnReturn = (pItem) => {
				pItem.Return();
			};

			var item = pool.Fetch();

			Assert.AreEqual(0, pool.free);
			Assert.AreEqual(true, item.IsFetched);
			Assert.AreEqual(1, item.FetchCount);

			pool.Return(item);

			Assert.AreEqual(false, item.IsFetched);
			Assert.AreEqual(1, pool.free);

			item = pool.Fetch();

			Assert.AreEqual(0, pool.free);
			Assert.AreEqual(2, item.FetchCount);

		}

	
	}

	public class CustomPoolItem
	{

		public bool IsFetched { get; private set; }

		public int FetchCount { get; private set; }

		public void Fetch()
		{
			IsFetched = true;
			FetchCount++;
		}

		public void Return()
		{
			IsFetched = false;
		}

	}
}


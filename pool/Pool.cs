// Copyright (c) 2014 Mattias Wargren

using System;
using System.Collections.Generic;

namespace pool
{
	public class Pool<T>
	{
		public Func<T> Allocate = () => { return default(T); };
		public Action<T> OnFetch = (pItem) => {};
		public Action<T> OnReturn = (pItem) => {};

		List<T> _freeItems = new List<T>();
		List<T> _usedItems = new List<T>();

		public int free { get { return _freeItems.Count; } }
		public int size { get { return _freeItems.Count + _usedItems.Count; } }
		private int index { get { return _freeItems.Count - 1; } }

		public void ReturnAll()
		{
			for (int i = _usedItems.Count-1; i >= 0; i--)
			{
				Return(_usedItems[i]);
			}
		}

		public void Warmup(int pAmount)
		{
			Expand(pAmount);
		}

		void Expand(int pAmount)
		{
			for (int i = 0; i < pAmount; i++)
			{
				T item = Allocate();
				Return(item);
			}
		}

		public T Fetch()
		{
			// check if pool needs to expand
			if (index < 0)
			{
				// expand
				Expand(1);
			}

			// collect item
			T item = _freeItems[index];

			// remove from list
			_freeItems.RemoveAt(index);

			// add to active
			_usedItems.Add(item);

			// callback
			OnFetch(item);

			// return
			return item;
		}

		public void Return(T pItem)
		{
			// remove from used
			if (_usedItems.Contains(pItem))
			{
				_usedItems.Remove(pItem);
			}

			// callback
			OnReturn(pItem);

			// add to free
			_freeItems.Add(pItem);
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ObjectPool
{
	public class ObjectPool<T> where T : class
	{
		private LinkedList<T> m_ListUsing;
		private Stack<T> m_ListWaiting;

		public ObjectPool()
		{
			m_ListUsing = new LinkedList<T>();
			m_ListWaiting = new Stack<T>();
		}

		public void Create(T item)
		{
			if (item == null) return;
			m_ListWaiting.Push(item);
		}

		public T Get()
		{
			if (m_ListWaiting.Count > 0) {
				T tmp = m_ListWaiting.Pop ();
				m_ListUsing.AddFirst (tmp);
				return tmp;
			}
			return default (T);
		}

		public bool Get(ref T value)
		{
			if (m_ListWaiting.Count > 0)
			{
				value = m_ListWaiting.Pop();
				m_ListUsing.AddFirst(value);
				return true;
			}
			return false;
		}

		public void Set(T item)
		{
			if (item == null) return;
			if (m_ListUsing.Contains(item)) {
				m_ListUsing.Remove(item);
			}
			if (m_ListWaiting.Contains(item) == false) {
				m_ListWaiting.Push (item);
			}
		}

		public void Set(int index)
		{
			T tmp = m_ListUsing.ElementAt (index);
			if (tmp == null) return;
			m_ListUsing.Remove(tmp);
			m_ListWaiting.Push (tmp);
		}

		public int Count() {
			return m_ListUsing.Count;
		}

		public int CountUnuse() {
			return m_ListWaiting.Count;
		}

		public T ElementAtIndex(int index) {
			if (index > m_ListUsing.Count - 1)
				return default (T);
			return m_ListUsing.ElementAt (index);
		}
	}
}
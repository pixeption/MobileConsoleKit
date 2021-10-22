using System.Collections.Generic;

namespace MobileConsole
{
	public class Pool <T> where T : new()
	{
		List<T> _pools;

		public Pool(int capacity = 0)
		{
            _pools = new List<T>();
			
			PrecreateObject(capacity);
		}

		void PrecreateObject(int capacity)
		{
            for (int i = 0; i < capacity; i++)
            {
				_pools.Add(CreateNewObject());
            }
		}

		public T Get()
		{
			if (_pools.Count > 0)
			{
				T obj = _pools[0];
				_pools.Remove(obj);
				
				return obj;
			}
			else
			{
				return CreateNewObject();
			}
		}

		public void Return(T obj)
		{
			if (obj != null)
			{
				_pools.Add(obj);
			}
		}

		public void Return(List<T> listObj)
		{
			if (listObj != null)
			{
				_pools.AddRange(listObj);
			}
		}

		T CreateNewObject()
		{
			return new T();
		}
	}
}
using System;

namespace Stateless
{
	public interface IListStore<T>
	{
		void Delete(T item);
		void Add(T item);
	}
}


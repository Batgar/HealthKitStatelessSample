using System;
using System.Collections.Generic;

namespace Stateless
{
	public interface IListStore<T>
	{
		void Delete(T item);
		void Add(T item);
		List<T> MainList {get;}
		void ResetList(List<T> newList);
	}
}


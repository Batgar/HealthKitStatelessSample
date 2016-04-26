using System;
using System.Collections.Generic;

namespace Stateless
{
	public interface IListView<T>
	{
		void Receive(IList<T> list, IList<Change> changeset);
	}
}


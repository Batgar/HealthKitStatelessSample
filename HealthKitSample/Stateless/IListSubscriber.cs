using System;
using System.Collections.Generic;

namespace Stateless
{
	public interface IListSubscriber<T>
	{
		void Receive(IList<T> list, IList<Change> changeset);
	}
}


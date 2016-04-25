using System;

namespace Stateless
{
	public interface ISubscriber<T>
	{
		void Receive(T state);
	}
}


using System;

namespace Stateless
{
	public interface IView<T> where T : IState
	{
		void Receive(T state);
	}
}


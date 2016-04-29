using System;
using System.Collections.Generic;

namespace Stateless
{
	public static class StateDispatcher<T> where T : IState, new()
	{
		public static void Bind(IView<T> subscriber)
		{
			if (!_subscribers.Contains (subscriber)) {
				_subscribers.Add (subscriber);
			}

			GenerateState ();

			Refresh ();
		}

		public static void Unbind(IView<T> subscriber)
		{
			_subscribers.Remove (subscriber);
		}

		public static void Refresh()
		{
			
			foreach (var s in _subscribers)
			{
				s.Receive(State);
			}
		}

		private static void GenerateState()
		{
			if (_internalStore == null) {
				_internalStore = new T ();
			}
		}

		private static T _internalStore;

		static List<IView<T>> _subscribers = new List<IView<T>>();

		static public T State { 
			get { 
				GenerateState (); 
				return _internalStore; 
			} 
		}
	}
}


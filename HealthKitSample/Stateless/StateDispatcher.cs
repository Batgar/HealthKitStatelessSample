using System;
using System.Collections.Generic;

namespace Stateless
{
	public static class StateDispatcher<T> where T : IStore, new()
	{
		public static void Bind(ISubscriber<T> subscriber)
		{
			if (!_subscribers.Contains (subscriber)) {
				_subscribers.Add (subscriber);
			}

			GenerateStore ();

			Refresh ();
		}

		public static void Unbind(ISubscriber<T> subscriber)
		{
			_subscribers.Remove (subscriber);
		}

		public static void Refresh()
		{
			foreach (var s in _subscribers)
			{
				s.Receive(Store);
			}
		}

		private static void GenerateStore()
		{
			if (_internalStore == null) {
				_internalStore = new T ();
			}
		}

		private static T _internalStore;

		static List<ISubscriber<T>> _subscribers = new List<ISubscriber<T>>();

		static public T Store { 
			get { 
				GenerateStore (); 
				return _internalStore; 
			} 
		}
	}
}


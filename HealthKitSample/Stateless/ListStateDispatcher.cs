using System;
using System.Collections.Generic;
using System.Linq;

namespace Stateless
{
	public static class ListStateDispatcher<T> where T : IIdentifiable
	{
		

		public static void Bind(IListSubscriber<T> subscriber)
		{
			if (!_subscribers.Contains (subscriber)) {
				_subscribers.Add (subscriber);
			}

			GenerateStore ();

			//If we just bound, then fire off a refresh to this subscriber that all elements were just inserted.
			if (_internalStore.Count > 0) {
				subscriber.Receive (Store, Store.Select ((o, i) => new Change (){ Index = i, ChangeSetType = Change.ChangeType.Insert }).ToList ());
			}
		}

		public static void Unbind(IListSubscriber<T> subscriber)
		{
			_subscribers.Remove (subscriber);
		}

		public static IListStore<T> MainListStore { get; set; } 

		public static void Refresh(List<T> newEntries, List<Change> changeset)
		{
			_internalStore = newEntries;

			foreach (var s in _subscribers)
			{
				s.Receive(Store, changeset);
			}
		}

		private static void GenerateStore()
		{
			if (_internalStore == null) {
				_internalStore = new List<T> ();
			}
		}

		private static List<T> _internalStore;

		static List<IListSubscriber<T>> _subscribers = new List<IListSubscriber<T>>();

		public static List<T> Store
		{
			get{
				if (_internalStore == null) {
					_internalStore = new List<T> ();
				}
				return _internalStore;
			}
		}

		public static void RemoveAtIndex(int index) {
			MainListStore.Delete (Store [index]);
		}

		public static void Add(T itemToAdd) {
			MainListStore.Add (itemToAdd);
		}
	}
}


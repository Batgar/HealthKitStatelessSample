using System;
using System.Collections.Generic;
using System.Linq;

namespace Stateless
{
	public class ListStateDispatcher<T, S> where T : IIdentifiable
		where S : IListStore<T>, new()
	{
		public void Bind(IListSubscriber<T> subscriber)
		{
			if (!_subscribers.Contains (subscriber)) {
				_subscribers.Add (subscriber);
			}

			//If we just bound, then fire off a refresh to this subscriber that all elements were just inserted.
			if (MainListStore.MainList.Count > 0) {
				subscriber.Receive (Store, Store.Select ((o, i) => new Change (){ Index = i, ChangeSetType = Change.ChangeType.Insert }).ToList ());
			}
		}

		public void Unbind(IListSubscriber<T> subscriber)
		{
			_subscribers.Remove (subscriber);
		}

		private IListStore<T> _mainListStore;

		public IListStore<T> MainListStore { 
			get {
				if (_mainListStore == null) {
					_mainListStore = new S ();
				}
				return _mainListStore;
			}
		}

		private void Refresh(List<T> newEntries, List<Change> changeset)
		{
			MainListStore.ResetList(newEntries);

			foreach (var s in _subscribers)
			{
				s.Receive(Store, changeset);
			}
		}

		public void Refresh(List<T> newEntries)
		{
			//Now we have our new values, just need to compute the changeset to pass through the Dispatcher.
			var changeset = ChangesetComputer.ComputeListDifferences(Store.Cast<IIdentifiable>().ToList(),
				newEntries.Cast<IIdentifiable>().ToList());

			Refresh (newEntries, changeset);
		}



		List<IListSubscriber<T>> _subscribers = new List<IListSubscriber<T>>();

		public List<T> Store
		{
			get{
				return MainListStore.MainList;
			}
		}

		public void RemoveAtIndex(int index) {
			MainListStore.Delete (Store [index]);
		}

		public void Add(T itemToAdd) {
			MainListStore.Add (itemToAdd);
		}
	}
}


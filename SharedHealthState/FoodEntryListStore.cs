using System;
using Stateless;
using System.Collections.Generic;

namespace SharedHealthState
{
	public class FoodEntryListStore : IListStore<FoodEntry>
	{
		#region IListStore implementation
		public void Delete (FoodEntry entry)
		{
			var healthDataStore = TinyIoC.TinyIoCContainer.Current.Resolve<IHealthDataStore> ();

			healthDataStore.RemoveFoodEntry (entry);
		}

		public void Add(FoodEntry entry)
		{
			var healthDataStore = TinyIoC.TinyIoCContainer.Current.Resolve<IHealthDataStore> ();
			healthDataStore.AddFoodEntry (entry);
		}

		public void ResetList (List<FoodEntry> newList)
		{
			_mainList = newList;
		}

		private List<FoodEntry> _mainList;

		public List<FoodEntry> MainList {
			get {
				if (_mainList == null) {
					_mainList = new List<FoodEntry> ();
				}
				return _mainList;
			}
		}
		#endregion

	}
}


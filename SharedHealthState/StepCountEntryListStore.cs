using System;
using Stateless;
using System.Collections.Generic;

namespace SharedHealthState
{
	public class StepCountEntryListStore : IListStore<StepCountEntry>
	{
		#region IListStore implementation

		public void Delete (StepCountEntry entry)
		{
			var healthDataStore = TinyIoC.TinyIoCContainer.Current.Resolve<IHealthDataStore> ();

			healthDataStore.RemoveStepCountEntry (entry);
		}

		public void Add (StepCountEntry entry)
		{
			var healthDataStore = TinyIoC.TinyIoCContainer.Current.Resolve<IHealthDataStore> ();

			healthDataStore.AddStepCountEntry (entry);
		}

		public void ResetList (System.Collections.Generic.List<StepCountEntry> newList)
		{
			_mainList = newList;
		}

		private List<StepCountEntry> _mainList;

		public List<StepCountEntry> MainList {
			get {
				if (_mainList == null) {
					_mainList = new List<StepCountEntry> ();
				}
				return _mainList;
			}
		}

		#endregion
	}
}


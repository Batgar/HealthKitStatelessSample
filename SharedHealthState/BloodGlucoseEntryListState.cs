using System;
using System.Linq;
using System.Collections.Generic;
using Stateless;

namespace SharedHealthState
{
	public class BloodGlucoseEntryListState : IListStore<BloodGlucoseEntry>
	{
		#region IListStore implementation
		public void Delete (BloodGlucoseEntry entry)
		{
			var healthDataStore = TinyIoC.TinyIoCContainer.Current.Resolve<IHealthDataStore> ();

			healthDataStore.RemoveBloodGlucoseEntry (entry);
		}

		public void Add(BloodGlucoseEntry entry)
		{
			var healthDataStore = TinyIoC.TinyIoCContainer.Current.Resolve<IHealthDataStore> ();
			healthDataStore.AddBloodGlucoseEntry (entry);
		}

		public void ResetList (List<BloodGlucoseEntry> newList)
		{
			_mainList = newList;
		}

		private List<BloodGlucoseEntry> _mainList;

		public List<BloodGlucoseEntry> MainList {
			get {
				if (_mainList == null) {
					_mainList = new List<BloodGlucoseEntry> ();
				}
				return _mainList;
			}
		}
		#endregion
		
	}

}


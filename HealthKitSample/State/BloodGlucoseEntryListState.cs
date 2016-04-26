using System;
using HealthKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using CoreFoundation;
using Stateless;

namespace HealthKitSample
{
	class BloodGlucoseEntryListState : IListStore<BloodGlucoseEntry>
	{
		#region IListStore implementation
		public void Delete (BloodGlucoseEntry entry)
		{
			HealthKitDataStore.RemoveBloodGlucoseEntry (entry);
		}

		public void Add(BloodGlucoseEntry entry)
		{
			HealthKitDataStore.AddBloodGlucoseEntry (entry);
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


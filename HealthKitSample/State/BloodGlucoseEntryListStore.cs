using System;
using HealthKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using CoreFoundation;
using Stateless;

namespace HealthKitSample
{
	class BloodGlucoseEntryListStore : IListStore<BloodGlucoseEntry>
	{
		#region IListStore implementation
		public void Delete (BloodGlucoseEntry entry)
		{
			HealthKitDataManager.RemoveBloodGlucoseEntry (entry);
		}

		public void Add(BloodGlucoseEntry entry)
		{
			HealthKitDataManager.AddBloodGlucoseEntry (entry);
		}
		#endregion
		
	}

}


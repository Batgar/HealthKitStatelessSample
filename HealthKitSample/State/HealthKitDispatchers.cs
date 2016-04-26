using System;
using Stateless;

namespace HealthKitSample
{
	internal static class HealthKitDispatchers
	{
		static ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListStore> _bloodGlucoseListStateDispatcher;

		internal static ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListStore> BloodGlucoseListStateDispatcher
		{
			get{
				if (_bloodGlucoseListStateDispatcher == null) {
					_bloodGlucoseListStateDispatcher = new ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListStore> ();
				}
				return _bloodGlucoseListStateDispatcher;
			}
		}
	}
}


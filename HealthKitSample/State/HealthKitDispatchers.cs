using System;
using Stateless;

namespace HealthKitSample
{
	internal static class HealthKitDispatchers
	{
		static ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListState> _bloodGlucoseListStateDispatcher;

		internal static ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListState> BloodGlucoseListStateDispatcher
		{
			get{
				if (_bloodGlucoseListStateDispatcher == null) {
					_bloodGlucoseListStateDispatcher = new ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListState> ();
				}
				return _bloodGlucoseListStateDispatcher;
			}
		}
	}
}


using System;
using Stateless;

namespace SharedHealthState
{
	public static class HealthStateDispatchers
	{
		static ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListState> _bloodGlucoseListStateDispatcher;

		public static ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListState> BloodGlucoseListStateDispatcher
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


using System;
using Stateless;

namespace SharedHealthState
{
	public static class HealthStateDispatchers
	{
		static ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListStore> _bloodGlucoseListStateDispatcher;
		static ListStateDispatcher<StepCountEntry, StepCountEntryListStore> _stepCountEntryListStateDispatcher;
		static ListStateDispatcher<FoodEntry, FoodEntryListStore> _foodEntryListStateDispatcher;

		public static ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListStore> BloodGlucoseListStateDispatcher
		{
			get{
				if (_bloodGlucoseListStateDispatcher == null) {
					_bloodGlucoseListStateDispatcher = new ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListStore> ();
				}
				return _bloodGlucoseListStateDispatcher;
			}
		}

		public static ListStateDispatcher<StepCountEntry, StepCountEntryListStore> StepCountListStateDispatcher
		{
			get{
				if (_stepCountEntryListStateDispatcher == null) {
					_stepCountEntryListStateDispatcher = new ListStateDispatcher<StepCountEntry, StepCountEntryListStore> ();
				}
				return _stepCountEntryListStateDispatcher;
			}
		}

		public static ListStateDispatcher<FoodEntry, FoodEntryListStore> FoodListStateDispatcher
		{
			get{
				if (_foodEntryListStateDispatcher == null) {
					_foodEntryListStateDispatcher = new ListStateDispatcher<FoodEntry, FoodEntryListStore> ();
				}
				return _foodEntryListStateDispatcher;
			}
		}
	}
}


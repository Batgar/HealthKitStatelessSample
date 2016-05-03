using System;

namespace SharedHealthState
{
	public interface IHealthDataStore
	{
		void Refresh();
		void RemoveBloodGlucoseEntry(BloodGlucoseEntry entry);
		void AddBloodGlucoseEntry(BloodGlucoseEntry entry);

		void RemoveStepCountEntry(StepCountEntry entry);
		void AddStepCountEntry(StepCountEntry entry);
	
		void RemoveFoodEntry(FoodEntry entry);
		void AddFoodEntry(FoodEntry entry);
	}
}


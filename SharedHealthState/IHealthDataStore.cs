using System;

namespace SharedHealthState
{
	public interface IHealthDataStore
	{
		void Refresh();
		void RemoveBloodGlucoseEntry(BloodGlucoseEntry entry);
		void AddBloodGlucoseEntry(BloodGlucoseEntry entry);
	}
}


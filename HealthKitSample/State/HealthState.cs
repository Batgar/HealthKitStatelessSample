using System;
using Stateless;
using System.Collections.Generic;

namespace HealthKitSample
{
	class HealthState : IState {

		public HealthState()
		{
			//Rev up the HealthKitDataManager.
			HealthKitDataStore.Refresh();

			//TODO: Subscribe to HealthKit state changes.

		}

		public double Height { get; set;}
		public double BloodGlucose {get; set;}
		public string BiologicalSex {get; set;}

	}
}


using System;
using Stateless;
using System.Collections.Generic;

namespace SharedHealthState
{
	public class HealthState : IState {

		public HealthState()
		{
			var healthDataStore = TinyIoC.TinyIoCContainer.Current.Resolve<IHealthDataStore> ();
			//Rev up the HealthKitDataManager.
			healthDataStore.Refresh();

			//TODO: Subscribe to HealthKit state changes.

		}

		public double Height { get; set;}
		public double BloodGlucose {get; set;}
		public string BiologicalSex {get; set;}

	}
}


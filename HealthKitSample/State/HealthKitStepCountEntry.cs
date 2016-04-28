using System;
using HealthKit;
using SharedHealthState;

namespace HealthKitSample
{
	public class HealthKitStepCountEntry : StepCountEntry
	{
		public HealthKitStepCountEntry (HKQuantitySample stepCountSample)
		{
			var countUnit = HKUnit.Count;
			StartEntryDateTime = DateTimeExtensions.NSDateToDateTime (stepCountSample.StartDate);
			EndEntryDateTime = DateTimeExtensions.NSDateToDateTime(stepCountSample.EndDate);
			Count = stepCountSample.Quantity.GetDoubleValue(countUnit);

			StepCountSample = stepCountSample;
		}

		public HKQuantitySample StepCountSample {get; private set;}

	}
}


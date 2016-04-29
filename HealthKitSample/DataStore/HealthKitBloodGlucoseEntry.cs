using System;
using HealthKit;
using SharedHealthState;

namespace HealthKitSample
{
	public class HealthKitBloodGlucoseEntry : BloodGlucoseEntry
	{
		public HealthKitBloodGlucoseEntry(HKQuantitySample bloodGlucoseSample)
		{
			var mgPerDL = HKUnit.FromString("mg/dL");
			StartEntryDateTime = DateTimeExtensions.NSDateToDateTime (bloodGlucoseSample.StartDate);
			EndEntryDateTime = DateTimeExtensions.NSDateToDateTime(bloodGlucoseSample.EndDate);
			BloodGlucoseValue = bloodGlucoseSample.Quantity.GetDoubleValue (mgPerDL);
			BloodGlucoseSample = bloodGlucoseSample;
		}

		public HKQuantitySample BloodGlucoseSample {get; private set;}
	}
}


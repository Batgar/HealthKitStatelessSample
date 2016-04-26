using System;
using System.Collections.Generic;
using Stateless;
using HealthKit;

namespace HealthKitSample
{
	//A blood glucose entry can never be edited, just added and deleted.
	public struct BloodGlucoseEntry : IIdentifiable
	{
		
		public BloodGlucoseEntry(HKQuantitySample bloodGlucoseSample)
		{
			var mgPerDL = HKUnit.FromString("mg/dL");
			StartEntryDateTime = DateTimeExtensions.NSDateToDateTime (bloodGlucoseSample.StartDate);
			EndEntryDateTime = DateTimeExtensions.NSDateToDateTime(bloodGlucoseSample.EndDate);
			BloodGlucoseValue = bloodGlucoseSample.Quantity.GetDoubleValue (mgPerDL);
			BloodGlucoseSample = bloodGlucoseSample;
		}

		public HKQuantitySample BloodGlucoseSample {get; private set;}
		public double BloodGlucoseValue {get; set;}
		public DateTime StartEntryDateTime {get; set;}
		public DateTime EndEntryDateTime {get; set;}

		public int ID
		{
			get {
				return BloodGlucoseValue.GetHashCode () + StartEntryDateTime.GetHashCode () + EndEntryDateTime.GetHashCode ();
			}
		}

		public override int GetHashCode ()
		{
			return this.ID;
		}

		public override bool Equals (object obj)
		{
			if (obj is BloodGlucoseEntry) {
				return ((BloodGlucoseEntry)obj).ID == ID;
			}
			return base.Equals (obj);
		}
	}



}


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



	public class ChangesetComputer {
		public static List<Change> ComputeListDifferences(List<BloodGlucoseEntry> lhs, List<BloodGlucoseEntry> rhs) {
			List<Change> changes = new List<Change> ();

			int i = 0;
			foreach (var entry in rhs) {
				if (!lhs.Contains (entry)) {
					changes.Add (new Change (){ ChangeSetType = Change.ChangeType.Insert, Index = i }); 
				}
				i++;
			}

			i = 0;
			foreach (var entry in lhs) {
				if (!rhs.Contains (entry)) {
					changes.Add (new Change () { ChangeSetType = Change.ChangeType.Delete, Index = i });
				}
				i++;
			}

			//There is no way to update a blood glucose entry, so we don't do any Update stuff.

			return changes;
		}
	}
}


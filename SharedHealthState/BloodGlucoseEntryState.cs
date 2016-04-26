using System;
using System.Collections.Generic;
using Stateless;

namespace SharedHealthState
{
	//A blood glucose entry can never be edited, just added and deleted.
	public class BloodGlucoseEntry : IIdentifiable
	{		

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


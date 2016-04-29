using System;
using System.Collections.Generic;
using Stateless;

namespace SharedHealthState
{
	//A blood glucose entry can never be edited, just added and deleted.
	public class BloodGlucoseEntry : IIdentifiable
	{		
		public BloodGlucoseEntry()
		{
		}

		public BloodGlucoseEntry(double glucoseValue)
		{
			BloodGlucoseValue = glucoseValue;
		}

		public double BloodGlucoseValue {get; protected set;}
		public DateTime StartEntryDateTime {get; protected set;}
		public DateTime EndEntryDateTime {get; protected set;}

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


using System;
using Stateless;

namespace SharedHealthState
{
	public class StepCountEntry : IIdentifiable
	{		
		public double Count {get; set;}
		public DateTime StartEntryDateTime {get; set;}
		public DateTime EndEntryDateTime {get; set;}

		public int ID
		{
			get {
				return Count.GetHashCode () + StartEntryDateTime.GetHashCode () + EndEntryDateTime.GetHashCode ();
			}
		}

		public override int GetHashCode ()
		{
			return this.ID;
		}

		public override bool Equals (object obj)
		{
			if (obj is StepCountEntry) {
				return ((StepCountEntry)obj).ID == ID;
			}
			return base.Equals (obj);
		}
	}
}


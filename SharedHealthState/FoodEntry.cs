using System;
using Stateless;

namespace SharedHealthState
{
	public class FoodEntry : IIdentifiable
	{
		public FoodEntry()
		{
		}

		public FoodEntry (string foodName, double potassium, double fiber)
		{
			FoodName = foodName;
			Potassium = potassium;
			Fiber = fiber;
		}

		public string FoodName {get; protected set;}
		public double Potassium {get; protected set;}
		public double Fiber {get; protected set;}
		public DateTime StartEntryDateTime {get; protected set;}
		public DateTime EndEntryDateTime {get; protected set;}

		public int ID
		{
			get {
				return Potassium.GetHashCode() + 
					FoodName.GetHashCode () + 
					Fiber.GetHashCode() +
					StartEntryDateTime.GetHashCode () + 
					EndEntryDateTime.GetHashCode ();
			}
		}

		public override int GetHashCode ()
		{
			return this.ID;
		}

		public override bool Equals (object obj)
		{
			if (obj is FoodEntry) {
				return ((FoodEntry)obj).ID == ID;
			}
			return base.Equals (obj);
		}
	}
}


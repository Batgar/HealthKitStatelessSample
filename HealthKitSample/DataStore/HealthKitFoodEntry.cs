using System;
using SharedHealthState;
using HealthKit;
using System.Linq;

namespace HealthKitSample
{
	public class HealthKitFoodEntry : FoodEntry
	{
		public HealthKitFoodEntry (HKCorrelation correlation)
		{
			FoodCorrelation = correlation;

			var mgUnit = HKUnit.FromString("mg");

			FoodName = correlation.Metadata.FoodType;

			double potassiumMg = double.MinValue;

			var potassiumQuantityType = HKQuantityType.GetQuantityType(HKQuantityTypeIdentifierKey.DietaryPotassium);
			var potassiumSample = correlation.GetObjects(potassiumQuantityType).FirstOrDefault();
			if (potassiumSample != null)
			{
				var potassiumQuantity = (potassiumSample as HKQuantitySample).Quantity;

				Potassium = potassiumQuantity.GetDoubleValue(mgUnit);
			}

			//Crack open the correlation and retrieve stuff and put it into our private members.

			StartEntryDateTime = DateTimeExtensions.NSDateToDateTime (correlation.StartDate);
			EndEntryDateTime = DateTimeExtensions.NSDateToDateTime(correlation.EndDate);

		}

		public HKCorrelation FoodCorrelation {get; private set;}
	}
}


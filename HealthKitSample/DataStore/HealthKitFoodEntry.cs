using System;
using SharedHealthState;
using HealthKit;
using System.Linq;
using Foundation;

namespace HealthKitSample
{
	public class HealthKitFoodEntry : FoodEntry
	{
		public HealthKitFoodEntry (HKCorrelation correlation)
		{
			FoodCorrelation = correlation;

			FoodName = (correlation.Metadata.Dictionary[HKMetadataKey.FoodType] as NSString);

			//Crack open the correlation and retrieve stuff and put it into our private members.
			SetLocalPropertyFromHealthKit (correlation, HKQuantityTypeIdentifierKey.DietaryPotassium, "mg", (potassium) => {
				Potassium = potassium;
			});

			SetLocalPropertyFromHealthKit (correlation, HKQuantityTypeIdentifierKey.DietaryFiber, "g", (fiber) => {
				Fiber = fiber;
			});

			StartEntryDateTime = DateTimeExtensions.NSDateToDateTime (correlation.StartDate);
			EndEntryDateTime = DateTimeExtensions.NSDateToDateTime(correlation.EndDate);

		}

		private void SetLocalPropertyFromHealthKit(HKCorrelation correlation, NSString quantityTypeIdentifier, string unitString, Action<double> setter)
		{
			var unit = HKUnit.FromString(unitString);
			var quantityType = HKQuantityType.GetQuantityType(quantityTypeIdentifier);
			var sample = correlation.GetObjects(quantityType).FirstOrDefault();
			if (sample != null)
			{
				var quantity = (sample as HKQuantitySample).Quantity;
				setter(quantity.GetDoubleValue(unit));
			}
		}

		public HKCorrelation FoodCorrelation {get; private set;}
	}
}


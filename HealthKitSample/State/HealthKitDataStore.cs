using System;
using HealthKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using CoreFoundation;
using Stateless;

namespace HealthKitSample
{
	public static class HealthKitDataStore
	{
		static HealthKitDataStore()
		{
			HealthStore = new HKHealthStore ();
		}

		static HKHealthStore HealthStore {get; set;}

		public static bool IsHealthDataAvailable {
			get {
				return HKHealthStore.IsHealthDataAvailable; 
			}
		}

		public static void GetUserAuthorizationIfNeeded()
		{
			if (IsHealthDataAvailable) {
				HealthStore.RequestAuthorizationToShare (HealthTypesToWrite(),
					HealthTypesToRead(),
					(success, error) => {						
						if (!success || error != null)
						{
							AlertManager.ShowError("Health Data", "Unable to access health data.");
						}
					});
			}
		}

		static NSString[] QuantityTypesToRead = new NSString[] {
			HKQuantityTypeIdentifierKey.DietaryEnergyConsumed,
			HKQuantityTypeIdentifierKey.BloodGlucose,
			HKQuantityTypeIdentifierKey.Height,
			HKQuantityTypeIdentifierKey.BodyMass,
		};

		static NSString[] CharacteristicTypesToRead = new NSString[] {
			HKCharacteristicTypeIdentifierKey.BiologicalSex,
			HKCharacteristicTypeIdentifierKey.DateOfBirth,
		};

		static NSSet HealthTypesToRead() {

			var quantityTypesToRead = QuantityTypesToRead.Select (q => HKObjectType.GetQuantityType (q));

			var characteristicTypesToRead = CharacteristicTypesToRead.Select (c => HKObjectType.GetCharacteristicType (c));

			var allHealthTypesToRead = new List<object>();

			allHealthTypesToRead.AddRange (quantityTypesToRead);
			allHealthTypesToRead.AddRange (characteristicTypesToRead);

			return new NSSet (allHealthTypesToRead.ToArray());
		}

		static NSSet HealthTypesToWrite() {
			var quantityTypesToRead = QuantityTypesToRead.Select (q => HKObjectType.GetQuantityType (q));

			var allHealthTypesToRead = new List<object>();

			allHealthTypesToRead.AddRange (quantityTypesToRead);

			return new NSSet (allHealthTypesToRead.ToArray());
		}

		static void RefreshQuantityValue(NSString quantityTypeKey, HKQuantityType quantityType)
		{			

			NSSortDescriptor timeSortDescriptor = new NSSortDescriptor(HKSample.SortIdentifierEndDate, false);

			// Since we are interested in retrieving the user's latest sample, we sort the samples in descending order, and set the limit to 1. We are not filtering the data, and so the predicate is set to nil.
			HKSampleQuery query = new HKSampleQuery(quantityType, null, 100,  new NSSortDescriptor[]{timeSortDescriptor}, new HKSampleQueryResultsHandler(new Action<HKSampleQuery,HKSample[],NSError>((query2, results, error) =>
				{
					if (results != null && results.Length > 0) {
						
						if (quantityTypeKey == HKQuantityTypeIdentifierKey.Height) {
							
							//We have height, process the last entry into inches.
							var quantitySample = results.LastOrDefault() as HKQuantitySample;

							var quantity = quantitySample.Quantity;

							var heightUnit = HKUnit.Inch;

							DispatchQueue.MainQueue.DispatchAsync(() => {
								var dataStore = StateDispatcher<HealthState>.State;
									
								dataStore.Height = quantity.GetDoubleValue(heightUnit);

								StateDispatcher<HealthState>.Refresh();								
							});

						} else if (quantityTypeKey == HKQuantityTypeIdentifierKey.BloodGlucose) {
							

							DispatchQueue.MainQueue.DispatchAsync(() => {
								//Refresh the views with the last known blood glucose quantity via HealthState and BloodGlucoseRecommendationState.
								var lastBloodGlucoseQuantity = (results.LastOrDefault() as HKQuantitySample).Quantity;

								var healthState = StateDispatcher<HealthState>.State;

								var mgPerDL = HKUnit.FromString("mg/dL");
								healthState.BloodGlucose = lastBloodGlucoseQuantity.GetDoubleValue(mgPerDL);


								//At this point all UI subscribers to the HealthState object will update.
								StateDispatcher<HealthState>.Refresh();			

							

								var recommendationStore = StateDispatcher<BloodGlucoseRecommendationState>.State;
								recommendationStore.BloodGlucose = healthState.BloodGlucose;

								//At this point all UI subscribers to the BloodGlucoseRecommendationState will update.
								StateDispatcher<BloodGlucoseRecommendationState>.Refresh();



								//Now we need to deliver all the blood glucose entries in a list to any listeners.
								var newBloodGlucoseEntries = new List<BloodGlucoseEntry>();

								//Now also deliver all blood glucose readings up to the UI via the 'diff engine' for easy UITableViewController based updating.
								foreach (var bloodGlucoseEntry in results)
								{
									var bloodGlucoseSample = bloodGlucoseEntry as HKQuantitySample;
									if (bloodGlucoseSample != null)
									{
										newBloodGlucoseEntries.Add(new BloodGlucoseEntry(bloodGlucoseSample) );

									}
								}

								HealthKitDispatchers.BloodGlucoseListStateDispatcher.Refresh(newBloodGlucoseEntries);

							});
						}
					}
				})));
			
			HealthStore.ExecuteQuery (query);
		}

		//This method handles all the HealthKit gymnastics to remove a blood glucose entry.
		public static void RemoveBloodGlucoseEntry(BloodGlucoseEntry entry)
		{			
			HealthStore.DeleteObject (entry.BloodGlucoseSample, new Action<bool, NSError> ((success, error) => {
				if (!success || error != null)
				{
					//NOTE: If this app didn't put the entry into the blood glucose list, then there will be an error on delete.
					AlertManager.ShowError("Health Kit", "Unable to delete glucose sample: " + error);
				}
				else
				{
					//Woo! We properly removed the last entry, make sure that any listeners to the glucose states are properly updated.
					RefreshQuantityValue(HKQuantityTypeIdentifierKey.BloodGlucose, HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.BloodGlucose));
				}
			}));
		}

		/// <summary>
		/// This method handles all the HealthKit gymnastics to add a blood glucose entry to the HealthKit data.
		/// </summary>
		/// <param name="entry">Entry.</param>
		public static void AddBloodGlucoseEntry(BloodGlucoseEntry entry)
		{
			var date = new NSDate ();
			var quantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.BloodGlucose);
			var mgPerDL = HKUnit.FromString("mg/dL");
			var quantity = HKQuantity.FromQuantity (mgPerDL, entry.BloodGlucoseValue);
			var sample = HKQuantitySample.FromType (quantityType, quantity, date, date);

			HealthStore.SaveObject(sample, new Action<bool, NSError>((success, error) => {
				if (!success || error != null)
				{
					//There may have been an add error for some reason.
					AlertManager.ShowError("Health Kit", "Unable to add glucose sample: " + error);
				}
				else
				{
					//Refresh all app wide blood glucose UI fields.
					RefreshQuantityValue(HKQuantityTypeIdentifierKey.BloodGlucose, quantityType);
				}
			}));
				
		}

		private static void RefreshCharacteristicValue(NSString characteristicTypeKey, HKCharacteristicType characteristicType)
		{
			if (characteristicTypeKey == HKCharacteristicTypeIdentifierKey.BiologicalSex) {
				NSError error = null;
				var biologicalSex = HealthStore.GetBiologicalSex (out error);
				if (error == null)
				{
					DispatchQueue.MainQueue.DispatchAsync (() => {
						var dataStore = StateDispatcher<HealthState>.State;

						dataStore.BiologicalSex = GetDisplayableBiologicalSex(biologicalSex.BiologicalSex);

						StateDispatcher<HealthState>.Refresh();
					});
				}
			}
		}

		private static string GetDisplayableBiologicalSex(HKBiologicalSex biologicalSex)
		{
			switch (biologicalSex) {
			case HKBiologicalSex.Female:
				return "Female";
			case HKBiologicalSex.Male:
				return "Male";
			case HKBiologicalSex.NotSet:
				return "Not Set";
			default:
				return "Other";
			}
		}


		public static void Refresh()
		{
			var quantityTypesToRead = QuantityTypesToRead.Select (q=> new {Key = q, QuantityType = HKObjectType.GetQuantityType (q)});
				
			foreach (var quantityTypeToRead in quantityTypesToRead) {
				RefreshQuantityValue (quantityTypeToRead.Key, quantityTypeToRead.QuantityType);
			}

			var characteristicTypesToRead = CharacteristicTypesToRead.Select (c => new {Key = c, CharacteristicType = HKObjectType.GetCharacteristicType (c)});

			foreach (var characteristicTypeToRead in characteristicTypesToRead) {
				RefreshCharacteristicValue (characteristicTypeToRead.Key, characteristicTypeToRead.CharacteristicType);
			}
		}

		//Create our structs for HealthKit data reading, and send them to a stateless UI. This should be fun!
	}
}


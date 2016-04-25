using System;
using HealthKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using CoreFoundation;
using Stateless;

namespace HealthKitSample
{
	public static class HealthKitDataManager
	{
		static HealthKitDataManager()
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
								var dataStore = StateDispatcher<HealthState>.Store;
									
								dataStore.Height = quantity.GetDoubleValue(heightUnit);

								StateDispatcher<HealthState>.Refresh();								
							});

						} else if (quantityTypeKey == HKQuantityTypeIdentifierKey.BloodGlucose) {
							

							DispatchQueue.MainQueue.DispatchAsync(() => {
								var lastBloodGlucoseQuantity = (results.LastOrDefault() as HKQuantitySample).Quantity;

								var dataStore = StateDispatcher<HealthState>.Store;

								var mgPerDL = HKUnit.FromString("mg/dL");
								dataStore.BloodGlucose = lastBloodGlucoseQuantity.GetDoubleValue(mgPerDL);

								StateDispatcher<HealthState>.Refresh();					

								var recommendationStore = StateDispatcher<BloodGlucoseRecommendationState>.Store;
								recommendationStore.BloodGlucose = dataStore.BloodGlucose;

								StateDispatcher<BloodGlucoseRecommendationState>.Refresh();

								List<BloodGlucoseEntry> newBloodGlucoseEntries = new List<BloodGlucoseEntry>();

								//Now also deliver all blood glucose readings up to the UI via the 'diff engine' for easy UITableViewController based updating.
								foreach (var bloodGlucoseEntry in results)
								{
									var bloodGlucoseSample = bloodGlucoseEntry as HKQuantitySample;
									if (bloodGlucoseSample != null)
									{
										newBloodGlucoseEntries.Add(new BloodGlucoseEntry(bloodGlucoseSample) );

									}
								}

								var oldBloodGlucoseEntries = ListStateDispatcher<BloodGlucoseEntry>.Store;

								//Now we have our new values, just need to compute the changeset to pass through the Dispatcher.
								var changeset = ChangesetComputer.ComputeListDifferences(oldBloodGlucoseEntries,
									newBloodGlucoseEntries);

								ListStateDispatcher<BloodGlucoseEntry>.MainListStore = new BloodGlucoseEntryListStore();

								//Broadcast out the changeset.
								ListStateDispatcher<BloodGlucoseEntry>.Refresh(newBloodGlucoseEntries, changeset);

							});
						}
					}
				})));
			
			HealthStore.ExecuteQuery (query);
		}


		public static void RemoveBloodGlucoseEntry(BloodGlucoseEntry entry)
		{			
			HealthStore.DeleteObject (entry.BloodGlucoseSample, new Action<bool, NSError> ((success, error) => {
				if (!success || error != null)
				{
					AlertManager.ShowError("Health Kit", "Unable to delete glucose sample: " + error);
				}
				else
				{
					RefreshQuantityValue(HKQuantityTypeIdentifierKey.BloodGlucose, HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.BloodGlucose));
				}
			}));
		}

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
					AlertManager.ShowError("Health Kit", "Unable to add glucose sample: " + error);
				}
				else
				{
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
						var dataStore = StateDispatcher<HealthState>.Store;

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


using System;
using HealthKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using CoreFoundation;
using Stateless;
using SharedHealthState;

namespace HealthKitSample
{
	internal class HealthKitDataStore : IHealthDataStore
	{
		public HealthKitDataStore()
		{
			HealthStore = new HKHealthStore ();

			_characteristicQueryHandlers = new Dictionary<NSString, Action> () {
				{
					HKCharacteristicTypeIdentifierKey.BiologicalSex,
					RefreshBiologicalSex
				}
			};
		}

		HKHealthStore HealthStore {get; set;}

		public bool IsHealthDataAvailable {
			get {
				return HKHealthStore.IsHealthDataAvailable; 
			}
		}

		public void GetUserAuthorizationIfNeeded()
		{
			if (IsHealthDataAvailable) {
				HealthStore.RequestAuthorizationToShare (HealthTypesToWrite(),
					HealthTypesToRead(),
					(success, error) => {						
						if (!success || error != null)
						{
							AlertManager.ShowError("Health Data", "Unable to access health data.");
						} else {
							//Refresh all data if we have auth!
							Refresh();
						}
					});
			}
		}

		static NSString[] QuantityTypesToRead = new NSString[] {
			HKQuantityTypeIdentifierKey.DietaryEnergyConsumed,
			HKQuantityTypeIdentifierKey.BloodGlucose,
			HKQuantityTypeIdentifierKey.Height,
			HKQuantityTypeIdentifierKey.BodyMass,
			HKQuantityTypeIdentifierKey.StepCount,
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
			//Characteristic Types are not able to be written, so we only use the quantity types.
			var quantityTypesToRead = QuantityTypesToRead.Select (q => HKObjectType.GetQuantityType (q));

			return new NSSet (quantityTypesToRead.ToArray());
		}

		static void RefreshHeight(HKSampleQuery query, HKSample[] results, NSError error)
		{
			//We have height, process the last entry into inches.
			var quantitySample = results.LastOrDefault() as HKQuantitySample;

			var quantity = quantitySample.Quantity;

			var heightUnit = HKUnit.Inch;

			DispatchQueue.MainQueue.DispatchAsync(() => {
				var dataStore = StateDispatcher<HealthState>.State;

				HealthStateMutator.MutateHeight(dataStore,
					() => quantity.GetDoubleValue(heightUnit));

				StateDispatcher<HealthState>.Refresh();								
			});
		}

		static void RefreshStepCount(HKSampleQuery query, HKSample[] results, NSError error)
		{			
			//Now we need to deliver all the blood glucose entries in a list to any listeners.
			var entries = new List<StepCountEntry>();

			//Now also deliver all blood glucose readings up to the UI via the 'diff engine' for easy UITableViewController based updating.
			foreach (var entry in results)
			{
				var sample = entry as HKQuantitySample;
				if (sample != null)
				{
					entries.Add(new HealthKitStepCountEntry(sample) );

				}
			}

			DispatchQueue.MainQueue.DispatchAsync(() => {
				HealthStateDispatchers.StepCountListStateDispatcher.Refresh(
					entries.Cast<StepCountEntry>().ToList());
			});
		}

		static void RefreshBloodGlucose(HKSampleQuery query, HKSample[] results, NSError error)
		{
			DispatchQueue.MainQueue.DispatchAsync(() => {
				//Refresh the views with the last known blood glucose quantity via HealthState and BloodGlucoseRecommendationState.
				var lastBloodGlucoseQuantity = (results.LastOrDefault() as HKQuantitySample).Quantity;

				var healthState = StateDispatcher<HealthState>.State;

				var mgPerDL = HKUnit.FromString("mg/dL");
				HealthStateMutator.MutateBloodGlucose(healthState,
					() =>  lastBloodGlucoseQuantity.GetDoubleValue(mgPerDL));


				//At this point all UI subscribers to the HealthState object will update.
				StateDispatcher<HealthState>.Refresh();			



				var recommendationStore = StateDispatcher<BloodGlucoseRecommendationState>.State;

				BloodGlucoseRecommendationMutator.MutateBloodGlucose(
					recommendationStore, () => healthState.BloodGlucose);

				//At this point all UI subscribers to the BloodGlucoseRecommendationState will update.
				StateDispatcher<BloodGlucoseRecommendationState>.Refresh();



				//Now we need to deliver all the blood glucose entries in a list to any listeners.
				var newBloodGlucoseEntries = new List<HealthKitBloodGlucoseEntry>();

				//Now also deliver all blood glucose readings up to the UI via the 'diff engine' for easy UITableViewController based updating.
				foreach (var bloodGlucoseEntry in results)
				{
					var bloodGlucoseSample = bloodGlucoseEntry as HKQuantitySample;
					if (bloodGlucoseSample != null)
					{
						newBloodGlucoseEntries.Add(new HealthKitBloodGlucoseEntry(bloodGlucoseSample) );

					}
				}

				HealthStateDispatchers.BloodGlucoseListStateDispatcher.Refresh(
					newBloodGlucoseEntries.Cast<BloodGlucoseEntry>().ToList());

			});
		}

		static Dictionary<NSString, Action<HKSampleQuery, HKSample[], NSError>> _sampleQueryHandlers = new Dictionary<NSString, Action<HKSampleQuery, HKSample[], NSError>> () {
			{
				HKQuantityTypeIdentifierKey.Height,
				RefreshHeight
			},
			{
				HKQuantityTypeIdentifierKey.StepCount,
				RefreshStepCount
			},
			{
				HKQuantityTypeIdentifierKey.BloodGlucose,
				RefreshBloodGlucose
			}
		};

		Dictionary<NSString, Action> _characteristicQueryHandlers;

		void RefreshBiologicalSex()
		{
			NSError error = null;
			var biologicalSex = HealthStore.GetBiologicalSex (out error);
			if (error == null)
			{
				DispatchQueue.MainQueue.DispatchAsync (() => {
					var dataStore = StateDispatcher<HealthState>.State;

					HealthStateMutator.MutateBiologicalSex(
						dataStore, () => GetDisplayableBiologicalSex(biologicalSex.BiologicalSex));

					StateDispatcher<HealthState>.Refresh();
				});
			}
		}

		void RefreshQuantityValue(NSString quantityTypeKey)
		{			
			var quantityType = HKObjectType.GetQuantityType (quantityTypeKey);

			NSSortDescriptor timeSortDescriptor = new NSSortDescriptor(HKSample.SortIdentifierEndDate, false);

			// Since we are interested in retrieving the user's latest sample, we sort the samples in descending order, and set the limit to 1. We are not filtering the data, and so the predicate is set to nil.
			HKSampleQuery query = new HKSampleQuery(quantityType, null, 100,  new NSSortDescriptor[]{timeSortDescriptor}, 
				new HKSampleQueryResultsHandler((query2, results, error) =>
				{
					Action<HKSampleQuery, HKSample[], NSError> handler = null;
					if (results!= null && results.Length > 0 && _sampleQueryHandlers.TryGetValue(quantityTypeKey, out handler))
					{
						handler(query2, results, error);
					}
					
				}));
			
			HealthStore.ExecuteQuery (query);
		}

		private void RefreshCharacteristicValue(NSString characteristicTypeKey)
		{
			Action refreshAction = null;
			if (_characteristicQueryHandlers.TryGetValue(characteristicTypeKey, out refreshAction)) {
				refreshAction();
			}
		}

		//This method handles all the HealthKit gymnastics to remove a blood glucose entry.
		public void RemoveBloodGlucoseEntry(BloodGlucoseEntry entry)
		{			
			//Cast the entry as a HealthKitBloodGlucoseEntry...
			HealthKitBloodGlucoseEntry hkBloodGlucoseEntry = entry as HealthKitBloodGlucoseEntry;
			HealthStore.DeleteObject (hkBloodGlucoseEntry.BloodGlucoseSample, new Action<bool, NSError> ((success, error) => {
				if (!success || error != null)
				{
					//NOTE: If this app didn't put the entry into the blood glucose list, then there will be an error on delete.
					AlertManager.ShowError("Health Kit", "Unable to delete glucose sample: " + error);
				}
				else
				{
					//Woo! We properly removed the last entry, make sure that any listeners to the glucose states are properly updated.
					RefreshQuantityValue(HKQuantityTypeIdentifierKey.BloodGlucose);
				}
			}));
		}

		/// <summary>
		/// This method handles all the HealthKit gymnastics to add a blood glucose entry to the HealthKit data.
		/// </summary>
		/// <param name="entry">Entry.</param>
		public void AddBloodGlucoseEntry(BloodGlucoseEntry entry)
		{
			var date = new NSDate ();
			var quantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.BloodGlucose);
			var mgPerDL = HKUnit.FromString("mg/dL");
			var quantity = HKQuantity.FromQuantity (mgPerDL, entry.BloodGlucoseValue);
			var sample = HKQuantitySample.FromType (quantityType, quantity, date, date);

			HealthStore.SaveObject(sample, (success, error) => {
				if (!success || error != null)
				{
					//There may have been an add error for some reason.
					AlertManager.ShowError("Health Kit", "Unable to add glucose sample: " + error);
				}
				else
				{
					//Refresh all app wide blood glucose UI fields.
					RefreshQuantityValue(HKQuantityTypeIdentifierKey.BloodGlucose);
				}
			});
				
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


		public void Refresh()
		{			
			foreach (var quantityTypeToRead in QuantityTypesToRead) {
				RefreshQuantityValue (quantityTypeToRead);
			}

			foreach (var characteristicTypeToRead in CharacteristicTypesToRead) {
				RefreshCharacteristicValue (characteristicTypeToRead);
			}
		}



		public void RemoveStepCountEntry (StepCountEntry entry)
		{
			//Cast the entry as a HealthKitBloodGlucoseEntry...
			HealthKitStepCountEntry hkStepCountEntry = entry as HealthKitStepCountEntry;
			HealthStore.DeleteObject (hkStepCountEntry.StepCountSample, (success, error) => {
				if (!success || error != null)
				{
					//NOTE: If this app didn't put the entry into the blood glucose list, then there will be an error on delete.
					AlertManager.ShowError("Health Kit", "Unable to delete step count sample: " + error);
				}
				else
				{
					//Woo! We properly removed the last entry, make sure that any listeners to the glucose states are properly updated.
					RefreshQuantityValue(HKQuantityTypeIdentifierKey.StepCount);
				}
			});
		}

		public void AddStepCountEntry (StepCountEntry entry)
		{
			var date = new NSDate ();
			var quantityType = HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.StepCount);
			var countUnit = HKUnit.Count;
			var quantity = HKQuantity.FromQuantity (countUnit, entry.Count);
			var sample = HKQuantitySample.FromType (quantityType, quantity, date, date);

			HealthStore.SaveObject(sample, (success, error) => {
				if (!success || error != null)
				{
					//There may have been an add error for some reason.
					AlertManager.ShowError("Health Kit", "Unable to add step count sample: " + error);
				}
				else
				{
					//Refresh all app wide blood glucose UI fields.
					RefreshQuantityValue(HKQuantityTypeIdentifierKey.StepCount);
				}
			});
		}
	}
}


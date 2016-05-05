using System;
using SharedHealthState;
using Stateless;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Android.Content;
using System.Threading.Tasks;
using Android.Util;
using System.Linq;
using Android.OS;
using Java.Util.Concurrent;
using Android.App;
using System.Collections.Generic;

namespace GoogleFitSample
{
	public class GoogleFitDataStore : IHealthDataStore
	{
		public GoogleFitDataStore(Activity activity)
		{
			_activity = activity;
			BuildFitnessClient (activity);
			mClient.Connect ();
		}

		private Activity _activity;

		long GetMsSinceEpochAsLong (DateTime dateTime)
		{
			return (long)dateTime.ToUniversalTime ()
				.Subtract (new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
				.TotalMilliseconds;
		}
		
		private async void RefreshHealthStateData ()
		{
			await UpdateHeight ();
			await UpdateFood ();
			UpdateBiologicalSex ();


			var stepEntries = await UpdateStepEntries ();

			_activity.RunOnUiThread(() => {
				StateDispatcher<HealthState>.Refresh ();
				if (stepEntries != null)
				{
					HealthStateDispatchers.StepCountListStateDispatcher.Refresh(stepEntries);
				}
			});
		}

		async Task<List<StepCountEntry>> UpdateStepEntries()
		{
			//https://github.com/xamarin/monodroid-samples/blob/master/google-services/Fitness/BasicHistoryApi/BasicHistoryApi/MainActivity.cs
			DateTime endTime = DateTime.Now;
			DateTime startTime = endTime.Subtract (TimeSpan.FromDays (7)); 
			long endTimeElapsed = GetMsSinceEpochAsLong (endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong (startTime);


			var readRequest = new DataReadRequest.Builder ()
				.Aggregate (Android.Gms.Fitness.Data.DataType.TypeStepCountDelta, Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta)
				.SetTimeRange(startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.BucketByTime(1, TimeUnit.Days)
				.Build ();

			var readResult = await FitnessClass.HistoryApi.ReadDataAsync (mClient, readRequest);

			//The result may need to popup additional user security dialogs for the user to grant access to the specific data points.
			if (!readResult.Status.IsSuccess) {
				if (readResult.Status.HasResolution) {
					readResult.Status.StartResolutionForResult (_activity, REQUEST_GET_REQUEST_PERMISSION);
					return null;
				}
			}

			var stepCountEntryList = new List<StepCountEntry>();
			foreach (var bucket in readResult.Buckets) {
				var dataSet = bucket.DataSets.LastOrDefault ();
				if (dataSet != null) {
					var value = GetLastValueInDataSet (dataSet);
					if (value != null) {
						stepCountEntryList.Add (new StepCountEntry (Convert.ToDouble(value.AsInt ())));
					}
				}
			}

			//We should have a bucket per day, parse it all out.
			return stepCountEntryList;
		}

		async Task UpdateFood()
		{
			
			DateTime endTime = DateTime.Now;

			DateTime startTime = endTime.Subtract (TimeSpan.FromDays (7)); 
			long endTimeElapsed = GetMsSinceEpochAsLong (endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong (startTime);


			var readRequest = new DataReadRequest.Builder ()
				.Read (Android.Gms.Fitness.Data.DataType.TypeNutrition)
				.SetTimeRange(startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.SetLimit(100)
				.Build ();

			var readResult = await FitnessClass.HistoryApi.ReadDataAsync (mClient, readRequest);

			//The result may need to popup additional user security dialogs for the user to grant access to the specific data points.
			if (!readResult.Status.IsSuccess) {
				if (readResult.Status.HasResolution) {
					readResult.Status.StartResolutionForResult (_activity, REQUEST_GET_REQUEST_PERMISSION);
					return;
				}
			}

			foreach (var dataSet in readResult.DataSets) {
				foreach (var dataPoint in dataSet.DataPoints) {
					//See if dataPoint has nutrient info in it.
					var foodNameValue = dataPoint.GetValue (Field.FieldFoodItem);
					var foodName = foodNameValue.AsString ();

					var nutrientsValue = dataPoint.GetValue (Field.FieldNutrients);
					//var calories = nutrientsValue.GetKeyValue (Field.FieldCalories);
					var i = 0;
				}
			}

		}

		async Task UpdateHeight()
		{
			//http://stackoverflow.com/questions/28482176/read-the-height-in-googlefit-in-android

			DateTime endTime = DateTime.Now;

			//TimeSpan has to be huge, otherwise we may never get the user's height.
			//May also have to query back for YEARS or multiple query in loop until you get an answer.
			//Don't like the Google Fit API...

			DateTime startTime = endTime.Subtract (TimeSpan.FromDays (1024)); 
			long endTimeElapsed = GetMsSinceEpochAsLong (endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong (startTime);


			var readRequest = new DataReadRequest.Builder ()
				.Read (Android.Gms.Fitness.Data.DataType.TypeHeight)
				.SetTimeRange(startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.SetLimit(1)
				.Build ();

			var readResult = await FitnessClass.HistoryApi.ReadDataAsync (mClient, readRequest);

			//The result may need to popup additional user security dialogs for the user to grant access to the specific data points.
			if (!readResult.Status.IsSuccess) {
				if (readResult.Status.HasResolution) {
					readResult.Status.StartResolutionForResult (_activity, REQUEST_GET_REQUEST_PERMISSION);
					return;
				}
			}

			var value = GetLastValueInDataSet (readResult.DataSets.LastOrDefault ());
			if (value != null) {
				HealthStateMutator.MutateHeight(
					StateDispatcher<HealthState>.State, () => value.AsFloat ());
			}
		}

		void UpdateBiologicalSex()
		{
			//https://www.npmjs.com/package/cordova-plugin-health 
			//-- Helpful mapping of HealthKit attributes to Google Fit constants.
			// Retrieval of gender is not supported, but can be added as a custom attribute for just this app.
			HealthStateMutator.MutateBiologicalSex(
				StateDispatcher<HealthState>.State, () => "Unknown");
		}

		const string DATE_FORMAT = "yyyy.MM.dd HH:mm:ss";

		Value GetLastValueInBucket(Bucket bucket)
		{
			DataSet dataSet = bucket.DataSets.LastOrDefault();
			return GetLastValueInDataSet (dataSet);
		}

		Value GetLastValueInDataSet (DataSet dataSet)
		{
			Log.Info (TAG, "Data returned for Data type: " + dataSet.DataType.Name);
			var dp = dataSet.DataPoints.LastOrDefault ();
			if (dp != null)
			{
				Log.Info (TAG, "Data point:");
				Log.Info (TAG, "\tType: " + dp.DataType.Name);
				Log.Info (TAG, "\tStart: " + new DateTime (1970, 1, 1).AddMilliseconds (
					dp.GetStartTime (TimeUnit.Milliseconds)).ToString (DATE_FORMAT));
				Log.Info (TAG, "\tEnd: " + new DateTime (1970, 1, 1).AddMilliseconds (
					dp.GetEndTime (TimeUnit.Milliseconds)).ToString (DATE_FORMAT));
				
				foreach (Field field in dp.DataType.Fields) {
					Log.Info (TAG, "\tField: " + field.Name +
						" Value: " + dp.GetValue (field));
					return dp.GetValue (field);
				}
			}
			return null;
		}

		GoogleApiClient mClient;

		public const string TAG = "BasicHistoryApi";
		const int REQUEST_OAUTH = 1;
		const int REQUEST_GET_REQUEST_PERMISSION = 2;

		void BuildFitnessClient (Activity activity)
		{
			var clientConnectionCallback = new ClientConnectionCallback ();
			clientConnectionCallback.OnConnectedImpl = () => {
				RefreshHealthStateData();
			};

			// Create the Google API Client
			mClient = new GoogleApiClient.Builder (activity)
				.AddApi (FitnessClass.HISTORY_API)
				.AddScope (new Scope (Scopes.FitnessActivityReadWrite))
				.AddConnectionCallbacks (clientConnectionCallback)
				.AddOnConnectionFailedListener (result => {
					Log.Info (TAG, "Connection failed. Cause: " + result);
					if (!result.HasResolution) {
						// Show the localized error dialog
						GooglePlayServicesUtil.GetErrorDialog (result.ErrorCode, activity, 0).Show ();
						return;
					}
					// The failure has a resolution. Resolve it.
					// Called typically when the app is not yet authorized, and an
					// authorization dialog is displayed to the user.
					if (!authInProgress) {
						try {
							Log.Info (TAG, "Attempting to resolve failed connection");
							authInProgress = true;
							result.StartResolutionForResult (activity, REQUEST_OAUTH);
						} catch (IntentSender.SendIntentException e) {
							Log.Error (TAG, "Exception while starting resolution activity", e);
						}
					}
				}).Build ();
		}

		public void ProcessActivityResult(int requestCode, Result resultCode)
		{
			if (requestCode == REQUEST_OAUTH) {
				authInProgress = false;
				if (resultCode == Result.Ok) {
					// Make sure the app is not already connected or attempting to connect
					if (!mClient.IsConnecting && !mClient.IsConnected) {
						mClient.Connect ();
					}
				}
			} else if (requestCode == REQUEST_GET_REQUEST_PERMISSION) {
				this.RefreshHealthStateData ();
			}
		}

		class ClientConnectionCallback : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks
		{
			public Action OnConnectedImpl { get; set; }

			public void OnConnected (Bundle connectionHint)
			{
				Log.Info (TAG, "Connected!!!");

				Task.Run (OnConnectedImpl);
			}

			public void OnConnectionSuspended (int cause)
			{
				if (cause == GoogleApiClient.ConnectionCallbacksConsts.CauseNetworkLost) {
					Log.Info (TAG, "Connection lost.  Cause: Network Lost.");
				} else if (cause == GoogleApiClient.ConnectionCallbacksConsts.CauseServiceDisconnected) {
					Log.Info (TAG, "Connection lost.  Reason: Service Disconnected");
				}
			}
		}

		private bool authInProgress;

		#region IHealthDataStore implementation

		public void Refresh ()
		{
			//This method is responsible for processing the Google Fit entries, and shooting out
			//HealthState, BloodGlucoseRecommendationState, and BloodGlucoseEntryListState / BloodBlucoseEntryState objects.
			//For now we are just going to have Google Fit shoot out HealthState, because BloodGlucose is not supported by Google Fit.
			RefreshHealthStateData();
		}

		public void RemoveBloodGlucoseEntry (BloodGlucoseEntry entry)
		{
			//Google Fit doesn't support Blood Glucose, but we could add it as a custom type.
			throw new NotImplementedException ();
		}

		public void AddBloodGlucoseEntry (BloodGlucoseEntry entry)
		{
			//Google Fit doesn't support Blood Glucose, but we could add it as a custom type. Hmmmm....
			throw new NotImplementedException ();
		}

		public void RemoveStepCountEntry (StepCountEntry entry)
		{
			//Google Fit doesn't support Blood Glucose, but we could add it as a custom type.
			throw new NotImplementedException ();
		}

		public async void AddStepCountEntry (StepCountEntry entry)
		{
			Log.Info (TAG, "Creating a new data insert request");

			// Set a start and end time for our data, using a start time of 1 hour before this moment.
			DateTime endTime = DateTime.Now;
			DateTime startTime = endTime.Subtract (TimeSpan.FromHours (1));
			long endTimeElapsed = GetMsSinceEpochAsLong (endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong (startTime);

			// Create a data source
			var dataSource = new DataSource.Builder ()
				.SetAppPackageName (this._activity)
				.SetDataType (Android.Gms.Fitness.Data.DataType.TypeStepCountDelta)
				.SetName (TAG + " - step count")
				.SetType (DataSource.TypeRaw)
				.Build ();

			// Create a data set
			//const int stepCountDelta = 1000;
			var dataSet = DataSet.Create (dataSource);
			DataPoint dataPoint = dataSet.CreateDataPoint ()
				.SetTimeInterval (startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds);
			dataPoint.GetValue (Field.FieldSteps).SetInt (Convert.ToInt32(entry.Count));
			dataSet.Add (dataPoint);

			Log.Info (TAG, "Inserting the dataset in the History API");
			var insertStatus =  await FitnessClass.HistoryApi.InsertDataAsync (mClient, dataSet);

			if (!insertStatus.IsSuccess) {
				Log.Info (TAG, "There was a problem inserting the dataset.");
				return;
			}

			Log.Info (TAG, "Data insert was successful!");

			RefreshHealthStateData ();
		}

		public void RemoveFoodEntry (FoodEntry entry)
		{
			throw new NotImplementedException ();
		}

		public void AddFoodEntry (FoodEntry entry)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}


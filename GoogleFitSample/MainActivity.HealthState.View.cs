using System;
using Stateless;
using SharedHealthState;
using Android.Widget;

namespace GoogleFitSample
{
	partial class MainActivity : IView<HealthState>
	{
		private void Bind() {
			StateDispatcher<HealthState>.Bind(this);
			StateDispatcher<BloodGlucoseRecommendationState>.Bind (this);
			HealthStateDispatchers.StepCountListStateDispatcher.Bind (this);
		}

		public void Receive(HealthState healthState)
		{
			var height = healthState.Height;
			var textView = FindViewById<TextView> (Resource.Id.heightInMeters);
			textView.Text = string.Format("Height: {0}", height);
		}
	}
}


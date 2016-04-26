using System;
using Stateless;
using SharedHealthState;

namespace HealthKitSample
{
	partial class ViewController : IView<BloodGlucoseRecommendationState>
	{
		public void Receive(BloodGlucoseRecommendationState state)
		{
			this.bloodGlucoseRecommendation.Text = state.RecommendationText;
		}
	}
}


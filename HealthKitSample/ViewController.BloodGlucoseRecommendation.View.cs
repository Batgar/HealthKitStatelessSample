using System;
using Stateless;


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


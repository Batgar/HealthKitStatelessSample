using System;
using Stateless;
using SharedHealthState;

namespace GoogleFitSample
{
	partial class MainActivity : IView<BloodGlucoseRecommendationState>
	{
		public void Receive(BloodGlucoseRecommendationState state)
		{
			var recommendationText = state.RecommendationText;
			//TODO: Send recommendation text to a label on the view.
		}
	}		
}


using System;
using Stateless;

namespace HealthKitSample
{
	public class BloodGlucoseRecommendationState : IState
	{
		public BloodGlucoseRecommendationState ()
		{
			
		}

		private double _bloodGlucose;

		public double BloodGlucose 
		{
			get { return _bloodGlucose;}
			set{ 
				_bloodGlucose = value;
				ResetRecommendations ();
			}
		}

		private void ResetRecommendations()
		{
			if (_bloodGlucose < 80) {
				RecommendationText = "Dude, you need to get some candy! Stat!";
			} else if (_bloodGlucose < 120) {
				RecommendationText = "All is cool";
			} else {
				RecommendationText = "Whoa! You have really high blood sugar. Might be time for insulin!";
			}
		}

		public string RecommendationText {get; private set;}
	}
}


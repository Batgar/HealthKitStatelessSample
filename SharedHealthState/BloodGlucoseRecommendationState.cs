using System;
using Stateless;

namespace SharedHealthState
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
			internal set{ 
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

	public static class BloodGlucoseRecommendationMutator
	{
		public static void MutateBloodGlucose(BloodGlucoseRecommendationState state, Func<double> mutator)
		{
			state.BloodGlucose = mutator ();
		}
	}
}


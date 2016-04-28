using System;
using Stateless;
using SharedHealthState;

namespace HealthKitSample
{
	partial class ViewController
	{
		private void Bind()
		{
			StateDispatcher<HealthState>.Bind(this);
			StateDispatcher<BloodGlucoseRecommendationState>.Bind (this);
			HealthStateDispatchers.StepCountListStateDispatcher.Bind (this);
		}

		private void Unbind()
		{
			StateDispatcher<HealthState>.Unbind(this);
			StateDispatcher<BloodGlucoseRecommendationState>.Unbind (this);
			HealthStateDispatchers.StepCountListStateDispatcher.Unbind (this);
		}
	}
}


using System;
using Stateless;

namespace HealthKitSample
{
	partial class ViewController
	{
		private void Bind()
		{
			StateDispatcher<HealthState>.Bind(this);
			StateDispatcher<BloodGlucoseRecommendationState>.Bind (this);
		}

		private void Unbind()
		{
			StateDispatcher<HealthState>.Unbind(this);
			StateDispatcher<BloodGlucoseRecommendationState>.Unbind (this);
		}
	}
}


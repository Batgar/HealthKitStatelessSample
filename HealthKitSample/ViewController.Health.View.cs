using System;
using Stateless;

namespace HealthKitSample
{
	partial class ViewController : IView<HealthState>
	{
		public void Receive(HealthState state)
		{
			this.weight.Text = state.Height.ToString();
			this.bloodGlucose.Text = state.BloodGlucose.ToString ();
			this.biologicalSex.Text = state.BiologicalSex;
		}
	}
}


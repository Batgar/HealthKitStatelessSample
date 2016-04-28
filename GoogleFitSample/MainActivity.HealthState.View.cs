using System;
using Stateless;
using SharedHealthState;

namespace GoogleFitSample
{
	partial class MainActivity : IView<HealthState>
	{
		private void Bind() {
			StateDispatcher<HealthState>.Bind(this);
		}

		public void Receive(HealthState healthState)
		{
			var i = 0;
		}
	}
}


using System;
using Stateless;
using System.Collections.Generic;

namespace SharedHealthState
{
	public class HealthState : IState {

		public double Height { get; internal set;}
		public double BloodGlucose {get; internal set;}
		public string BiologicalSex {get; internal set;}

	}

	public static class HealthStateMutator {
		public static void MutateHeight(HealthState state, Func<double> mutator)
		{
			state.Height = mutator();
		}

		public static void MutateBloodGlucose(HealthState state, Func<double> mutator)
		{
			state.BloodGlucose = mutator();
		}

		public static void MutateBiologicalSex(HealthState state, Func<string> mutator)
		{
			state.BiologicalSex = mutator();
		}
	}
}


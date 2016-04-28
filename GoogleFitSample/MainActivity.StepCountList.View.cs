using System;
using Stateless;
using SharedHealthState;
using Android.Widget;
using System.Linq;

namespace GoogleFitSample
{
	partial class MainActivity : IListView<StepCountEntry>
	{	

		#region IListView implementation

		public void Receive (System.Collections.Generic.IList<SharedHealthState.StepCountEntry> list, System.Collections.Generic.IList<Stateless.Change> changeset)
		{
			var button = FindViewById<Button> (Resource.Id.stepsButton);
			button.Text = string.Format ("Steps: {0}", list.Sum (s => s.Count));
		}

		#endregion
	}
}


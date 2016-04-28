
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GoogleFitSample
{
	[Activity (Label = "StepCountListActivity")]			
	public class StepCountListActivity : ListActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			this.SetContentView (this.LayoutInflater.Inflate (Resource.Layout.StepCountList, null));

			this.ListAdapter = new StepCountListAdapter(this);

			Button button = FindViewById<Button> (Resource.Id.addStepCountBtn);

			button.Click += delegate {
				(this.ListAdapter as StepCountListAdapter).AddStepEntry(1545);
			};



		}
	}
}


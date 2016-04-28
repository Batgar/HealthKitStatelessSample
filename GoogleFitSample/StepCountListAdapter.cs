using System;
using Stateless;
using SharedHealthState;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.App;

namespace GoogleFitSample
{
	public class StepCountListAdapter : BaseAdapter<StepCountEntry>, IListView<StepCountEntry>
	{
		public StepCountListAdapter (Activity activity)
		{
			Context = activity;

			HealthStateDispatchers.StepCountListStateDispatcher.Bind (this);
		}

		#region IListView implementation

		public void Receive (System.Collections.Generic.IList<StepCountEntry> list, System.Collections.Generic.IList<Change> changeset)
		{
			this.NotifyDataSetChanged ();	
		}

		#endregion

		Activity Context {get; set;}

		private List<StepCountEntry> StepCountEntries {
			get {
				return HealthStateDispatchers.StepCountListStateDispatcher.MainListStore.MainList;
			}
		}

		public override StepCountEntry this[int index] {
			get {
				if (StepCountEntries != null && StepCountEntries.Count > 0) {
					return StepCountEntries [index];
				}
				return null;
			}
		}

		public override int Count {
			get {
				if (StepCountEntries != null) {
					return StepCountEntries.Count;
				}
				return 0;
			}
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
				view = Context.LayoutInflater.Inflate(Resource.Layout.StepCountListItem, null);
			view.FindViewById<TextView>(Resource.Id.stepCount).Text = string.Format("Steps {0}" , StepCountEntries[position].Count);
			return view;
		}

		internal void AddStepEntry(int stepCount)
		{
			HealthStateDispatchers.StepCountListStateDispatcher.Add (new StepCountEntry () { Count = stepCount });
		}
	}
}


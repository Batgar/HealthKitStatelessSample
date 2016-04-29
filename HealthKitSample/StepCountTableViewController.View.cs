using System;
using SharedHealthState;

namespace HealthKitSample
{
	partial class StepCountTableViewController : GeneralListViewTableViewController<StepCountEntry, StepCountEntryListStore>
	{
		#region implemented abstract members of GeneralListViewTableViewController

		protected override Stateless.ListStateDispatcher<SharedHealthState.StepCountEntry, SharedHealthState.StepCountEntryListStore> GetDispatcher ()
		{
			return HealthStateDispatchers.StepCountListStateDispatcher;
		}

		protected override void UpdateTableViewCell (UIKit.UITableViewCell cell, SharedHealthState.StepCountEntry state)
		{
			cell.TextLabel.Text = state.Count.ToString();
		}

		protected override string TableCellIdentifier {
			get {
				return "StepCountTableCellIdentifier";
			}
		}

		#endregion

		//This is where the "+" toolbar button will bind to.
		private void AddStepCountEntry(double steps)
		{
			this.AddRow(new StepCountEntry(steps));
		}
	}
}


using System;
using System.Collections.Generic;
using Stateless;
using Foundation;
using SharedHealthState;

namespace HealthKitSample
{
	partial class BloodGlucoseTableViewController : GeneralListViewTableViewController<BloodGlucoseEntry, BloodGlucoseEntryListState>
	{
		#region implemented abstract members of GeneralListViewTableViewController
		protected override Stateless.ListStateDispatcher<BloodGlucoseEntry, BloodGlucoseEntryListState> GetDispatcher ()
		{
			return HealthStateDispatchers.BloodGlucoseListStateDispatcher;
		}
		protected override void UpdateTableViewCell (UIKit.UITableViewCell cell, BloodGlucoseEntry state)
		{
			cell.TextLabel.Text = state.BloodGlucoseValue.ToString();
		}
		protected override string TableCellIdentifier {
			get {
				return "BloodGlucoseTableCellIdentifier";
			}
		}
		#endregion

		//This is where the "+" toolbar button will bind to.
		private void AddBloodGlucoseEntry(double bloodGlucoseValue)
		{
			this.AddRow(new BloodGlucoseEntry(){BloodGlucoseValue = bloodGlucoseValue});
		}
	}
}


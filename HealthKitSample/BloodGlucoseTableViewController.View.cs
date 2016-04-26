using System;
using System.Collections.Generic;
using Stateless;
using Foundation;

namespace HealthKitSample
{
	partial class BloodGlucoseTableViewController : GeneralListViewTableViewController<BloodGlucoseEntry, BloodGlucoseEntryListState>
	{
		#region implemented abstract members of GeneralListViewTableViewController
		protected override Stateless.ListStateDispatcher<HealthKitSample.BloodGlucoseEntry, HealthKitSample.BloodGlucoseEntryListState> GetDispatcher ()
		{
			return HealthKitDispatchers.BloodGlucoseListStateDispatcher;
		}
		protected override void UpdateTableViewCell (UIKit.UITableViewCell cell, HealthKitSample.BloodGlucoseEntry state)
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


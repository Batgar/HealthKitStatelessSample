using System;
using System.Collections.Generic;
using Stateless;
using Foundation;

namespace HealthKitSample
{
	partial class BloodGlucoseTableViewController : IListView<BloodGlucoseEntry>
	{
		private void Bind()
		{
			HealthKitDispatchers.BloodGlucoseListStateDispatcher.Bind (this);
		}

		private void Unbind()
		{
			HealthKitDispatchers.BloodGlucoseListStateDispatcher.Unbind (this);
		}

		public void Receive(IList<BloodGlucoseEntry> entries, IList<Change> changeset) {
			
			foreach (var change in changeset) {
				switch (change.ChangeSetType) {
				case Change.ChangeType.Delete:
					{
						this.TableView.DeleteRows (new NSIndexPath[] { NSIndexPath.FromRowSection (change.Index, 0) }, UIKit.UITableViewRowAnimation.Right);
						break;
					}
				case Change.ChangeType.Insert:
					{
						this.TableView.InsertRows (new NSIndexPath[] { NSIndexPath.FromRowSection (change.Index, 0) }, UIKit.UITableViewRowAnimation.Left);
						break;
					}
				}
			}
		}

		public override nint NumberOfSections (UIKit.UITableView tableView)
		{
			if (HealthKitDispatchers.BloodGlucoseListStateDispatcher.Store.Count > 0) {
				return 1;
			}
			return 0;
		}

		public override nint RowsInSection (UIKit.UITableView tableView, nint section)
		{
			return HealthKitDispatchers.BloodGlucoseListStateDispatcher.Store.Count;
		}

		public override UIKit.UITableViewCell GetCell (UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var tableCell = this.TableView.DequeueReusableCell("BloodGlucoseTableCellIdentifier");

			var dataForRow = HealthKitDispatchers.BloodGlucoseListStateDispatcher.Store [indexPath.Row];

			tableCell.TextLabel.Text = dataForRow.BloodGlucoseValue.ToString();

			return tableCell;
		}

		public override void CommitEditingStyle (UIKit.UITableView tableView, UIKit.UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			if (editingStyle == UIKit.UITableViewCellEditingStyle.Delete) {
				HealthKitDispatchers.BloodGlucoseListStateDispatcher.RemoveAtIndex (indexPath.Row);
			}
		}

		private void AddBloodGlucoseEntry(double bloodGlucoseValue)
		{
			HealthKitDispatchers.BloodGlucoseListStateDispatcher.Add(new BloodGlucoseEntry(){BloodGlucoseValue = bloodGlucoseValue});
		}

	}
}


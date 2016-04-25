using System;
using System.Collections.Generic;
using Stateless;
using Foundation;

namespace HealthKitSample
{
	partial class BloodGlucoseTableViewController : IListSubscriber<BloodGlucoseEntry>
	{
		private void Bind()
		{
			ListStateDispatcher<BloodGlucoseEntry>.Bind (this);
		}

		private void Unbind()
		{
			ListStateDispatcher<BloodGlucoseEntry>.Unbind (this);
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
			if (ListStateDispatcher<BloodGlucoseEntry>.Store.Count > 0) {
				return 1;
			}
			return 0;
		}

		public override nint RowsInSection (UIKit.UITableView tableView, nint section)
		{
			return ListStateDispatcher<BloodGlucoseEntry>.Store.Count;
		}

		public override UIKit.UITableViewCell GetCell (UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var tableCell = this.TableView.DequeueReusableCell("BloodGlucoseTableCellIdentifier");

			var dataForRow = ListStateDispatcher<BloodGlucoseEntry>.Store [indexPath.Row];

			tableCell.TextLabel.Text = dataForRow.BloodGlucoseValue.ToString();

			return tableCell;
		}

		public override void CommitEditingStyle (UIKit.UITableView tableView, UIKit.UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			if (editingStyle == UIKit.UITableViewCellEditingStyle.Delete) {
				ListStateDispatcher<BloodGlucoseEntry>.RemoveAtIndex (indexPath.Row);
			}
		}

		private void AddBloodGlucoseEntry(double bloodGlucoseValue)
		{
			ListStateDispatcher<BloodGlucoseEntry>.Add(new BloodGlucoseEntry(){BloodGlucoseValue = bloodGlucoseValue});
		}

	}
}


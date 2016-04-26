using System;
using UIKit;
using Stateless;
using System.Collections.Generic;
using Foundation;

namespace HealthKitSample
{
	public abstract class GeneralListViewTableViewController<T,S> : UITableViewController, IListView<T>
		where T : IIdentifiable
		where S : IListStore<T>, new()
	{
		protected abstract ListStateDispatcher<T, S> GetDispatcher();

		public GeneralListViewTableViewController (IntPtr handle) : base (handle)
		{
		}
		
		protected void Bind()
		{
			GetDispatcher().Bind (this);
		}

		protected void Unbind()
		{
			GetDispatcher().Unbind (this);
		}

		public void Receive(IList<T> entries, IList<Change> changeset) {

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
			if (GetDispatcher().Store.Count > 0) {
				return 1;
			}
			return 0;
		}

		public override nint RowsInSection (UIKit.UITableView tableView, nint section)
		{
			return GetDispatcher().Store.Count;
		}

		public override UIKit.UITableViewCell GetCell (UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var tableCell = this.TableView.DequeueReusableCell(TableCellIdentifier);

			var dataForRow = GetDispatcher().Store [indexPath.Row];

			UpdateTableViewCell (tableCell, dataForRow);

			return tableCell;
		}

		protected abstract string TableCellIdentifier { get;}

		protected abstract void UpdateTableViewCell(UITableViewCell cell, T state);

		public override void CommitEditingStyle (UIKit.UITableView tableView, UIKit.UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			if (editingStyle == UIKit.UITableViewCellEditingStyle.Delete) {
				GetDispatcher().RemoveAtIndex (indexPath.Row);
			}
		}

		protected void AddRow(T entry)
		{
			GetDispatcher ().Add (entry);
		}

	}
}


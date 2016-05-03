using System;
using SharedHealthState;

namespace HealthKitSample
{
	partial class FoodTableViewController : GeneralListViewTableViewController<FoodEntry, FoodEntryListStore>
	{
		#region implemented abstract members of GeneralListViewTableViewController

		protected override Stateless.ListStateDispatcher<SharedHealthState.FoodEntry, SharedHealthState.FoodEntryListStore> GetDispatcher ()
		{
			return HealthStateDispatchers.FoodListStateDispatcher;
		}

		protected override void UpdateTableViewCell (UIKit.UITableViewCell cell, SharedHealthState.FoodEntry state)
		{
			cell.TextLabel.Text = state.FoodName;
			cell.DetailTextLabel.Text = string.Format ("Potassium: {0} mg -- Fiber: {1} g", state.Potassium, state.Fiber);
		}

		protected override string TableCellIdentifier {
			get {
				return "FoodEntryTableCellIdentifier";
			}
		}

		#endregion

		//This is where the "+" toolbar button will bind to.
		private void AddFoodEntry(string foodName, double potassium, double fiber)
		{
			this.AddRow(new FoodEntry(foodName, potassium, fiber));
		}
	}
}


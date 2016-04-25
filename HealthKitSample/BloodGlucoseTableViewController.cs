using System;

using UIKit;

namespace HealthKitSample
{
	public partial class BloodGlucoseTableViewController : UITableViewController
	{
		public BloodGlucoseTableViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//This table view allows us to add / remove items. Woo!
			this.NavigationItem.RightBarButtonItems= new UIBarButtonItem[]{
				new UIBarButtonItem(UIBarButtonSystemItem.Add, new EventHandler((o,e) => {
					AddBloodGlucoseEntry(98);
				})), 
				this.EditButtonItem};

			// Perform any additional setup after loading the view, typically from a nib.
			Bind();

		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			Unbind ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}


	}
}



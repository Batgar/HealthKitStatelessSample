// WARNING
//
// This file has been generated automatically by Xamarin Studio Business to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace HealthKitSample
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UILabel biologicalSex { get; set; }

		[Outlet]
		UIKit.UILabel bloodGlucose { get; set; }

		[Outlet]
		UIKit.UILabel bloodGlucoseRecommendation { get; set; }

		[Outlet]
		UIKit.UILabel weeklyStepCountValue { get; set; }

		[Outlet]
		UIKit.UILabel weight { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (biologicalSex != null) {
				biologicalSex.Dispose ();
				biologicalSex = null;
			}

			if (bloodGlucose != null) {
				bloodGlucose.Dispose ();
				bloodGlucose = null;
			}

			if (bloodGlucoseRecommendation != null) {
				bloodGlucoseRecommendation.Dispose ();
				bloodGlucoseRecommendation = null;
			}

			if (weight != null) {
				weight.Dispose ();
				weight = null;
			}

			if (weeklyStepCountValue != null) {
				weeklyStepCountValue.Dispose ();
				weeklyStepCountValue = null;
			}
		}
	}
}

using System;
using UIKit;
using CoreFoundation;

namespace HealthKitSample
{
	public static class AlertManager
	{
		

		public static void ShowError(string title, string message)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				var alert = new UIAlertView (title, message, null, "OK", null);
				alert.Show ();
			});
		}

		public static void ShowInfo(string title, string message)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				var alert = new UIAlertView (title, message, null, "OK", null);
				alert.Show ();
			});
		}
	}
}


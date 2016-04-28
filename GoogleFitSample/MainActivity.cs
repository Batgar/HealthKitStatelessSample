using Android.App;
using Android.Widget;
using Android.OS;
using SharedHealthState;
using Android.Content;

namespace GoogleFitSample
{
	[Activity (Label = "GoogleFitSample", MainLauncher = true, Icon = "@mipmap/icon")]
	public partial class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			TinyIoC.TinyIoCContainer.Current.Register<IHealthDataStore> (new GoogleFitDataStore (this));

			this.Bind ();

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			var googleFitDataStore = TinyIoC.TinyIoCContainer.Current.Resolve<IHealthDataStore> () as GoogleFitDataStore;
			googleFitDataStore.ProcessActivityResult (requestCode, resultCode);
		}
	}
}



using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace mindTheApp
{
	[Activity (Label = "mindTheApp", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		private void NotifyWebBrowser(){

			int id = 0;
			NotificationManager notificationManager = this.GetSystemService (Context.NotificationService) as NotificationManager;
			var n = new Notification.Builder(this).SetContentTitle("AppWasOpened" + id)
													.SetContentText("text" + id)
													.SetSmallIcon(Resource.Drawable.Icon);
			notificationManager.Notify (id, n.Build());

		}
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// ernesto
			var l = new LogReader ();
			System.Threading.Tasks.Task.Factory.StartNew (l.TryLogs);
			LogReader.AddCallback ("com.android.chrome", this.NotifyWebBrowser);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
	
			button.Click += delegate {
				var myIntent = new Intent (this, typeof(AppChooserActivity));
				StartActivityForResult (myIntent, 0);
			};
	
			//this.ApplicationContext.StartService ();

			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetDisplayShowTitleEnabled (true);
			// ActionBar.SetCustomView (Resource.Layout.loweractionbar);
			// ActionBar.SetDisplayShowCustomEnabled (true);

		}
//		[Application(UiOptions = UiOptions.SplitActionBarWhenNarrow)]
//		public class App : Application {
//			public App(IntPtr javaReference, JniHandleOwnership transfer) 
//				: base(javaReference, transfer) { }
//		}
//		[Activity(Label = "", UiOptions=Android.Content.PM.UiOptions.SplitActionBarWhenNarrow)]
	
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok) {
				var helloLabel = FindViewById<TextView> (Resource.Id.textView1);
				helloLabel.Text = data.GetStringExtra("packageName");
			}
		}
	
	
	}


}

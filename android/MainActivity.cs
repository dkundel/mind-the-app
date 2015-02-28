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

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			var l = new LogReader ();
			System.Threading.Tasks.Task.Factory.StartNew (l.TryLogs);
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};
			LogReader.AddCallback ("com.android.chrome", this.NotifyWebBrowser);
			//this.ApplicationContext.StartService ();

			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetDisplayShowTitleEnabled (true);
			ActionBar.SetCustomView (Resource.Layout.loweractionbar);
			ActionBar.SetDisplayShowCustomEnabled (true);

		}
//		[Application(UiOptions = UiOptions.SplitActionBarWhenNarrow)]
//		public class App : Application {
//			public App(IntPtr javaReference, JniHandleOwnership transfer) 
//				: base(javaReference, transfer) { }
//		}
//		[Activity(Label = "", UiOptions=Android.Content.PM.UiOptions.SplitActionBarWhenNarrow)]
	}

}

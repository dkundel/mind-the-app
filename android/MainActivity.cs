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

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// ernesto
			var l = new LogReader ();
			System.Threading.Tasks.Task.Factory.StartNew (l.TryLogs);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

//			ImageView view = FindViewById<ImageView> (Resource.Id.frontImage);
//			view.SetImageResource (Resource.Drawable.Icon);
			LogReader.SetActivity (this);

			Button appN = FindViewById<Button> (Resource.Id.appNotification);
			Button webN = FindViewById<Button> (Resource.Id.webNotification);

			appN.Click += delegate {
				StartActivity(typeof(Conditionals.ConditionalPicker));
			};

			webN.Click += delegate {
				StartActivity(typeof(Chrome));
			};

			//StartActivity(typeof(AppChooserActivity));
			//StartActivity(typeof(Chrome));
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
	

	
	
	}


}

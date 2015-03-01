
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;



namespace mindTheApp
{
	[Activity (Label = "Choose a App")]			
	public class AppChooserActivity : Activity
	{
		List<ApplicationListItem> applicationListItems = new List<ApplicationListItem>();
		ListView listView;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.AppChooserList);
			listView = FindViewById<ListView>(Resource.Id.List);
		
			var appList = this.PackageManager.GetInstalledApplications (0);			
			for (int i = 0; i < appList.Count (); i++) {
				if (appList [i].SourceDir.StartsWith("/data/app/")) {
					applicationListItems.Add (new ApplicationListItem () {
						Heading = appList [i].LoadLabel (this.PackageManager),
						ImageDrawable = appList [i].LoadIcon (this.PackageManager)
					});
				}
			}

			applicationListItems = applicationListItems.OrderBy (o => o.Heading).ToList ();

			listView.Adapter = new ApplicationListAdapter(this, applicationListItems);
			listView.ItemClick += OnListItemClick;
		}
		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = applicationListItems[e.Position];
			Android.Widget.Toast.MakeText(this, t.Heading, Android.Widget.ToastLength.Short).Show();
		}
	}
}
	
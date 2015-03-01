
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
using System.Net.Http;
using System.Collections.Specialized;
using System.Net;
using Android.Webkit;

namespace mindTheApp
{
	[Activity (Label = "Add URL Notification")]			
	public class Chrome : Activity
	{
		private EditText name,trigger,message;

		private string endpoint = "http://mindtheapp.ngrok.com/reminders";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Chrome);
			string name = Intent.GetStringExtra ("name") ?? "";
			string url = Intent.GetStringExtra ("url") ?? "";
			string message = Intent.GetStringExtra ("message") ?? "";

			this.name = FindViewById<EditText> (Resource.Id.name);
			this.message = FindViewById<EditText> (Resource.Id.message);
			this.trigger = FindViewById<EditText> (Resource.Id.trigger);

			Button btn = FindViewById<Button> (Resource.Id.saveBtn);
			btn.Click += delegate {
				this.Save();
				Finish();
			};
			// Create your application here
		}

		private void Save(){

			//HttpClient client = new HttpClient ();
			NameValueCollection args = new NameValueCollection ();
			args.Add ("name", this.name.Text);
			args.Add ("message", this.message.Text);
			args.Add ("trigger", this.trigger.Text);

			string data = "name=" + this.name.Text + "&message=" + this.message.Text + "&trigger=" + this.trigger.Text;

			WebView web = FindViewById<WebView> (Resource.Id.browser);

			web.Settings.JavaScriptEnabled = true;

			web.PostUrl (
				this.endpoint,
				System.Text.Encoding.UTF8.GetBytes(data));
				
			//Finish ();
		}
	}
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace mindTheApp
{
	public static class Conditionals{

		public interface Conditional
		{
			bool Check (Activity activity);
			string Render ();
		}
			
		public class TimeOfTheDay : Conditional{

			private int startHour, startMinute, endHour, endMinute;

			public TimeOfTheDay(int startHour,int startMinute,int endHour,int endMinute){

				this.startHour = startHour;
				this.startMinute = startMinute;
				this.endHour = endHour;
				this.endMinute = endMinute;
			}

			public bool Check(Activity a){

				var time = new DateTime ();
				return time.Hour >= this.startHour && time.Hour < this.endHour 
						&& time.Minute >= this.startMinute && time.Minute < this.endMinute;
			}

			public string Render(){
				return "Time between " + this.startHour + " and " + this.endHour;
			}
		}

		[Activity (Label = "conditionalsPicker")]
		public class ConditionalPicker : Activity{

			public static string AppTrigger = "Ix";
			private string app;
			private static int id = 0;

			protected void CreateTrigger(){

				CheckBox enabled = FindViewById<CheckBox> (Resource.Id.enabled);
				CheckBox withRange = FindViewById<CheckBox> (Resource.Id.times);
				TimePicker start = FindViewById<TimePicker> (Resource.Id.early);
				TimePicker late = FindViewById<TimePicker> (Resource.Id.late);
				EditText msg = FindViewById<EditText> (Resource.Id.message);

				if (enabled.Checked) {

					Action<Activity> a;

					if (withRange.Checked) {

						int h0 = start.CurrentHour.IntValue();
						int hn = late.CurrentMinute.IntValue();
						int m0 = start.CurrentHour.IntValue();
						int mn = late.CurrentMinute.IntValue();
					
						a = delegate(Activity obj) {
							DateTime time = new DateTime ();
							if (time.Hour >= h0 && time.Hour < hn
							   && time.Minute >= m0 && time.Minute < mn) {
								NotificationManager notificationManager = obj.GetSystemService (Context.NotificationService) as NotificationManager;
								var n = new Notification.Builder (obj)
									.SetContentTitle ("Mind you?")
									.SetContentText (msg.Text)
									.SetSmallIcon (Resource.Drawable.Icon);
								notificationManager.Notify (id, n.Build ());
							
							}
						}; 
					}else{
						a = delegate(Activity obj) {
							NotificationManager notificationManager = obj.GetSystemService (Context.NotificationService) as NotificationManager;
							var n = new Notification.Builder (obj)
									.SetContentTitle ("Mind you?")
									.SetContentText (msg.Text)
									.SetSmallIcon (Resource.Drawable.Icon);
							notificationManager.Notify (id, n.Build ());
						};
					}

					LogReader.AddCallback(this.app,a);
				} else {
					LogReader.RemoveCallback (this.app);
				}
			}

			protected override void OnStart ()
			{
				base.OnStart ();
				SetContentView (Resource.Layout.Conditionals);
				this.app = Intent.Extras.GetString (AppTrigger);
				CheckBox enabled = FindViewById<CheckBox> (Resource.Id.enabled);
				enabled.CheckedChange = delegate {
					this.CreateTrigger();
				}
			}
		}
	}
}


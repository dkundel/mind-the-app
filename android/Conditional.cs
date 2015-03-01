
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

		[Activity (Label = "Create Reminder")]
		public class ConditionalPicker : Activity{

			public static string AppTrigger = "Ix";
			private string app;
			private string fullAppName = "Choose an app";
			private static int id = 0;
			private TextView time_display;
			private Button pick_button;
			private TextView time_display2;
			private Button pick_button2;

			private int hour;
			private int minute;
			private int hour2;
			private int minute2;

			const int TIME_DIALOG_ID = 0;
			const int TIME_DIALOG_ID2 = 1;

			protected void CreateTrigger(){

				Switch toggleButton1 = FindViewById<Switch> (Resource.Id.toggleButton1);
				Switch withRange = FindViewById<Switch> (Resource.Id.times);
// 				TimePicker start = FindViewById<TimePicker> (Resource.Id.early);
//				TimePicker late = FindViewById<TimePicker> (Resource.Id.late);
				EditText msg = FindViewById<EditText> (Resource.Id.message);

					if (toggleButton1.Activated || toggleButton1.Checked) {

					Action<Activity> a;

					if (withRange.Checked) {

						int h0 = hour;
						int hn = minute;
						int m0 = hour2;
						int mn = minute2;
						Android.Util.Log.Info("num1"+ h0, "num2" + m0);

						a = delegate(Activity obj) {
							DateTime time = new DateTime ();
							if (time.Hour >= h0 && time.Hour < hn
							   && time.Minute >= m0 && time.Minute < mn) {
								NotificationManager notificationManager = obj.GetSystemService (Context.NotificationService) as NotificationManager;
								var n = new Notification.Builder (obj)
									.SetContentTitle ("Mind you?")
									.SetContentText (msg.Text)
									.SetSmallIcon (Resource.Drawable.Icon)
									.SetPriority((int)NotificationPriority.Max)
									.SetDefaults(NotificationDefaults.Vibrate);
								notificationManager.Notify (id, n.Build ());
							}
							Android.Util.Log.Info("num1"+ h0, "num2" + m0);
						}; 
					}else{
						a = delegate(Activity obj) {
							NotificationManager notificationManager = obj.GetSystemService (Context.NotificationService) as NotificationManager;
							var n = new Notification.Builder (obj)
									.SetContentTitle ("Mind you?")
									.SetContentText (msg.Text)
									.SetSmallIcon (Resource.Drawable.Icon)
								.SetPriority((int)NotificationPriority.High)
								.SetDefaults(NotificationDefaults.Vibrate);
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

				Button chooseAppButton = FindViewById<Button> (Resource.Id.chooseApp);
				chooseAppButton.Text = this.app;
				chooseAppButton.Click += delegate {
					var myIntent = new Intent (this, typeof(AppChooserActivity));
					StartActivityForResult (myIntent, 0);
				};

				Button donebutton = FindViewById<Button> (Resource.Id.donebutton);

				donebutton.Click += delegate {
					Finish();
					//StartActivity(typeof(MainActivity));
				};

				this.app = Intent.GetStringExtra (AppTrigger);
				Switch toggleButton1 = FindViewById<Switch> (Resource.Id.toggleButton1);
				toggleButton1.CheckedChange += delegate {
					this.CreateTrigger();
				};

				// Capture our View elements
				time_display = FindViewById<TextView> (Resource.Id.early2);
				time_display2 = FindViewById<TextView> (Resource.Id.late2);
				pick_button = FindViewById<Button> (Resource.Id.pickTime);
				pick_button2 = FindViewById<Button> (Resource.Id.pickTime2);
				// Add a click listener to the button
				pick_button.Click += (o, e) => ShowDialog (TIME_DIALOG_ID);
				pick_button2.Click += (o, e) => ShowDialog (TIME_DIALOG_ID2);

				// Get the current time
				hour = DateTime.Now.Hour;
				minute = DateTime.Now.Minute;

				hour2 = DateTime.Now.Hour;
				minute2 = DateTime.Now.Minute;


				// Display the current date
				UpdateDisplay ();
			}
			// Updates the time we display in the TextView
			private void UpdateDisplay ()
			{
				string time = string.Format ("{0}:{1}", hour, minute.ToString ().PadLeft (2, '0'));
				string time2 = string.Format ("{0}:{1}", hour2, minute2.ToString ().PadLeft (2, '0'));
				time_display.Text = time;
				time_display2.Text = time2;
			}

			private void TimePickerCallback (object sender, TimePickerDialog.TimeSetEventArgs e)
			{
				hour = e.HourOfDay;
				minute = e.Minute;
				UpdateDisplay ();
			}

			private void TimePickerCallback2 (object sender, TimePickerDialog.TimeSetEventArgs e)
			{
				hour2 = e.HourOfDay;
				minute2 = e.Minute;
				UpdateDisplay ();
			}

			protected override Dialog OnCreateDialog (int id)
			{
				if (id == TIME_DIALOG_ID)
					return new TimePickerDialog (this, TimePickerCallback, hour, minute, false);
				if (id == TIME_DIALOG_ID2)
					return new TimePickerDialog (this, TimePickerCallback2, hour2, minute2, false);

				return null;
			}

			protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
			{
				base.OnActivityResult(requestCode, resultCode, data);
				if (resultCode == Result.Ok) {
					this.fullAppName = data.GetStringExtra("appName");
					this.app = data.GetStringExtra ("packageName");
				}
			}
		}
	}
}
	

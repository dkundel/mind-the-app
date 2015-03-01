
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

		public class ConditionalContext{

			private Action<Conditional> cb;

			public ConditionalContext(Action<Conditional> cb){
				this.cb = cb;
			}

			public Action<Conditional> Callback{

				get { return this.cb;}
			}
		}

		public class ConditionalActivity : Activity{

			protected ConditionalContext ctx;

			protected override void OnStart ()
			{
				int ix = Intent.Extras.GetInt (ConditionalPicker.Ix);
				ctx = ConditionalPicker.GetCtx (ix);
				base.OnStart ();
			}
		}

		[Activity (Label = "conditionalsPicker")]
		public class ConditionalPicker : Activity{

			public static string Ix = "Ix";

			private static int currIx = 0;

			private static Dictionary<int,ConditionalContext> ctx = new Dictionary<int,ConditionalContext> ();

			private void InitPicker(Type t, ConditionalContext c){

				Intent init = new Intent(this,t);
				var ix = ++currIx;
				ctx[ix] = c;
				init.PutExtra (Ix, ix);
				StartActivity (init);
			}

			protected override void OnStart ()
			{
				base.OnStart ();
				SetContentView (Resource.Layout.Conditionals);
				/*
				Spinner selectors = (Spinner) FindViewById<Spinner> (Resource.Id.actions);
				ArrayAdapter conds = ArrayAdapter.CreateFromResource (this.ApplicationContext, Resource.Array.conditions,Android.Resource.Layout.SimpleSpinnerItem);
				Action<Conditional> call = x => Android.Util.Log.Info ("Created cond", "Created cond");

				selectors.ItemSelected += delegate(object sender, AdapterView.ItemSelectedEventArgs e) {
						this.InitPicker (actions [e.Position].Item1, new ConditionalContext (call));
					};
				selectors.Adapter = conds;
					//[i] = () => this.InitPicker(actions[i].Item1,new ConditionalContext(call));

			*/
			}

			public static ConditionalContext GetCtx(int ix){
				return ctx [ix];
			}
		}
	}
}


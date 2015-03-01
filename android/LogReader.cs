using System;
using Java.Lang;
using Java.Util;
using Android.Runtime;
using System.Threading.Tasks;
using Android.Content;
using Android.App;
using System.Collections.Generic;

namespace mindTheApp
{
	[Service]
	public class LogReader : Service
	{
		private static Dictionary<string,Action<Activity>> callbacks = new Dictionary<string,Action<Activity>>();
		private static Activity act;
		private Process pr;
		//private string cmd = "logcat | grep \"I/ActivityManager\""
		private string cmd = "logcat";
		//private string[] cmd = new string[]{"logcat","-d"};
		NotificationManager notificationManager;
		public LogReader ()
		{
			this.pr = Runtime.GetRuntime().Exec(cmd);
		}

		public static void SetActivity(Activity a){act = a;}

		public static void AddCallback(string package,Action<Activity> cb){

			callbacks[package] = cb;
		}

		public static void RemoveCallback(string package){
			if(callbacks.ContainsKey(package))
				callbacks.Remove(package);
		}

		private void RunService(){
			Task.Factory.StartNew (this.TryLogs);
		}

		public override void OnStart (Intent intent, int startId)
		{
			base.OnStart (intent, startId);
			this.RunService ();
		}

		public override StartCommandResult OnStartCommand(Intent intent,StartCommandFlags flags, int id){

			//this.RunService ();
			return base.OnStartCommand(intent,flags,id);
		}

		public override Android.OS.IBinder OnBind (Android.Content.Intent intent)
		{
			throw new NotImplementedException ();
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();      
		}

		public async void TryLogs(){
			var id = 0;
			var scn = new System.IO.StreamReader(pr.InputStream);
			//var s = new Android.Runtime.InputStreamAdapter (pr.OutputStream);

			do {
				await Task.Delay(1000);
				//this.pr = Runtime.GetRuntime().Exec(cmd);
				string line = "";
				do{

					line = await scn.ReadLineAsync();

					if(line != null){
						mindTheApp.parser.PResult<Tuple<int,string>,string> result = mindTheApp.logParser.pActivity.Parse(line);
					
						if(result.Success){

							foreach(var kvp in callbacks){

								if(result.Value.Item2.Contains(kvp.Key)){

									Action a = delegate{
										kvp.Value.Invoke(act);
									};

									await Task.Factory.StartNew(a);
								}
							}
						}
					}
				}while(line != null);
			} while(true);
		}
	}
}


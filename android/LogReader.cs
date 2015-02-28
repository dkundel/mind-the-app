using System;
using Java.Lang;
using Java.Util;
using Android.Runtime;
using System.Threading.Tasks;
using Android.Content;
using Android.App;

namespace mindTheApp
{
	[Service]
	public class LogReader : Service
	{
		private Process pr;
		private string cmd = "logcat | grep \"I/ActivityManager\"";
		//private string[] cmd = new string[]{"logcat","-d"};
		public LogReader ()
		{
			this.pr = Runtime.GetRuntime().Exec(cmd);
		}

		private void RunService(){
			Task.Factory.StartNew (this.TryLogs);
		}

		public override void OnStart (Intent intent, int startId)
		{
			base.OnStart (intent, startId);
			this.RunService ();
			Android.Util.Log.Info ("TheService", "Service");
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

			var scn = new System.IO.StreamReader(pr.InputStream);
			//var s = new Android.Runtime.InputStreamAdapter (pr.OutputStream);

			do {
				var line = await scn.ReadLineAsync();
				mindTheApp.parser.PResult<Tuple<int,string>,string> t = mindTheApp.logParser.pActivity.Parse(line);
				Android.Util.Log.Info("MindTheApp","Service");
				if(t.Success)
					Android.Util.Log.Info("MindTheApp","Parsed :" + t.Value.Item2);
				//else 
					//Android.Util.Log.Info("MindTheApp","Read: " + line);
			} while(true);
		}
	}
}


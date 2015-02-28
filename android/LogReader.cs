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
			var id = 0;
			var scn = new System.IO.StreamReader(pr.InputStream);
			//var s = new Android.Runtime.InputStreamAdapter (pr.OutputStream);

			do {
				await Task.Delay(1000);
				id++;
				//this.pr = Runtime.GetRuntime().Exec(cmd);
				string line = "";
				do{

					line = await scn.ReadLineAsync();

					if(line != null){
						mindTheApp.parser.PResult<Tuple<int,string>,string> t = mindTheApp.logParser.pActivity.Parse(line);
					if(t.Success){
						Android.Util.Log.Info("MindTheApp","Parsed :" + t.Value.Item2);
						if(act != null){

							this.notificationManager = act.GetSystemService (Context.NotificationService) as NotificationManager;
							var n = new Notification.Builder(act).SetContentTitle("AppWasOpened" + id).SetContentText("text" + id).SetSmallIcon(Resource.Drawable.Icon);
							this.notificationManager.Notify (id, n.Build());
							//act = null;
						}
				}
				}
				}while(line != null);
				//else 
					//Android.Util.Log.Info("MindTheApp","Read: " + line);
			} while(true);
		}
	}
}


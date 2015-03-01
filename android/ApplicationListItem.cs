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
    public class ApplicationListItem 
	{
        public string AppName { get; set; }
		public string AppPackageName { get; set; }
		public Android.Graphics.Drawables.Drawable ImageDrawable { get; set; }
    }
}
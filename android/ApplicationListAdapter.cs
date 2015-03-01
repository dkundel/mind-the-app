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
	public class ApplicationListAdapter : BaseAdapter<ApplicationListItem> {
		List<ApplicationListItem> items;
		Activity context;
		public ApplicationListAdapter(Activity context, List<ApplicationListItem> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override ApplicationListItem this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];
			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.AppChooserEntry, null);
			view.FindViewById<TextView>(Resource.Id.Text1).Text = item.AppName;
			view.FindViewById<ImageView>(Resource.Id.Image).SetImageDrawable(item.ImageDrawable);
			return view;
		}
	}
}


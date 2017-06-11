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
using Android.Graphics;
using Android.Graphics.Drawables;

namespace Pathify.CustomAdapters
{
    class ImageAdapter : BaseAdapter
    {
        private Context context;
        private List<Bitmap> images;

        public ImageAdapter(Context context, List<Bitmap> images)
        {
            this.context = context;
            this.images = images;
        }

        public override int Count
        {
            get{ return images.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        // Create a new ImageView for each item referenced by the Adapter
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if (convertView == null)
            {  // If it's not recycled, initialize some attributes
                imageView = new ImageView(context);
                imageView.LayoutParameters = new GridView.LayoutParams(200, 200);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                imageView.SetPadding(4, 8, 4, 8);
            }
            else
            {
                imageView = (ImageView)convertView;
            }

            imageView.SetImageBitmap(images[position]);
            return imageView;
        }
    }
}
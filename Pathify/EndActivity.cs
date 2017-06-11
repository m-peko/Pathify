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
using Android.Content.PM;
using Android.Graphics;
using Newtonsoft.Json;
using Android.Graphics.Drawables;
using Android.Util;

namespace Pathify
{
    [Activity(Label = "Pathify", Icon = "@drawable/Logo", Theme = "@style/NoActionBar",
              ScreenOrientation = ScreenOrientation.Portrait,
              WindowSoftInputMode = SoftInput.StateVisible|SoftInput.AdjustPan)]
    public class EndActivity : Activity
    {
        private EditText mTitle;
        private EditText mDescription;
        private GridView mImageGrid;
        private TextView mWarning;
        private Button mBtnSave;

        private Database.Database database;
        private Data.User user;
        private List<Bitmap> images;
        private List<Data.Point> points;

        private CustomAdapters.ImageAdapter imageAdapter;

        private bool validTitle = false;
        private bool validDescription = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);

            // Set our view from the "map" layout resource
            SetContentView(Resource.Layout.End);

            database = new Database.Database();
            user = new Data.User();
            images = new List<Bitmap>();
            points = new List<Data.Point>();

            // Deserialize objects sent from the previous activity
            user = JsonConvert.DeserializeObject<Data.User>(Intent.GetStringExtra("user"));
            images = Base64ToBitmap(JsonConvert.DeserializeObject<List<String>>(Intent.GetStringExtra("images")));
            points = JsonConvert.DeserializeObject<List<Data.Point>>(Intent.GetStringExtra("points"));

            mTitle = FindViewById<EditText>(Resource.Id.txtTitle);
            mDescription = FindViewById<EditText>(Resource.Id.txtDescription);
            mImageGrid = FindViewById<GridView>(Resource.Id.gridView);
            mWarning = FindViewById<TextView>(Resource.Id.txtEndWarning);
            mBtnSave = FindViewById<Button>(Resource.Id.btnSave);

            imageAdapter = new CustomAdapters.ImageAdapter(this, images);
            mImageGrid.Adapter = imageAdapter;
            //mImageGrid.ItemClick += mImageGrid_ItemClick;

            mTitle.TextChanged += mTitle_TextChanged;
            mDescription.TextChanged += mDescription_TextChanged;
            mBtnSave.Click += mBtnSave_Click;
        }

        private void mTitle_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (mTitle.Text == string.Empty)
            {
                mTitle.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                validTitle = false;
            }
            else
            {
                mTitle.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                validTitle = true;
            }
        }

        private void mDescription_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (mDescription.Text == string.Empty)
            {
                mDescription.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                validDescription = false;
            }
            else
            {
                mDescription.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                validDescription = true;
            }
        }

        private void mImageGrid_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Toast.MakeText(this, e.Position, ToastLength.Short).Show();
        }

        private void mBtnSave_Click(object sender, EventArgs e)
        {
            if (validTitle == true && validDescription == true)
            {
                if (database.trackingEnd(user.Username, mTitle.Text, mDescription.Text, images, points))
                {
                    Intent intent = new Intent(this, typeof(MapActivity));
                    intent.PutExtra("user", JsonConvert.SerializeObject(user));
                    this.StartActivity(intent);
                    this.OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
                    this.Finish();
                }
                else
                {
                    mWarning.Text = "Wrong credentials!";
                }
            }
            else
            {
                if (validTitle == false)
                {
                    mTitle.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }

                if (validDescription == false)
                {
                    mDescription.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }
            }
        }
        private List<Bitmap> Base64ToBitmap(List<String> base64Strings)
        {
            List<Bitmap> images = new List<Bitmap>();

            for (int i = 0; i < base64Strings.Count; i++)
            {
                byte[] imageAsBytes = Base64.Decode(base64Strings[i], Base64Flags.Default);
                images.Add(BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length));
            }

            return images;
        }
    }
}
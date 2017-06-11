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
using Android.Gms.Maps;
using Newtonsoft.Json;
using Android.Gms.Maps.Model;
using Android.Util;
using Android.Provider;
using Android.Graphics;
using System.IO;

namespace Pathify
{
    [Activity(Label = "Pathify", Icon = "@drawable/Logo", Theme = "@style/NoActionBar",
              ScreenOrientation = ScreenOrientation.Portrait)]
    public class MapActivity : Activity, IOnMapReadyCallback, BackgroundService.ILocationData
    {
        private GoogleMap mMap;
        private MarkerOptions markerOptions;
        private Marker marker;

        private Button mBtnStartTracking;
        private Button mBtnSOS;
        private ImageButton mBtnTakePhoto;
        private Button mBtnStopTracking;
        
        private Database.Database database;
        private Data.User user;
        private List<Bitmap> images;
        private List<Data.Point> points;
        
        private BackgroundService.LocationService locationService;
        private BackgroundService.LocationBroadcastReceiver broadcastReceiver;
        private bool broadcastReceiverRegistered;

        private bool trackingActive = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);

            // Set our view from the "map" layout resource
            SetContentView(Resource.Layout.Map);

            database = new Database.Database();
            markerOptions = new MarkerOptions();
            markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.Logo));

            user = new Data.User();

            // Deserialize object sent from the previous activity
            user = JsonConvert.DeserializeObject<Data.User>(Intent.GetStringExtra("user"));

            mBtnStartTracking = FindViewById<Button>(Resource.Id.btnStartTracking);
            mBtnSOS = FindViewById<Button>(Resource.Id.btnSOS);
            mBtnTakePhoto = FindViewById<ImageButton>(Resource.Id.btnTakePhoto);
            mBtnStopTracking = FindViewById<Button>(Resource.Id.btnStopTracking);

            mBtnStartTracking.Click += mBtnStartTracking_Click;
            mBtnSOS.Click += mBtnSOS_Click;
            mBtnTakePhoto.Click += mBtnTakePhoto_Click;
            mBtnStopTracking.Click += mBtnStopTracking_Click;

            SetUpMap();
        }

        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;

            LatLng startPosition = new LatLng(43.5172, 16.4682);
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(startPosition, 11);
            mMap.MoveCamera(camera);
        }

        private void mBtnStartTracking_Click(object sender, EventArgs e)
        {
            try
            {
                if (locationService == null)
                {
                    locationService = new BackgroundService.LocationService();
                    Intent intent = new Intent(this, locationService.Class);
                    StartService(intent);

                    RegisterBroadcastReceiver();
                }

                images = new List<Bitmap>();
                points = new List<Data.Point>();

                trackingActive = true;

                Toast.MakeText(this, "Tracking started", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Log.Debug("Method StartTracking", ex.Message);
            }
        }

        private void mBtnSOS_Click(object sender, EventArgs e)
        {
            
        }

        private void mBtnTakePhoto_Click(object sender, EventArgs e)
        {
            Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(cameraIntent, 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                Bitmap bitmap = (Bitmap)data.Extras.Get("data");

                if (images != null)
                {
                    images.Add(bitmap);
                }

                MediaStore.Images.Media.InsertImage(ContentResolver, bitmap, DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            }
        }

        private void mBtnStopTracking_Click(object sender, EventArgs e)
        {
            try
            {
                if (trackingActive == true)
                {
                    UnRegisterBroadcastReceiver();

                    // Stop location service
                    if (locationService != null)
                    {
                        StopService(new Intent(this, locationService.Class));
                    }

                    // Clear map
                    if (mMap != null)
                    {
                        mMap.Clear();
                        if (marker != null)
                        {
                            marker.Remove();
                        }
                    }

                    LatLng startPosition = new LatLng(43.5172, 16.4682);
                    CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(startPosition, 11);
                    mMap.MoveCamera(camera);

                    Intent intent = new Intent(this, typeof(EndActivity));
                    intent.PutExtra("user", JsonConvert.SerializeObject(user));
                    intent.PutExtra("images", JsonConvert.SerializeObject(BitmapToBase64(images)));
                    intent.PutExtra("points", JsonConvert.SerializeObject(points));
                    this.StartActivity(intent);
                    this.OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
                    this.Finish();

                    if (images != null)
                    {
                        images.Clear();
                    }

                    if (points != null)
                    {
                        points.Clear();
                    }

                    trackingActive = false;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Method StopTracking", ex.Message);
            }
        }

        public void OnLocationChanged(LatLng point)
        {
            if (marker != null)
            {
                marker.Remove();
            }

            points.Add(new Data.Point(point.Latitude, point.Longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

            markerOptions.SetPosition(point);
            marker = mMap.AddMarker(markerOptions);
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(point, 15);
            mMap.MoveCamera(camera);
        }

        private void RegisterBroadcastReceiver()
        {
            broadcastReceiverRegistered = true;
            IntentFilter filter = new IntentFilter(BackgroundService.LocationBroadcastReceiver.LOCATION_CHANGED);
            filter.AddCategory(Intent.CategoryDefault);
            broadcastReceiver = new BackgroundService.LocationBroadcastReceiver();
            RegisterReceiver(broadcastReceiver, filter);
        }

        private void UnRegisterBroadcastReceiver()
        {
            if (broadcastReceiver != null && broadcastReceiverRegistered == true)
            {
                broadcastReceiverRegistered = false;
                UnregisterReceiver(broadcastReceiver);
            }
        }

        private List<String> BitmapToBase64(List<Bitmap> images)
        {
            List<String> base64Strings = new List<String>();

            for (int i = 0; i < images.Count; i++)
            {
                MemoryStream stream = new MemoryStream();
                images[i].Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                byte[] byteArray = stream.ToArray();
                base64Strings.Add(Base64.EncodeToString(byteArray, Base64.Default));
            }

            return base64Strings;
        }

        protected override void OnResume()
        {
            base.OnResume();
            RegisterBroadcastReceiver();
        }

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
                UnRegisterBroadcastReceiver();
                if (locationService != null)
                {
                    StopService(new Intent(this, locationService.Class));
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Method OnDestroy", ex.Message);
            }
        }
    }
}
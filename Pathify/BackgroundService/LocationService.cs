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
using Android.Locations;
using Android.Util;

namespace Pathify.BackgroundService
{
    [Service]
    class LocationService : Service
    {
        private LocationManager locationManager;
        private LocationListener locationListener;
        private string locationProvider;

        public LocationService()
        {
            Log.Debug("Class LocationService", "Default constructor");
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            locationManager.RemoveUpdates(locationListener);
        }

        private void BroadcastStarted(Location location)
        {
            Intent broadcastIntent = new Intent();
            // Send latitude and longitude to LocationBroadcastReceiver
            broadcastIntent.PutExtra("latitude", location.Latitude);
            broadcastIntent.PutExtra("longitude", location.Longitude);
            broadcastIntent.SetAction(LocationBroadcastReceiver.LOCATION_CHANGED);
            broadcastIntent.AddCategory(Intent.CategoryDefault);
            SendBroadcast(broadcastIntent);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            try
            {
                InitializeLocationManager();

                FindLocationProvider();

                locationListener = new LocationListener(locationProvider);

                // Request location updates
                locationManager.RequestLocationUpdates(locationProvider, 0, 0, locationListener);

                // event is triggered every time location changes
                locationListener.mOnLocationChanged += locationListener_mOnLocationChanged; ;
            }
            catch (Exception ex)
            {
                Log.Debug("Method OnStartCommand", ex.Message);
            }

            return StartCommandResult.Sticky;
        }

        private void locationListener_mOnLocationChanged(object sender, CustomEventArgs.OnLocationChangedEventArgs e)
        {
            BroadcastStarted(e.Location);
        }

        private void FindLocationProvider()
        {
            // Find best location provider - GPS, network...
            Criteria locationServiceCriteria = new Criteria
            {
                Accuracy = Accuracy.Fine
            };

            IList<string> acceptableLocationProviders = locationManager.GetProviders(locationServiceCriteria, true);

            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }
        }

        private void InitializeLocationManager()
        {
            if (locationManager == null)
            {
                locationManager = (LocationManager)GetSystemService(LocationService);
            }
        }
    }
}
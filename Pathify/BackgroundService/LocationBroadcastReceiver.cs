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
using Android.Gms.Maps.Model;

namespace Pathify.BackgroundService
{
    [BroadcastReceiver]
    class LocationBroadcastReceiver : BroadcastReceiver
    {
        public static readonly string LOCATION_CHANGED = "LOCATION_CHANGED";

        private ILocationData mInterface;

        private LatLng locationPoint;

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == LOCATION_CHANGED)
            {
                try
                {
                    locationPoint = new LatLng(intent.GetDoubleExtra("latitude", 0), intent.GetDoubleExtra("longitude", 0));

                    mInterface = (ILocationData)context;
                    mInterface.OnLocationChanged(locationPoint);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(context, "ERROR: " + ex.Message, ToastLength.Short).Show();
                }
            }
        }
    }
}
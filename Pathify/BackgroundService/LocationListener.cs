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
    class LocationListener : Java.Lang.Object, ILocationListener
    {
        private Location lastLocation;

        public event EventHandler<CustomEventArgs.OnLocationChangedEventArgs> mOnLocationChanged;

        public LocationListener(string provider)
        {
            lastLocation = new Location(provider);
        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                lastLocation.Set(location);
                if (mOnLocationChanged != null)
                {
                    mOnLocationChanged.Invoke(this, new CustomEventArgs.OnLocationChangedEventArgs(location));
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Method OnLocationChanged", ex.Message);
            }
        }

        public void OnProviderDisabled(string provider)
        {
            
        }

        public void OnProviderEnabled(string provider)
        {
            
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }
    }
}
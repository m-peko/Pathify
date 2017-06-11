using Android.Views;
using Android.Widget;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Util;

using System.Xml;

namespace Pathify.Data
{
    public struct Point
    {
        public double latitude;
        public double longitude;
        public string time;

        public Point(double latitude, double longitude, string currentTime)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.time = currentTime;
        }
    };
}
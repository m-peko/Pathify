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

namespace Pathify.CustomEventArgs
{
    class OnLocationChangedEventArgs : EventArgs
    {
        private Location location;

        public Location Location
        {
            get { return location; }
            set { location = value; }
        }

        public OnLocationChangedEventArgs(Location location) : base()
        {
            Location = location;
        }
    }
}
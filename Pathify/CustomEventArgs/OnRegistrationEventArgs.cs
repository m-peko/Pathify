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

namespace Pathify.CustomEventArgs
{
    class OnRegistrationEventArgs : EventArgs
    {
        private Data.User user;

        public Data.User User
        {
            get { return User; }
            set { user = value; }
        }

        public OnRegistrationEventArgs(Data.User user) : base()
        {
            User = user;
        }
    }
}
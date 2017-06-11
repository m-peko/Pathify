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
    class OnLoginEventArgs : EventArgs
    {
        private Data.User user;
        private bool rememberMe;

        public Data.User User
        {
            get { return user; }
            set { user = value; }
        }

        public bool RememberMe
        {
            get { return rememberMe; }
            set { rememberMe = value; }
        }

        public OnLoginEventArgs(Data.User user, bool rememberMe) : base()
        {
            User = user;
            RememberMe = rememberMe;
        }
    }
}
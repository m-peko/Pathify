using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.Maps;
using Newtonsoft.Json;

namespace Pathify
{
    [Activity(Label = "Pathify", Icon = "@drawable/Logo", Theme = "@style/NoActionBar",
              ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private Button mBtnOpenLogin;
        private Button mBtnOpenRegistration;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            mBtnOpenLogin = FindViewById<Button>(Resource.Id.btnOpenLogin);
            mBtnOpenRegistration = FindViewById<Button>(Resource.Id.btnOpenRegistration);

            mBtnOpenLogin.Click += mBtnOpenLogin_Click;
            mBtnOpenRegistration.Click += mBtnOpenRegistration_Click;
        }

        private void mBtnOpenLogin_Click(object sender, EventArgs e)
        {
            // Pull up Login dialog
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            Dialogs.LoginDialog loginDialog = new Dialogs.LoginDialog();
            loginDialog.SetStyle(DialogFragmentStyle.Normal, Resource.Style.DialogFragmentFullScreen); // Set full screen dialog fragment
            loginDialog.Show(transaction, "dialog fragment");

            loginDialog.mOnLogin += loginDialog_mOnLogin;
        }

        private void mBtnOpenRegistration_Click(object sender, EventArgs e)
        {
            // Pull up Registration dialog
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            Dialogs.RegistrationDialog registrationDialog = new Dialogs.RegistrationDialog();
            registrationDialog.SetStyle(DialogFragmentStyle.Normal, Resource.Style.DialogFragmentFullScreen); // Set full screen dialog fragment
            registrationDialog.Show(transaction, "dialog fragment");

            registrationDialog.mOnRegistration += registrationDialog_mOnRegistration;
        }

        private void loginDialog_mOnLogin(object sender, CustomEventArgs.OnLoginEventArgs e)
        {
            Data.User user = new Data.User(e.User.Username, e.User.Password);

            if (e.RememberMe == true)
            {
                ISharedPreferences sharedPref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
                ISharedPreferencesEditor editor = sharedPref.Edit();
                editor.PutString("username", e.User.Username.Trim());
                editor.PutString("password", e.User.Password.Trim());
                editor.Apply();
            }

            Intent intent = new Intent(this, typeof(MapActivity));
            intent.PutExtra("user", JsonConvert.SerializeObject(user));
            this.StartActivity(intent);
            this.OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
            this.Finish();
        }

        private void registrationDialog_mOnRegistration(object sender, CustomEventArgs.OnRegistrationEventArgs e)
        {
            Data.User user = new Data.User(e.User.Username, e.User.FirstName, e.User.LastName, e.User.Email, e.User.Password);

            Intent intent = new Intent(this, typeof(MapActivity));
            intent.PutExtra("user", JsonConvert.SerializeObject(user));
            this.StartActivity(intent);
            this.OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
            this.Finish();
        }
    }
}


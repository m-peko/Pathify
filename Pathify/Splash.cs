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
using Newtonsoft.Json;

namespace Pathify
{
    [Activity(Label = "Pathify", MainLauncher = true)]
    public class Splash : Activity
    {
        private Database.Database database;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            database = new Database.Database();

            ISharedPreferences sharedPref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            string username = sharedPref.GetString("username", string.Empty);
            string password = sharedPref.GetString("password", string.Empty);

            // User has not saved any informations
            if (username == string.Empty || password == string.Empty)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                this.StartActivity(intent);
                this.Finish();
            }
            else
            {
                Data.User user = new Data.User(username, password);

                // User has saved informations
                // Check in database if the credentials are right
                if (database.loginValidation(user))
                {
                    Intent intent = new Intent(this, typeof(MapActivity));
                    intent.PutExtra("user", JsonConvert.SerializeObject(user));
                    this.StartActivity(intent);
                    this.Finish();
                }
                // Credentials are not right
                else
                {
                    ISharedPreferencesEditor editor = sharedPref.Edit();
                    editor.Clear();
                    editor.Apply();

                    Intent intent = new Intent(this, typeof(MainActivity));
                    this.StartActivity(intent);
                    this.Finish();
                }
            }
        }
    }
}
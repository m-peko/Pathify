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
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Pathify.Dialogs
{
    class LoginDialog : DialogFragment
    {
        private ImageButton mBackBtn;
        private EditText mUsername;
        private EditText mPassword;
        private CheckBox mChkBoxRememberMe;
        private TextView mWarning;
        private Button mBtnLogin;

        public event EventHandler<CustomEventArgs.OnLoginEventArgs> mOnLogin;

        private bool validUsername = false;
        private bool validPassword = false;

        private Database.Database database;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.Login, container, false);

            database = new Database.Database();

            mBackBtn = view.FindViewById<ImageButton>(Resource.Id.btnLogBack);
            mUsername = view.FindViewById<EditText>(Resource.Id.txtLogUsername);
            mPassword = view.FindViewById<EditText>(Resource.Id.txtLogPassword);
            mChkBoxRememberMe = view.FindViewById<CheckBox>(Resource.Id.chkBoxLogRememberMe);
            mWarning = view.FindViewById<TextView>(Resource.Id.txtLogWarning);
            mBtnLogin = view.FindViewById<Button>(Resource.Id.btnLogin);

            mBackBtn.Click += mBackBtn_Click;
            mUsername.TextChanged += mUsername_TextChanged;
            mPassword.TextChanged += mPassword_TextChanged;
            mBtnLogin.Click += mBtnLogin_Click;

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); // Set title bar to invisible

            // Set background color to black with opacity
            Drawable backgroundColor = new ColorDrawable(Color.Black);
            backgroundColor.SetAlpha(170);
            Dialog.Window.SetBackgroundDrawable(backgroundColor);
            
            base.OnActivityCreated(savedInstanceState);
            
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; // Set animation
        }

        private void mBackBtn_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void mUsername_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (mUsername.Text == string.Empty)
            {
                mUsername.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                validUsername = false;
            }
            else
            {
                mUsername.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                validUsername = true;
            }
        }

        private void mPassword_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (mPassword.Text == string.Empty)
            {
                mPassword.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                validPassword = false;
            }
            else
            {
                mPassword.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                validPassword = true;
            }
        }

        private void mBtnLogin_Click(object sender, EventArgs e)
        {
            if (validUsername == true && validPassword == true)
            {
                Data.User user = new Data.User(mUsername.Text, mPassword.Text);

                if (database.loginValidation(user))
                {
                    if (mOnLogin != null)
                    {
                        mOnLogin.Invoke(this, new CustomEventArgs.OnLoginEventArgs(user, mChkBoxRememberMe.Checked));
                    }
                    mWarning.Text = string.Empty;
                    this.Dismiss();
                }
                else
                {
                    mWarning.Text = "Wrong credentials!";
                }
            }
            else
            {
                if (validUsername == false)
                {
                    mUsername.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }

                if (validPassword == false)
                {
                    mPassword.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }
            }
        }
    }
}
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
using System.Text.RegularExpressions;

namespace Pathify.Dialogs
{
    class RegistrationDialog : DialogFragment
    {
        private ImageButton mBackBtn;
        private EditText mUsername;
        private EditText mFirstName;
        private EditText mLastName;
        private EditText mEmail;
        private EditText mPassword;
        private EditText mConfirmPassword;
        private TextView mWarning;
        private Button mBtnRegistration;

        public event EventHandler<CustomEventArgs.OnRegistrationEventArgs> mOnRegistration;

        private bool validUsername = false;
        private bool validFirstName = false;
        private bool validLastName = false;
        private bool validEmail = false;
        private bool validPassword = false;
        private bool validConfirmPassword = false;

        private Database.Database database;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.Registration, container, false);

            database = new Database.Database();

            mBackBtn = view.FindViewById<ImageButton>(Resource.Id.btnRegBack);
            mUsername = view.FindViewById<EditText>(Resource.Id.txtRegUsername);
            mFirstName = view.FindViewById<EditText>(Resource.Id.txtRegFirstName);
            mLastName = view.FindViewById<EditText>(Resource.Id.txtRegLastName);
            mEmail = view.FindViewById<EditText>(Resource.Id.txtRegEmail);
            mPassword = view.FindViewById<EditText>(Resource.Id.txtRegPassword);
            mConfirmPassword = view.FindViewById<EditText>(Resource.Id.txtRegConfirmPassword);
            mWarning = view.FindViewById<TextView>(Resource.Id.txtRegWarning);
            mBtnRegistration = view.FindViewById<Button>(Resource.Id.btnRegistration);

            mBackBtn.Click += mBackBtn_Click;
            mUsername.TextChanged += mUsername_TextChanged;
            mFirstName.TextChanged += mFirstName_TextChanged;
            mLastName.TextChanged += mLastName_TextChanged;
            mEmail.TextChanged += mEmail_TextChanged;
            mPassword.TextChanged += mPassword_TextChanged;
            mConfirmPassword.TextChanged += mConfirmPassword_TextChanged;
            mBtnRegistration.Click += mBtnRegistration_Click;

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

        // Check if email is valid
        private bool emailValidation(string email)
        {
            bool valid = false;
            Regex regExpression = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regExpression.Match(email);
            if (match.Success)
            {
                valid = true;
            }
            return valid;
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

        private void mFirstName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (mFirstName.Text == string.Empty)
            {
                mFirstName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                validFirstName = false;
            }
            else
            {
                mFirstName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                validFirstName = true;
            }
        }

        private void mLastName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (mLastName.Text == string.Empty)
            {
                mLastName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                validLastName = false;
            }
            else
            {
                mLastName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                validLastName = true;
            }
        }

        private void mEmail_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (mEmail.Text == string.Empty || !emailValidation(mEmail.Text))
            {
                mEmail.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                validEmail = false;
            }
            else
            {
                mEmail.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                validEmail = true;
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

        private void mConfirmPassword_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (mConfirmPassword.Text == string.Empty || mConfirmPassword.Text != mPassword.Text)
            {
                mConfirmPassword.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                validConfirmPassword = false;
            }
            else
            {
                mConfirmPassword.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                validConfirmPassword = true;
            }
        }

        private void mBtnRegistration_Click(object sender, EventArgs e)
        {
            if (validUsername == true && validFirstName == true && validLastName == true && validEmail == true
                && validPassword == true && validConfirmPassword == true)
            {
                Data.User user = new Data.User(mUsername.Text, mFirstName.Text, mLastName.Text,
                                               mEmail.Text, mPassword.Text);

                if (database.newRegistration(user))
                {
                    if (mOnRegistration != null)
                    {
                        mOnRegistration.Invoke(this, new CustomEventArgs.OnRegistrationEventArgs(user));
                    }
                    mWarning.Text = string.Empty;
                    this.Dismiss();
                }
                else
                {
                    mWarning.Text = "Something went wrong!";
                }
            }
            else
            {
                if (validUsername == false)
                {
                    mUsername.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }

                if (validFirstName == false)
                {
                    mFirstName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }

                if (validLastName == false)
                {
                    mLastName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }

                if (validEmail == false)
                {
                    mEmail.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }

                if (validPassword == false)
                {
                    mPassword.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }

                if (validConfirmPassword == false)
                {
                    mConfirmPassword.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Warning, 0);
                }
            }
        }
    }
}
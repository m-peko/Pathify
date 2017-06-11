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
using System.Net;
using System.IO;
using Android.Util;
using Android.Graphics;
using Java.IO;

namespace Pathify.Database
{
    public struct Image
    {
        public string img;

        public Image(string img)
        {
            this.img = img;
        }
    }

    class Database
    {
        private string web = "http://pathify.trema.hr/api/";

        public bool loginValidation(Data.User user)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(web + "login");
            string responseString = string.Empty;

            var postData = "username=" + user.Username;
            postData += "&password=" + user.Password;

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Debug("Method loginValidation", ex.Message);
            }

            return responseString == "1" ? true : false;
        }

        public bool newRegistration(Data.User user)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(web + "register");
            string responseString = string.Empty;

            var postData = "email=" + user.Email;
            postData += "&username=" + user.Username;
            postData += "&password=" + user.Password;
            postData += "&firstname=" + user.FirstName;
            postData += "&lastname=" + user.LastName;

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Debug("Method registrationValidation", ex.Message);
            }

            return responseString == "1" ? true : false;
        }

        public bool trackingEnd(string username, string title, string description, List<Bitmap> images, List<Data.Point> points)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(web + "trip");
            string responseString = string.Empty;

            List<Image> base64Strings = BitmapToBase64(images);

            var postData = "username=" + username;
            postData += "&description=" + description;
            postData += "&name=" + title;
            postData += "&points=" + Newtonsoft.Json.JsonConvert.SerializeObject(points);
            postData += "&images=" + Newtonsoft.Json.JsonConvert.SerializeObject(base64Strings);

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Debug("Method trackingEnd", ex.Message);
            }

            return responseString == "1" ? true : false ;
        }

        private List<Image> BitmapToBase64(List<Bitmap> images)
        {
            List<Image> base64Strings = new List<Image>();

            for (int i = 0; i < images.Count; i++)
            {
                MemoryStream stream = new MemoryStream();
                images[i].Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                byte[] byteArray = stream.ToArray();
                base64Strings.Add(new Image(Base64.EncodeToString(byteArray, Base64.Default)));
                //base64Strings.Add(Base64.EncodeToString(byteArray, Base64.Default));
            }

            return base64Strings;
        }
    }
}
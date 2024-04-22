using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace password_manager
{
    internal class Constants
    {
        private static readonly HttpClient client_ = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(3000)
        };
        public static HttpClient client
        {
            get { return client_; }
        }
        public static bool Test_mode = false; // ÍRD ÁT TRUE-RA ha külső telefonról tesztelsz.

        public static readonly string ip_address = "http://192.168.0.116:5000"; //külső cim: 87.242.60.55
    }
}
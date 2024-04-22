using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace password_manager
{
    internal class Login
    {
        public Login(string name, string username, string password, string url, string note)
        {
            this.name = name;
            this.username = username;
            this.password = password;
            this.url = url;
            this.note = note;
        }

        private string name { get; set; }
        private string password { get; set; }
        private string username { get; set; }
        private string url { get; set; }
        private string note { get; set; }

        public string Name { get { return name; } }
        public string Password { get { return password; }}
        public string Username { get { return username; }}
        public string Url { get { return url; }}
        public string Note { get { return note; }}

    }
}
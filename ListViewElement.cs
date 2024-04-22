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
    internal class ListViewElement
    {
        public ListViewElement(string name, string username, string _id)
        {
            this.name = User.CurrentDecryptionMethod( name, CryptoHelper.key, User.salt);
            if (!String.IsNullOrEmpty(username))
            {
                this.username = User.CurrentDecryptionMethod(username, CryptoHelper.key, User.salt);
            }
            this._id = _id;
        }

        private string name { get; set; }
        private string username { get; set; }
        private string _id { get; set; }

        public string Name { get { return name; } } 
        public string Username { get { return username; } }
        public string _Id { get { return _id; } }
    }
}
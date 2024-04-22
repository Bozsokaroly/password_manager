using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace password_manager
{
    public class AddLoginFragment : AndroidX.Fragment.App.Fragment
    {
        Spinner typeSpinner;
        EditText nameInput;
        EditText emailInput;
        EditText passwordInput;
        EditText urlInput;
        EditText noteInput;
        Button addLogin;
        string id;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SpinnerHelper.SetActivity(this.Activity);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.new_login_add_layout, container, false);
            typeSpinner = view.FindViewById<Spinner>(Resource.Id.type_spinner);
            nameInput = view.FindViewById<EditText>(Resource.Id.name_input);
            emailInput = view.FindViewById<EditText>(Resource.Id.email_input);
            passwordInput = view.FindViewById<EditText>(Resource.Id.password_input);
            urlInput = view.FindViewById<EditText>(Resource.Id.url_input);
            noteInput = view.FindViewById<EditText>(Resource.Id.note_input);
            addLogin = view.FindViewById<Button>(Resource.Id.add_button);


            typeSpinner.ItemSelected -= new EventHandler<AdapterView.ItemSelectedEventArgs>(TypeSpinner_ItemSelected);
            typeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(TypeSpinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                this.Context, Resource.Array.types, Android.Resource.Layout.SimpleSpinnerItem);
            typeSpinner.Adapter = adapter;

            if (Arguments != null)
            {
                ((AndroidX.AppCompat.App.AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_modify);
                id = Arguments.GetString("id", "");
                FillEditText();
                typeSpinner.Enabled = false;
            }
            else
            {
                ((AndroidX.AppCompat.App.AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_add_item);
            }
            addLogin.Click += async (sender, e) =>
            {
                await AddButton_ClickAsync(sender, e);
            };


          

            return view;
        }
        private async void FillEditText()
        {
            Dictionary<string, object> data = await API_RequestsData.GetDataFromDBAsync(this.Context, id);
            if (data != null)
            {
                nameInput.Text = data["name"] != null ? User.CurrentDecryptionMethod(data["name"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                emailInput.Text = data["username"] != null ? User.CurrentDecryptionMethod(data["username"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                passwordInput.Text = data["password"] != null ? User.CurrentDecryptionMethod(data["password"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                urlInput.Text = data["url"] != null ? User.CurrentDecryptionMethod(data["url"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                noteInput.Text = data["note"] != null ? User.CurrentDecryptionMethod(data["note"].ToString(), CryptoHelper.key, User.salt) : string.Empty;

                addLogin.Text = "módosítás";
            }
        }
        private void TypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            AndroidX.Fragment.App.Fragment fragment = SpinnerHelper.SpinnerSelected(e.Position);

            if (fragment != null)
            {
                SpinnerHelper.ShowFragment(fragment, fragment.GetType().Name);
            }
        }

        private async Task AddButton_ClickAsync(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameInput.Text))
            {
                Toast.MakeText(this.Context, GetString(Resource.String.name_cant_empty), ToastLength.Short).Show();
                nameInput.RequestFocus();
            }
            else
            {
                JObject json = new JObject
            {
                {"type", typeSpinner.SelectedItemPosition.ToString() },
                { "name",User.CurrentEncryptionMethod.Invoke( nameInput.Text,CryptoHelper.key, User.salt) },
                { "username",User.CurrentEncryptionMethod.Invoke( emailInput.Text,CryptoHelper.key, User.salt)},
                { "password",User.CurrentEncryptionMethod.Invoke( passwordInput.Text,CryptoHelper.key, User.salt)},
                { "url",User.CurrentEncryptionMethod.Invoke( urlInput.Text,CryptoHelper.key, User.salt)},
                { "note",User.CurrentEncryptionMethod.Invoke( noteInput.Text,CryptoHelper.key, User.salt)}
            };

                if (id != null)
                {
                    API_RequestsData.UpdateDataToDB(this.Context, json, id);
                }
                else
                {
                    API_RequestsData.AddDataToDB(this.Context, json);
                }

                VaultFragment fragment = new VaultFragment();
                ParentFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, fragment)
                    .AddToBackStack(null)
                    .Commit();
            }

        }
    }
}
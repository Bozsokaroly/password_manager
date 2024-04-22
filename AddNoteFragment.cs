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
    public class AddNoteFragment : AndroidX.Fragment.App.Fragment
    {
        Spinner typeSpinner;
        EditText nameInput;
        EditText noteInput;
        Button addButton;
        string id;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SpinnerHelper.SetActivity(this.Activity);
            // Create your fragment here
        }

        private async void FillEditText()
        {
            Dictionary<string, object> data = await API_RequestsData.GetDataFromDBAsync(this.Context, id);
            if (data != null)
            {
                nameInput.Text = data["name"] != null ? User.CurrentDecryptionMethod(data["name"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                noteInput.Text = data["note"] != null ? User.CurrentDecryptionMethod(data["note"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                addButton.Text = "módosítás";
            }
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.new_note_layout, container, false);

            typeSpinner = view.FindViewById<Spinner>(Resource.Id.type_spinner);
            nameInput = view.FindViewById<EditText>(Resource.Id.name_input);
            noteInput = view.FindViewById<EditText>(Resource.Id.note_input);
            addButton = view.FindViewById<Button>(Resource.Id.add_button);
            typeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(TypeSpinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
            this.Context, Resource.Array.types, Android.Resource.Layout.SimpleSpinnerItem);
            typeSpinner.Adapter = adapter;
            typeSpinner.SetSelection(3);
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
            addButton.Click += async (sender, e) =>
            {
                await AddButton_ClickAsync(sender, e);
            };


            return view;
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
                { "name", User.CurrentEncryptionMethod.Invoke( nameInput.Text,CryptoHelper.key, User.salt) },
                { "note", User.CurrentEncryptionMethod.Invoke( noteInput.Text,CryptoHelper.key, User.salt) }
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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.Transliterator;

namespace password_manager
{
    public class AddIdentifyFragment : AndroidX.Fragment.App.Fragment
    {
        Spinner typeSpinner;
        EditText nameInput;
        EditText firstNameInput;
        EditText lastNameInput;
        EditText cardNumberInput;
        EditText yearOfExpireInput;
        Spinner monthOfExpireInput;
        EditText addressInput;
        EditText extraInfoOneInput;
        EditText extraInfoTwoInput;
        EditText extraInfoThreeInput;
        EditText noteInput;
        Button addButton;
        string id;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SpinnerHelper.SetActivity(this.Activity);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.new_identify_layout, container, false);
            typeSpinner = view.FindViewById<Spinner>(Resource.Id.type_spinner);
            nameInput = view.FindViewById<EditText>(Resource.Id.name_input);
            firstNameInput = view.FindViewById<EditText>(Resource.Id.firstname_input);
            lastNameInput = view.FindViewById<EditText>(Resource.Id.lastname_input);
            cardNumberInput = view.FindViewById<EditText>(Resource.Id.cardnumber_input);
            yearOfExpireInput = view.FindViewById<EditText>(Resource.Id.year_input);
            monthOfExpireInput = view.FindViewById<Spinner>(Resource.Id.month_input);
            addressInput = view.FindViewById<EditText>(Resource.Id.address_input);
            extraInfoOneInput = view.FindViewById<EditText>(Resource.Id.extraone_input);
            extraInfoTwoInput = view.FindViewById<EditText>(Resource.Id.extratwo_input);
            extraInfoThreeInput = view.FindViewById<EditText>(Resource.Id.extrathree_input);
            noteInput = view.FindViewById<EditText>(Resource.Id.note_input);
            addButton = view.FindViewById<Button>(Resource.Id.add_button);

            typeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(TypeSpinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
            this.Context, Resource.Array.types, Android.Resource.Layout.SimpleSpinnerItem);
            typeSpinner.Adapter = adapter;
            typeSpinner.SetSelection(2); // card default

            monthOfExpireInput.Adapter = ArrayAdapter.CreateFromResource(
            this.Context, Resource.Array.months, Android.Resource.Layout.SimpleSpinnerItem);

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

        private async void FillEditText()
        {
            Dictionary<string, object> data = await API_RequestsData.GetDataFromDBAsync(this.Context, id);
            if (data != null)
            {
                nameInput.Text = data["name"] != null ? User.CurrentDecryptionMethod(data["name"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                firstNameInput.Text = data["first_name"] != null ? User.CurrentDecryptionMethod(data["first_name"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                lastNameInput.Text = data["last_name"] != null ? User.CurrentDecryptionMethod(data["last_name"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                cardNumberInput.Text = data["card_number"] != null ? User.CurrentDecryptionMethod(data["card_number"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                yearOfExpireInput.Text = data["year_of_expire"] != null ? User.CurrentDecryptionMethod(data["year_of_expire"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                monthOfExpireInput.SetSelection(data["month_of_expire"] != null ? Convert.ToInt32(data["month_of_expire"]) : 0);
                addressInput.Text = data["address"] != null ? User.CurrentDecryptionMethod(data["address"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                extraInfoOneInput.Text = data["extra_info_one"] != null ? User.CurrentDecryptionMethod(data["extra_info_one"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                extraInfoTwoInput.Text = data["extra_info_two"] != null ? User.CurrentDecryptionMethod(data["extra_info_two"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                extraInfoThreeInput.Text = data["extra_info_three"] != null ? User.CurrentDecryptionMethod(data["extra_info_three"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                noteInput.Text = data["note"] != null ? User.CurrentDecryptionMethod(data["note"].ToString(), CryptoHelper.key, User.salt) : string.Empty;
                addButton.Text = "módosítás";

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
                { "name", User.CurrentEncryptionMethod.Invoke( nameInput.Text,CryptoHelper.key, User.salt)  },
                { "first_name", User.CurrentEncryptionMethod.Invoke( firstNameInput.Text,CryptoHelper.key, User.salt)  },
                { "last_name", User.CurrentEncryptionMethod.Invoke( lastNameInput.Text,CryptoHelper.key, User.salt)  },
                { "card_number", User.CurrentEncryptionMethod.Invoke( cardNumberInput.Text,CryptoHelper.key, User.salt)  },
                { "year_of_expire", User.CurrentEncryptionMethod.Invoke( yearOfExpireInput.Text,CryptoHelper.key, User.salt)  },
                { "month_of_expire", monthOfExpireInput.SelectedItemPosition.ToString() },
                { "address", User.CurrentEncryptionMethod.Invoke( addressInput.Text,CryptoHelper.key, User.salt)  },
                { "extra_info_one",User.CurrentEncryptionMethod.Invoke( extraInfoOneInput.Text,CryptoHelper.key, User.salt)  },
                { "extra_info_two", User.CurrentEncryptionMethod.Invoke( extraInfoTwoInput.Text,CryptoHelper.key, User.salt)  },
                { "extra_info_three", User.CurrentEncryptionMethod.Invoke( extraInfoThreeInput.Text,CryptoHelper.key, User.salt)  },
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
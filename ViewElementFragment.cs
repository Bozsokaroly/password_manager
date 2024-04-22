using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.FloatingActionButton;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace password_manager
{
    public class ViewElementFragment : AndroidX.Fragment.App.Fragment
    {
        LinearLayout linearLayout;
        ImageView imageView;
        FloatingActionButton edit_button;
        string type = null;
        string id = null;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
  
            if (Arguments != null)
            {
                id = Arguments.GetString("_id", "");
            }
            LoadDataAsync(id);
            
        }

        public override void OnResume()
        {
            base.OnResume();
            LoadDataAsync(id);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((AndroidX.AppCompat.App.AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_view_item);
            View view = inflater.Inflate(Resource.Layout.view_element_layout, container, false);
            linearLayout = view.FindViewById<LinearLayout>(Resource.Id.linear_layout);
            imageView = view.FindViewById<ImageView>(Resource.Id.imageview);
            edit_button = view.FindViewById<FloatingActionButton>(Resource.Id.edit_button);
            edit_button.Click -= Edit_button_Click;
            edit_button.Click += Edit_button_Click;
            return view;
        }

        private void Edit_button_Click(object sender, EventArgs e)
        {
            AndroidX.Fragment.App.Fragment fragment = type switch
            {
                "3" => new AddNoteFragment(),
                "2" => new AddIdentifyFragment(),
                "1" => new AddCardFragment(),
                "0" => new AddLoginFragment(),
                _ => null
            };

            if (fragment != null)
            {
                Bundle args = new Bundle();
                args.PutString("id", id.ToString());
                fragment.Arguments = args;
                ParentFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, fragment, fragment.GetType().Name)
                    .AddToBackStack(null)
                    .Commit();
            }
        }

        private async void LoadDataAsync(string id)
        {
            String[] months = Resources.GetStringArray(Resource.Array.months);
            String[] companies = Resources.GetStringArray(Resource.Array.companies); // Itt javítottam egy elírást
            Dictionary<string, object> data = await API_RequestsData.GetDataFromDBAsync(this.Context, id);

            if (data == null) return;

            SetImageResource(data);
            PopulateLayoutWithData(data, months, companies);
        }

        private void SetImageResource(Dictionary<string, object> data)
        {
            var typeToDrawable = new Dictionary<string, int>
            {
                ["3"] = Resource.Drawable.notes,
                ["2"] = Resource.Drawable.identify,
                ["1"] = Resource.Drawable.card,
                ["0"] = Resource.Drawable.user
            };

            var itemType = data.ElementAt(1).Value.ToString();
            type = data.ElementAt(1).Value.ToString();
            imageView.SetImageResource(typeToDrawable.TryGetValue(itemType, out var drawableId) ? drawableId : Resource.Drawable.padlock);
        }

        private void PopulateLayoutWithData(Dictionary<string, object> data, String[] months, String[] companies)
        {
            linearLayout.RemoveAllViews();
            var dataList = data.ToList();
            var itemsToProcess = dataList.Skip(2).Take(dataList.Count - 3);

            foreach (var item in itemsToProcess) // Elhagyjuk az első kettő elemet
            {
                if (string.IsNullOrEmpty(item.Value.ToString())) continue;

                var label = CreateLabel(item.Key);
                linearLayout.AddView(label);

                var value = ResolveValue(item, months, companies);
                var valueView = CreateValueView(value);
                linearLayout.AddView(valueView);
            }
        }

        private TextView CreateLabel(string key)
        {
            var resourceId = Resources.GetIdentifier(key, "string", Context.PackageName);
            var text = resourceId > 0 ? GetString(resourceId) : key;  // Ha nincs resource, használja a kulcsot


            var textView = new TextView(this.Context)
            {
                Text = text,
                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
            };
            return textView;
        }

        private EditText CreateValueView(string text)
        {
            var editText = new EditText(this.Context)
            {
                Text = text,
                Focusable = false,
                FocusableInTouchMode = false,
                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
            };
            return editText;
        }

        private string ResolveValue(KeyValuePair<string, object> item, String[] months, String[] companies)
        {
            switch (item.Key)
            {
                case "company":
                    return companies[Convert.ToInt32(item.Value)];
                case "month_of_expire":
                    return months[Convert.ToInt32(item.Value)];
                default:
                    return User.CurrentDecryptionMethod.Invoke(item.Value.ToString(), CryptoHelper.key, User.salt);
            }
        }

    }
}
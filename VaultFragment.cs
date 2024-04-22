using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace password_manager
{
    public class VaultFragment : AndroidX.Fragment.App.Fragment
    {
        AndroidX.ConstraintLayout.Widget.ConstraintLayout login;
        AndroidX.ConstraintLayout.Widget.ConstraintLayout card;
        AndroidX.ConstraintLayout.Widget.ConstraintLayout identify;
        AndroidX.ConstraintLayout.Widget.ConstraintLayout note;
        FloatingActionButton addButton;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((AndroidX.AppCompat.App.AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_vault);
            View view = inflater.Inflate(Resource.Layout.vault_layout, container, false);

            login = view.FindViewById<AndroidX.ConstraintLayout.Widget.ConstraintLayout>(Resource.Id.logins);
            card = view.FindViewById<AndroidX.ConstraintLayout.Widget.ConstraintLayout>(Resource.Id.card);
            identify = view.FindViewById<AndroidX.ConstraintLayout.Widget.ConstraintLayout>(Resource.Id.identify);
            note = view.FindViewById<AndroidX.ConstraintLayout.Widget.ConstraintLayout>(Resource.Id.note);
            addButton = view.FindViewById<FloatingActionButton>(Resource.Id.add_button);
            //click events
            login.Click += Login_Click;
            card.Click += Card_Click;
            identify.Click += Identify_Click;
            note.Click += Note_Click;
            addButton.Click += AddButton_Click;

            return view;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddLoginFragment fragment = new AddLoginFragment();
            ParentFragmentManager.BeginTransaction()
                .Replace(Resource.Id.container, fragment)
                .AddToBackStack(null)
                .Commit(); 
        }

        private void Note_Click(object sender, EventArgs e)
        {
            GoToDataListViewFragment("3");
        }

        private void Identify_Click(object sender, EventArgs e)
        {
            GoToDataListViewFragment("2");
        }

        private void Card_Click(object sender, EventArgs e)
        {
            GoToDataListViewFragment("1");
        }

        private void Login_Click(object sender, EventArgs e)
        {
            GoToDataListViewFragment("0");
        }
        private void GoToDataListViewFragment(string type)
        {
            DataListViewFragment fragment = new DataListViewFragment();
            Bundle args = new Bundle();
            args.PutString("type", type);
            fragment.Arguments = args;
            ParentFragmentManager.BeginTransaction()
                .Replace(Resource.Id.container, fragment)
                .AddToBackStack(null)
                .Commit();
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace password_manager
{
    internal static class SpinnerHelper
    {
        private static FragmentActivity activity;

        public static void SetActivity(Context context)
        {
            if (context is FragmentActivity getactivity)
            {
                activity = getactivity;
            }
            else
            {
                throw new InvalidOperationException("The context is not a FragmentActivity.");
            }
        }
        public static AndroidX.Fragment.App.Fragment SpinnerSelected(int position)
        {
            return position switch
            {
                0 => new AddLoginFragment(),
                1 => new AddCardFragment(),
                2 => new AddIdentifyFragment(),
                3 => new AddNoteFragment(),
                _ => null,
            } ;
        }
        public static void ShowFragment(AndroidX.Fragment.App.Fragment fragment, string tag)
        {
            if (activity == null) throw new InvalidOperationException("Activity cannot be null when showing a fragment.");

            var fragmentManager = activity.SupportFragmentManager;
            // Ellenőrzés, hogy ugyanez a fragment van-e már megjelenítve
            AndroidX.Fragment.App.Fragment currentFragment = fragmentManager.FindFragmentById(Resource.Id.container);
            if (currentFragment != null && currentFragment.Tag == tag)
                return;

            // Fragment megjelenítése, ha másik van aktív
            var transaction = fragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.container, fragment, tag);
            transaction.AddToBackStack(null);
            transaction.Commit();
        }
    }
}
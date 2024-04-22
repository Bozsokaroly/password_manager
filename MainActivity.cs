using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Icu.Text;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.Navigation;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Android.Content.ClipData;

namespace password_manager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        ImageView hun;
        ImageView eng;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);


            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
            View header = navigationView.GetHeaderView(0);

            hun = navigationView.FindViewById<ImageView>(Resource.Id.hun);
            eng = navigationView.FindViewById<ImageView>(Resource.Id.eng);
            hun.Click += Hun_Click;
            eng.Click += Eng_Click;

            TextView header_message = header.FindViewById<TextView>(Resource.Id.username);
            header_message.Text = $"{GetString(Resource.String.greeting)} {User.name}!";
            VaultFragment fragment = new VaultFragment();
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, fragment).AddToBackStack(null).Commit();


        }

        public void ShowFragment(AndroidX.Fragment.App.Fragment fragment)
        {
           SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.container, fragment)
                .AddToBackStack(null)
                .Commit();
        }
        public override void OnBackPressed()
        {

            if (SupportFragmentManager.BackStackEntryCount > 1)
            {
                SupportFragmentManager.PopBackStack();
            }
            else
            {
                base.OnBackPressed(); // Hagyd, hogy az Activity kezelje a vissza gombot (pl. kilépés az appból)
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }




        //public override void OnBackPressed()
        //{
        //    DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
        //    if (drawer.IsDrawerOpen(GravityCompat.Start))
        //    {
        //        drawer.CloseDrawer(GravityCompat.Start);
        //    }
        //    else
        //    {
        //        //Toast.MakeText(this, "Nem léphetsz vissza. Jelentkezz ki!", ToastLength.Short).Show();
        //        base.OnBackPressed();
        //    }
        //}



        #region NavigateMenu
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            AndroidX.Fragment.App.Fragment fragment = null;

            switch (id)
            {
                case Resource.Id.nav_vault:
                    fragment = new VaultFragment();
                    break;
                case Resource.Id.nav_generator:
                    fragment = new GeneratorFragment();
                    break;
                case Resource.Id.nav_send:
                    fragment = new SendFragment();
                    break;
                case Resource.Id.nav_export:
                    fragment = new ExportFragment();
                    break;
                case Resource.Id.nav_logout:
                    User.Logout();
                    CryptoHelper.Logout();
                    StartActivity(typeof(LoginActivity));
                    Finish();
                    break;
            }

            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, fragment).AddToBackStack(null).Commit();
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        #endregion

        #region LangSettings
        private void Eng_Click(object sender, System.EventArgs e)
        {
            LocaleManager.ChangeLanguage(this, "en");
        }

        private void Hun_Click(object sender, System.EventArgs e)
        {
            LocaleManager.ChangeLanguage(this, "hu");
        }



        protected override void AttachBaseContext(Context @newBase)
        {
            base.AttachBaseContext(LocaleManager.SetLocale(@newBase));
        }
        public static void RefreshActivity(Context context)
        {
            var intent = new Intent(context, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            context.StartActivity(intent);
        }
        public Context updateBaseContextLocale(Context context)
        {
            var sharedPreferences = Application.Context.GetSharedPreferences("AppLanguage", FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            Java.Util.Locale locale = null;
            switch (sharedPreferences.GetString("lang", "en"))
            {
                case "en":
                    locale = new Java.Util.Locale("en");
                    break;
                case "hu":
                    locale = new Java.Util.Locale("hu");
                    break;
                default:
                    locale = new Java.Util.Locale("en");
                    break;
            }

            Java.Util.Locale.Default = locale;

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                Configuration configuration = context.Resources.Configuration;
                configuration.SetLocale(locale);

                return context.CreateConfigurationContext(configuration);
            }
            else
            {
                Resources resources = context.Resources;
                Configuration configuration = resources.Configuration;
                #pragma warning disable CS0618 // Type or member is obsolete
                configuration.Locale = locale;
                resources.UpdateConfiguration(configuration, resources.DisplayMetrics);
                #pragma warning restore CS0618 // Type or member is obsolete

                return context;
            }
        }
        #endregion
    }
}
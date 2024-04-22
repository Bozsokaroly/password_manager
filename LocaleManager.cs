using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace password_manager
{
    internal static class LocaleManager
    {
        public static Context SetLocale(Context context)
        {
            var sharedPreferences = context.GetSharedPreferences("AppLanguage", FileCreationMode.Private);
            Java.Util.Locale locale = new Java.Util.Locale(sharedPreferences.GetString("lang", "en"));
            Java.Util.Locale.Default = locale;
            User.lang = sharedPreferences.GetString("lang", "en");

            Configuration config = new Configuration();
            config.SetLocale(locale);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return context.CreateConfigurationContext(config);
            }
            else
            {
                context.Resources.UpdateConfiguration(config, context.Resources.DisplayMetrics);
                return context;
            }
        }

        public static void RefreshActivity(Activity activity)
        {
            Intent intent = new Intent(activity, activity.GetType());
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            activity.StartActivity(intent);
        }

        public static void ChangeLanguage(Activity activity, string langCode)
        {
            var sharedPreferences = activity.GetSharedPreferences("AppLanguage", FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.PutString("lang", langCode);
            editor.Apply();

            RefreshActivity(activity);
        }
    }
}
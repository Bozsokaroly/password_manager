using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace password_manager
{
    public class GeneratorFragment : AndroidX.Fragment.App.Fragment
    {
        ImageView refresh;
        ImageView copy;
        TextView password;
        TextView counter;
        AndroidX.AppCompat.Widget.AppCompatSeekBar seekbar;

        AndroidX.AppCompat.Widget.SwitchCompat upperCaseSwitch;
        AndroidX.AppCompat.Widget.SwitchCompat lowerCaseSwitch;
        AndroidX.AppCompat.Widget.SwitchCompat numbers;
        AndroidX.AppCompat.Widget.SwitchCompat specialCharacters;

        private const string SpecialCharacters = "!@#$%^&*_.?";
        private const string Numbers = "0123456789";
        private const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        int length;
        string characterList;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((AndroidX.AppCompat.App.AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_generator);
            View view = inflater.Inflate(Resource.Layout.generator_layout, container, false);
            refresh = view.FindViewById<ImageView>(Resource.Id.refresh);
            copy = view.FindViewById<ImageView>(Resource.Id.copy);
            password = view.FindViewById<TextView>(Resource.Id.generated_password);
            counter = view.FindViewById<TextView>(Resource.Id.counter);
            seekbar = view.FindViewById<AndroidX.AppCompat.Widget.AppCompatSeekBar>(Resource.Id.seekbar);
            upperCaseSwitch = view.FindViewById<AndroidX.AppCompat.Widget.SwitchCompat>(Resource.Id.uppercase_switch);
            lowerCaseSwitch = view.FindViewById<AndroidX.AppCompat.Widget.SwitchCompat>(Resource.Id.lowercase_switch);
            numbers = view.FindViewById<AndroidX.AppCompat.Widget.SwitchCompat>(Resource.Id.numerics_switch);
            specialCharacters = view.FindViewById<AndroidX.AppCompat.Widget.SwitchCompat>(Resource.Id.special_switch);

            upperCaseSwitch.CheckedChange += UpperCaseSwitch_CheckedChange;
            lowerCaseSwitch.CheckedChange += LowerCaseSwitch_CheckedChange;
            numbers.CheckedChange += Numbers_CheckedChange;
            specialCharacters.CheckedChange += SpecialCharacters_CheckedChange;
            lowerCaseSwitch.Checked = true;
            numbers.Checked = true;


            seekbar.Min = 5;
            seekbar.Max= 25;
            seekbar.ProgressChanged += Seekbar_ProgressChanged;
            seekbar.Progress = 10;

            copy.Click += Copy_Click;
            refresh.Click += Refresh_Click;

            

            return view;
        }
        private string RemoveCharacters(string source, string charactersToRemove)
        {
            return new string(source.Where(c => !charactersToRemove.Contains(c)).ToArray());
        }
        private void SpecialCharacters_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                characterList += SpecialCharacters;
            }
            else
            {
                characterList = RemoveCharacters(characterList, SpecialCharacters);
            }
        }
        private void Numbers_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                characterList += Numbers;
            }
            else
            {
                characterList = RemoveCharacters(characterList, Numbers);
            }
        }

        private void LowerCaseSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                characterList += LowerCaseLetters;
            }
            else
            {
                characterList = RemoveCharacters(characterList, LowerCaseLetters);
            }
        }

        private void UpperCaseSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                characterList += UpperCaseLetters;
            }
            else
            {
                characterList = RemoveCharacters(characterList, UpperCaseLetters);
            }
        }

        private void Seekbar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            counter.Text = Convert.ToString(e.Progress);
            length = e.Progress;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (characterList.Length <= 0)
            {
                Toast.MakeText(this.Context, GetString(Resource.String.nothing_selected), ToastLength.Long).Show();
                return;
            }
            Random random = new Random();
            char[] generated_password = new char[length];
            for (int i = 0; i < length; i++)
            {
                // Egy random karakter kiszedése a karakterlistából
                char randomChar = characterList[random.Next(characterList.Length)];
                // a random karakter hozzáadása a jelszóhoz
                generated_password[i] = randomChar;
            }
            password.Text = new string(generated_password);
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            var clipboard = (ClipboardManager)this.Context.GetSystemService(Context.ClipboardService);
            var clip = ClipData.NewPlainText("simple text", password.Text.ToString());
            clipboard.PrimaryClip = clip;
            Toast.MakeText(this.Context, GetString(Resource.String.password_copy), ToastLength.Long).Show();
        }
    }
}
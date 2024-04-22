using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net;
using Com.Tomergoldst.Tooltips;
using System.Text;
using Newtonsoft.Json.Linq;
using AndroidX.AppCompat.App;
using Android.Nfc;
using Android.Util;
using Android.Views.InputMethods;
using AndroidX.Core.Content;

namespace password_manager
{
    [Activity(Label = "RegisterActivity", Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.AdjustResize)]
    public class RegisterActivity : AppCompatActivity, ToolTipsManager.ITipListener
    {
        private bool isPasswordVisible = false;
        private ToolTipsManager _toolTipsManager;
        EditText username;
        EditText email;
        EditText password;
        ImageView showPassword;
        Button register;
        Button back;
        RadioGroup encrypt;
        AndroidX.ConstraintLayout.Widget.ConstraintLayout layout;
        int selectedEncryption;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_layout);

            layout = FindViewById<AndroidX.ConstraintLayout.Widget.ConstraintLayout>(Resource.Id.container);
            showPassword = FindViewById<ImageView>(Resource.Id.show_password);
            username = FindViewById<EditText>(Resource.Id.username_input);
            email = FindViewById<EditText>(Resource.Id.email_input);
            password = FindViewById<EditText>(Resource.Id.password_input);
            register = FindViewById<Button>(Resource.Id.register_button);
            back = FindViewById<Button>(Resource.Id.back_button);
            encrypt = FindViewById<RadioGroup>(Resource.Id.encryption_radio_group);
            register.Click += Register_ClickAsync;
            back.Click += Back_Click;
            password.Touch += Password_Touch;
            password.TextChanged += Password_TextChanged;
            password.FocusChange += Password_FocusChange;
            encrypt.CheckedChange += Encrypt_CheckedChange;
            showPassword.Click += ShowPassword_Click1;

            _toolTipsManager = new ToolTipsManager(this);
        }

        private void Password_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
            {
                password.RequestFocus();
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(password, ShowFlags.Implicit);
                password.SetSelection(GetCharIndexFromX(password, e.Event.GetX()));
            }
        }

        private int GetCharIndexFromX(EditText editText, float x)
        {
            if (editText == null || editText.Layout == null)
                return 0;

            int line = editText.Layout.GetLineForVertical((int)(editText.ScrollY + x));
            int offset = editText.Layout.GetOffsetForHorizontal(line, x);
            return offset;
        }
        private void Password_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (password.Text.Length > 0)
            {
                _toolTipsManager.FindAndDismiss(password);
            }
            else
            {
                password.PostDelayed(() => {
                    ShowToolTip(GetString(Resource.String.password_require));
                }, 500); // 500 ms késleltetés
            }
        }

        private void Password_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (password.HasFocus && string.IsNullOrEmpty(password.Text))
            {
                password.PostDelayed(() => {
                    ShowToolTip(GetString(Resource.String.password_require));
                }, 500); // 500 ms késleltetés
            }
            else
            {
                _toolTipsManager.FindAndDismiss(password);
            }
        }

        private void ShowToolTip(string message)
        {
            ToolTip.Builder builder = new ToolTip.Builder(this, password, layout, message, ToolTip.PositionAbove);
            builder.SetBackgroundColor(ContextCompat.GetColor(this, Resource.Color.colorAccent));
            builder.SetAlign(ToolTip.AlignCenter);
            _toolTipsManager.Show(builder.Build());
        }
        public void OnTipDismissed(View view, int anchorViewId, bool byUser)
        {
            // ide kell irni valamit ha eltűnik a tooltip
        }
        protected override void AttachBaseContext(Context @newBase)
        {
            base.AttachBaseContext(LocaleManager.SetLocale(@newBase));
        }
        private void ShowPassword_Click1(object sender, EventArgs e)
        {
            if (!isPasswordVisible)
            {
                password.InputType = Android.Text.InputTypes.TextVariationVisiblePassword | Android.Text.InputTypes.ClassText;
                password.SetSelection(password.Text.Length);
                isPasswordVisible = true;
            }
            else
            {
                password.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
                password.SetSelection(password.Text.Length);
                isPasswordVisible = false;
            }
        }

        private void Encrypt_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            int aesRadioButtonId = Resource.Id.radio_aes;
            selectedEncryption = e.CheckedId == aesRadioButtonId ? 0 : 1;

        }

        private void Back_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
        }

        private async void Register_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                JObject json = new JObject
                {
                    { "email", email.Text },
                    { "name", username.Text },
                    { "password", password.Text },
                    { "encrypt", selectedEncryption.ToString() }
                };
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = await Constants.client.PostAsync($"{Constants.ip_address}/users/register?lang={User.lang}", content);
                var responseString = await response.Content.ReadAsStringAsync();
                JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                if (response.IsSuccessStatusCode)
                {
                    Toast.MakeText(this, responsejson["message"].ToString(), ToastLength.Short).Show();
                    Intent intent = new Intent(this, typeof(LoginActivity));
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(this, responsejson["message"].ToString(), ToastLength.Short).Show();
                }
            }
            catch (WebException webex)
            {
                Toast.MakeText(this, $"{GetString(Resource.String.network_error)} {webex.Message}.", ToastLength.Short).Show();
            }
            catch (System.OperationCanceledException ex)
            {
                Toast.MakeText(this, $"{GetString(Resource.String.network_error)} {ex.Message}.", ToastLength.Short).Show();
            }
            catch (System.Exception hiba)
            {
                Toast.MakeText(this, $"{GetString(Resource.String.generic_error)} {hiba.Message}.", ToastLength.Short).Show();
            }
            return;
        }
    }
}
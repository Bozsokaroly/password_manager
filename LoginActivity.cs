using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Extensions;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Java.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AndroidX.Biometric;
using Android.Gms.Common.Apis;
using Android.Net;

namespace password_manager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme",WindowSoftInputMode =SoftInput.AdjustResize, MainLauncher = true)]
    public partial class LoginActivity : AppCompatActivity
    {
        private bool isPasswordVisible = false;
        EditText email;
        EditText password;
        ImageView showPassword;
        Button login;
        Button register;
        EditText masterPasswordInput;
        Button biometricsButton;
        Button submitButton;
        TextView descriptionText;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Create your application here

            if (GetLoginState())
            {
                SetContentView(Resource.Layout.master_password_layout);
                showPassword = FindViewById<ImageView>(Resource.Id.show_password);
                masterPasswordInput = FindViewById<EditText>(Resource.Id.master_password_input);
                biometricsButton = FindViewById<Button>(Resource.Id.biometrics_button);
                submitButton = FindViewById<Button>(Resource.Id.master_password_button);
                descriptionText = FindViewById<TextView>(Resource.Id.description_text);

                showPassword.Click += ShowPassword_Click;

                var sharedPreferences = Application.Context.GetSharedPreferences("MasterPassword", FileCreationMode.Private);

                User.SetData();
                if (sharedPreferences.GetBoolean("IsPasswordSet", false) )
                {
                    CryptoHelper.SetKey();
                    HaveMasterPassword();
                }
                else
                {
                    Setup_MasterPassword();
                }

            }
            else
            {
                SetContentView(Resource.Layout.login_layout);

                showPassword = FindViewById<ImageView>(Resource.Id.show_password);
                email = FindViewById<EditText>(Resource.Id.email_input);
                password = FindViewById<EditText>(Resource.Id.password_input);
                login = FindViewById<Button>(Resource.Id.login_button);
                register = FindViewById<Button>(Resource.Id.register_button);

                register.Click += Register_Click;
                login.Click += Login_Click;
                showPassword.Click += ShowPassword_Click1;

            }

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

        protected override void AttachBaseContext(Context @newBase)
        {
            base.AttachBaseContext(LocaleManager.SetLocale(@newBase));
        }



        private void ShowPassword_Click(object sender, EventArgs e)
        {
            if (!isPasswordVisible)
            {
                masterPasswordInput.InputType = Android.Text.InputTypes.TextVariationVisiblePassword | Android.Text.InputTypes.ClassText;
                masterPasswordInput.SetSelection(masterPasswordInput.Text.Length);
                isPasswordVisible = true;
            }
            else
            {
                masterPasswordInput.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
                masterPasswordInput.SetSelection(masterPasswordInput.Text.Length);
                isPasswordVisible = false;
            }
        }

        #region MasterPassword
        private void Setup_MasterPassword()
        {
            try
            {
                //lekérdezni hogy van e master kulcs már 
                descriptionText.Text = System.String.Format(GetString(Resource.String.type_master_password));
                biometricsButton.Clickable = false;
                submitButton.Click += SubmitButton_Click;

            }
            catch (System.Exception e)
            {

                throw;
            }

        }
        private async Task GenerateKeyAsync(string password, byte[] salt)
        {
            byte[] key = CryptoHelper.GenerateKey(masterPasswordInput.Text, User.salt);  //kigenerálom a kulcsot
            await CryptoHelper.SaveKeyAsync(key);                                       //elmentem a telóba
            CryptoHelper.SetKey();                                                      //elmentem a változóba

        }
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!IsValidPassword(masterPasswordInput.Text))
            {
                Toast.MakeText(this, GetString(Resource.String.password_too_short), ToastLength.Long).Show();
                return;
            }

            var sharedPreferences = Application.Context.GetSharedPreferences("MasterPassword", FileCreationMode.Private);
            if (sharedPreferences.GetBoolean("IsPasswordSet", false))
            {
                VerifyPassword();
            }
            else
            {
                SetNewPassword();
            }
        }
        private bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 6;
        }
        private void VerifyPassword()
        {
            bool check = CryptoHelper.CheckKey(masterPasswordInput.Text, User.salt);
            if (check)
            {
                NavigateToMainActivity();
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.incorrect_password), ToastLength.Long).Show();
            }
        }

        private async void SetNewPassword()
        {
            await GenerateKeyAsync(masterPasswordInput.Text, User.salt);

            string verify = await API_RequestsVerify.GetVerifyFromDBAsync(this);         //kulcs generálás , mentés, változóba rakás
            if (!string.IsNullOrEmpty(verify)) 
            {
                bool response = CryptoHelper.Verify(verify, CryptoHelper.key);
                if (response)
                {
                    SetPasswordSetTrue();                                                                                   //beállítom hogy meglett a master kulcs
                    NavigateToMainActivity();
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.registered_incorrect_password), ToastLength.Short).Show();
                }
            }
            else
            {
                JObject json = new JObject {
                { "verify", User.CurrentEncryptionMethod.Invoke(User.name, CryptoHelper.key, User.salt) }};                                                                                                       //feltöltöm ezt a db-be
                if (await API_RequestsVerify.AddVerifyToDB(this, json))
                {
                    SetPasswordSetTrue();                                                                                   //beállítom hogy meglett a master kulcs
                    NavigateToMainActivity();
                }
            }

            
        }

        private void NavigateToMainActivity()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        private void SetPasswordSetTrue()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences("MasterPassword", FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.PutBoolean("IsPasswordSet", true);
            editor.Commit();
        }
        private void HaveMasterPassword()
        {
            ShowBiometricPrompt();
            submitButton.Click += SubmitButton_Click;
            biometricsButton.Click += BiometricsButton_Click;
        }

        private void BiometricsButton_Click(object sender, EventArgs e)
        {
            ShowBiometricPrompt();
        }
        #endregion

        #region biometrics
        private void Callback_AuthenticationSucceeded(object sender, EventArgs e)
        {
            RunOnUiThread(() => Toast.MakeText(this, GetString(Resource.String.auth_succeeded), ToastLength.Short).Show());
            CryptoHelper.SetKey();
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        private void Callback_AuthenticationError(object sender, AuthenticationErrorEventArgs e)
        {
            RunOnUiThread(() => Toast.MakeText(this, $"{GetString(Resource.String.auth_error)} + { e.ErrorMessage}", ToastLength.Long).Show());
        }

        private void Callback_AuthenticationFailed(object sender, EventArgs e)
        {
            RunOnUiThread(() => Toast.MakeText(this, GetString(Resource.String.auth_failed), ToastLength.Short).Show());
        }
        private void ShowBiometricPrompt()
        {
            var executor = ContextCompat.GetMainExecutor(this);
            var callback = new AuthenticationCallback();

            callback.AuthenticationSucceeded += Callback_AuthenticationSucceeded;
            callback.AuthenticationError += Callback_AuthenticationError;
            callback.AuthenticationFailed += Callback_AuthenticationFailed;

            var promptInfo = new BiometricPrompt.PromptInfo.Builder()
                .SetTitle(GetString(Resource.String.biometric_login))
                .SetSubtitle(GetString(Resource.String.biometric_title))
                .SetNegativeButtonText(GetString(Resource.String.biometric_cancel))
                .Build();

            // A BiometricPrompt objektum itt jön létre, a callbackkel együtt
            var biometricPrompt = new BiometricPrompt(this, executor, callback);

            // Itt használjuk az Authenticate metódust a korábban létrehozott promptInfoval
            biometricPrompt.Authenticate(promptInfo);
        }
        #endregion

        #region Register
        private void Register_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }
        #endregion

        #region Login
        private void SaveLoginState()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences("Login", FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.PutBoolean("IsLoggedIn", true);
            editor.Commit();
        }
        private bool GetLoginState()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences("Login", FileCreationMode.Private);
            return sharedPreferences.GetBoolean("IsLoggedIn", false);
        }
        private async void Login_Click(object sender, EventArgs e)
        {
            if (Constants.Test_mode)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                return;
            }
            try
            {
                JObject json = new JObject
                {
                    { "email", email.Text },
                    { "password", password.Text }
                };
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = await Constants.client.PostAsync($"{Constants.ip_address}/users/login?lang={User.lang}", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    JObject data = (JObject)JObject.Parse(responseString)["data"];
                    if (User.Login(data))
                    {
                        Toast.MakeText(this, responsejson["message"].ToString(), ToastLength.Short).Show();
                        User.SetData();
                        SaveLoginState();
                        Intent intent = new Intent(this, typeof(LoginActivity));
                        StartActivity(intent);

                    }
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
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
        #endregion
    }
}